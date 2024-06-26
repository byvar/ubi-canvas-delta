namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_BossPlantNodeComponent_Template : ActorComponent_Template {
		public StringID anim;
		public float animPlayRate;
		public Vec2d cycleVector;
		public Placeholder triggerEvent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RO) {
				anim = s.SerializeObject<StringID>(anim, name: "anim");
				animPlayRate = s.Serialize<float>(animPlayRate, name: "animPlayRate");
				cycleVector = s.SerializeObject<Vec2d>(cycleVector, name: "cycleVector");
				triggerEvent = s.SerializeObject<Placeholder>(triggerEvent, name: "triggerEvent");
			} else {
				anim = s.SerializeObject<StringID>(anim, name: "anim");
				animPlayRate = s.Serialize<float>(animPlayRate, name: "animPlayRate");
				cycleVector = s.SerializeObject<Vec2d>(cycleVector, name: "cycleVector");
			}
		}
		public override uint? ClassCRC => 0x4237FE9C;
	}
}

