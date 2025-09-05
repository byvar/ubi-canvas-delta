namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_DialogBalloonInteraction : COL_ObjectInteraction {
		public float time;
		public LocalisationId text;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			time = s.Serialize<float>(time, name: "time");
			text = s.SerializeObject<LocalisationId>(text, name: "text");
		}
		public override uint? ClassCRC => 0x69E1D1E9;
	}
}

