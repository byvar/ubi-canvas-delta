namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class MusicTreeBlockRandom : MusicTreeBlock {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEBF8CDA9;
	}
}

