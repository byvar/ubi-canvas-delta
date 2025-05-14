namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_WM_ControllerComponent_Template : ActorComponent_Template {
		public float moveX;
		public float moveFactorMultiplier;
		public StringID standAnim;
		public StringID walkAnim;
		public StringID jumpAnim;
		public StringID swimAnim;
		public StringID flyAnim;
		public Ray_WM_LeadAbility_Template leadAbility;
		public Ray_WM_FollowAbility_Template followAbility;
		public Ray_HeartShield_Template heartShield;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			moveX = s.Serialize<float>(moveX, name: "moveX");
			moveFactorMultiplier = s.Serialize<float>(moveFactorMultiplier, name: "moveFactorMultiplier");
			standAnim = s.SerializeObject<StringID>(standAnim, name: "standAnim");
			walkAnim = s.SerializeObject<StringID>(walkAnim, name: "walkAnim");
			jumpAnim = s.SerializeObject<StringID>(jumpAnim, name: "jumpAnim");
			swimAnim = s.SerializeObject<StringID>(swimAnim, name: "swimAnim");
			flyAnim = s.SerializeObject<StringID>(flyAnim, name: "flyAnim");
			leadAbility = s.SerializeObject<Ray_WM_LeadAbility_Template>(leadAbility, name: "leadAbility");
			followAbility = s.SerializeObject<Ray_WM_FollowAbility_Template>(followAbility, name: "followAbility");
			heartShield = s.SerializeObject<Ray_HeartShield_Template>(heartShield, name: "heartShield");
		}
		public override uint? ClassCRC => 0x2B26168C;
	}
}

