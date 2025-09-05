namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionRangeAttack_Template : COL_BTActionBase_Template {
		public StringID animAttack;
		public StringID targetBoneID;
		public StringID projectileAnchorBoneID;
		public Path projectileActorPath;
		public Generic<PhysShape> detectionShape;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animAttack = s.SerializeObject<StringID>(animAttack, name: "animAttack");
			targetBoneID = s.SerializeObject<StringID>(targetBoneID, name: "targetBoneID");
			projectileAnchorBoneID = s.SerializeObject<StringID>(projectileAnchorBoneID, name: "projectileAnchorBoneID");
			projectileActorPath = s.SerializeObject<Path>(projectileActorPath, name: "projectileActorPath");
			detectionShape = s.SerializeObject<Generic<PhysShape>>(detectionShape, name: "detectionShape");
		}
		public override uint? ClassCRC => 0x7C29B7AC;
	}
}

