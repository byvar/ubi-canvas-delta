namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RFR)]
	public partial class Ray_BossPlantArenaAIComponent : AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9A7EE819;
	}
}

