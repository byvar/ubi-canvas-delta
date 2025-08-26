namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.RM)]
	public partial class Texture : Resource {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE54BF2CB;
	}
}

