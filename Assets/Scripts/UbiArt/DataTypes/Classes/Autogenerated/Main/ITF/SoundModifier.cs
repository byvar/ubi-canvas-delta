namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class SoundModifier : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x73500C15;
	}
}

