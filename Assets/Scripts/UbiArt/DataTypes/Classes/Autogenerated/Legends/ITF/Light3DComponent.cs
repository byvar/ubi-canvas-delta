namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion)]
	public partial class Light3DComponent : ActorComponent {
		public float near;
		public float far;
		public Color color;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				near = s.Serialize<float>(near, name: "near");
				far = s.Serialize<float>(far, name: "far");
				color = s.SerializeObject<Color>(color, name: "color");
			}
		}
		public override uint? ClassCRC => 0x6734D562;
	}
}

