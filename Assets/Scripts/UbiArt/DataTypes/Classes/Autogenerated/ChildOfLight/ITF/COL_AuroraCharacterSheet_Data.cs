namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_AuroraCharacterSheet_Data : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB3384FF2;
	}
}

