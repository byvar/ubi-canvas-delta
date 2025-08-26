namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_DoorTeleport : EventTeleport {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE5366E4D;
	}
}

