namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_SpikyFlowerComponent : ActorComponent {
		public uint UInt_00 { get; set; }
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Platform == GamePlatform.Vita) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			}
		}
		public override uint? ClassCRC => 0xD97190AD;
	}
}

