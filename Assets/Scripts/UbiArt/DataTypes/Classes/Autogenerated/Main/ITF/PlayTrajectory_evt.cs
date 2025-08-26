namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RLVersion | GameFlags.RM)]
	public partial class PlayTrajectory_evt : SequenceEventWithActor {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x53A76BDD;
	}
}

