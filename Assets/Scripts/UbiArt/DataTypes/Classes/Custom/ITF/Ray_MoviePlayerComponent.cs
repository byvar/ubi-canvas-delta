namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_MoviePlayerComponent : MoviePlayerComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x441BA694;
	}
}

