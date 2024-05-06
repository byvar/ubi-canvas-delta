namespace UbiArt {
	public class ContainerFile<T> : ICSerializable, IObjectContainer where T : ICSerializable, new() {
		public T obj;

		public void Serialize(CSerializerObject s, string name) {
			s.InitSerializer();
			s.IncreaseMemCount(typeof(T));
			obj = s.SerializeObject<T>(obj);
			s.CloseSerializer();
		}

		public ContainerFile() { }
		public ContainerFile(T obj) { this.obj = obj; }
	}
}
