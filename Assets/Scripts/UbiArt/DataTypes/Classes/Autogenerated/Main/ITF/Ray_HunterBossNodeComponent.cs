namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_HunterBossNodeComponent : LinkComponent {
		public Placeholder data;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				data = s.SerializeObject<Placeholder>(data, name: "data");
			}
		}
		public override uint? ClassCRC => 0xC3B84AD9;
	}
}

