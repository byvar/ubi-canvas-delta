namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class PolyLine : BaseObject {
		public PolyPointList PolyPointList = new PolyPointList();
		public AABB AABB = new AABB();
		public PolyLine.Connection connection = new Connection();
		public CListO<PolyLineEdge> POINTS;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_DataEditable)) {
					POINTS = s.SerializeObject<CListO<PolyLineEdge>>(POINTS, name: "POINTS");
				}
			} else {
				PolyPointList = s.SerializeObject<PolyPointList>(PolyPointList, name: "PolyPointList");
				AABB = s.SerializeObject<AABB>(AABB, name: "AABB");
				if (s.HasFlags(SerializeFlags.DataBin)) {
					connection = s.SerializeObject<PolyLine.Connection>(connection, name: "Connection");
				}
			}
		}
		[Games(GameFlags.VH | GameFlags.RA)]
		public partial class Connection : CSerializable {
			public uint PreviousId = 0xFFFFFFFF;
			public Vec2d PosInit;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				PreviousId = s.Serialize<uint>(PreviousId, name: "PreviousId");
				PosInit = s.SerializeObject<Vec2d>(PosInit, name: "PosInit");
			}
		}
		public override uint? ClassCRC => 0x732A7AA3;
	}
}

