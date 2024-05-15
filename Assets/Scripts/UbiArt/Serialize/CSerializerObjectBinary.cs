using System;
using System.IO;
using System.Text;
using UbiArt.FileFormat;
using System.Threading.Tasks;

namespace UbiArt {
	public class CSerializerObjectBinary : CSerializerObject {
		public CSerializerObjectBinary(Context context, IBinaryLowLevelSerializer serializer) : base(context) {
			BinarySerializer = serializer;
			if (BinarySerializer is Reader) { // TODO: Make this depend on a property?
				Mode = BinaryMode.Read;
			} else if (BinarySerializer is Writer) {
				Mode = BinaryMode.Write;
			} else {
				throw new Exception("Unsupported serializer interface");
			}
		}
		public CSerializerObjectBinary(Context context, BinaryFile file, BinaryMode mode) : base(context) {
			Mode = mode;
			File = file;
			if (Mode == BinaryMode.Read) {
				BinarySerializer = File.CreateReader();
			} else {
				BinarySerializer = File.CreateWriter();
			}
		}
		public override SerializerProperties Properties => base.Properties | SerializerProperties.Binary | SerializerProperties.BinSkip; // 0x11

		public BinaryFile File { get; protected set; }
		public IBinaryLowLevelSerializer BinarySerializer { get; protected set; }
		public override Pointer CurrentPointer => new Pointer(CurrentPosition, File);
		public override long CurrentPosition => BinarySerializer.Position;
		public override long Length => BinarySerializer.Length;

		public uint dataVersion { get; set; } = 1;

		public enum BinaryMode {
			Read,
			Write
		}
		public BinaryMode Mode { get; protected set; }

		public override void Dispose() {
			base.Dispose();
			BinarySerializer?.Dispose();
		}

		public override void Goto(long position) {
			if (!Disposed) BinarySerializer.Position = position;
		}

		public override object Serialize(object obj, Type type, string name = null, Options options = Options.None) {
			if (Type.GetTypeCode(type) != TypeCode.Object) {
				obj = Type.GetTypeCode(type) switch {
					TypeCode.Boolean => SerializeBool((bool)obj, name: name, options: options),
					TypeCode.SByte => BinarySerializer.SerializeSByte((sbyte)obj),
					TypeCode.Byte => BinarySerializer.SerializeUByte((byte)obj),
					TypeCode.Int16 => BinarySerializer.SerializeInt16((short)obj),
					TypeCode.UInt16 => BinarySerializer.SerializeUInt16((ushort)obj),
					TypeCode.Int32 => BinarySerializer.SerializeInt32((int)obj),
					TypeCode.UInt32 => BinarySerializer.SerializeUInt32((uint)obj),
					TypeCode.Int64 => BinarySerializer.SerializeInt64((long)obj),
					TypeCode.UInt64 => BinarySerializer.SerializeUInt64((ulong)obj),
					TypeCode.Single => BinarySerializer.SerializeSingle((float)obj),
					TypeCode.Double => BinarySerializer.SerializeDouble((double)obj),
					TypeCode.Char => BinarySerializer.SerializeChar((char)obj),
					TypeCode.String => SerializeString8((string)obj, name: name, options: options),
					_ => throw new Exception($"Unsupported TypeCode {Type.GetTypeCode(type)}")
				};
			} else if (type == typeof(CString)) {
				obj = SerializeCString((CString)obj);
			} else if (type == typeof(BasicString)) {
				obj = SerializeBasicString((BasicString)obj);
			} else if (type == typeof(byte[])) {
				obj = SerializeByteArrayWithCount((byte[])obj);
			} else {
				if (obj == null) {
					var ctor = type.GetConstructor(Type.EmptyTypes);
					if (ctor == null) {
						throw new Exception("Constructor is null");
					}
					obj = ctor.Invoke(new object[] { });
				}
				if (obj is ICSerializable) {
					IncreaseLevel();
					((ICSerializable)obj).Serialize(this, name);
					DecreaseLevel();
					if(Mode == BinaryMode.Read) AddToStringCache(obj);
				}
			}
			return obj;
		}

