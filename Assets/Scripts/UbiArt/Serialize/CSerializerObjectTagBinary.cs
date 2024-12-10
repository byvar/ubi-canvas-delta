﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UbiArt.CRC;
using UbiArt.FileFormat;

namespace UbiArt {
	public class CSerializerObjectTagBinary : CSerializerObject {
		public static Dictionary<UAFInfo, uint> fieldCRC = new Dictionary<UAFInfo, uint>();
		protected Stack<long> endPos = new Stack<long>();
		protected bool fakeSerializeMode = false;

		public CSerializerObjectTagBinary(Context context, IBinaryLowLevelSerializer serializer) : base(context) {
			if (serializer is Reader r) {
				Reader = r;
			} else {
				throw new NotImplementedException("Writing is not yet supported by CSerializerObjectTagBinary");
			}
		}
		public CSerializerObjectTagBinary(Context context, BinaryFile file) : this(context, file.CreateReader()) {
			File = file;
		}

		public override SerializerProperties Properties => base.Properties | SerializerProperties.Binary | SerializerProperties.NoRaw; // 0x81

		public BinaryFile File { get; protected set; }
		public Reader Reader { get; protected set; }
		public override Pointer CurrentPointer => new Pointer(CurrentPosition, File);
		public override long CurrentPosition => Reader.BaseStream.Position;
		public override long Length => Reader.BaseStream.Length;

		public override void Dispose() {
			base.Dispose();
			Reader?.Dispose();
		}

		public override void Goto(long position) {
			if (!Disposed) Reader.BaseStream.Position = position;
		}



		protected object FakeSerializePrimitive<T>(T t, string name = null) {
			if (t != null) return t;
			// Get the type
			var type = typeof(T);

			TypeCode typeCode = Type.GetTypeCode(type);

			if (typeCode == TypeCode.Object) {
				if (type == typeof(CString)) {
					return new CString();
				} else if (type == typeof(BasicString)) {
					return new BasicString();
				} else if (type == typeof(byte[])) {
					return new byte[0];
				} else {
					throw new Exception(CurrentPointer + ": Field with name " + name + " is not a valid primitive type");
				}
			}

