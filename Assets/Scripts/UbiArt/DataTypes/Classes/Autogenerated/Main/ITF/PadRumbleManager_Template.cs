namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.LegendsAndUp)]
	public partial class PadRumbleManager_Template : TemplateObj {
		public CListO<PadRumble> rumbles;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			rumbles = s.SerializeObject<CListO<PadRumble>>(rumbles, name: "rumbles");
		}
		public override uint? ClassCRC => 0xCBC543B4;
	}
}

