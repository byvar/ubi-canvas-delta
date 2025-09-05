namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionCharge_Template : COL_BTActionBase_Template {
		public StringID animCharge;
		public Generic<PhysShape> detectionShape;
		public float targetDistThreshold;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			animCharge = s.SerializeObject<StringID>(animCharge, name: "animCharge");
			detectionShape = s.SerializeObject<Generic<PhysShape>>(detectionShape, name: "detectionShape");
			targetDistThreshold = s.Serialize<float>(targetDistThreshold, name: "targetDistThreshold");
		}
		public override uint? ClassCRC => 0xE5816A26;
	}
}