			switch (typeCode) {
				case TypeCode.Boolean: return default(bool);
				case TypeCode.SByte: return default(sbyte);
				case TypeCode.Byte: return default(byte);
				case TypeCode.Int16: return default(short);
				case TypeCode.UInt16: return default(ushort);
				case TypeCode.Int32: return default(int);
				case TypeCode.UInt32: return default(uint);
				case TypeCode.Int64: return default(long);
				case TypeCode.UInt64: return default(ulong);
				case TypeCode.Single: return default(float);
				case TypeCode.Double: return default(double);
				case TypeCode.String: return "";
				case TypeCode.Char: return default(char);
				default: throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
			}
		}
		protected T FakeSerializeObject<T>(T t, string name, Type objType) where T : ICSerializable, new() {
			if (t != null) return t;
			if (objType == typeof(Vec2d)) {
				t = (T)(object)Vec2d.Zero;
			} else if (objType == typeof(Vec3d)) {
				t = (T)(object)Vec3d.Zero;
			} else if (objType == typeof(Vec4d)) {
				t = (T)(object)Vec4d.Zero;
			} else if (objType == typeof(Color)) {
				t = (T)(object)Color.Zero;
			} else {
				if (t == null) t = new T();
				IncreaseLevel();
				t.Serialize(this, name);
				DecreaseLevel();
			}
			return t;
		}
		protected T SerializeObjectFull<T>(T obj, string name, Type objType) where T : ICSerializable, new() {
			if (fakeSerializeMode) {
				return FakeSerializeObject<T>(obj, name, objType);
			}
			if (objType == typeof(Vec2d)) {
				obj = (T)(object)new Vec2d(Reader.ReadSingle(), Reader.ReadSingle());
			} else if (objType == typeof(Vec3d)) {
				obj = (T)(object)new Vec3d(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
			} else if (objType == typeof(Vec4d)) {
				obj = (T)(object)new Vec4d(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
			} else if (objType == typeof(Color)) {
				float b = Reader.ReadSingle();
				float g = Reader.ReadSingle();
				float r = Reader.ReadSingle();
				float a = Reader.ReadSingle();
				obj = (T)(object)new Color(r,g,b,a);
			} else {
				if (obj == null) obj = new T();
				IncreaseLevel();
				bool entered = false;
				if (GetTagCode(objType) == 200 && !objType.IsDefined(typeof(SerializeEmbedAttribute), false)) {
					string typeName;
					if (obj is CSerializable) {
						typeName = ((CSerializable)(ICSerializable)obj).ClassName;
					} else {
						typeName = objType.Name;
						if (typeName.Contains('`')) typeName = typeName.Substring(0, typeName.IndexOf("`"));
					}
					entered = ReadTag(typeName, 200);
				}
				obj.Serialize(this, name);
				if (IsSerializerLoggerEnabled && entered && endPos.Peek() != Reader.BaseStream.Position) {
					Context.SerializerLogger.Log($"{LogPrefix}ERROR: NOT FULLY READ");
				}
				if (entered) Reader.BaseStream.Position = endPos.Pop();
				DecreaseLevel();
				Context.AddToStringCache(obj);
			}
			return obj;
		}
		// Helper method which returns an object so we can cast it
		protected object ReadPrimitiveAsObject<T>(T t, string name = null, Options options = Options.None) {
			if (fakeSerializeMode) {
				return FakeSerializePrimitive(t, name: name);
			}
			// Get the type
			var type = typeof(T);

			TypeCode typeCode = Type.GetTypeCode(type);

			if (typeCode == TypeCode.Object) {
				if (type == typeof(CString)) {
					CString s = new CString(Reader.ReadString16());
					Context.AddToStringCache(s);
					return s;
				} else if (type == typeof(BasicString)) {
					BasicString s = new BasicString(Reader.ReadString8());
					Context.AddToStringCache(s);
					return s;
				} else if (type == typeof(byte[])) {
					int numBytes = Reader.ReadInt32();
					return Reader.ReadBytes(numBytes);
				} else {
					throw new Exception(CurrentPointer + ": Field with name " + name + " is not a valid primitive type");
				}
			}

			switch (typeCode) {

				case TypeCode.Boolean:
					bool asByte = false;
					if (options.HasFlag(Options.ForceAsByte)) {
						asByte = true;
					} else if (options.HasFlag(Options.BoolAsByte)) {
						asByte = HasFlags(SerializeFlags.PropertyEdit_Save);
					}
					if (asByte) {
						uint tmp = Reader.ReadByte();
						if (tmp == 1) {
							return true;
						} else if (tmp != 0) {
							Context?.SystemLogger?.LogWarning(CurrentPointer + ": Bool with name " + name + " was " + tmp + "!");
							return true;
						} else {
							return false;
						}
					} else {
						uint tmp = Reader.ReadUInt32();
						if (tmp == 1) {
							return true;
						} else if (tmp != 0) {
							Context?.SystemLogger?.LogWarning(CurrentPointer + ": Bool with name " + name + " was " + tmp + "!");
							return true;
						} else {
							return false;
						}
					}

				case TypeCode.SByte:
					return Reader.ReadSByte();

				case TypeCode.Byte:
					return Reader.ReadByte();

				case TypeCode.Int16:
					return Reader.ReadInt16();

				case TypeCode.UInt16:
					return Reader.ReadUInt16();

				case TypeCode.Int32:
					return Reader.ReadInt32();

				case TypeCode.UInt32:
					return Reader.ReadUInt32();

				case TypeCode.Int64:
					return Reader.ReadInt64();

				case TypeCode.UInt64:
					return Reader.ReadUInt64();

				case TypeCode.Single:
					return Reader.ReadSingle();

				case TypeCode.Double:
					return Reader.ReadDouble();
				case TypeCode.String:
					string s = Reader.ReadString8();
					Context.AddToStringCache(s);
					return s;
				case TypeCode.Char:
					return Reader.ReadChar();
				default:
					throw new NotSupportedException($"The specified generic type ('{name}') can not be read from the reader");
			}
		}

		public override object Serialize(object obj, Type type, string name = null, Options options = Options.None) {
			if (fakeSerializeMode) {
				return FakeSerialize(obj, type, name: name);
			}
			if (Type.GetTypeCode(type) != TypeCode.Object) {
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean:
						uint tmp = Reader.ReadUInt32();
						if (tmp == 1) {
							obj = true;
						} else if (tmp != 0) {
							Context?.SystemLogger?.LogWarning(CurrentPointer + ": Bool with name " + name + " was " + tmp + "!");
							obj = true;
						} else {
							obj = false;
						}
						//obj = (object)reader.ReadBoolean();
						break;
					case TypeCode.Byte: obj = (object)Reader.ReadByte(); break;
					case TypeCode.Char: obj = (object)Reader.ReadChar(); break;
					case TypeCode.String: obj = (object)Reader.ReadString8(); Context.AddToStringCache(obj); break;
					case TypeCode.Single: obj = (object)Reader.ReadSingle(); break;
					case TypeCode.Double: obj = (object)Reader.ReadDouble(); break;
					case TypeCode.UInt16: obj = (object)Reader.ReadUInt16(); break;
					case TypeCode.UInt32: obj = (object)Reader.ReadUInt32(); break;
					case TypeCode.UInt64: obj = (object)Reader.ReadUInt64(); break;
					case TypeCode.Int16: obj = (object)Reader.ReadInt16(); break;
					case TypeCode.Int32: obj = (object)Reader.ReadInt32(); break;
					case TypeCode.Int64: obj = (object)Reader.ReadInt64(); break;
					default: throw new Exception("Unsupported TypeCode " + Type.GetTypeCode(type));
				}
			} else if (type == typeof(CString)) {
				obj = new CString(Reader.ReadString16());
				Context.AddToStringCache(obj);
			} else if (type == typeof(BasicString)) {
				obj = new BasicString(Reader.ReadString8());
				Context.AddToStringCache(obj);
			} else if (type == typeof(byte[])) {
				int numBytes = Reader.ReadInt32();
				obj = Reader.ReadBytes(numBytes);
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
					bool entered = false;
					if (!IsValueType(type)
						&& !type.IsDefined(typeof(SerializeEmbedAttribute), false)) {
						string typeName;
						if (obj is CSerializable) {
							typeName = ((CSerializable)obj).ClassName;
						} else {
							typeName = type.Name;
							if (typeName.Contains('`')) typeName = typeName.Substring(0, typeName.IndexOf("`"));
						}
						entered = ReadTag(typeName, 200);
					}
					((ICSerializable)obj).Serialize(this, name);
					if (IsSerializerLoggerEnabled && entered && endPos.Peek() != Reader.BaseStream.Position) {
						Context.SerializerLogger.Log($"{LogPrefix}ERROR: NOT FULLY READ");
					}
					if (entered) Reader.BaseStream.Position = endPos.Pop();
					DecreaseLevel();
					Context.AddToStringCache(obj);
				}
			}
			return obj;
		}

