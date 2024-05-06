using System.Collections.Generic;

namespace UbiArt {
	// For serializing ICSerializable objects
	[SerializeEmbed]
	public class CRefList<T> : CListO<T> where T : ICSerializable, new() {
		public CRefList() { }
		public CRefList(List<T> list) : base(list) { }

		public override void Serialize(CSerializerObject s, string name) {
			//s.Context.SystemLogger?.Log("Serializing List: " + name);
			uint count = (uint)container.Count;
			count = s.Serialize<uint>(count, name: name);
			if(count != container.Count) Resize((int)count);
			string typeName = "VAL";
			if (count > 0 && !s.IsValueType(typeof(T))) {
				typeName = null;
			}
			s.IncreaseMemCount(typeof(uint), count: count); // Allocate pointers before loop
			for (int i = 0; i < count; i++) {
				T obj = container[i];
				if (s.ArrayEntryStart(name: name, index: i)) {
					s.IncreaseMemCount(typeof(T), count: 1); // Allocate objects one by one
					obj = s.SerializeObject<T>(obj, name: typeName);
					s.ArrayEntryStop();
				}
				container[i] = obj;
			}
		}
	}
}
