﻿namespace UbiArt {
	[SerializeEmbed]
	public class Nullable<T> : ICSerializable, IObjectContainer where T : ICSerializable, new() {
		public bool read;
		public T value;

		public void Serialize(CSerializerObject s, string name) {
			read = s.Serialize<bool>(read, name: "read");
			if (read) {
				s.IncreaseMemCount(typeof(T));
				value = s.SerializeObject<T>(value, name: name);
			}
		}

		public bool IsNull => !read;

		public Nullable() { }
		public Nullable(T value) {
			if (value != null) {
				read = true;
				this.value = value;
			}
		}
	}
}
