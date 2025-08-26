namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.RAVersion)]
	public partial class GameModeParameters : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE83C2A9B;
	}
}

