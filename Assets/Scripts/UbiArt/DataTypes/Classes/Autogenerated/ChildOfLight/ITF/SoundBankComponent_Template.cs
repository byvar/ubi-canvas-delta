namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class SoundBankComponent_Template : ActorComponent_Template {
		public SoundGUID Bank;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			Bank = s.SerializeObject<SoundGUID>(Bank, name: "Bank");
		}
		public override uint? ClassCRC => 0x1BF38B3F;
	}
}

