namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_ShooterSpawnerModifierComponent_Template : TimedSpawnerModifierComponent_Template {
		public CListO<StringID> tweenInstructionSetList;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			tweenInstructionSetList = s.SerializeObject<CListO<StringID>>(tweenInstructionSetList, name: "tweenInstructionSetList");
		}
		public override uint? ClassCRC => 0xB9AEB965;
	}
}

