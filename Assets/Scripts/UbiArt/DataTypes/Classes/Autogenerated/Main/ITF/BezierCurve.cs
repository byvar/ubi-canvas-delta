namespace UbiArt.ITF {
	[Games(GameFlags.RJR | GameFlags.RFR | GameFlags.VH | GameFlags.RA)]
	public partial class BezierCurve : CSerializable {
		public CListO<BezierCurve.Point> points;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				points = s.SerializeObject<CListO<BezierCurve.Point>>(points, name: "points");
			}
		}
		[Games(GameFlags.RJR | GameFlags.RFR | GameFlags.VH | GameFlags.RA)]
		public partial class Point : CSerializable {
			public Vec3d pos;
			public Vec3d tanA;
			public Vec3d tanB;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					pos = s.SerializeObject<Vec3d>(pos, name: "pos");
					tanA = s.SerializeObject<Vec3d>(tanA, name: "tanA");
					tanB = s.SerializeObject<Vec3d>(tanB, name: "tanB");
				}
			}
		}
	}
}

