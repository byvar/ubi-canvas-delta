namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionCharge : COL_BTActionBase {
		public Generic<PhysShape> detectionShape;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			detectionShape = s.SerializeObject<Generic<PhysShape>>(detectionShape, name: "detectionShape");
		}
		public override uint? ClassCRC => 0x03B598BA;
	}
}

