namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleControllerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA5B22574;
	}
}

