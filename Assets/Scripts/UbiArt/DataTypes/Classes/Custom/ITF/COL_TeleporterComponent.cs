namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TeleporterComponent : ActorComponent {
		public StringID interactionAction;
		public COL_TeleportTargetComponent.COL_TeleportInfo teleportInfos;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Deprecate)) {
				interactionAction = s.SerializeObject<StringID>(interactionAction, name: "interactionAction");
			}
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				teleportInfos = s.SerializeObject<COL_TeleportTargetComponent.COL_TeleportInfo>(teleportInfos, name: "teleportInfos");
			}
		}
		public override uint? ClassCRC => 0xA136B948;
	}
}

