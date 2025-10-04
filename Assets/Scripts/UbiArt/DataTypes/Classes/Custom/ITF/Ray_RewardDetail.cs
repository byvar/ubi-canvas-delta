namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_RewardDetail : CSerializable {
		public StringID id;
		public StringID name;
		public CArrayO<Generic<Ray_RewardTrigger_Base>> REWARD_TRIGGER;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			id = s.SerializeObject<StringID>(id, name: "id");
			name = s.SerializeObject<StringID>(name, name: "name");
			REWARD_TRIGGER = s.SerializeObject<CArrayO<Generic<Ray_RewardTrigger_Base>>>(REWARD_TRIGGER, name: "REWARD_TRIGGER");
		}
	}
}

