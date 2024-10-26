namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RAVersion)]
	public partial class RO2_HomeTreeGpeComponent_Template : ActorComponent_Template {
		public AppearMode appearMode = AppearMode.ScaleAndFade;
		public float fadeDuration = 1;
		public StringID appearAnim = new StringID("opening");
		public StringID idleAnim = new StringID(0x4459e8e);
		public bool scaleActor;
		public float trunkAttachCurveLimit = 6;
		public float trunkAttachCurveWidth = 4;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				appearMode = s.Serialize<AppearMode>(appearMode, name: "appearMode");
				fadeDuration = s.Serialize<float>(fadeDuration, name: "fadeDuration");
				appearAnim = s.SerializeObject<StringID>(appearAnim, name: "appearAnim");
				idleAnim = s.SerializeObject<StringID>(idleAnim, name: "idleAnim");
				scaleActor = s.Serialize<bool>(scaleActor, name: "scaleActor", options: CSerializerObject.Options.BoolAsByte);
				trunkAttachCurveLimit = s.Serialize<float>(trunkAttachCurveLimit, name: "trunkAttachCurveLimit");
				trunkAttachCurveWidth = s.Serialize<float>(trunkAttachCurveWidth, name: "trunkAttachCurveWidth");
			} else {
				appearMode = s.Serialize<AppearMode>(appearMode, name: "appearMode");
				fadeDuration = s.Serialize<float>(fadeDuration, name: "fadeDuration");
				appearAnim = s.SerializeObject<StringID>(appearAnim, name: "appearAnim");
				idleAnim = s.SerializeObject<StringID>(idleAnim, name: "idleAnim");
				scaleActor = s.Serialize<bool>(scaleActor, name: "scaleActor");
				trunkAttachCurveLimit = s.Serialize<float>(trunkAttachCurveLimit, name: "trunkAttachCurveLimit");
				trunkAttachCurveWidth = s.Serialize<float>(trunkAttachCurveWidth, name: "trunkAttachCurveWidth");
			}
		}
		public enum AppearMode {
			[Serialize("AppearMode_None"        )] None = 0,
			[Serialize("AppearMode_ScaleAndFade")] ScaleAndFade = 1,
			[Serialize("AppearMode_Anim"        )] Anim = 2,
			[Serialize("AppearMode_Grow"        )] Grow = 3,
		}
		public override uint? ClassCRC => 0x22B261CD;
	}
}

