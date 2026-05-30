using System.Linq;

namespace UbiArt.Animation {
	public class KeyArray<T> : CSerializable {
		public CArrayP<ulong> keys = new CArrayP<ulong>();

		public CArrayO<StringID> KeysLegends {
			get {
				if (keys == null) return null;
				return new CArrayO<StringID>(keys.Select(k => new StringID((uint)k)).ToArray());
			}
			set {
				if (value == null) keys = null;
				keys = new CArrayP<ulong>(value.Select(v => (ulong)(v?.stringID ?? uint.MaxValue)).ToArray());
			}
		}
		public CArray<T> values = new CArray<T>();

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RA || s.Settings.Game == Game.RM || s.Settings.Game == Game.VH) {
				keys = s.SerializeObject<CArrayP<ulong>>(keys, name: "keys");
			} else {
				KeysLegends = s.SerializeObject<CArrayO<StringID>>(KeysLegends, name: "keys");
			}
			values = s.SerializeObject<CArray<T>>(values, name: "values");
		}

		public int GetKeyIndex(StringID key) {
			if (keys != null) {
				return keys.IndexOf(key.stringID);
			}
			return -1;
		}
		public int GetKeyIndex(ulong key) {
			if (keys != null) {
				return keys.IndexOf(key);
			}
			return -1;
		}
		public int KeysCount => keys?.Count ?? 0;

		public ulong GetKeyFromValue(T value) {
			var ind = values.IndexOf(value);
			if (ind == -1) return ulong.MaxValue;
			if (keys != null) {
				return keys[ind];
			}
			return ulong.MaxValue;
		}
		public StringID GetKeyFromValueSID(T value) {
			var ind = values.IndexOf(value);
			if (ind == -1) return StringID.Invalid;
			if (keys != null) {
				return new StringID((uint)keys[ind]);
			}
			return StringID.Invalid;
		}
		public StringID GetKeyAtIndex(int index) {
			if (keys != null) return new StringID((uint)keys[index]);
			return null;
		}
		public T GetValueAtIndex(int index) {
			if (values != null) return values[index];
			return default;
		}
		public bool Lookup(StringID key, out T value) {
			var index = GetKeyIndex(key);
			if (index != -1) {
				value = values[index];
				return true;
			}
			value = default;
			return false;
		}
		public bool Lookup(ulong key, out T value) {
			var index = GetKeyIndex(key);
			if (index != -1) {
				value = values[index];
				return true;
			}
			value = default;
			return false;
		}
		public void AddKeyValue(ulong key, T value) {
			keys.Add(key);
			values.Add(value);
		}
		public void AddKeyValue(StringID key, T value) {
			keys.Add(key?.stringID ?? uint.MaxValue);
			values.Add(value);
		}
		public void SetKeyValue(int index, ulong key, T value) {
			keys[index] = key;
			values[index] = value;
		}
		public void SetKeyValue(int index, StringID key, T value) {
			keys[index] = key?.stringID ?? uint.MaxValue;
			values[index] = value;
		}
	}
}
