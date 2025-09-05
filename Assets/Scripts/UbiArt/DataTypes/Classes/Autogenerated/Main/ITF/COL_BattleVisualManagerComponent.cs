namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleVisualManagerComponent : ActorComponent {
		public ObjectId frontResolverId;
		public Color initialStateColor;
		public Color idleStateColor;
		public Color selectionStateColor;
		public Color actionStateColor;
		public float idleToSelectDuration;
		public float idleToActionDuration;
		public float selectToIdleDuration;
		public float actionToIdleDuration;
		public Curve2D transitionCurve;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			frontResolverId = s.SerializeObject<ObjectId>(frontResolverId, name: "frontResolverId");
			initialStateColor = s.SerializeObject<Color>(initialStateColor, name: "initialStateColor");
			idleStateColor = s.SerializeObject<Color>(idleStateColor, name: "idleStateColor");
			selectionStateColor = s.SerializeObject<Color>(selectionStateColor, name: "selectionStateColor");
			actionStateColor = s.SerializeObject<Color>(actionStateColor, name: "actionStateColor");
			idleToSelectDuration = s.Serialize<float>(idleToSelectDuration, name: "idleToSelectDuration");
			idleToActionDuration = s.Serialize<float>(idleToActionDuration, name: "idleToActionDuration");
			selectToIdleDuration = s.Serialize<float>(selectToIdleDuration, name: "selectToIdleDuration");
			actionToIdleDuration = s.Serialize<float>(actionToIdleDuration, name: "actionToIdleDuration");
			transitionCurve = s.SerializeObject<Curve2D>(transitionCurve, name: "transitionCurve");
		}
		public override uint? ClassCRC => 0x327E716F;
	}
}

