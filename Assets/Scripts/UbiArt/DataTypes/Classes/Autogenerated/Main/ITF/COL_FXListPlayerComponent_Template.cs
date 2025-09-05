namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_FXListPlayerComponent_Template : ActorComponent_Template {
		public CListO<COL_FXListPlayerComponent_Template.FXListEntry> fxList;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			fxList = s.SerializeObject<CListO<COL_FXListPlayerComponent_Template.FXListEntry>>(fxList, name: "fxList");
		}
		public override uint? ClassCRC => 0x655B4AA1;

		[Games(GameFlags.COL)]
		public partial class FXListEntry : CSerializable {
			public StringID fxName;
			public float startTimeMin;
			public float startTimeMax;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				fxName = s.SerializeObject<StringID>(fxName, name: "fxName");
				startTimeMin = s.Serialize<float>(startTimeMin, name: "startTimeMin");
				startTimeMax = s.Serialize<float>(startTimeMax, name: "startTimeMax");
			}
		}
	}
}

