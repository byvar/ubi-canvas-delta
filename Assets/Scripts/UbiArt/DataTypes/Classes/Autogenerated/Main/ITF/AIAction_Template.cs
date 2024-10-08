namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RL | GameFlags.RAVersion)]
	public partial class AIAction_Template : CSerializable {
		public StringID action;
		public StringID endMarker;
		public bool useRootPos;
		public bool finishOnContact;
		public Vec2d rootPosScale;
		public float ignoreContactDuration;
		public string debugName;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion <= EngineVersion.RO || s.Settings.Platform == GamePlatform.Vita) {
				if (this is AIDestroyAction_Template or AIDisableAction_Template) return;
			}
			action = s.SerializeObject<StringID>(action, name: "action");
			endMarker = s.SerializeObject<StringID>(endMarker, name: "endMarker");
			useRootPos = s.Serialize<bool>(useRootPos, name: "useRootPos");
			finishOnContact = s.Serialize<bool>(finishOnContact, name: "finishOnContact");
			rootPosScale = s.SerializeObject<Vec2d>(rootPosScale, name: "rootPosScale");
			ignoreContactDuration = s.Serialize<float>(ignoreContactDuration, name: "ignoreContactDuration");
			if (s.Settings.EngineVersion == EngineVersion.RO) {
				if (!s.HasProperties(SerializerProperties.Binary) && s.HasFlags(SerializeFlags.Group_Data)) {
					debugName = s.Serialize<string>(debugName, name: "debugName");
				}
			}
		}
		public override uint? ClassCRC => 0xA6F57F72;
	}
}

