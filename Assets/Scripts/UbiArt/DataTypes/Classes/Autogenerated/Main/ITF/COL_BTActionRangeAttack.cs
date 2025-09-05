namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionRangeAttack : COL_BTActionBase {
		public Generic<PhysShape> detectionShape;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				detectionShape = s.SerializeObject<Generic<PhysShape>>(detectionShape, name: "detectionShape");
			}
		}
		public override uint? ClassCRC => 0x1323BFB6;
	}
}

