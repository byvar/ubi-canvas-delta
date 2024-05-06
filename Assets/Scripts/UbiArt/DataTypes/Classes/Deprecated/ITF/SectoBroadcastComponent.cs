namespace UbiArt.ITF {
	[Games(GameFlags.RL, platforms: PlatformFlags.Vita)] // Only on Vita!
	public partial class SectoBroadcastComponent : ActorComponent {
		public uint UInt_00 { get; set; }
		public uint UInt_01 { get; set; }
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_01 = s.Serialize<uint>(UInt_01, name: nameof(UInt_01));
		}
		public override uint? ClassCRC => 0x8ABF30E1;
	}
}
