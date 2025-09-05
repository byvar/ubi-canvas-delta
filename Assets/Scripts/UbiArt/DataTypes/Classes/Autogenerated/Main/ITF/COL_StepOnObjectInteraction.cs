namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_StepOnObjectInteraction : COL_ObjectInteraction {
		public bool stepOn;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			stepOn = s.Serialize<bool>(stepOn, name: "stepOn", options: CSerializerObject.Options.BoolAsByte);
		}
		public override uint? ClassCRC => 0xEDEDB11E;
	}
}

