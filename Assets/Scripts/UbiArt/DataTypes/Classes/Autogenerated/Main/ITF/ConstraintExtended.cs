namespace UbiArt.ITF {
	[Games(GameFlags.RJR | GameFlags.RFR | GameFlags.VH | GameFlags.RA)]
	public partial class ConstraintExtended : CSerializable {
		public float offset;
		public float timeToIncrease = 1f;
		public float timeToWaitBeforeDecrease = 5f;
		public float timeToDecrease = 2f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				offset = s.Serialize<float>(offset, name: "offset");
				timeToIncrease = s.Serialize<float>(timeToIncrease, name: "timeToIncrease");
				timeToWaitBeforeDecrease = s.Serialize<float>(timeToWaitBeforeDecrease, name: "timeToWaitBeforeDecrease");
				timeToDecrease = s.Serialize<float>(timeToDecrease, name: "timeToDecrease");
			}
		}
	}
}

