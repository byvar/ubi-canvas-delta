namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_DoorComponent : ActorComponent {
		public float detectRange;
		public bool automaticOpening;
		public float openCursor;
		public bool isLocked;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				detectRange = s.Serialize<float>(detectRange, name: "detectRange");
				automaticOpening = s.Serialize<bool>(automaticOpening, name: "automaticOpening");
			}
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				openCursor = s.Serialize<float>(openCursor, name: "openCursor");
				isLocked = s.Serialize<bool>(isLocked, name: "isLocked");
			}
		}
		public override uint? ClassCRC => 0x45E0160D;
	}
}

