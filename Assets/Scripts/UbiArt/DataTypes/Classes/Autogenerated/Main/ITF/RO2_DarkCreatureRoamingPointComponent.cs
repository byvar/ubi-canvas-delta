namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_DarkCreatureRoamingPointComponent : ActorComponent {
		public float TimerMin;
		public float TimerMax;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				TimerMin = s.Serialize<float>(TimerMin, name: "TimerMin");
				TimerMax = s.Serialize<float>(TimerMax, name: "TimerMax");
			}
		}
		public override uint? ClassCRC => 0xC3E02D49;
	}
}

