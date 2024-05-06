namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_GameMaterial_Template : GameMaterial_Template {
		public bool Bounce;
		public BOUNCETYPE BounceType;
		public bool Wave;
		public bool climbable;
		public bool hangable;
		public bool IgnoreCorners;
		public bool Character;
		public bool IgnoreLowRoof;
		public bool CanFruitStick;
		public bool climbVertical;
		public bool climbSlide;
		public bool climbForceSideJump;
		public bool noSquashDamage;
		public bool bounceOnCrushAttack;
		public uint Dangerous;
		public float movePlatformSpeedXMultiplier;
		public float movePlatformSpeedYMultiplier;
		public bool shooterBounce;
		public float djembe;
		public float float__19;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RFR) {
				Bounce = s.Serialize<bool>(Bounce, name: "Bounce");
				BounceType = s.Serialize<BOUNCETYPE>(BounceType, name: "BounceType");
				Wave = s.Serialize<bool>(Wave, name: "Wave");
				climbable = s.Serialize<bool>(climbable, name: "climbable");
				hangable = s.Serialize<bool>(hangable, name: "hangable");
				IgnoreCorners = s.Serialize<bool>(IgnoreCorners, name: "IgnoreCorners");
				Character = s.Serialize<bool>(Character, name: "Character");
				IgnoreLowRoof = s.Serialize<bool>(IgnoreLowRoof, name: "IgnoreLowRoof");
				CanFruitStick = s.Serialize<bool>(CanFruitStick, name: "CanFruitStick");
				climbVertical = s.Serialize<bool>(climbVertical, name: "climbVertical");
				climbSlide = s.Serialize<bool>(climbSlide, name: "climbSlide");
				climbForceSideJump = s.Serialize<bool>(climbForceSideJump, name: "climbForceSideJump");
				noSquashDamage = s.Serialize<bool>(noSquashDamage, name: "noSquashDamage");
				bounceOnCrushAttack = s.Serialize<bool>(bounceOnCrushAttack, name: "bounceOnCrushAttack");
				Dangerous = s.Serialize<uint>(Dangerous, name: "Dangerous");
				movePlatformSpeedXMultiplier = s.Serialize<float>(movePlatformSpeedXMultiplier, name: "movePlatformSpeedXMultiplier");
				movePlatformSpeedYMultiplier = s.Serialize<float>(movePlatformSpeedYMultiplier, name: "movePlatformSpeedYMultiplier");
				shooterBounce = s.Serialize<bool>(shooterBounce, name: "shooterBounce");
				djembe = s.Serialize<float>(djembe, name: "djembe");
				float__19 = s.Serialize<float>(float__19, name: "float__19");
			} else {
				Bounce = s.Serialize<bool>(Bounce, name: "Bounce");
				BounceType = s.Serialize<BOUNCETYPE>(BounceType, name: "BounceType");
				Wave = s.Serialize<bool>(Wave, name: "Wave");
				climbable = s.Serialize<bool>(climbable, name: "climbable");
				hangable = s.Serialize<bool>(hangable, name: "hangable");
				IgnoreCorners = s.Serialize<bool>(IgnoreCorners, name: "IgnoreCorners");
				Character = s.Serialize<bool>(Character, name: "Character");
				IgnoreLowRoof = s.Serialize<bool>(IgnoreLowRoof, name: "IgnoreLowRoof");
				CanFruitStick = s.Serialize<bool>(CanFruitStick, name: "CanFruitStick");
				climbVertical = s.Serialize<bool>(climbVertical, name: "climbVertical");
				climbSlide = s.Serialize<bool>(climbSlide, name: "climbSlide");
				climbForceSideJump = s.Serialize<bool>(climbForceSideJump, name: "climbForceSideJump");
				noSquashDamage = s.Serialize<bool>(noSquashDamage, name: "noSquashDamage");
				bounceOnCrushAttack = s.Serialize<bool>(bounceOnCrushAttack, name: "bounceOnCrushAttack");
				Dangerous = s.Serialize<uint>(Dangerous, name: "Dangerous");
				movePlatformSpeedXMultiplier = s.Serialize<float>(movePlatformSpeedXMultiplier, name: "movePlatformSpeedXMultiplier");
				movePlatformSpeedYMultiplier = s.Serialize<float>(movePlatformSpeedYMultiplier, name: "movePlatformSpeedYMultiplier");
				shooterBounce = s.Serialize<bool>(shooterBounce, name: "shooterBounce");
				djembe = s.Serialize<float>(djembe, name: "djembe");
			}
		}
		public enum BOUNCETYPE {
			[Serialize("BOUNCETYPE_ENEMY"            )] ENEMY = 1,
			[Serialize("BOUNCETYPE_BUMPER"           )] BUMPER = 2,
			[Serialize("BOUNCETYPE_POLYLINE"         )] POLYLINE = 3,
			[Serialize("BOUNCETYPE_WAVE"             )] WAVE = 4,
			[Serialize("BOUNCETYPE_WINDTUNEL"        )] WINDTUNEL = 5,
			[Serialize("BOUNCETYPE_TALKINGHAT"       )] TALKINGHAT = 6,
		}
		public override uint? ClassCRC => 0xA58BAE74;
	}
}

