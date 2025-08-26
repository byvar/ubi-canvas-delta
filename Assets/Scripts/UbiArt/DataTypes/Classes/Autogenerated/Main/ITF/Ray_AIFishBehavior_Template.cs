namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIFishBehavior_Template : Ray_AIGroundBaseBehavior_Template {
		public Generic<AIAction_Template> struggle;
		public Generic<AIAction_Template> release;
		public Generic<Ray_AIReceiveCameraEjectHitAction_Template> spikeHit;
		public float releaseSpeed;
		public float releaseAccel;
		public int rotateOnRelease;
		public Angle rotationSpeed;
		public float struggleOffsetAmplitude;
		public float struggleOffsetFrequency;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			struggle = s.SerializeObject<Generic<AIAction_Template>>(struggle, name: "struggle");
			release = s.SerializeObject<Generic<AIAction_Template>>(release, name: "release");
			spikeHit = s.SerializeObject<Generic<Ray_AIReceiveCameraEjectHitAction_Template>>(spikeHit, name: "spikeHit");
			releaseSpeed = s.Serialize<float>(releaseSpeed, name: "releaseSpeed");
			releaseAccel = s.Serialize<float>(releaseAccel, name: "releaseAccel");
			rotateOnRelease = s.Serialize<int>(rotateOnRelease, name: "rotateOnRelease");
			rotationSpeed = s.SerializeObject<Angle>(rotationSpeed, name: "rotationSpeed");
			struggleOffsetAmplitude = s.Serialize<float>(struggleOffsetAmplitude, name: "struggleOffsetAmplitude");
			struggleOffsetFrequency = s.Serialize<float>(struggleOffsetFrequency, name: "struggleOffsetFrequency");
		}
		public override uint? ClassCRC => 0x9258751C;
	}
}

