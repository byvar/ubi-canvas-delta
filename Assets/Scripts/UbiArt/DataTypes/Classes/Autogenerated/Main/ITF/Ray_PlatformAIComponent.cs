namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_PlatformAIComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x480E5DC6;
	}
}

