namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class BaseCameraComponent_Template : ActorComponent_Template {
		public bool startAsMainCam;
		public float rampUpDestinationCoeff;
		public float rampUpCoeff;
		public bool remote;
		public int updateWhilePaused;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				startAsMainCam = s.Serialize<bool>(startAsMainCam, name: "startAsMainCam");
				rampUpDestinationCoeff = s.Serialize<float>(rampUpDestinationCoeff, name: "rampUpDestinationCoeff");
				rampUpCoeff = s.Serialize<float>(rampUpCoeff, name: "rampUpCoeff");
			} else if(s.Settings.Game == Game.COL) {
				startAsMainCam = s.Serialize<bool>(startAsMainCam, name: "startAsMainCam", options: CSerializerObject.Options.BoolAsByte);
				remote = s.Serialize<bool>(remote, name: "remote", options: CSerializerObject.Options.BoolAsByte);
				rampUpCoeff = s.Serialize<float>(rampUpCoeff, name: "rampUpCoeff");
				updateWhilePaused = s.Serialize<int>(updateWhilePaused, name: "updateWhilePaused");
			} else {
				startAsMainCam = s.Serialize<bool>(startAsMainCam, name: "startAsMainCam");
				rampUpCoeff = s.Serialize<float>(rampUpCoeff, name: "rampUpCoeff");
			}
		}
		public override uint? ClassCRC => 0x680B2173;
	}
}

