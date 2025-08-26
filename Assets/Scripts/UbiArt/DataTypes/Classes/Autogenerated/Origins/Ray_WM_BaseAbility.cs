namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_BaseAbility : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2C7F394D;
	}
}

