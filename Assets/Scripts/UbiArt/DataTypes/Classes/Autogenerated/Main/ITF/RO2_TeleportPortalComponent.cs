namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class RO2_TeleportPortalComponent : ActorComponent {
		public bool oneWay;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				oneWay = s.Serialize<bool>(oneWay, name: "oneWay");
			}
		}
		public override uint? ClassCRC => 0x3A5A0D4E;
	}
}

