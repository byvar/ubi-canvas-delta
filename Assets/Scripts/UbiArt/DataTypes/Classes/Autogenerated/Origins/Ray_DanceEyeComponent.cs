namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DanceEyeComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x60D5A505;
	}
}

