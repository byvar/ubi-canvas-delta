using System;
using UbiArt.ITF;

namespace UbiArt {
	public class Generic<T> : ICSerializable, IObjectContainer, IGeneric where T : CSerializable {
		[Serialize("$ClassName$")] public StringID className;
		public T obj;
		public bool serializeClassName = true;

		public string ClassName => obj?.ClassName;
		public Type ObjectType {
			get {
				Type type = ObjectFactory.classes[className.stringID];
				if (type.ContainsGenericParameters) {
					if (!typeof(T).IsGenericType) {
						throw new Exception($"Generic parameters error with type {type}. Expecting type {typeof(T)}.");
					}
					type = type.MakeGenericType(typeof(T).GetGenericArguments());
				}
				return type;
			}
		}

		public Generic() {
			className = new StringID();
		}

		public Generic(T obj) {
			if (obj != null && obj.ClassCRC.HasValue) {
				className = new StringID(obj.ClassCRC.Value);
				this.obj = obj;
			} else {
				className = new StringID();
			}
		}

		public bool IsNull {
			get {
				return className == null || className.IsNull;
			}
		}

		public object GenericObject { get => obj; set => obj = (T)value; }
		public StringID GenericClassName { get => className; set => className = value; }

		public T2 GetObject<T2>() where T2 : T => (T2)obj;
		public void SetObject(T obj) => this.obj = obj;

		public void SerializeClassName(CSerializerObject s) {
			if (s.Settings.EngineVersion <= EngineVersion.RO) {
				className = s.SerializeObject<StringID>(className, name: "NAME");
			} else {
				className = s.SerializeObject<StringID>(className, name: "$ClassName$");
			}
		}

		public void SerializeObject(CSerializerObject s) {
			/*s.Serialize(this, GetType().GetField(nameof(className)),
				(SerializeAttribute)GetType().GetField(nameof(className)).GetCustomAttributes(typeof(SerializeAttribute), false).First());*/
			if (className.IsNull) {
				obj = default;
			} else {
				if (ObjectFactory.classes.ContainsKey(className.stringID)) {
					/*if (s.log) {
						MapLoader.Loader.Log(pos + ":" + new string(' ', (s.Indent + 1) * 2) + "$ClassName$ - " + className.stringID.ToString("X8") + "(" + ObjectFactory.classes[className.stringID] + ")");
					}*/
					Type type = ObjectFactory.classes[className.stringID];
					if (type.ContainsGenericParameters) {
						if (!typeof(T).IsGenericType) {
							s.Context.SystemLogger?.LogError(s.CurrentPointer + " - Generic parameters error with type " + type + ". Expecting type " + typeof(T) + ".");
							throw new Exception(s.CurrentPointer + " - Generic parameters error with type " + type + ". Expecting type " + typeof(T) + ".");
						}
						type = type.MakeGenericType(typeof(T).GetGenericArguments());
					}
					s.IncreaseMemCount(type);
					obj = s.SerializeGeneric<T>(obj, type);
				} else {
					if (s is CSerializerObjectTagBinary) {
						s.Context.SystemLogger?.LogWarning("CRC " + className.stringID.ToString("X8")
							+ " found at " + s.CurrentPointer
							+ " while reading container of type " + typeof(T) + " is not yet supported!");
						className.stringID = 0xFFFFFFFF;
					} else {
						s.Context.SystemLogger?.LogError("CRC " + className.stringID.ToString("X8")
							+ " found at " + s.CurrentPointer
							+ " while reading container of type " + typeof(T) + " is not yet supported!");
						throw new NotImplementedException("CRC " + className.stringID.ToString("X8")
							+ " found at position " + s.CurrentPointer
							+ " while reading container of type " + typeof(T) + " is not yet supported!");
					}
				}
			}
		}

		public void Serialize(CSerializerObject s, string name) {
			if (serializeClassName) {
				SerializeClassName(s);
			}
			SerializeObject(s);
		}

		public void MakeNull() {
			className = null;
			obj = null;
		}
	}
}
