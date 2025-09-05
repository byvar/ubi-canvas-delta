namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TweenControllerComponent_Template : COL_InteractComponent_Template {
		public float animDuration;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animDuration = s.Serialize<float>(animDuration, name: "animDuration");
		}
		public override uint? ClassCRC => 0x3C593C14;
	}
}

