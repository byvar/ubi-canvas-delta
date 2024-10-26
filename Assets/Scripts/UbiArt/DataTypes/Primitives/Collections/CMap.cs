﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt {
	[SerializeEmbed]
	public class CMap<TKey,TValue> : IDictionary<TKey,TValue>, ICSerializable, IObjectContainer {
		Dictionary<TKey, TValue> container = new Dictionary<TKey, TValue>();

		private const uint baseNodeSize = 16;

		public void Serialize(CSerializerObject s, string name) {
			uint count = (uint)Count;
			count = s.Serialize<uint>(count, name: name);
			var copy = container.ToList();
			Resize(copy, (int)count);
			var customSize = baseNodeSize + s.GetMemorySize(typeof(TKey)) + s.GetMemorySize(typeof(TValue));// + 4; // +4 because they're pointers
			s.IncreaseMemCount(typeof(TKey), count: count, customSize: customSize);
			for (int i = 0; i < count; i++) {
				TKey key = copy[i].Key;
				TValue val = copy[i].Value;
				if (s.ArrayEntryStart(name: name, index: i)) {
					key = s.SerializeGeneric(key, name: "KEY");
					val = s.SerializeGeneric(val, name: "VAL");
					s.ArrayEntryStop();
					copy[i] = new KeyValuePair<TKey, TValue>(key, val);
				}
			}
			container.Clear();
			foreach (var k in copy) {
				container.Add(k.Key, k.Value);
			}
		}

		#region Dictionary interface
		private void Resize(List<KeyValuePair<TKey, TValue>> container, int sz) {
			int cur = container.Count;
			if (sz < cur)
				container.RemoveRange(sz, cur - sz);
			else if (sz > cur) {
				if (sz > container.Capacity) container.Capacity = sz;
				container.AddRange(Enumerable.Repeat(default(KeyValuePair<TKey, TValue>), sz - cur));
			}
		}

		public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)container)[key]; set => ((IDictionary<TKey, TValue>)container)[key] = value; }

		public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)container).Keys;

		public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)container).Values;

		public int Count => ((IDictionary<TKey, TValue>)container).Count;

		public bool IsReadOnly => ((IDictionary<TKey, TValue>)container).IsReadOnly;

		public void Add(TKey key, TValue value) {
			((IDictionary<TKey, TValue>)container).Add(key, value);
		}

		public void Add(KeyValuePair<TKey, TValue> item) {
			((IDictionary<TKey, TValue>)container).Add(item);
		}

		public void Clear() {
			((IDictionary<TKey, TValue>)container).Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item) {
			return ((IDictionary<TKey, TValue>)container).Contains(item);
		}

		public bool ContainsKey(TKey key) {
			return ((IDictionary<TKey, TValue>)container).ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			((IDictionary<TKey, TValue>)container).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return ((IDictionary<TKey, TValue>)container).GetEnumerator();
		}

		public bool Remove(TKey key) {
			return ((IDictionary<TKey, TValue>)container).Remove(key);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item) {
			return ((IDictionary<TKey, TValue>)container).Remove(item);
		}

		public bool TryGetValue(TKey key, out TValue value) {
			return ((IDictionary<TKey, TValue>)container).TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IDictionary<TKey, TValue>)container).GetEnumerator();
		}
		#endregion
	}
}
