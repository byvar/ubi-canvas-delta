namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RFR)]
	public partial class CostumeInfo : CSerializable {
		public StringID id;
		public uint prize;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			id = s.SerializeObject<StringID>(id, name: "id");
			prize = s.Serialize<uint>(prize, name: "prize");
		}
	}
}

