namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class FlyingComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x360FBDE1;
	}
}

