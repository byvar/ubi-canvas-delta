namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class GameScreenBase : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB7EE7796;
	}
}

