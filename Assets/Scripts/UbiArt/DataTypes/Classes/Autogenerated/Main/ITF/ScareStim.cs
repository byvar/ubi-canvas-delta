namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RL | GameFlags.RAVersion)]
	public partial class ScareStim : EventStim {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1DBFF097;
	}
}

