namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_PlatformTreeComponent : ActorComponent {
		public int startActivated;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				startActivated = s.Serialize<int>(startActivated, name: "startActivated");
			}
		}
		public override uint? ClassCRC => 0xF4D18DFC;
	}
}