		public override T SerializeGeneric<T>(T obj, Type type = null, string name = null, int? index = null) {
			var logPrefix = LogPrefix;

			var usedType = type ?? typeof(T);

			bool isBigObject = IsBigObject(usedType) && name != null;
			bool isShortLog = IsShortLog(usedType); 
			
			if (IsSerializerLoggerEnabled && name != null && isBigObject) {
				Context.SerializerLogger.Log($"{logPrefix}({usedType}) {name}:");
			}

			try {
				obj = (T)Serialize(obj, usedType, name: name);
			/*} catch(Exception ex) {
				throw new Exception($"Exception occurred at {CurrentPointer}", ex);*/
			} finally {
				EndShortLog(isShortLog);
				if (IsSerializerLoggerEnabled && name != null && !isBigObject) {
					Context.SerializerLogger.Log($"{logPrefix}({usedType}) {name} - {ShortLog(obj)}");
				}
			}
			return obj;
		}

		public override byte[] SerializeBytes(byte[] obj, long count) {
			return BinarySerializer.SerializeBytes(obj, (int)count);
		}

		public override bool ArrayEntryStart(string name, int index) {
			if (IsSerializerLoggerEnabled) {
				Context.SerializerLogger.Log($"{LogPrefix} {name}[{index}]:");
			}
			return base.ArrayEntryStart(name, index);
		}

		public override T SerializeGenericPureBinary<T>(T obj, Type type = null, string name = null, int? index = null) {
			return SerializeGeneric<T>(obj, type: type, name: name, index: index);
		}

		public override T Serialize<T>(T obj, string name = null, int? index = null, Options options = Options.None) {
			var logPrefix = LogPrefix;

			T t = (T)Serialize(obj, typeof(T), name: name, options: options);

			if (IsSerializerLoggerEnabled && name != null) {
				Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name} - {ShortLog(t)}");
			}
			return t;
		}

