namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_InventoryAssetsComponent_Template : ActorComponent_Template {
		public bool spawnEquippedItems;
		public bool renderToParentTexture;
		public string wingsMeshName;
		public CListO<StringID> defaultItems;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			spawnEquippedItems = s.Serialize<bool>(spawnEquippedItems, name: "spawnEquippedItems", options: CSerializerObject.Options.BoolAsByte);
			renderToParentTexture = s.Serialize<bool>(renderToParentTexture, name: "renderToParentTexture", options: CSerializerObject.Options.BoolAsByte);
			wingsMeshName = s.Serialize<string>(wingsMeshName, name: "wingsMeshName");
			defaultItems = s.SerializeObject<CListO<StringID>>(defaultItems, name: "defaultItems");
		}
		public override uint? ClassCRC => 0x193E8B71;
	}
}

