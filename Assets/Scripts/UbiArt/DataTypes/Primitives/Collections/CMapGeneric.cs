using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt {
	[SerializeEmbed]
	public class CMapGeneric<TKey,TValue> : IDictionary<TKey,Generic<TValue>>, ICSerializable, IObjectContainer where TValue : CSerializable {
		Dictionary<TKey, Generic<TValue>> container = new Dictionary<TKey, Generic<TValue>>();


		public void Serialize(CSerializerObject s, string name) {
			uint count = (uint)Count;
			count = s.Serialize<uint>(count, name: name);
			var copy = container.ToList();
			Resize(copy, (int)count);
			s.IncreaseMemCount(typeof(TKey), count: count);
			for (int i = 0; i < count; i++) {
				TKey key = copy[i].Key;
				Generic<TValue> val = copy[i].Value ?? new Generic<TValue>();
				if (s.ArrayEntryStart(name: name, index: i)) {
					if (s.Settings.Game == Game.RL && !s.IsCustomSerializer) {
						val.SerializeClassName(s);
						key = s.SerializeGeneric(key, name: "KEY");
						val.serializeClassName = false;
						val = s.SerializeGeneric(val, name: "VAL");
						val.serializeClassName = true;
					} else {
						// Class name is serialized after key in origins, so like a normal CMap<TKey, Generic<TValue>>
						key = s.SerializeGeneric(key, name: "KEY");
						val = s.SerializeGeneric(val, name: "VAL");
					}
					s.ArrayEntryStop();
					copy[i] = new KeyValuePair<TKey, Generic<TValue>>(key, val);
				}
			}
			container.Clear();
			foreach (var k in copy) {
				container.Add(k.Key, k.Value);
			}
		}

		#region Dictionary interface
		private void Resize(List<KeyValuePair<TKey, Generic<TValue>>> container, int sz) {
			int cur = container.Count;
			if (sz < cur)
				container.RemoveRange(sz, cur - sz);
			else if (sz > cur) {
				if (sz > container.Capacity) container.Capacity = sz;
				container.AddRange(Enumerable.Repeat(default(KeyValuePair<TKey, Generic<TValue>>), sz - cur));
			}
		}

		public Generic<TValue> this[TKey key] { get => ((IDictionary<TKey, Generic<TValue>>)container)[key]; set => ((IDictionary<TKey, Generic<TValue>>)container)[key] = value; }

		public ICollection<TKey> Keys => ((IDictionary<TKey, Generic<TValue>>)container).Keys;

		public ICollection<Generic<TValue>> Values => ((IDictionary<TKey, Generic<TValue>>)container).Values;

		public int Count => ((IDictionary<TKey, Generic<TValue>>)container).Count;

		public bool IsReadOnly => ((IDictionary<TKey, Generic<TValue>>)container).IsReadOnly;

		public void Add(TKey key, Generic<TValue> value) {
			((IDictionary<TKey, Generic<TValue>>)container).Add(key, value);
		}

		public void Add(KeyValuePair<TKey, Generic<TValue>> item) {
			((IDictionary<TKey, Generic<TValue>>)container).Add(item);
		}

		public void Clear() {
			((IDictionary<TKey, Generic<TValue>>)container).Clear();
		}

		public bool Contains(KeyValuePair<TKey, Generic<TValue>> item) {
			return ((IDictionary<TKey, Generic<TValue>>)container).Contains(item);
		}

		public bool ContainsKey(TKey key) {
			return ((IDictionary<TKey, Generic<TValue>>)container).ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, Generic<TValue>>[] array, int arrayIndex) {
			((IDictionary<TKey, Generic<TValue>>)container).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, Generic<TValue>>> GetEnumerator() {
			return ((IDictionary<TKey, Generic<TValue>>)container).GetEnumerator();
		}

		public bool Remove(TKey key) {
			return ((IDictionary<TKey, Generic<TValue>>)container).Remove(key);
		}

		public bool Remove(KeyValuePair<TKey, Generic<TValue>> item) {
			return ((IDictionary<TKey, Generic<TValue>>)container).Remove(item);
		}

		public bool TryGetValue(TKey key, out Generic<TValue> value) {
			return ((IDictionary<TKey, Generic<TValue>>)container).TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IDictionary<TKey, Generic<TValue>>)container).GetEnumerator();
		}
		#endregion
	}
}
