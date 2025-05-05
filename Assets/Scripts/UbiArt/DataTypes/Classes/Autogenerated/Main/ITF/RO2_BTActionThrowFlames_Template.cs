namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_BTActionThrowFlames_Template : BTAction_Template {
		public StringID animation;
		public StringID endAnimation;
		public CListO<StringID> fxNames;
		public CListO<StringID> fxMarkerStart;
		public CListO<StringID> fxMarkerStop;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				animation = s.SerializeObject<StringID>(animation, name: "animation");
				endAnimation = s.SerializeObject<StringID>(endAnimation, name: "endAnimation");
				fxNames = s.SerializeObject<CListO<StringID>>(fxNames, name: "fxNames");
				fxMarkerStart = s.SerializeObject<CListO<StringID>>(fxMarkerStart, name: "fxMarkerStart");
				fxMarkerStop = s.SerializeObject<CListO<StringID>>(fxMarkerStop, name: "fxMarkerStop");
			} else {
				animation = s.SerializeObject<StringID>(animation, name: "animation");
				endAnimation = s.SerializeObject<StringID>(endAnimation, name: "endAnimation");
			}
		}
		public override uint? ClassCRC => 0xB64EF794;
	}
}

