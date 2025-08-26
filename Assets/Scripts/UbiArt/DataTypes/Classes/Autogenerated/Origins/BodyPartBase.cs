namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class BodyPartBase : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFE4DC9C2;
	}
}

