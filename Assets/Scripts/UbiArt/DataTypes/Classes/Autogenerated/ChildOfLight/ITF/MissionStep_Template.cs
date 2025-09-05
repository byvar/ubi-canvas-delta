namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionStep_Template : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE828973B;
	}
}