		public object FakeSerialize(object obj, Type type, string name = null) {
			if (obj != null) {
				return obj;
			}
			if (Type.GetTypeCode(type) != TypeCode.Object) {
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean: obj = default(bool); break;
					case TypeCode.Byte: obj = default(byte); break;
					case TypeCode.Char: obj = default(char); break;
					case TypeCode.String: obj = ""; break;
					case TypeCode.Single: obj = default(float); break;
					case TypeCode.Double: obj = default(double); break;
					case TypeCode.UInt16: obj = default(ushort); break;
					case TypeCode.UInt32: obj = default(uint); break;
					case TypeCode.UInt64: obj = default(ulong); break;
					case TypeCode.Int16: obj = default(short); break;
					case TypeCode.Int32: obj = default(int); break;
					case TypeCode.Int64: obj = default(long); break;
					default: throw new Exception("Unsupported TypeCode " + Type.GetTypeCode(type));
				}
			} else if (type == typeof(Vec2d)) {
				obj = default(Vec2d);
			} else if (type == typeof(Vec3d)) {
				obj = default(Vec3d);
			} else if (type == typeof(Vec4d)) {
				obj = default(Vec4d);
			} else if (type == typeof(Color)) {
				obj = default(Color);
			} else if (type == typeof(CString)) {
				obj = new CString();
			} else if (type == typeof(BasicString)) {
				obj = new BasicString();
			} else if (type == typeof(byte[])) {
				obj = new byte[0];
			} else {
				var ctor = type.GetConstructor(Type.EmptyTypes);
				if (ctor == null) {
					throw new Exception("Constructor is null");
				}
				obj = ctor.Invoke(new object[] { });
				if (obj is ICSerializable) {
					IncreaseLevel();
					((ICSerializable)obj).Serialize(this, name);
					DecreaseLevel();
				}
			}
			return obj;
		}

