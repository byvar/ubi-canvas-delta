namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_CyclopeusAIComponent : Ray_SimpleAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x76E1C782;
	}
}

