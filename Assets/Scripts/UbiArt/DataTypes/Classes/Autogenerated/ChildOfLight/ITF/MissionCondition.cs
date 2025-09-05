namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionCondition : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5CD92C0A;
	}
}

