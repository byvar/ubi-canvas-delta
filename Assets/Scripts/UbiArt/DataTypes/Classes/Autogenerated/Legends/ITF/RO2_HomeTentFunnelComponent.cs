namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_HomeTentFunnelComponent : RO2_HomeComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x45783299;
	}
}

