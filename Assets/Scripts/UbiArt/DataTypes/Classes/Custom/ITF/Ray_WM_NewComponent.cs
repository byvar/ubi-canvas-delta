namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_NewComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x09F89F24;
	}
}

