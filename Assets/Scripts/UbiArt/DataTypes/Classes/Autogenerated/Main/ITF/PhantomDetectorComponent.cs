namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PhantomDetectorComponent : ShapeDetectorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x8ADEC12A;
	}
}

