namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIOceanSpiderBehavior_Template : TemplateAIBehavior {
		public CListO<Ray_AIOceanSpiderBehavior_Template.ActionTemplate> Actions;
		public CListO<Ray_AIOceanSpiderBehavior_Template.InstructionTemplate> Instructions;
		public CListO<Ray_AIOceanSpiderBehavior_Template.InstructionTemplate> InactiveInstructions;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			Actions = s.SerializeObject<CListO<Ray_AIOceanSpiderBehavior_Template.ActionTemplate>>(Actions, name: "Actions");
			Instructions = s.SerializeObject<CListO<Ray_AIOceanSpiderBehavior_Template.InstructionTemplate>>(Instructions, name: "Instructions");
			InactiveInstructions = s.SerializeObject<CListO<Ray_AIOceanSpiderBehavior_Template.InstructionTemplate>>(InactiveInstructions, name: "InactiveInstructions");
		}
		public override uint? ClassCRC => 0x70445B7F;

		[Games(GameFlags.RO)]
		public partial class InstructionTemplate : CSerializable {
			public StringID actionName;
			public uint playCount;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				actionName = s.SerializeObject<StringID>(actionName, name: "actionName");
				playCount = s.Serialize<uint>(playCount, name: "playCount");
			}
		}
		[Games(GameFlags.RO)]
		public partial class ActionTemplate : CSerializable {
			public StringID name;
			public Generic<AIAction_Template> action;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				name = s.SerializeObject<StringID>(name, name: "name");
				action = s.SerializeObject<Generic<AIAction_Template>>(action, name: "action");
			}
		}
	}
}

