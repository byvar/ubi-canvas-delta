namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class BlendTreeNodeBlend_Template<T> : BlendTreeNodeTemplate<T> {
		public CListO<Generic<BlendTreeNodeTemplate<T>>> leafs;
		public bool ignoreRuleChanges;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion <= EngineVersion.RO && this is MusicTreeNodePlayMusic_Template) return;
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				leafs = s.SerializeObject<CListO<Generic<BlendTreeNodeTemplate<T>>>>(leafs, name: "leafs");
			}
			ignoreRuleChanges = s.Serialize<bool>(ignoreRuleChanges, name: "ignoreRuleChanges");
		}
		public override uint? ClassCRC => 0x422981DB;
	}
}

