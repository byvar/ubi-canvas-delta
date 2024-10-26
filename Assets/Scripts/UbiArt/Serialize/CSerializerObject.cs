using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UbiArt {
	public abstract class CSerializerObject : IDisposable {
		public SerializeFlags flags;
		public virtual SerializerProperties Properties { get; }
		protected int indent;
		public int Depth => indent;
		public bool Disposed { get; set; }

		public Context Context { get; set; }
		public Settings Settings => Context?.Settings;
		public virtual bool IsCustomSerializer => false;

		public CSerializerObject(Context context) {
			Context = context;
		}


		#region Serializer Logging

		/// <summary>
		/// Indicates if logging is enabled for the serialization
		/// </summary>
		public bool IsSerializerLoggerEnabled => Context.SerializerLogger.IsEnabled && !DisableSerializerLoggerForObject;

		/// <summary>
		/// Writes a line to the serializer log, if enabled
		/// </summary>
		/// <param name="logString">The string to log</param>
		/// <param name="args">The log string arguments</param>
		[StringFormatMethod("logString")]
		public abstract void Log(string logString, params object[] args);

		protected bool DisableSerializerLoggerForObject { get; set; }

		protected bool IsShortLog(Type type) {
			if (IsSerializerLoggerEnabled && (typeof(ICSerializableShortLog).IsAssignableFrom(type))) {
				DisableSerializerLoggerForObject = true;
				return true;
			}
			return false;
		}
		protected void EndShortLog(bool wasDisabled) {
			if (wasDisabled) DisableSerializerLoggerForObject = false;
		}
		protected string ShortLog<T>(T t) => (t as ICSerializableShortLog)?.SerializeLog(this) ?? t?.ToString() ?? "null";
		protected bool IsBigObject(Type type) => IsSerializerLoggerEnabled
				&& (typeof(CSerializable).IsAssignableFrom(type) || typeof(IObjectContainer).IsAssignableFrom(type))
				&& !typeof(ICSerializableShortLog).IsAssignableFrom(type);

		#endregion

		#region Encoding
		protected abstract void DoEncoded(
			IStreamEncoder encoder,
			Action action,
			Endian? endianness = null,
			string filename = null);

		public abstract void DoEncrypted(
			uint[] encryptionKey,
			Action action,
			string name = null);

		public abstract void DoCompressed(
			Action action,
			string name = null);

		public abstract void DoEndian(
			Action action,
			Endian endianness);

		#endregion

		#region Object Size
		public virtual void InitSerializer() { }
		public virtual void SerializeObjectSize(Type type) { }
		public virtual void IncreaseMemCount(Type type, uint count = 1, uint? customSize = null) { }
		public virtual void CloseSerializer() { }
		public virtual uint GetMemorySize(Type type) {
			if (HasProperties(SerializerProperties.LIP)
				&& Settings.EngineVersion > EngineVersion.RO) {
				uint sizeOf = 0;

				if (Settings.MemoryData != null) {
					if (Settings.MemoryData.TryGetSize(type, out var storedSize)) {
						sizeOf = storedSize;
					}
				}
				return sizeOf;
			}
			return 0;
		}
		#endregion

		public abstract object Serialize(object obj, Type type, string name = null, Options options = Options.None);
		
		public abstract T Serialize<T>(T obj, string name = null, int? index = null, Options options = Options.None);
		public abstract T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null, int? index = null, Options options = Options.None) where T : ICSerializable, new();
		
		public virtual T SerializeChoiceList<T>(T obj, string name = null, Options options = Options.None, string empty = null, List<Tuple<string, StringID>> choices = null) {
			return Serialize<T>(obj, name: name, options: options);
		}
		public virtual T SerializeChoiceListObject<T>(T obj, Action<T> onPreSerialize = null, string name = null, Options options = Options.None, string empty = null, List<Tuple<string, StringID>> choices = null) where T : ICSerializable, new() {
			return SerializeObject<T>(obj, onPreSerialize: onPreSerialize, name: name, options: options);
		}

		public virtual void Goto(long position) { }
		public virtual async Task FillCacheForRead(long byteCount) { await Task.CompletedTask; }

		public abstract T SerializeGeneric<T>(T obj, Type type = null, string name = null, int? index = null);
		public abstract T SerializeGenericPureBinary<T>(T obj, Type type = null, string name = null, int? index = null);
		public abstract byte[] SerializeBytes(byte[] obj, long count);
		public abstract Pointer CurrentPointer { get; }
		public abstract long CurrentPosition { get; }
		public abstract long Length { get; }


		public virtual void ResetPosition() => Goto(0);
		protected void IncreaseLevel() {
			indent++;
		}
		protected void DecreaseLevel() {
			indent--;
		}
		public virtual bool ArrayEntryStart(string name, int index) {
			IncreaseLevel();
			return true;
		}
		public virtual void ArrayEntryStop() {
			DecreaseLevel();
		}

		public virtual bool SerializeEditorButton(string name) {
			return false;
		}

		public uint GetTagCode(Type type) {
			if (Type.GetTypeCode(type) != TypeCode.Object) {
				switch (Type.GetTypeCode(type)) {
					case TypeCode.Boolean: return (uint)UAFTag.Bool;
					case TypeCode.Byte: return (uint)UAFTag.UChar;
					case TypeCode.Char: return (uint)UAFTag.Char;
					case TypeCode.String: return (uint)UAFTag.String8;
					case TypeCode.Single: return (uint)UAFTag.Float;
					case TypeCode.Double: return (uint)UAFTag.Double;
					case TypeCode.UInt16: return (uint)UAFTag.UShort;
					case TypeCode.UInt32: return (uint)UAFTag.UInt;
					case TypeCode.UInt64: return (uint)UAFTag.Long;
					case TypeCode.Int16: return (uint)UAFTag.Short;
					case TypeCode.Int32: return (uint)UAFTag.Int;
					case TypeCode.Int64: return (uint)UAFTag.UInt;
					default: throw new Exception("Unsupported TypeCode " + Type.GetTypeCode(type));
				}
			} else if (typeMapping.ContainsKey(type)) {
				return (uint)typeMapping[type];
			} else {
				return 200;
			}
		}
		public bool IsValueType(Type type) {
			if(Type.GetTypeCode(type) != TypeCode.Object)
				return true;
			if (typeMapping.ContainsKey(type))
				return true;
			// Add specific checks here
			if(type == typeof(ITF.ObjectPath))
				return true;

			return false;
		}

		public bool HasFlags(SerializeFlags flags) {
			switch (flags) {
				case SerializeFlags.Deprecate:
					if ((this.Properties & (SerializerProperties.BinSkip | SerializerProperties.NoDeprecate)) == SerializerProperties.None) {
						return ((this.flags & SerializeFlags.Data_Load) != SerializeFlags.None);
					}
					return false;
				case SerializeFlags.DataRaw:
					if ((this.Properties & (SerializerProperties.BinSkip | SerializerProperties.NoRaw)) == SerializerProperties.None) {
						return ((this.flags & SerializeFlags.Group_DataEditable) != SerializeFlags.None);
					}
					return false;
				case SerializeFlags.DataBin:
					return (((this.Properties & SerializerProperties.Binary) != SerializerProperties.None) &&
						((this.flags & SerializeFlags.Group_Data) != SerializeFlags.None));
				default:
					return ((this.flags & flags) != SerializeFlags.None);
			}
		}
		public bool HasProperties(SerializerProperties properties) {
			return ((this.Properties & properties) != SerializerProperties.None);
		}

		protected void ConvertTypeBefore(ref object obj, string name, Type type, Type fieldType) {
			if (type == typeof(byte) && fieldType == typeof(bool)) {
				if (((bool)obj) == true) {
					obj = (byte)1;
				} else {
					obj = (byte)0;
				}
			} else if (type == typeof(uint) && fieldType == typeof(ObjectRef)) {
				ObjectRef objref = ((ObjectRef)obj);
				obj = objref == null ? (uint)0xFFFFFFFF : objref.objectRef;
			} else if (type == typeof(float) && fieldType == typeof(Angle)) {
				Angle a = ((Angle)obj);
				obj = a == null ? 0 : a.angle;
			} else if (type == typeof(PathRef) && fieldType == typeof(Path)) {
				Path p = ((Path)obj);
				obj = ((PathRef)p);
			}
		}

		protected void ConvertTypeAfter(ref object obj, string name, Type type, Type fieldType) {
			if (type == typeof(byte) && fieldType == typeof(bool)) {
				if (((byte)obj) == 1) {
					obj = true;
				} else if (((byte)obj) != 0) {
					throw new Exception(CurrentPointer + ": BoolAsByte with name " + name + " was " + ((byte)obj) + "!");
					//obj = false;
				} else {
					obj = false;
				}
			} else if (type == typeof(uint) && fieldType == typeof(ObjectRef)) {
				obj = (ObjectRef)(uint)obj;
			} else if (type == typeof(float) && fieldType == typeof(Angle)) {
				obj = (Angle)(float)obj;
			} else if (type == typeof(PathRef) && fieldType == typeof(Path)) {
				obj = (Path)(PathRef)obj;
			}
		}

		public virtual void Dispose() {
			Disposed = true;
		}

		[Flags]
		public enum Options {
			None		= 0,
			BoolAsByte	= 1,
			ForceAsByte = 1 << 1,
			DontIncreaseMemCount = 1 << 2,
			UTF8        = 1 << 3
		}

		public enum UAFTag {
			Char = 0,
			UChar,
			Short,
			Int,
			UShort,
			UInt,
			Long,
			Float,
			Double,
			Bool,
			Vec2d,
			Vec3d,
			Matrix44,
			Unk13,
			Unk14,
			Color,
			ColorInteger,
			Angle,
			Volume,
			Path,
			StringID,
			String8,
			LocalisationID
		}
		public struct UAFInfo {
			public uint type;
			public string name;
		}

		protected Dictionary<Type, UAFTag> typeMapping = new Dictionary<Type, UAFTag> {
			{ typeof(char),           UAFTag.Char           },
			{ typeof(byte),           UAFTag.UChar          },
			{ typeof(short),          UAFTag.Short          },
			{ typeof(int),            UAFTag.Int            },
			{ typeof(ushort),         UAFTag.UShort         },
			{ typeof(uint),           UAFTag.UInt           },
			{ typeof(long),           UAFTag.Long           },
			{ typeof(ulong),          UAFTag.Long           },
			{ typeof(float),          UAFTag.Float          },
			{ typeof(double),         UAFTag.Double         },
			{ typeof(bool),           UAFTag.Bool           },
			{ typeof(Vec2d),          UAFTag.Vec2d          },
			{ typeof(Vec3d),          UAFTag.Vec3d          },
			{ typeof(Matrix44),       UAFTag.Matrix44       },
			{ typeof(Color),          UAFTag.Color          },
			{ typeof(ColorInteger),   UAFTag.ColorInteger   },
			{ typeof(ObjectRef),      UAFTag.UInt           },
			{ typeof(Angle),          UAFTag.Angle          },
			{ typeof(AngleAmount),    UAFTag.Angle          },
			{ typeof(Volume),         UAFTag.Volume         },
			{ typeof(Path),           UAFTag.Path           },
			{ typeof(PathRef),        UAFTag.Path           },
			{ typeof(StringID),       UAFTag.StringID       },
			{ typeof(string),         UAFTag.String8        },
			{ typeof(LocalisationId), UAFTag.LocalisationID },
		};
	}
}
