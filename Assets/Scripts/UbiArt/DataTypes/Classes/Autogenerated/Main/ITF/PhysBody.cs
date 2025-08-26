namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class PhysBody : PhysCollidable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x422CCDC4;
	}
}

