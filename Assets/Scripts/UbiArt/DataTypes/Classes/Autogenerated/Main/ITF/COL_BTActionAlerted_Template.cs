namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionAlerted_Template : COL_BTActionBase_Template {
		public float timeBeforeCharge;
		public bool ignoreZ;
		public StringID animStandToAlerted;
		public StringID animAlerted;
		public StringID animAlertedToStand;
		public StringID animAlertedToCharge;
		public Generic<PhysShape> detectionShape;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			timeBeforeCharge = s.Serialize<float>(timeBeforeCharge, name: "timeBeforeCharge");
			ignoreZ = s.Serialize<bool>(ignoreZ, name: "ignoreZ", options: CSerializerObject.Options.BoolAsByte);
			animStandToAlerted = s.SerializeObject<StringID>(animStandToAlerted, name: "animStandToAlerted");
			animAlerted = s.SerializeObject<StringID>(animAlerted, name: "animAlerted");
			animAlertedToStand = s.SerializeObject<StringID>(animAlertedToStand, name: "animAlertedToStand");
			animAlertedToCharge = s.SerializeObject<StringID>(animAlertedToCharge, name: "animAlertedToCharge");
			detectionShape = s.SerializeObject<Generic<PhysShape>>(detectionShape, name: "detectionShape");
		}
		public override uint? ClassCRC => 0x02999C6C;
	}
}

