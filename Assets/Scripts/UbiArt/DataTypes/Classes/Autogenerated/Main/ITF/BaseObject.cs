namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class BaseObject : CSerializable {
		public ObjectId OBJECTID;
		public uint VitaProperty;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.EngineVersion == EngineVersion.RO) {
				OBJECTID = s.SerializeObject<ObjectId>(OBJECTID, name: "OBJECTID");
				if (s.Settings.Platform == GamePlatform.Vita) {
					VitaProperty = s.Serialize<uint>(VitaProperty, name: nameof(VitaProperty));
				}
			}
		}
		public override uint? ClassCRC => 0xBFC64EFB;
	}
}

