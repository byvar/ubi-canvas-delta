namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_TrunkComponent : ActorComponent {
		public Vec2d max = new Vec2d(2, 1);
		public float heightUnderLeftCorner = -3;
		public float heightUnderRightCorner = -3;
		public float hookLengthLeft = 1;
		public float hookLengthRight = 1;
		public bool onlyOneTrigger;
		public bool setUpEnabled;
		public float osciSummitHeight = 20;
		public Angle osciAngularAccel = 0.04363323f;
		public Angle osciAngularSpeedInit = 0.02617994f;
		public bool acceptCheckPointSave = true;
		public RO2_TrunkComponent.RO2_TrunkCorner cornerLowerLeft;
		public RO2_TrunkComponent.RO2_TrunkCorner cornerLowerRight;
		public RO2_TrunkComponent.RO2_TrunkCorner cornerUpperLeft;
		public RO2_TrunkComponent.RO2_TrunkCorner cornerUpperRight;
		public float checkpointAngle;
		public Vec2d checkpointPos;
		public bool isStill = true;
		public bool trunkIsStoped;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				max = s.SerializeObject<Vec2d>(max, name: "max");
				heightUnderLeftCorner = s.Serialize<float>(heightUnderLeftCorner, name: "heightUnderLeftCorner");
				heightUnderRightCorner = s.Serialize<float>(heightUnderRightCorner, name: "heightUnderRightCorner");
				hookLengthLeft = s.Serialize<float>(hookLengthLeft, name: "hookLengthLeft");
				hookLengthRight = s.Serialize<float>(hookLengthRight, name: "hookLengthRight");
			}
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				onlyOneTrigger = s.Serialize<bool>(onlyOneTrigger, name: "onlyOneTrigger");
				setUpEnabled = s.Serialize<bool>(setUpEnabled, name: "setUpEnabled");
				osciSummitHeight = s.Serialize<float>(osciSummitHeight, name: "osciSummitHeight");
				osciAngularAccel = s.SerializeObject<Angle>(osciAngularAccel, name: "osciAngularAccel");
				osciAngularSpeedInit = s.SerializeObject<Angle>(osciAngularSpeedInit, name: "osciAngularSpeedInit");
				acceptCheckPointSave = s.Serialize<bool>(acceptCheckPointSave, name: "acceptCheckPointSave");
			}
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				cornerLowerLeft = s.SerializeObject<RO2_TrunkComponent.RO2_TrunkCorner>(cornerLowerLeft, name: "cornerLowerLeft");
				cornerLowerRight = s.SerializeObject<RO2_TrunkComponent.RO2_TrunkCorner>(cornerLowerRight, name: "cornerLowerRight");
				cornerUpperLeft = s.SerializeObject<RO2_TrunkComponent.RO2_TrunkCorner>(cornerUpperLeft, name: "cornerUpperLeft");
				cornerUpperRight = s.SerializeObject<RO2_TrunkComponent.RO2_TrunkCorner>(cornerUpperRight, name: "cornerUpperRight");
				checkpointAngle = s.Serialize<float>(checkpointAngle, name: "checkpointAngle");
				checkpointPos = s.SerializeObject<Vec2d>(checkpointPos, name: "checkpointPos");
				isStill = s.Serialize<bool>(isStill, name: "isStill");
				trunkIsStoped = s.Serialize<bool>(trunkIsStoped, name: "trunkIsStoped");
			}
		}
		[Games(GameFlags.RA)]
		public partial class RO2_TrunkCorner : CSerializable {
			public Vec2d localPos;
			public Color color = Color.White;
			public bool isPined;
			public Angle angularSpeed;
			public bool isInCol;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				localPos = s.SerializeObject<Vec2d>(localPos, name: "localPos");
				color = s.SerializeObject<Color>(color, name: "color");
				isPined = s.Serialize<bool>(isPined, name: "isPined");
				angularSpeed = s.SerializeObject<Angle>(angularSpeed, name: "angularSpeed");
				isInCol = s.Serialize<bool>(isInCol, name: "isInCol");
			}
		}
		public override uint? ClassCRC => 0xB40A3FAA;
	}
}

