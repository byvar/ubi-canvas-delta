namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleActionManagerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE292BB54;
	}
}

