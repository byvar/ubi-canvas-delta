namespace UbiArt.ITF {
	[Games(GameFlags.RJR | GameFlags.RFR)]
	public partial class LevelInfo : CSerializable {
		public BasicString BasicString__0;
		public BasicString BasicString__1;
		public uint uint__2;
		public RJR_PowerUp RJR_PowerUp__3;
		public uint uint__8;
		public Gadgets Gadgets__9;
		public CListO<UnlockedGadgets> CList_UnlockedGadgets__10;
		public Gadgets Gadgets__11;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RFR) {
				BasicString__0 = s.Serialize<BasicString>(BasicString__0, name: "BasicString__0");
				BasicString__1 = s.Serialize<BasicString>(BasicString__1, name: "BasicString__1");
				uint__2 = s.Serialize<uint>(uint__2, name: "uint__2");
				RJR_PowerUp__3 = s.SerializeObject<RJR_PowerUp>(RJR_PowerUp__3, name: "RJR_PowerUp__3");
				uint__8 = s.Serialize<uint>(uint__8, name: "uint__8");
				Gadgets__9 = s.SerializeObject<Gadgets>(Gadgets__9, name: "Gadgets__9");
				CList_UnlockedGadgets__10 = s.SerializeObject<CListO<UnlockedGadgets>>(CList_UnlockedGadgets__10, name: "CList<UnlockedGadgets>__10");
				Gadgets__11 = s.SerializeObject<Gadgets>(Gadgets__11, name: "Gadgets__11");
			} else {
				BasicString__0 = s.Serialize<BasicString>(BasicString__0, name: "BasicString__0");
				BasicString__1 = s.Serialize<BasicString>(BasicString__1, name: "BasicString__1");
				uint__2 = s.Serialize<uint>(uint__2, name: "uint__2");
				RJR_PowerUp__3 = s.SerializeObject<RJR_PowerUp>(RJR_PowerUp__3, name: "RJR_PowerUp__3");
			}
		}
	}
}

