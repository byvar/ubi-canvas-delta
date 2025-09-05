namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleFlowModifierComponent_Template : ActorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD685A0CF;
	}
}

