namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_ChangeRoomDoorComponent : ActorComponent {
		public int goIn;
		public int goOut;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				goIn = s.Serialize<int>(goIn, name: "goIn");
				goOut = s.Serialize<int>(goOut, name: "goOut");
			}
		}
		public override uint? ClassCRC => 0xF7BEB94A;
	}
}

