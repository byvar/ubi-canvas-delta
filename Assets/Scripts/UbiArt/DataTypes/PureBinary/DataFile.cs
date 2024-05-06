namespace UbiArt {
	public class DataFile : CSerializable {
		public byte[] Data { get; set; }

		protected override void SerializeImpl(CSerializerObject s) {
			Data = s.SerializeBytes(Data, s.Length);
		}
	}
}
