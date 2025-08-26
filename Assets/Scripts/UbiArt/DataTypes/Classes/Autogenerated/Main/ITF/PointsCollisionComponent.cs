namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PointsCollisionComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x12ACE724;
	}
}