		public override T SerializeGeneric<T>(T obj, Type type = null, string name = null, int? index = null) {
			/*Pointer pos = log && index.HasValue ? Position : null;
			bool isBigObject = log && index.HasValue && (typeof(CSerializable).IsAssignableFrom(typeof(T)) || typeof(IObjectContainer).IsAssignableFrom(typeof(T)));
			if (log && index.HasValue && isBigObject) {
				Context.Log(pos + ":" + new string(' ', (Indent + 1) * 2) + name + "[" + index.Value + "]:");
			}*/
			long position = Reader.BaseStream.Position;
			Type objType = type ?? typeof(T);
			if (endPos.Count > 0 && position >= endPos.Peek()) {
				if (obj == null) {
					bool prevFakeSerializeMode = fakeSerializeMode;
					fakeSerializeMode = true;
					object obj2 = Serialize(null, objType, name: name);
					if(type != null) ConvertTypeAfter(ref obj2, name, type, typeof(T));
					obj = (T)obj2;
					fakeSerializeMode = prevFakeSerializeMode;
				}
			} else if (name != null && !objType.IsDefined(typeof(SerializeEmbedAttribute), false)) {
				uint uafType = index.HasValue ? 200 + (uint)index.Value : GetTagCode(objType);
				if (ReadTag(name, uafType)) {
					var logPrefix = LogPrefix;

					bool isBigObject = IsBigObject(objType) && name != null;
					bool isLogTemporarilyDisabled = IsShortLog(objType);

					if (!fakeSerializeMode && IsSerializerLoggerEnabled && name != null && isBigObject) {
						Context.SerializerLogger.Log($"{logPrefix}({objType}) {name}:");
					}

					try {
						object obj2 = Serialize(obj, objType, name: name);
						if (type != null) ConvertTypeAfter(ref obj2, name, type, typeof(T));
						obj = (T)obj2;
					} finally {
						if (isLogTemporarilyDisabled) DisableSerializerLoggerForObject = false;
						if (IsSerializerLoggerEnabled && endPos.Peek() != Reader.BaseStream.Position) {
							Context.SerializerLogger.Log($"{LogPrefix}ERROR: NOT FULLY READ");
						}
						Reader.BaseStream.Position = endPos.Pop();
						if (!fakeSerializeMode && IsSerializerLoggerEnabled && name != null && !isBigObject) {
							Context.SerializerLogger.Log($"{logPrefix}({objType}) {name} - {ShortLog(obj)}");
						}
					}

				} else {
					if (obj == null) {
						bool prevFakeSerializeMode = fakeSerializeMode;
						fakeSerializeMode = true;
						object obj2 = Serialize(null, objType, name: name);
						if (type != null) ConvertTypeAfter(ref obj2, name, type, typeof(T));
						obj = (T)obj2;
						fakeSerializeMode = prevFakeSerializeMode;
					}
				}
			} else {
				var logPrefix = LogPrefix;
				bool isBigObject = IsBigObject(objType);
				bool isLogTemporarilyDisabled = IsShortLog(objType);
				if (!fakeSerializeMode && IsSerializerLoggerEnabled && index.HasValue && isBigObject) {
					Context.SerializerLogger.Log($"{logPrefix}({objType}) {name}[{index.Value}]:");
				}

				try {
					object obj2 = Serialize(obj, objType, name: name);
					if (type != null) ConvertTypeAfter(ref obj2, name, type, typeof(T));
					obj = (T)obj2;
				} finally {
					if (isLogTemporarilyDisabled) DisableSerializerLoggerForObject = false;
					if (!fakeSerializeMode && IsSerializerLoggerEnabled && index.HasValue && !isBigObject) {
						Context.SerializerLogger.Log($"{logPrefix}({objType}) {name}[{index.Value}] - {obj}");
					}
				}
			}
			return obj;

			/*if (log && index.HasValue && !isBigObject) {
				Context.Log(pos + ":" + new string(' ', (Indent + 1) * 2) + name + "[" + index.Value + "] - " + obj);
			}*/
		}

		public override byte[] SerializeBytes(byte[] obj, long count) {
			if (fakeSerializeMode) return obj;
			return Reader.ReadBytes((int)count);
		}

