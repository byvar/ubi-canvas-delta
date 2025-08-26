namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class TrailComponent : GraphicComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA2325B50;
	}
}

