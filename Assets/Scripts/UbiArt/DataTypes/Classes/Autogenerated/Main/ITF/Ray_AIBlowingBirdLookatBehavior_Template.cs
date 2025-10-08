namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIBlowingBirdLookatBehavior_Template : TemplateAIBehavior {
		public Generic<AIAction_Template> uturn;
		public Generic<AIAction_Template> attack;
		public float beginDuration;
		public Angle rotationSpeed;
		public Angle minAngle;
		public Angle maxAngle;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			uturn = s.SerializeObject<Generic<AIAction_Template>>(uturn, name: "uturn");
			attack = s.SerializeObject<Generic<AIAction_Template>>(attack, name: "attack");
			beginDuration = s.Serialize<float>(beginDuration, name: "beginDuration");
			rotationSpeed = s.SerializeObject<Angle>(rotationSpeed, name: "rotationSpeed");
			minAngle = s.SerializeObject<Angle>(minAngle, name: "minAngle");
			maxAngle = s.SerializeObject<Angle>(maxAngle, name: "maxAngle");
		}
		public override uint? ClassCRC => 0x374B614A;
	}
}

