namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class LinkComponent : ActorComponent {
		public CListO<ChildEntry> Children = new CListO<ChildEntry>();
		public CListO<ObjectId> ChildrenObjId;
		public CListO<ObjectPath> LinkedChildren;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					Children = s.SerializeObject<CListO<ChildEntry>>(Children, name: "Children");
				}
				if (!s.HasProperties(SerializerProperties.Binary) && s.HasFlags(SerializeFlags.Data_Load)) {
					ChildrenObjId = s.SerializeObject<CListO<ObjectId>>(ChildrenObjId, name: "ChildrenObjId");
					LinkedChildren = s.SerializeObject<CListO<ObjectPath>>(LinkedChildren, name: "LinkedChildren");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					Children = s.SerializeObject<CListO<ChildEntry>>(Children, name: "Children");
				}
			}
		}
		public override uint? ClassCRC => 0x44376F1B;
	}
}

