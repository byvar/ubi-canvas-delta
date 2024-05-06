using System;
using System.Collections.Generic;

namespace UbiArt {
	public class SerializableCache {
		public SerializableCache(ISystemLogger systemLog) {
			SystemLog = systemLog;
			Structs = new Dictionary<Type, Dictionary<StringID, ICSerializable>>();
		}

		protected ISystemLogger SystemLog { get; }

		public Dictionary<Type, Dictionary<StringID, ICSerializable>> Structs { get; }

		public ICSerializable Get(StringID id, Type type) {
			if (id == null)
				return default;

			if (!Structs.ContainsKey(type) || !Structs[type].ContainsKey(id))
				return default;

			return Structs[type][id];
		}
		public T Get<T>(StringID id)
			where T : class, ICSerializable {
			return (T)Get(id, typeof(T));
		}
		public T Get<T>(Path path)
			where T : class, ICSerializable => Get<T>(path?.stringID);

		public void Add(StringID id, ICSerializable serializable, Type type) {
			if (!Structs.ContainsKey(type))
				Structs[type] = new Dictionary<StringID, ICSerializable>();

			if (!Structs[type].ContainsKey(id))
				Structs[type][id] = serializable;
			else
				SystemLog?.LogWarning("Duplicate StringID {0} for type {1}", id, type);
		}
		public void Add<T>(StringID id, T serializable)
			where T : class, ICSerializable {
			Type type = typeof(T);

			Add(id, serializable, type);
		}

		public void Clear() => Structs.Clear();
	}
}