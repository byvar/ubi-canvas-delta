namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BattleAudioVisualComponent_Template : ActorComponent_Template {
		public Vec2d spawnOffset;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			spawnOffset = s.SerializeObject<Vec2d>(spawnOffset, name: "spawnOffset");
		}
		public override uint? ClassCRC => 0x9E1A196D;
	}
}

