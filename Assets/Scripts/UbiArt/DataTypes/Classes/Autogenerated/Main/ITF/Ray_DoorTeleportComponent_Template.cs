namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_DoorTeleportComponent_Template : ActorComponent_Template {
		public StringID exitPointId;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			exitPointId = s.SerializeObject<StringID>(exitPointId, name: "exitPointId");
		}
		public override uint? ClassCRC => 0x6655D7D2;
	}
}

