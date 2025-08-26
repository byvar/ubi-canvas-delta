namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RM)]
	public partial class AIFollowActorAction : AIAction {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC6F78EC0;
	}
}

