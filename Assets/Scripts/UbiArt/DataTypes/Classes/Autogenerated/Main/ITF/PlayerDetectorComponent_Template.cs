namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PlayerDetectorComponent_Template : ShapeDetectorComponent_Template {
		public bool allowDeadActors;
		public bool firstPlayerOnly;
		public uint allPlayerInMode = 0xffffffff;
		public float maxDetectionRadius = 5f;
		public bool allowTouchScreenPlayer;
		public bool ignoreZ;
		public bool drcPlayerOnly;
		public bool detectPivotOnly;
		public int playerId;
		public int playerToDetect;
		public bool allowAIControlledPlayer;

		public int vitaProperty;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				playerId = s.Serialize<int>(playerId, name: "playerId");
				allowDeadActors = s.Serialize<bool>(allowDeadActors, name: "allowDeadActors");
				firstPlayerOnly = s.Serialize<bool>(firstPlayerOnly, name: "firstPlayerOnly");
				allPlayerInMode = s.Serialize<uint>(allPlayerInMode, name: "allPlayerInMode");
				maxDetectionRadius = s.Serialize<float>(maxDetectionRadius, name: "maxDetectionRadius");
				if (s.Settings.Platform == GamePlatform.Vita) {
					vitaProperty = s.Serialize<int>(vitaProperty, name: nameof(vitaProperty));
				}
			} else if (s.Settings.Game == Game.RL) {
				allowDeadActors = s.Serialize<bool>(allowDeadActors, name: "allowDeadActors");
				firstPlayerOnly = s.Serialize<bool>(firstPlayerOnly, name: "firstPlayerOnly");
				allPlayerInMode = s.Serialize<uint>(allPlayerInMode, name: "allPlayerInMode");
				maxDetectionRadius = s.Serialize<float>(maxDetectionRadius, name: "maxDetectionRadius");
				allowTouchScreenPlayer = s.Serialize<bool>(allowTouchScreenPlayer, name: "allowTouchScreenPlayer");
				ignoreZ = s.Serialize<bool>(ignoreZ, name: "ignoreZ");
				drcPlayerOnly = s.Serialize<bool>(drcPlayerOnly, name: "drcPlayerOnly");
			} else if (s.Settings.Game == Game.COL) {
				allowDeadActors = s.Serialize<bool>(allowDeadActors, name: "allowDeadActors");
				playerToDetect = s.Serialize<int>(playerToDetect, name: "playerToDetect");
				allPlayerInMode = s.Serialize<uint>(allPlayerInMode, name: "allPlayerInMode");
				maxDetectionRadius = s.Serialize<float>(maxDetectionRadius, name: "maxDetectionRadius");
				allowTouchScreenPlayer = s.Serialize<bool>(allowTouchScreenPlayer, name: "allowTouchScreenPlayer");
				allowAIControlledPlayer = s.Serialize<bool>(allowAIControlledPlayer, name: "allowAIControlledPlayer");
				ignoreZ = s.Serialize<bool>(ignoreZ, name: "ignoreZ");
				drcPlayerOnly = s.Serialize<bool>(drcPlayerOnly, name: "drcPlayerOnly");
			} else {
				allowDeadActors = s.Serialize<bool>(allowDeadActors, name: "allowDeadActors");
				firstPlayerOnly = s.Serialize<bool>(firstPlayerOnly, name: "firstPlayerOnly");
				allPlayerInMode = s.Serialize<uint>(allPlayerInMode, name: "allPlayerInMode");
				maxDetectionRadius = s.Serialize<float>(maxDetectionRadius, name: "maxDetectionRadius");
				allowTouchScreenPlayer = s.Serialize<bool>(allowTouchScreenPlayer, name: "allowTouchScreenPlayer");
				ignoreZ = s.Serialize<bool>(ignoreZ, name: "ignoreZ");
				drcPlayerOnly = s.Serialize<bool>(drcPlayerOnly, name: "drcPlayerOnly");
				detectPivotOnly = s.Serialize<bool>(detectPivotOnly, name: "detectPivotOnly");
			}
		}
		public override uint? ClassCRC => 0x3A92D482;
	}
}

