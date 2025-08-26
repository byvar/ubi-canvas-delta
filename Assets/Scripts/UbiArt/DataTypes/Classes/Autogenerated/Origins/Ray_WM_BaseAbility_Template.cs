namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_BaseAbility_Template : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC70ABF01;
	}
}

