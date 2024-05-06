namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_TriggerBounceComponent_Template : ActorComponent_Template {
		public Generic<PhysShape> phantom;
		public uint hurtLevel;
		public BOUNCETYPE bounceType;
		public uint faction;
		public float retriggerDelay;
		public StringID idleAnim;
		public StringID bounceAnim;
		public Vec2d direction;
		public bool useDirAsWorldAngle;
		public bool stickFruit;
		public bool sendBounceOnHit;
		public bool radial;
		public bool radialConstrained;
		public float height;
		public float height2;
		public float speed;
		public float multiplier;
		public Vec2d offset;
		public Angle detectRange;
		public bool sendMoreThanOnce;
		public bool useBounceToPickable;
		public bool useAngleToDiscardBounceToPickable;
		public uint ignoreHitLevel;
		public bool blocksHits;
		public bool disableAtBounce;
		public bool registerToAIManager;
		public bool playBounceAnimOnHit;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			phantom = s.SerializeObject<Generic<PhysShape>>(phantom, name: "phantom");
			hurtLevel = s.Serialize<uint>(hurtLevel, name: "hurtLevel");
			bounceType = s.Serialize<BOUNCETYPE>(bounceType, name: "bounceType");
			faction = s.Serialize<uint>(faction, name: "faction");
			retriggerDelay = s.Serialize<float>(retriggerDelay, name: "retriggerDelay");
			idleAnim = s.SerializeObject<StringID>(idleAnim, name: "idleAnim");
			bounceAnim = s.SerializeObject<StringID>(bounceAnim, name: "bounceAnim");
			direction = s.SerializeObject<Vec2d>(direction, name: "direction");
			useDirAsWorldAngle = s.Serialize<bool>(useDirAsWorldAngle, name: "useDirAsWorldAngle");
			stickFruit = s.Serialize<bool>(stickFruit, name: "stickFruit");
			sendBounceOnHit = s.Serialize<bool>(sendBounceOnHit, name: "sendBounceOnHit");
			radial = s.Serialize<bool>(radial, name: "radial");
			radialConstrained = s.Serialize<bool>(radialConstrained, name: "radialConstrained");
			height = s.Serialize<float>(height, name: "height");
			height2 = s.Serialize<float>(height2, name: "height2");
			speed = s.Serialize<float>(speed, name: "speed");
			multiplier = s.Serialize<float>(multiplier, name: "multiplier");
			offset = s.SerializeObject<Vec2d>(offset, name: "offset");
			detectRange = s.SerializeObject<Angle>(detectRange, name: "detectRange");
			sendMoreThanOnce = s.Serialize<bool>(sendMoreThanOnce, name: "sendMoreThanOnce");
			useBounceToPickable = s.Serialize<bool>(useBounceToPickable, name: "useBounceToPickable");
			useAngleToDiscardBounceToPickable = s.Serialize<bool>(useAngleToDiscardBounceToPickable, name: "useAngleToDiscardBounceToPickable");
			ignoreHitLevel = s.Serialize<uint>(ignoreHitLevel, name: "ignoreHitLevel");
			blocksHits = s.Serialize<bool>(blocksHits, name: "blocksHits");
			disableAtBounce = s.Serialize<bool>(disableAtBounce, name: "disableAtBounce");
			registerToAIManager = s.Serialize<bool>(registerToAIManager, name: "registerToAIManager");
			playBounceAnimOnHit = s.Serialize<bool>(playBounceAnimOnHit, name: "playBounceAnimOnHit");
		}
		public enum BOUNCETYPE {
			[Serialize("BOUNCETYPE_ENEMY" )] ENEMY = 1,
			[Serialize("BOUNCETYPE_BUMPER")] BUMPER = 2,
		}
		public override uint? ClassCRC => 0x21368B71;
	}
}

