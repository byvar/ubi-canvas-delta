namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_BubbleSpawnerComponent : ActorComponent {
		public float bubbleLifetime;
		public float bubbleFloatForceX;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				bubbleLifetime = s.Serialize<float>(bubbleLifetime, name: "bubbleLifetime");
				bubbleFloatForceX = s.Serialize<float>(bubbleFloatForceX, name: "bubbleFloatForceX");
			}
		}
		public override uint? ClassCRC => 0xE98D7883;
	}
}

