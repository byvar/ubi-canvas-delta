namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_LumMusicManagerAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x824506FB;
	}
}

