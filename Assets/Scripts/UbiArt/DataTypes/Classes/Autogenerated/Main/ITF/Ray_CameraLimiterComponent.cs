namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_CameraLimiterComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4D903DA6;
	}
}

