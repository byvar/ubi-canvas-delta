namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.LegendsAndUp)]
	public partial class BoxInterpolatorComponent_Template : InterpolatorComponent_Template {
		public AABB innerAABB;
		public AABB outerAABB;
		public Angle AABBRotation;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RO) {
				innerAABB = s.SerializeObject<AABB>(innerAABB, name: "innerAABB");
				outerAABB = s.SerializeObject<AABB>(outerAABB, name: "outerAABB");
				AABBRotation = s.SerializeObject<Angle>(AABBRotation, name: "AABBRotation");
			} else {
			}
		}
		public override uint? ClassCRC => 0x71F87656;
	}
}