		public override bool ArrayEntryStart(string name, int index) {
			if (ReadTag(name, (uint)(200 + index))) {
				if (IsSerializerLoggerEnabled) {
					Context.SerializerLogger.Log($"{LogPrefix}{name}[{index}]:");
				}
				return base.ArrayEntryStart(name, index);
			} else return false;
		}

		public override void ArrayEntryStop() {
			base.ArrayEntryStop();

			if (IsSerializerLoggerEnabled && endPos.Peek() != Reader.BaseStream.Position) {
				Context.SerializerLogger.Log($"{LogPrefix}ERROR: NOT FULLY READ");
			}
			Reader.BaseStream.Position = endPos.Pop();
		}

		protected bool ReadTag(string name, uint type) {
			if (fakeSerializeMode) return false;
			if(CurrentPosition >= Length) return false;
			UAFInfo info = new UAFInfo {
				name = name,
				type = type
			};
			var logPrefix = LogPrefix;
			uint crc = Reader.ReadUInt32();
			uint size = Reader.ReadUInt32();
			uint requiredCRC = 0;
			if (fieldCRC.ContainsKey(info)) {
				requiredCRC = fieldCRC[info];
			} else {
				requiredCRC = GetCRC(name, type);
				fieldCRC[info] = requiredCRC;
			}
			if (crc == requiredCRC) {
				//if (IsSerializerLoggerEnabled) Context.SerializerLogger.Log($"{logPrefix}TAG: {name} ({type})");
				endPos.Push(CurrentPosition + size);
				return true;
			} else {
				//if (IsSerializerLoggerEnabled) Context.SerializerLogger.Log($"{logPrefix}Read tag failed: {name} ({type}) (Read:{crc:X8} != Tried:{requiredCRC:X8})");
				Reader.BaseStream.Seek(-8, System.IO.SeekOrigin.Current);
				return false;
			}
		}

		protected uint GetCRC(string str, uint? classTypeInt) {
			byte[] stringBytes = ASCIIEncoding.ASCII.GetBytes(str);
			Crc crc = new Crc(new Parameters("CRC-32", 32, 0x04C11DB7, 0xFFFFFFFF, false, false, 0xFFFFFFFF, 0xCBF43926));
			uint computedCRC = CrcHelper.ToUInt32(crc.ComputeHash(stringBytes));
			if (classTypeInt.HasValue) {
				byte[] classType = BitConverter.GetBytes(classTypeInt.Value);
				crc = new Crc(new Parameters("CRC-32", 32, 0x04C11DB7, computedCRC, false, false, 0xFFFFFFFF, 0xCBF43926));
				return CrcHelper.ToUInt32(crc.ComputeHash(classType));
			} else {
				return computedCRC;
			}
		}

		public override T SerializeGenericPureBinary<T>(T obj, Type type = null, string name = null, int? index = null) {
			var logPrefix = LogPrefix;

			var usedType = type ?? typeof(T);
			bool isBigObject = IsBigObject(usedType);
			bool isLogTemporarilyDisabled = IsShortLog(usedType);

			if (IsSerializerLoggerEnabled && name != null && isBigObject) {
				Context.SerializerLogger.Log($"{logPrefix}({usedType}) {name}:");
			}

			try {
				object obj2 = Serialize(obj, usedType, name: name);
				obj = (T)obj2;
			} finally {
				if (isLogTemporarilyDisabled) DisableSerializerLoggerForObject = false;
				if (IsSerializerLoggerEnabled && name != null && !isBigObject) {
					Context.SerializerLogger.Log($"{logPrefix}({usedType}) {name} - {ShortLog(obj)}");
				}
			}
			return obj;
		}

