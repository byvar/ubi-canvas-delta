namespace UbiArt.ITF {
	[Games(GameFlags.LegendsAndUp)]
	public partial class UIScrollbar : UIComponent {
		public StringID textBoxFriendly;
		public float speed;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				textBoxFriendly = s.SerializeObject<StringID>(textBoxFriendly, name: "textBoxFriendly");
				speed = s.Serialize<float>(speed, name: "speed");
			}
		}
		public override uint? ClassCRC => 0x6B682541;
	}
}

