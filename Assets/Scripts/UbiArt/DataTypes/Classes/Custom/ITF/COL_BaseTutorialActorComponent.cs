namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BaseTutorialActorComponent : ActorComponent {
		public bool forceDisplay;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			forceDisplay = s.Serialize<bool>(forceDisplay, name: "forceDisplay", options: CSerializerObject.Options.BoolAsByte);
		}

		public override uint? ClassCRC => 0x3FCCCEB7;
	}
}
