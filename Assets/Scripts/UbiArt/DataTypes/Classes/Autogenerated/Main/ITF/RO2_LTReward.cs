namespace UbiArt.ITF {
	[Games(GameFlags.RA)]
	public partial class RO2_LTReward : CSerializable {
		public uint id = uint.MaxValue;
		public uint rewardType = uint.MaxValue;
		public uint permut;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			id = s.Serialize<uint>(id, name: "id");
			rewardType = s.Serialize<uint>(rewardType, name: "rewardType");
			permut = s.Serialize<uint>(permut, name: "permut");
		}
	}
}