		public override T Serialize<T>(T obj, string name = null, int? index = null, Options options = Options.None) {
			//return SerializeGeneric<T>(obj, name: name, index: index);

			long position = Reader.BaseStream.Position;
			if (endPos.Count > 0 && position >= endPos.Peek()) {
				if (obj == null) {
					bool prevFakeSerializeMode = fakeSerializeMode;
					fakeSerializeMode = true;
					obj = (T)ReadPrimitiveAsObject<T>(obj, name: name, options: options);
					fakeSerializeMode = prevFakeSerializeMode;
				}
			} else if (name != null) {
				Type objType = typeof(T);
				uint uafType = index.HasValue ? 200 + (uint)index.Value : GetTagCode(objType);
				if (ReadTag(name, uafType)) {
					var logPrefix = LogPrefix;
					obj = (T)ReadPrimitiveAsObject<T>(obj, name: name, options: options);

					if (IsSerializerLoggerEnabled && endPos.Peek() != Reader.BaseStream.Position) {
						Context.SerializerLogger.Log($"{LogPrefix}ERROR: NOT FULLY READ");
					}
					Reader.BaseStream.Position = endPos.Pop();
					if (!fakeSerializeMode && IsSerializerLoggerEnabled && name != null) {
						Context.SerializerLogger.Log($"{logPrefix}({objType}) {name} - {ShortLog(obj)}");
					}
				} else {
					if (obj == null) {
						bool prevFakeSerializeMode = fakeSerializeMode;
						fakeSerializeMode = true;
						obj = (T)ReadPrimitiveAsObject<T>(obj, name: name, options: options);
						fakeSerializeMode = prevFakeSerializeMode;
					}
				}
			} else {
				var logPrefix = LogPrefix;
				obj = (T)ReadPrimitiveAsObject<T>(obj, name: name, options: options);
				if (!fakeSerializeMode && IsSerializerLoggerEnabled && name != null) {
					Type objType = typeof(T);
					Context.SerializerLogger.Log($"{logPrefix}({objType}) {name} - {ShortLog(obj)}");
				}
			}
			return obj;
		}

