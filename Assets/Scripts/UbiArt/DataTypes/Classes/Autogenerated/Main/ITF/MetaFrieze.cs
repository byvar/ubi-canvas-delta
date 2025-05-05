namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class MetaFrieze : Pickable {
		public PolyPointList PointsList;
		public CListO<GFXPrimitiveParam> PrimitiveParameters;
		public uint ConfigCRC;
		public Path ConfigName;
		public bool SwitchExtremityStart;
		public bool SwitchExtremityStop;
		public uint SwitchTexturePipeExtremity;
		public bool IsFriendlyNameValid;
		public CArrayO<ObjectPath> FriezePath;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_Data)) {
					PointsList = s.SerializeObject<PolyPointList>(PointsList, name: "PointsList");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					FriezePath = s.SerializeObject<CArrayO<ObjectPath>>(FriezePath, name: "FriezePath");
					PrimitiveParameters = s.SerializeObject<CListO<GFXPrimitiveParam>>(PrimitiveParameters, name: "PrimitiveParameters");
					ConfigCRC = s.Serialize<uint>(ConfigCRC, name: "ConfigCRC");
				}
				ConfigName = s.SerializeObject<Path>(ConfigName, name: "ConfigName");
				SwitchExtremityStart = s.Serialize<bool>(SwitchExtremityStart, name: "SwitchExtremityStart");
				SwitchExtremityStop = s.Serialize<bool>(SwitchExtremityStop, name: "SwitchExtremityStop");
				SwitchTexturePipeExtremity = s.Serialize<uint>(SwitchTexturePipeExtremity, name: "SwitchTexturePipeExtremity");
				IsFriendlyNameValid = s.Serialize<bool>(IsFriendlyNameValid, name: "IsFriendlyNameValid");
			} else if (s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_Data)) {
					PointsList = s.SerializeObject<PolyPointList>(PointsList, name: "PointsList");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					ConfigCRC = s.Serialize<uint>(ConfigCRC, name: "ConfigCRC");
				}
				ConfigName = s.SerializeObject<Path>(ConfigName, name: "ConfigName");
				SwitchExtremityStart = s.Serialize<bool>(SwitchExtremityStart, name: "SwitchExtremityStart", options: CSerializerObject.Options.BoolAsByte);
				SwitchExtremityStop = s.Serialize<bool>(SwitchExtremityStop, name: "SwitchExtremityStop", options: CSerializerObject.Options.BoolAsByte);
				SwitchTexturePipeExtremity = s.Serialize<uint>(SwitchTexturePipeExtremity, name: "SwitchTexturePipeExtremity");
				IsFriendlyNameValid = s.Serialize<bool>(IsFriendlyNameValid, name: "IsFriendlyNameValid", options: CSerializerObject.Options.BoolAsByte);
			} else {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_Data)) {
					PointsList = s.SerializeObject<PolyPointList>(PointsList, name: "PointsList");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					PrimitiveParameters = s.SerializeObject<CListO<GFXPrimitiveParam>>(PrimitiveParameters, name: "PrimitiveParameters");
					ConfigCRC = s.Serialize<uint>(ConfigCRC, name: "ConfigCRC");
				}
				ConfigName = s.SerializeObject<Path>(ConfigName, name: "ConfigName");
				SwitchExtremityStart = s.Serialize<bool>(SwitchExtremityStart, name: "SwitchExtremityStart");
				SwitchExtremityStop = s.Serialize<bool>(SwitchExtremityStop, name: "SwitchExtremityStop");
				SwitchTexturePipeExtremity = s.Serialize<uint>(SwitchTexturePipeExtremity, name: "SwitchTexturePipeExtremity");
				IsFriendlyNameValid = s.Serialize<bool>(IsFriendlyNameValid, name: "IsFriendlyNameValid");
			}
		}
		public override uint? ClassCRC => 0xA2E15233;
	}
}

