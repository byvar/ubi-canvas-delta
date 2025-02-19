namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_HeartNfcComponent_Template : ActorComponent_Template {
		public float zOffset;
		public float smooth = 0.01f;
		public float doubleSmooth = 0.01f;
		public float spiralCount = 5;
		public float spiralAmplitude = 5;
		public float targetReachedDistance = 1;
		public float fadeOutDuration = 0.2f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			zOffset = s.Serialize<float>(zOffset, name: "zOffset");
			smooth = s.Serialize<float>(smooth, name: "smooth");
			doubleSmooth = s.Serialize<float>(doubleSmooth, name: "doubleSmooth");
			spiralCount = s.Serialize<float>(spiralCount, name: "spiralCount");
			spiralAmplitude = s.Serialize<float>(spiralAmplitude, name: "spiralAmplitude");
			targetReachedDistance = s.Serialize<float>(targetReachedDistance, name: "targetReachedDistance");
			fadeOutDuration = s.Serialize<float>(fadeOutDuration, name: "fadeOutDuration");
		}
		public override uint? ClassCRC => 0xFA935954;
	}
}

