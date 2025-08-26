namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class UIMenuPageComponent : ActorComponent {
		public uint TriggerButton;
		public Vec3d RelativePosFromCamera;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				TriggerButton = s.Serialize<uint>(TriggerButton, name: "TriggerButton");
				RelativePosFromCamera = s.SerializeObject<Vec3d>(RelativePosFromCamera, name: "RelativePosFromCamera");
			}
		}
		public override uint? ClassCRC => 0x5E3B408B;
	}
}

