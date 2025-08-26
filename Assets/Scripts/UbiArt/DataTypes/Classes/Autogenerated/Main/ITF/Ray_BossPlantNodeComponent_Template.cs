namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_BossPlantNodeComponent_Template : ActorComponent_Template {
		public StringID anim;
		public float animPlayRate;
		public Vec2d cycleVector;
		public Ray_EventNodeReached triggerEvent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			anim = s.SerializeObject<StringID>(anim, name: "anim");
			animPlayRate = s.Serialize<float>(animPlayRate, name: "animPlayRate");
			cycleVector = s.SerializeObject<Vec2d>(cycleVector, name: "cycleVector");
			triggerEvent = s.SerializeObject<Ray_EventNodeReached>(triggerEvent, name: "triggerEvent");
		}
		public override uint? ClassCRC => 0x4237FE9C;
	}
}

