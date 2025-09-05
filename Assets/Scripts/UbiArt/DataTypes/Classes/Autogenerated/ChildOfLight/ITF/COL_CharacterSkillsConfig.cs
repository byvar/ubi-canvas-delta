namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_CharacterSkillsConfig : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x58E80430;
	}
}

