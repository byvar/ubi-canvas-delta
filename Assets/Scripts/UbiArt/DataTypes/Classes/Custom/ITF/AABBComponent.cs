namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class AABBComponent : ActorComponent {
		public AABB aabb;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			aabb = s.SerializeObject<AABB>(aabb, name: "aabb");
		}

		public override uint? ClassCRC => 0x834D49DD;
	}
}
