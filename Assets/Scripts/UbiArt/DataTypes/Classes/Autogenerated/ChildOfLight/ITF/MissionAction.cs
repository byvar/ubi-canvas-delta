namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class MissionAction : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x350855D2;
	}
}

