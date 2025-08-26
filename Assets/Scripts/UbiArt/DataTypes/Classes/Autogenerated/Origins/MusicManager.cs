namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class MusicManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF2CDE67A;
	}
}

