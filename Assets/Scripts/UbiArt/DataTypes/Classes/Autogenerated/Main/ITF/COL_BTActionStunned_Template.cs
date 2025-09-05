namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_BTActionStunned_Template : COL_BTActionBase_Template {
		public StringID anim;
		public StringID fx;
		public float stunDuration;
		public float minStunDuration;
		public float postStunDuration;
		public float minTimeBetween2Stuns;
		public float secondStunThreshold;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			anim = s.SerializeObject<StringID>(anim, name: "anim");
			fx = s.SerializeObject<StringID>(fx, name: "fx");
			stunDuration = s.Serialize<float>(stunDuration, name: "stunDuration");
			minStunDuration = s.Serialize<float>(minStunDuration, name: "minStunDuration");
			postStunDuration = s.Serialize<float>(postStunDuration, name: "postStunDuration");
			minTimeBetween2Stuns = s.Serialize<float>(minTimeBetween2Stuns, name: "minTimeBetween2Stuns");
			secondStunThreshold = s.Serialize<float>(secondStunThreshold, name: "secondStunThreshold");
		}
		public override uint? ClassCRC => 0x568F2BFF;
	}
}

