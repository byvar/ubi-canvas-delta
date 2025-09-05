namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleAudioVisualComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x0C7AAE1F;
	}
}

