namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.RA)]
	public partial class FriezeControllerComponent_Template : LinkComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6D88A005;
	}
}

