namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TutorialManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x120662E0;
	}
}

