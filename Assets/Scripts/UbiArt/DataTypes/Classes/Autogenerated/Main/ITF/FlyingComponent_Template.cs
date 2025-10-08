namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class FlyingComponent_Template : ActorComponent_Template {
		public FlyingComponent_Template.FlyingComponentDebugData DebugData;
		public float MoveVecBlendFactor;
		public float MoveSpeed;
		public float MoveDirMaxAngle;
		public float MoveDirMinAngle;
		public float MoveDirAmplitude;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			DebugData = s.SerializeObject<FlyingComponent_Template.FlyingComponentDebugData>(DebugData, name: "DebugData");
			MoveVecBlendFactor = s.Serialize<float>(MoveVecBlendFactor, name: "MoveVecBlendFactor");
			MoveSpeed = s.Serialize<float>(MoveSpeed, name: "MoveSpeed");
			MoveDirMaxAngle = s.Serialize<float>(MoveDirMaxAngle, name: "MoveDirMaxAngle");
			MoveDirMinAngle = s.Serialize<float>(MoveDirMinAngle, name: "MoveDirMinAngle");
			MoveDirAmplitude = s.Serialize<float>(MoveDirAmplitude, name: "MoveDirAmplitude");
		}
		public override uint? ClassCRC => 0x7E676187;

		public class FlyingComponentDebugData : CSerializable {
			public int Used;
			public StringID SnapToWaypointId;
			public int OnPlayer;
			public float CollideInvForce;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				Used = s.Serialize<int>(Used, name: "Used");
				SnapToWaypointId = s.SerializeObject<StringID>(SnapToWaypointId, name: "SnapToWaypointId");
				OnPlayer = s.Serialize<int>(OnPlayer, name: "OnPlayer");
				CollideInvForce = s.Serialize<float>(CollideInvForce, name: "CollideInvForce");
			}
		}
	}
}

