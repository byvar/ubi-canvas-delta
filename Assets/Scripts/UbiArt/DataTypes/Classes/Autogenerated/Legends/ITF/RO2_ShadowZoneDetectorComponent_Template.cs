namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_ShadowZoneDetectorComponent_Template : ShapeDetectorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x22BC7C7D;
	}
}

