namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_BasicBullet_Template : CSerializable {
		public Generic<PhysShape> stimShape;
		public FACTION faction;
		public float lifetime;
		public uint numHits;
		public int skipSuccessiveHits;
		public RECEIVEDHITTYPE hitType;
		public uint hitLevel;
		public int destroyOnExitScreen;
		public int destroyOnEnvironment;
		public int checkEnvironmentHit;
		public int checkEnvironment;
		public Vec2d defaultSpeed;
		public float rotationSpeed;
		public int allowBounce;
		public uint bounceMaxNb;
		public int cameraRelative;
		public int autoSeek;
		public float autoSeekStartDelay;
		public float autoSeekRefreshDelay;
		public float autoSeekRange;
		public Angle autoSeekMaxTurnAngle;
		public StringID fxNameImpact;
		public StringID fxNameBounce;
		public StringID fxNamePaf;
		public StringID fxNameDeath;
		public int fxKillAllBefore;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			stimShape = s.SerializeObject<Generic<PhysShape>>(stimShape, name: "stimShape");
			faction = s.Serialize<FACTION>(faction, name: "faction");
			lifetime = s.Serialize<float>(lifetime, name: "lifetime");
			numHits = s.Serialize<uint>(numHits, name: "numHits");
			skipSuccessiveHits = s.Serialize<int>(skipSuccessiveHits, name: "skipSuccessiveHits");
			hitType = s.Serialize<RECEIVEDHITTYPE>(hitType, name: "hitType");
			hitLevel = s.Serialize<uint>(hitLevel, name: "hitLevel");
			destroyOnExitScreen = s.Serialize<int>(destroyOnExitScreen, name: "destroyOnExitScreen");
			destroyOnEnvironment = s.Serialize<int>(destroyOnEnvironment, name: "destroyOnEnvironment");
			checkEnvironmentHit = s.Serialize<int>(checkEnvironmentHit, name: "checkEnvironmentHit");
			checkEnvironment = s.Serialize<int>(checkEnvironment, name: "checkEnvironment");
			defaultSpeed = s.SerializeObject<Vec2d>(defaultSpeed, name: "defaultSpeed");
			rotationSpeed = s.Serialize<float>(rotationSpeed, name: "rotationSpeed");
			allowBounce = s.Serialize<int>(allowBounce, name: "allowBounce");
			bounceMaxNb = s.Serialize<uint>(bounceMaxNb, name: "bounceMaxNb");
			cameraRelative = s.Serialize<int>(cameraRelative, name: "cameraRelative");
			autoSeek = s.Serialize<int>(autoSeek, name: "autoSeek");
			autoSeekStartDelay = s.Serialize<float>(autoSeekStartDelay, name: "autoSeekStartDelay");
			autoSeekRefreshDelay = s.Serialize<float>(autoSeekRefreshDelay, name: "autoSeekRefreshDelay");
			autoSeekRange = s.Serialize<float>(autoSeekRange, name: "autoSeekRange");
			autoSeekMaxTurnAngle = s.SerializeObject<Angle>(autoSeekMaxTurnAngle, name: "autoSeekMaxTurnAngle");
			fxNameImpact = s.SerializeObject<StringID>(fxNameImpact, name: "fxNameImpact");
			fxNameBounce = s.SerializeObject<StringID>(fxNameBounce, name: "fxNameBounce");
			fxNamePaf = s.SerializeObject<StringID>(fxNamePaf, name: "fxNamePaf");
			fxNameDeath = s.SerializeObject<StringID>(fxNameDeath, name: "fxNameDeath");
			fxKillAllBefore = s.Serialize<int>(fxKillAllBefore, name: "fxKillAllBefore");
		}
		public enum FACTION {
			[Serialize("FACTION_UNKNOWN"                     )] FACTION_UNKNOWN = -1,
			[Serialize("RAY_FACTION_NEUTRAL"                 )] RAY_FACTION_NEUTRAL = 0,
			[Serialize("RAY_FACTION_FRIENDLY"                )] RAY_FACTION_FRIENDLY = 1,
			[Serialize("RAY_FACTION_ENEMY"                   )] RAY_FACTION_ENEMY = 2,
			[Serialize("RAY_FACTION_MEGAENEMY"               )] RAY_FACTION_MEGAENEMY = 3,
			[Serialize("RAY_FACTION_DARKTOON"                )] RAY_FACTION_DARKTOON = 4,
			[Serialize("RAY_FACTION_FISH"                    )] RAY_FACTION_FISH = 5,
			[Serialize("RAY_FACTION_DARKTOONWALL"            )] RAY_FACTION_DARKTOONWALL = 6,
			[Serialize("RAY_FACTION_PLUM"                    )] RAY_FACTION_PLUM = 7,
			[Serialize("RAY_FACTION_BREAKABLE"               )] RAY_FACTION_BREAKABLE = 8,
			[Serialize("RAY_FACTION_PLAYER"                  )] RAY_FACTION_PLAYER = 9,
			[Serialize("RAY_FACTION_BOMBDEFUSED"             )] RAY_FACTION_BOMBDEFUSED = 10,
			[Serialize("RAY_FACTION_RELICCHEST"              )] RAY_FACTION_RELICCHEST = 11,
			[Serialize("RAY_FACTION_GHOST"                   )] RAY_FACTION_GHOST = 12,
			[Serialize("RAY_FACTION_ELECTOON"                )] RAY_FACTION_ELECTOON = 13,
			[Serialize("RAY_FACTION_BOSSMORAY"               )] RAY_FACTION_BOSSMORAY = 14,
			[Serialize("RAY_FACTION_BUMPER"                  )] RAY_FACTION_BUMPER = 15,
			[Serialize("RAY_FACTION_PROJECTILE"              )] RAY_FACTION_PROJECTILE = 16,
			[Serialize("RAY_FACTION_EXPLOSION"               )] RAY_FACTION_EXPLOSION = 19,
			[Serialize("RAY_FACTION_BLOWFISH"                )] RAY_FACTION_BLOWFISH = 20,
			[Serialize("RAY_FACTION_BUBBLE"                  )] RAY_FACTION_BUBBLE = 21,
			[Serialize("RAY_FACTION_ENEMYBULLET"             )] RAY_FACTION_ENEMYBULLET = 22,
			[Serialize("RAY_FACTION_MEGAENEMYEXCEPTBREAKABLE")] RAY_FACTION_MEGAENEMYEXCEPTBREAKABLE = 23,
		}
		public enum RECEIVEDHITTYPE {
			[Serialize("RECEIVEDHITTYPE_UNKNOWN"    )] UNKNOWN = -1,
			[Serialize("RECEIVEDHITTYPE_FRONTPUNCH" )] FRONTPUNCH = 0,
			[Serialize("RECEIVEDHITTYPE_UPPUNCH"    )] UPPUNCH = 1,
			[Serialize("RECEIVEDHITTYPE_EJECTXY"    )] EJECTXY = 3,
			[Serialize("RECEIVEDHITTYPE_HURTBOUNCE" )] HURTBOUNCE = 4,
			[Serialize("RECEIVEDHITTYPE_DARKTOONIFY")] DARKTOONIFY = 5,
			[Serialize("RECEIVEDHITTYPE_EARTHQUAKE" )] EARTHQUAKE = 6,
			[Serialize("RECEIVEDHITTYPE_SHOOTER"    )] SHOOTER = 7,
		}
		public override uint? ClassCRC => 0xC00229D2;
	}
}

