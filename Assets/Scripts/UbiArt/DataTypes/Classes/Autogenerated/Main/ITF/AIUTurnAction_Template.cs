namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class AIUTurnAction_Template : AIAction_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6AFCDDD0;
	}
}

