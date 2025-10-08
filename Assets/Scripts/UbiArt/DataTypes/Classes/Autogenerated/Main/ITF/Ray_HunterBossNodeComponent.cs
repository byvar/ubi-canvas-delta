namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_HunterBossNodeComponent : LinkComponent {
		public Ray_EventQueryHunterNode data;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				data = s.SerializeObject<Ray_EventQueryHunterNode>(data, name: "data");
			}
		}
		public override uint? ClassCRC => 0xC3B84AD9;
	}
}

