namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_UIEventDisplayTutorial : Event {
		public StringID tutorialID;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			tutorialID = s.SerializeObject<StringID>(tutorialID, name: "tutorialID");
		}
		public override uint? ClassCRC => 0xADEF30F4;
	}
}

