namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionAction_Template : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6FAB466D;
	}
}

