namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_GS_AdversarialGoldenPotato : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x46B080AC;
	}
}

