namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_OceanSpiderAIComponent : Ray_SimpleAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA77F502B;
	}
}

