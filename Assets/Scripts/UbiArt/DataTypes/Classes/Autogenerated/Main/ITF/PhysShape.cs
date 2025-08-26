namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PhysShape : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9A8F1B0D;
	}
}

