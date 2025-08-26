namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DarktoonTrapHoleComponent_Template : ActorComponent_Template {
		public StringID anim;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			anim = s.SerializeObject<StringID>(anim, name: "anim");
		}
		public override uint? ClassCRC => 0x429B9A87;
	}
}

