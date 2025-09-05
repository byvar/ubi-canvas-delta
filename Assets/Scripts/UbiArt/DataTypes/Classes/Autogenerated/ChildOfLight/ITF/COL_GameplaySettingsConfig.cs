namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_GameplaySettingsConfig : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x15AC4D38;
	}
}

