namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_ShooterTurretAIComponent : Ray_MultiPiecesActorAIComponent {
		public TimedSpawnerData timedSpawnerData;
		public float fixedAngle;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			timedSpawnerData = s.SerializeObject<TimedSpawnerData>(timedSpawnerData, name: "timedSpawnerData");
			fixedAngle = s.Serialize<float>(fixedAngle, name: "fixedAngle");
		}
		public override uint? ClassCRC => 0xC97160CC;
	}
}

