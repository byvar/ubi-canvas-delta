namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class BezierCurveComponent : ActorComponent {
		public BezierCurve bezier;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.COL) {
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					bezier = s.SerializeObject<BezierCurve>(bezier, name: "bezier");
				}
			}
		}
		public override uint? ClassCRC => 0x75ABD328;
	}
}

