namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class Bind : CSerializable {
		public ObjectPath parentPath;
		public Vec3d offsetPos;
		public float offsetAngle;
		public Type type;
		public uint typeData;
		public bool useParentFlip;
		public bool useParentScale;
		public bool useParentAlpha;
		public bool useRelativeZ;
		public bool removeWithParent;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_Data)) {
					parentPath = s.SerializeObject<ObjectPath>(parentPath, name: "parentPath");
					offsetPos = s.SerializeObject<Vec3d>(offsetPos, name: "offsetPos");
					offsetAngle = s.Serialize<float>(offsetAngle, name: "offsetAngle");
					type = s.Serialize<Type>(type, name: "type");
					typeData = s.Serialize<uint>(typeData, name: "typeData");
				}
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_DataEditable)) {
					useParentFlip = s.Serialize<bool>(useParentFlip, name: "useParentFlip");
					useParentScale = s.Serialize<bool>(useParentScale, name: "useParentScale");
					useParentAlpha = s.Serialize<bool>(useParentAlpha, name: "useParentAlpha");
					useRelativeZ = s.Serialize<bool>(useRelativeZ, name: "useRelativeZ");
					removeWithParent = s.Serialize<bool>(removeWithParent, name: "removeWithParent");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_Data)) {
					parentPath = s.SerializeObject<ObjectPath>(parentPath, name: "parentPath");
					offsetPos = s.SerializeObject<Vec3d>(offsetPos, name: "offsetPos");
					offsetAngle = s.Serialize<float>(offsetAngle, name: "offsetAngle");
					type = s.Serialize<Type>(type, name: "type");
					typeData = s.Serialize<uint>(typeData, name: "typeData");
				}
				if (s.HasFlags(SerializeFlags.Editor | SerializeFlags.Group_DataEditable)) {
					useParentFlip = s.Serialize<bool>(useParentFlip, name: "useParentFlip");
					useParentScale = s.Serialize<bool>(useParentScale, name: "useParentScale");
					useParentAlpha = s.Serialize<bool>(useParentAlpha, name: "useParentAlpha");
					removeWithParent = s.Serialize<bool>(removeWithParent, name: "removeWithParent");
				}
			}
		}
		public enum Type {
			[Serialize("Root"              )] Root = 0,
			[Serialize("BoneName"          )] BoneName = 1,
			[Serialize("ProceduralBoneName")] ProceduralBoneName = 2,
		}
	}
}