		public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null, int? index = null, Options options = Options.None) {
			//return SerializeGeneric<T>(obj, name: name, index: index);
			long position = Reader.BaseStream.Position;
			Type objType = typeof(T);
			if (endPos.Count > 0 && position >= endPos.Peek()) {
				if (obj == null) {
					bool prevFakeSerializeMode = fakeSerializeMode;
					fakeSerializeMode = true;
					obj = SerializeObjectFull<T>(obj, name: name, objType);
					fakeSerializeMode = prevFakeSerializeMode;
				}
			} else if (name != null && !objType.IsDefined(typeof(SerializeEmbedAttribute), false)) {
				uint uafType = index.HasValue ? 200 + (uint)index.Value : GetTagCode(objType);
				/*if (name == "collisionData" && log) {
					Pointer pos = log ? new Pointer(position, Context.GetFileByReader(reader)) : null;
					Context.Log(pos + ":" + new string(' ', (Indent + 1) * 2) + "(" + typeof(T) + ") " + name + ":" + " - " + index + " - " + objType + " - " + uafType);
				}*/
				if (ReadTag(name, uafType)) {
					var logPrefix = LogPrefix;
					bool isBigObject = IsBigObject(objType);
					bool isLogTemporarilyDisabled = IsShortLog(objType);

					if (!fakeSerializeMode && IsSerializerLoggerEnabled && isBigObject && name != null) {
						Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name}:");
					}

					try {
						obj = SerializeObjectFull<T>(default, name, objType);
					} finally {
						if (IsSerializerLoggerEnabled && endPos.Peek() != Reader.BaseStream.Position) {
							Context.SerializerLogger.Log($"{LogPrefix}ERROR: NOT FULLY READ");
						}
						Reader.BaseStream.Position = endPos.Pop();

						if (isLogTemporarilyDisabled) DisableSerializerLoggerForObject = false;
						if (!fakeSerializeMode && IsSerializerLoggerEnabled && !isBigObject && name != null) {
							Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name} - {ShortLog(obj)}");
						}
					}
				} else {
					if (obj == null) {
						bool prevFakeSerializeMode = fakeSerializeMode;
						fakeSerializeMode = true;
						obj = SerializeObjectFull<T>(obj, name: name, objType);
						fakeSerializeMode = prevFakeSerializeMode;
					}
				}
			} else {
				var logPrefix = LogPrefix;
				bool isBigObject = IsBigObject(objType);
				bool isLogTemporarilyDisabled = IsShortLog(objType);

				if (!fakeSerializeMode && IsSerializerLoggerEnabled && isBigObject && name != null) {
					Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name}:");
				}

				try {
					obj = SerializeObjectFull<T>(obj, name, objType);
				} finally {
					if (isLogTemporarilyDisabled) DisableSerializerLoggerForObject = false;
					if (!fakeSerializeMode && IsSerializerLoggerEnabled && !isBigObject && name != null) {
						Context.SerializerLogger.Log($"{logPrefix}({typeof(T)}) {name} - {ShortLog(obj)}");
					}
				}
			}
			return obj;
		}

		#region Logging

		protected string LogPrefix => IsSerializerLoggerEnabled ? $"(R) {CurrentPointer}:{new string(' ', (Depth + 1) * 2)}" : null;
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
			long start = Reader.BaseStream.Position;
			using MemoryStream memStream = new();
			encoder.DecodeStream(Reader.BaseStream, memStream);
			memStream.Position = 0;
			long end = Reader.BaseStream.Position;
			long encodedLength = end - start;

			// Add the stream
			BinaryStreamFile sf = new(
				context: Context,
				name: key,
				stream: memStream,
				endianness: endianness ?? File.Endianness);

			var reader = Reader;
			var file = File;

			var tuple = new Tuple<string, UbiArtFile>(key, sf);

			try {
				File = sf;
				Reader = sf.CreateReader();
				long decodedLength = Reader.BaseStream.Length;
				Context.Loader.virtualFiles.Add(tuple);

				//var test_export_path = $"{Context.BasePath}\\test_new_{encoder.Name}.lol";
				//byte[] bytes = this.Reader.ReadBytes((int)decodedLength);
				//Util.ByteArrayToFile(test_export_path, bytes);
				//this.Reader.BaseStream.Position = 0;


				//DoAt(sf.StartPointer, () => {
				Log("Decoded data using {0} at {1} with decoded length {2} and encoded length {3}",
					encoder.Name, offset, decodedLength, encodedLength);
				action();
				if (CurrentPosition != decodedLength)
					Context?.SystemLogger?.LogWarning("Encoded block {0} was not fully deserialized: Serialized size: {1} != Total size: {2}",
						key, CurrentPointer, decodedLength);
				//});
			} finally {
				Reader = reader;
				File = file;
				Context.Loader.virtualFiles.Remove(tuple);
			}
		}
		public override void DoEncrypted(uint[] encryptionKey, Action action, string name = null) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			if(encryptionKey == null)
				throw new ArgumentNullException(nameof(encryptionKey));

			Crc crc = new Crc(new Parameters("CRC-32", 32, 0x04C11DB7, 0xFFFFFFFF, false, false, 0xFFFFFFFF, 0xCBF43926));
			byte[] encryptionKeyBytes = new byte[encryptionKey.Length * 4];
			for (int i = 0; i < encryptionKey.Length; i++) {
				encryptionKeyBytes[(i * 4) + 0] = (byte)((encryptionKey[i] >>  0) & 0xFF);
				encryptionKeyBytes[(i * 4) + 1] = (byte)((encryptionKey[i] >>  8) & 0xFF);
				encryptionKeyBytes[(i * 4) + 2] = (byte)((encryptionKey[i] >> 16) & 0xFF);
				encryptionKeyBytes[(i * 4) + 3] = (byte)((encryptionKey[i] >> 24) & 0xFF);
			}
			uint computedCRC = CrcHelper.ToUInt32(crc.ComputeHash(encryptionKeyBytes));
			uint uafType = computedCRC;
			if (ReadTag(name, uafType)) {
				var end = endPos.Peek();
				var len = end - CurrentPosition;
				DoEncoded(new TeaEncoder(len, encryptionKey), () => {
					// Stored as Base64.
					DoEncoded(new Base64Encoder(Length), () => {
						action();
					});
				});
			}
		}
		public override void DoCompressed(Action action,
			long? compressedSize = null, long? decompressedSize = null, string name = null) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			if (ReadTag(name, 300)) {
				var end = endPos.Peek();
				var endian = Reader.IsLittleEndian;
				Reader.IsLittleEndian = true;
				var decompLen = Reader.ReadUInt32();
				Reader.IsLittleEndian = endian;
				var len = end - CurrentPosition;
				DoEncoded(new ZlibEncoder((uint)len, decompLen), () => {
					action();
				});
			}
		}


		public override void DoEndian(Action action, Endian endianness) {
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			var curEndian = Reader.IsLittleEndian;
			try {
				Reader.IsLittleEndian = endianness == Endian.Little;
				action();
			} finally {
				Reader.IsLittleEndian = curEndian;
			}
		}
		#endregion
	}
}
