namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CharacterInfoHUD : UIMenuBasic {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBCF7562E;
	}
}

