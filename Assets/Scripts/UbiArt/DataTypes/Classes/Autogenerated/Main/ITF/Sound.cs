namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL)]
	public partial class Sound : Resource {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9B7EB41F;
	}
}

