namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionCondition_Template : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB8164942;
	}
}

