namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class PolyPointList : CSerializable {
		public CListO<PolyLineEdge> LocalPoints = new CListO<PolyLineEdge>();
		public bool Loop;
		public float Length;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			LocalPoints = s.SerializeObject<CListO<PolyLineEdge>>(LocalPoints, name: "LocalPoints");
			Loop = s.Serialize<bool>(Loop, name: "Loop");
			if (s.HasFlags(SerializeFlags.DataBin)) {
				Length = s.Serialize<float>(Length, name: "Length");
			}
		}
	}
}