		public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null, int? index = null, Options options = Options.None) {
			var logPrefix = LogPrefix;

			bool isBigObject = IsBigObject(typeof(T));
			bool isShortLog = IsShortLog(typeof(T));

			if (IsSerializerLoggerEnabled && isBigObject && name != null) {
				Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name}:");
			}

			IncreaseLevel();
			try {
				if (obj == null) obj = new T();
				obj.Serialize(this, name);
			} finally {
				DecreaseLevel();
				AddToStringCache(obj);

				EndShortLog(isShortLog);
				if (IsSerializerLoggerEnabled && !isBigObject && name != null) {
					Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name} - {ShortLog(obj)}");
				}
			}
			return obj;
		}

		public override async Task FillCacheForRead(long byteCount) {
			if (Mode == BinaryMode.Read) {
				await Context.FileManager.FillCacheForReadAsync(byteCount, (Reader)BinarySerializer);
			}
		}

		void AddToStringCache(object obj) {
			if (Mode == BinaryMode.Read)
				Context.AddToStringCache(obj);
		}


		#region Logging

		protected string LogPrefix => IsSerializerLoggerEnabled ? $"({(Mode == BinaryMode.Read ? "R" : "W")}) {CurrentPointer}:{new string(' ', (Depth + 1) * 2)}" : null;
		//protected string LogPrefix => IsSerializerLoggerEnabled ? $"{new string(' ', (Depth + 1) * 2)}" : null;


		public override void Log(string logString, params object[] args) {
			if (IsSerializerLoggerEnabled)
				Context.SerializerLogger.Log(LogPrefix + String.Format(logString, args));
		}

		#endregion

		#region Encoding

		protected override void DoEncoded(IStreamEncoder encoder, Action action, Endian? endianness = null, string filename = null) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			if (encoder == null) {
				action();
				return;
			}

			// Stream key
			Pointer offset = CurrentPointer;
			string key = filename ?? $"{offset}_{encoder.Name}";

			// Decode the data into a stream
			long start = BinarySerializer.Position;
			long end = start;
			long encodedLength = 0, decodedLength = 0;

			using MemoryStream decodedStream = new();

			if (Mode == BinaryMode.Read) {
				encoder.DecodeStream(BinarySerializer.BaseStream, decodedStream);
				decodedStream.Position = 0;
				end = BinarySerializer.Position;
				encodedLength = end - start;
			}

			// Add the stream
			BinaryStreamFile sf = new(
				context: Context,
				name: key,
				stream: decodedStream,
				endianness: endianness ?? File.Endianness);

			var oldSerializer = BinarySerializer;
			var file = File;

			var tuple = new Tuple<string, UbiArtFile>(key, sf);
			
			try {
				File = sf;
				BinarySerializer = Mode == BinaryMode.Read ? sf.CreateReader() : sf.CreateWriter();
				Context.Loader.virtualFiles.Add(tuple);

				if (Mode == BinaryMode.Read) {
					decodedLength = BinarySerializer.Length;
					Log("Decoded data using {0} at {1} with decoded length {2} and encoded length {3}",
						encoder.Name, offset, decodedLength, encodedLength);
				}
				action();

				if (Mode == BinaryMode.Read) {
					if (CurrentPosition != decodedLength)
						Context?.SystemLogger?.LogWarning("Encoded block {0} was not fully deserialized: Serialized size: {1} != Total size: {2}",
							key, CurrentPointer, decodedLength);
				} else {
					// Encode the stream to the current file
					decodedStream.Position = 0;
					encoder.EncodeStream(decodedStream, BinarySerializer.BaseStream);
				}
			} finally {
				BinarySerializer = oldSerializer;
				File = file;
				Context.Loader.virtualFiles.Remove(tuple);
			}
		}

		public override void DoEncrypted(uint[] encryptionKey, Action action, string name = null) {
			throw new NotImplementedException();
		}

		public override void DoCompressed(Action action, string name = null) {
			throw new NotImplementedException();
		}

		public override void DoEndian(Action action, Endian endianness) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			var curEndian = BinarySerializer.IsLittleEndian;
			try {
				BinarySerializer.IsLittleEndian = endianness == Endian.Little;
				action();
			} finally {
				BinarySerializer.IsLittleEndian = curEndian;
			}
		}
		#endregion

		#region Object Size
		protected virtual void IncreaseMemCountForString(string s, Encoding encoding) { }

		public override void InitSerializer() {
			base.InitSerializer();

			if (Settings.EngineVersion > EngineVersion.RO) {
				dataVersion = Serialize<uint>(dataVersion, name: "dataVersion");
			}
		}
		#endregion

		#region Helpers
		protected string SerializeString8(string value, string name = null, Options options = Options.None) {
			var encoding = options.HasFlag(Options.UTF8) ? Encoding.UTF8 : Settings.DefaultEncoding;
			string s = BinarySerializer.SerializeString8(value, encoding: encoding);
			if (!options.HasFlag(Options.DontIncreaseMemCount)) IncreaseMemCountForString(s, encoding);

			if (Mode == BinaryMode.Read) Context.AddToStringCache(s);

			return s;
		}
		protected bool SerializeBool(bool value, string name = null, Options options = Options.None) {
			bool asByte = false;
			if (options.HasFlag(Options.ForceAsByte)) {
				asByte = true;
			} else if (options.HasFlag(Options.BoolAsByte)) {
				asByte = HasFlags(SerializeFlags.PropertyEdit_Save);
			}
			uint value32 = value ? (uint)1 : 0;

			if (asByte) {
				value32 = BinarySerializer.SerializeUByte((byte)value32);
			} else {
				value32 = BinarySerializer.SerializeUInt32(value32);
			}

			if (Mode == BinaryMode.Read) {
				if (value32 == 1) {
					value = true;
				} else if (value32 != 0) {
					Context?.SystemLogger?.LogWarning($"{CurrentPointer}: Bool with name {name} was {value32}!");
					value = true;
				} else {
					value = false;
				}
			}
			return value;
		}
		protected CString SerializeCString(CString value) {
			if (value == null) value = new CString();
			value.str = BinarySerializer.SerializeString16(value);
			if (Mode == BinaryMode.Read) Context.AddToStringCache(value);
			return value;
		}
		protected BasicString SerializeBasicString(BasicString value) {
			if (value == null) value = new BasicString();
			value.str = BinarySerializer.SerializeString8(value);
			if (Mode == BinaryMode.Read) Context.AddToStringCache(value);
			return value;
		}
		protected byte[] SerializeByteArrayWithCount(byte[] value) {
			uint length = BinarySerializer.SerializeUInt32((uint)(value?.Length ?? 0));

			if(value == null)
				value = new byte[length];
			else if (value.Length != length)
				Array.Resize(ref value, (int)length);

			return BinarySerializer.SerializeBytes(value, value.Length);
		}
		#endregion
	}
}
