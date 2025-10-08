namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIDarktoonusCyclopeusBehavior_Template : TemplateAIBehavior {
		public Generic<AIPlayAnimAction_Template> idle;
		public Generic<AIPlayAnimAction_Template> prepareFocus;
		public Generic<AIPlayAnimAction_Template> focus;
		public Generic<AIAction_Template> attack;
		public Generic<AIPlayAnimAction_Template> waitHit;
		public float distanceDetect;
		public Angle angleMaxDetect;
		public float countDownAttack;
		public float speed;
		public float speedBack;
		public float timeWaiting;
		public float timeFocus;
		public int mustBeStickToAttack;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			idle = s.SerializeObject<Generic<AIPlayAnimAction_Template>>(idle, name: "idle");
			prepareFocus = s.SerializeObject<Generic<AIPlayAnimAction_Template>>(prepareFocus, name: "prepareFocus");
			focus = s.SerializeObject<Generic<AIPlayAnimAction_Template>>(focus, name: "focus");
			attack = s.SerializeObject<Generic<AIAction_Template>>(attack, name: "attack");
			waitHit = s.SerializeObject<Generic<AIPlayAnimAction_Template>>(waitHit, name: "waitHit");
			distanceDetect = s.Serialize<float>(distanceDetect, name: "distanceDetect");
			angleMaxDetect = s.SerializeObject<Angle>(angleMaxDetect, name: "angleMaxDetect");
			countDownAttack = s.Serialize<float>(countDownAttack, name: "countDownAttack");
			speed = s.Serialize<float>(speed, name: "speed");
			speedBack = s.Serialize<float>(speedBack, name: "speedBack");
			timeWaiting = s.Serialize<float>(timeWaiting, name: "timeWaiting");
			timeFocus = s.Serialize<float>(timeFocus, name: "timeFocus");
			mustBeStickToAttack = s.Serialize<int>(mustBeStickToAttack, name: "mustBeStickToAttack");
		}
		public override uint? ClassCRC => 0x24D2B959;
	}
}

