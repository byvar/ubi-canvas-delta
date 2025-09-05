namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionStep : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x41DE0CB2;
	}
}

