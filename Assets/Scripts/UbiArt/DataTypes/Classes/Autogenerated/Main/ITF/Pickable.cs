namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class Pickable : BaseObject {
		public Vec3d globalPos;
		public float RELATIVEZ;
		public string USERFRIENDLY;
		public CArrayO<ObjectPath> UPDATEDEPENDENCYLIST;
		public bool STARTPAUSE;
		public bool isEnabled = true;
		public Vec2d POS2D = Vec2d.Zero;
		public DeviceInfo__Device_Speed ObjectDeviceSpeed = (DeviceInfo__Device_Speed)0x7FFFFFFF;
		public DeviceInfo__Device_Speed ObjectDeviceSpeed_L2 = (DeviceInfo__Device_Speed)0x7FFFFFFF;
		public Path INSTANCEDATAFILE;
		public uint persistenceId;
		public bool USEVIEWFRUSTUMFLAG = true;
		public Vec2d SCALE = Vec2d.One;
		public int OFFSCREEN_UPDATE_OVERRIDE;
		public UpdateType UPDATE_TYPE;
		public AABB AABB_RELATIVE;
		public Vec3d CURPOS;
		public float CURANGLE;
		public Vec2d CURSCALE;
		public int ISALLOWEDFORCELL;
		public Angle ANGLE = new Angle(0);
		public bool xFLIPPED;

		public Enum_VH_0 Enum_VH_0__22;

		public Vec2d Vita_00 { get; set; }
		public Vec2d Vita_01 { get; set; }

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RO || s.Settings.Game == Game.RFR) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					POS2D = s.SerializeObject<Vec2d>(POS2D, name: "POS2D");
					RELATIVEZ = s.Serialize<float>(RELATIVEZ, name: "RELATIVEZ");
					ANGLE = (Angle)s.Serialize<float>((float)ANGLE, name: "ANGLE");
					SCALE = s.SerializeObject<Vec2d>(SCALE, name: "SCALE");
					USERFRIENDLY = s.Serialize<string>(USERFRIENDLY, name: "USERFRIENDLY");
					OFFSCREEN_UPDATE_OVERRIDE = s.Serialize<int>(OFFSCREEN_UPDATE_OVERRIDE, name: "OFFSCREEN_UPDATE_OVERRIDE");
					UPDATE_TYPE = s.Serialize<UpdateType>(UPDATE_TYPE, name: "UPDATE_TYPE");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					AABB_RELATIVE = s.SerializeObject<AABB>(AABB_RELATIVE, name: "AABB_RELATIVE");
				}
				if (s.HasFlags(SerializeFlags.Editor)) {
					CURPOS = s.SerializeObject<Vec3d>(CURPOS, name: "CURPOS");
					CURANGLE = s.Serialize<float>(CURANGLE, name: "CURANGLE");
					CURSCALE = s.SerializeObject<Vec2d>(CURSCALE, name: "CURSCALE");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					ISALLOWEDFORCELL = s.Serialize<int>(ISALLOWEDFORCELL, name: "ISALLOWEDFORCELL");
					isEnabled = s.Serialize<bool>(isEnabled, name: "isEnabled");
				}
			} else if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					RELATIVEZ = s.Serialize<float>(RELATIVEZ, name: "RELATIVEZ");
					SCALE = s.SerializeObject<Vec2d>(SCALE, name: "SCALE");
					xFLIPPED = s.Serialize<bool>(xFLIPPED, name: "xFLIPPED");
					USERFRIENDLY = s.Serialize<string>(USERFRIENDLY, name: "USERFRIENDLY");
					UPDATEDEPENDENCYLIST = s.SerializeObject<CArrayO<ObjectPath>>(UPDATEDEPENDENCYLIST, name: "UPDATEDEPENDENCYLIST");
				}
				if (s.Settings.Platform == GamePlatform.Vita) {
					Vita_00 = s.SerializeObject<Vec2d>(Vita_00, name: nameof(Vita_00));
					Vita_01 = s.SerializeObject<Vec2d>(Vita_01, name: nameof(Vita_01));
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					isEnabled = s.Serialize<bool>(isEnabled, name: "isEnabled", options: CSerializerObject.Options.BoolAsByte);
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					POS2D = s.SerializeObject<Vec2d>(POS2D, name: "POS2D");
					ANGLE = s.SerializeObject<Angle>(ANGLE, name: "ANGLE");
					INSTANCEDATAFILE = s.SerializeObject<Path>(INSTANCEDATAFILE, name: "INSTANCEDATAFILE");
				}
			} else if (s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					RELATIVEZ = s.Serialize<float>(RELATIVEZ, name: "RELATIVEZ");
					SCALE = s.SerializeObject<Vec2d>(SCALE, name: "SCALE");
					xFLIPPED = s.Serialize<bool>(xFLIPPED, name: "xFLIPPED", options: CSerializerObject.Options.BoolAsByte);
					USERFRIENDLY = s.Serialize<string>(USERFRIENDLY, name: "USERFRIENDLY");
					UPDATEDEPENDENCYLIST = s.SerializeObject<CArrayO<ObjectPath>>(UPDATEDEPENDENCYLIST, name: "UPDATEDEPENDENCYLIST");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					isEnabled = s.Serialize<bool>(isEnabled, name: "isEnabled", options: CSerializerObject.Options.BoolAsByte);
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					POS2D = s.SerializeObject<Vec2d>(POS2D, name: "POS2D");
					ANGLE = s.SerializeObject<Angle>(ANGLE, name: "ANGLE");
					INSTANCEDATAFILE = s.SerializeObject<Path>(INSTANCEDATAFILE, name: "INSTANCEDATAFILE");
					OBJECTID = s.SerializeObject<ObjectId>(OBJECTID, name: "OBJECTID");
				}
			} else if (s.Settings.Game == Game.VH) {
				if (s.HasFlags(SerializeFlags.Flags16)) {
					globalPos = s.SerializeObject<Vec3d>(globalPos, name: "globalPos");
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					RELATIVEZ = s.Serialize<float>(RELATIVEZ, name: "RELATIVEZ");
					SCALE = s.SerializeObject<Vec2d>(SCALE, name: "SCALE");
					xFLIPPED = s.Serialize<bool>(xFLIPPED, name: "xFLIPPED");
					USERFRIENDLY = s.Serialize<string>(USERFRIENDLY, name: "USERFRIENDLY");
					UPDATEDEPENDENCYLIST = s.SerializeObject<CArrayO<ObjectPath>>(UPDATEDEPENDENCYLIST, name: "UPDATEDEPENDENCYLIST");
					STARTPAUSE = s.Serialize<bool>(STARTPAUSE, name: "STARTPAUSE");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					isEnabled = s.Serialize<bool>(isEnabled, name: "isEnabled");
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					POS2D = s.SerializeObject<Vec2d>(POS2D, name: "POS2D");
					ANGLE = s.SerializeObject<Angle>(ANGLE, name: "ANGLE");
					Enum_VH_0__22 = s.Serialize<Enum_VH_0>(Enum_VH_0__22, name: "Enum_VH_0__22");
					INSTANCEDATAFILE = s.SerializeObject<Path>(INSTANCEDATAFILE, name: "INSTANCEDATAFILE");
				}
				persistenceId = s.Serialize<uint>(persistenceId, name: "persistenceId");
			} else {
				if (s.HasFlags(SerializeFlags.Flags16)) {
					globalPos = s.SerializeObject<Vec3d>(globalPos, name: "globalPos");
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					RELATIVEZ = s.Serialize<float>(RELATIVEZ, name: "RELATIVEZ");
					ANGLE = s.SerializeObject<Angle>(ANGLE, name: "ANGLE");
					SCALE = s.SerializeObject<Vec2d>(SCALE, name: "SCALE");
					xFLIPPED = s.Serialize<bool>(xFLIPPED, name: "xFLIPPED");
					USERFRIENDLY = s.Serialize<string>(USERFRIENDLY, name: "USERFRIENDLY");
					UPDATEDEPENDENCYLIST = s.SerializeObject<CArrayO<ObjectPath>>(UPDATEDEPENDENCYLIST, name: "UPDATEDEPENDENCYLIST");
					STARTPAUSE = s.Serialize<bool>(STARTPAUSE, name: "STARTPAUSE");
				}
				if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
					isEnabled = s.Serialize<bool>(isEnabled, name: "isEnabled");
				}
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					POS2D = s.SerializeObject<Vec2d>(POS2D, name: "POS2D");
					ObjectDeviceSpeed = s.Serialize<DeviceInfo__Device_Speed>(ObjectDeviceSpeed, name: "ObjectDeviceSpeed");
					ObjectDeviceSpeed_L2 = s.Serialize<DeviceInfo__Device_Speed>(ObjectDeviceSpeed_L2, name: "ObjectDeviceSpeed_L2");
					INSTANCEDATAFILE = s.SerializeObject<Path>(INSTANCEDATAFILE, name: "INSTANCEDATAFILE");
				}
				persistenceId = s.Serialize<uint>(persistenceId, name: "persistenceId");
				USEVIEWFRUSTUMFLAG = s.Serialize<bool>(USEVIEWFRUSTUMFLAG, name: "USEVIEWFRUSTUMFLAG");
			}
		}
		public enum DeviceInfo__Device_Speed {
			[Serialize("DeviceInfo::Device_SpeedLow"               )] Low = 1,
			[Serialize("DeviceInfo::Device_SpeedLowMedium"         )] LowMedium = 3,
			[Serialize("DeviceInfo::Device_SpeedLowMediumHigh"     )] LowMediumHigh = 7,
			[Serialize("DeviceInfo::Device_SpeedMedium"            )] Medium = 2,
			[Serialize("DeviceInfo::Device_SpeedMediumHigh"        )] MediumHigh = 6,
			[Serialize("DeviceInfo::Device_SpeedMediumHighVeryHigh")] MediumHighVeryHigh = 14,
			[Serialize("DeviceInfo::Device_SpeedHigh"              )] High = 4,
			[Serialize("DeviceInfo::Device_SpeedHighVeryHigh"      )] HighVeryHigh = 12,
			[Serialize("DeviceInfo::Device_SpeedVeryHigh"          )] VeryHigh = 8,
			[Serialize("DeviceInfo::Device_SpeedAll"               )] All = 15,
		}
		public enum Enum_VH_0 {
			[Serialize("Value_1" )] Value_1 = 1,
			[Serialize("Value_3" )] Value_3 = 3,
			[Serialize("Value_2" )] Value_2 = 2,
			[Serialize("Value_6" )] Value_6 = 6,
			[Serialize("Value_4" )] Value_4 = 4,
			[Serialize("Value__1")] Value__1 = -1,
		}
		public enum UpdateType {
			[Serialize("Pickable::UpdateType_OnlyIfVisible")] OnlyIfVisible = 0,
			[Serialize("Pickable::UpdateType_OffscreenAllowed")] OffscreenAllowed = 1,
		}
		public override uint? ClassCRC => 0x2371CC09;
	}
}

