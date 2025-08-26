namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_StimComponent : ShapeComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF6BA20BB;
	}
}

