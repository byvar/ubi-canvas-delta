namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class SubSceneActor : Actor {
		public Path RELATIVEPATH;
		public bool EMBED_SCENE;
		public bool IS_SINGLE_PIECE;
		public bool ZFORCED;
		public bool DIRECT_PICKING = true;
		public VIEWTYPE viewType = VIEWTYPE.ALL;
		public Nullable<Scene> SCENE;
		public Generic<Scene> SCENE_ORIGINS;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR || s.Settings.Game == Game.RO) {
				if (s.HasFlags(SerializeFlags.Editor)) {
					RELATIVEPATH = s.SerializeObject<Path>(RELATIVEPATH, name: "RELATIVEPATH");
					EMBED_SCENE = s.Serialize<bool>(EMBED_SCENE, name: "EMBED_SCENE");
					IS_SINGLE_PIECE = s.Serialize<bool>(IS_SINGLE_PIECE, name: "IS_SINGLE_PIECE");
					ZFORCED = s.Serialize<bool>(ZFORCED, name: "ZFORCED");
					xFLIPPED = s.Serialize<bool>(xFLIPPED, name: "xFLIPPED");
					parentBindOrigins = s.SerializeObject<Nullable<ActorBind>>(parentBindOrigins, name: "parentBind"); // Serialized a second time
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					RELATIVEPATH = s.SerializeObject<Path>(RELATIVEPATH, name: "RELATIVEPATH");
					EMBED_SCENE = s.Serialize<bool>(EMBED_SCENE, name: "EMBED_SCENE");
					IS_SINGLE_PIECE = s.Serialize<bool>(IS_SINGLE_PIECE, name: "IS_SINGLE_PIECE");
					ZFORCED = s.Serialize<bool>(ZFORCED, name: "ZFORCED");
					if (EMBED_SCENE) {
						SCENE_ORIGINS = s.SerializeObject<Generic<Scene>>(SCENE_ORIGINS, name: "SCENE");
					}
				}
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.VH || s.Settings.Game == Game.COL) {
				RELATIVEPATH = s.SerializeObject<Path>(RELATIVEPATH, name: "RELATIVEPATH");
				EMBED_SCENE = s.Serialize<bool>(EMBED_SCENE, name: "EMBED_SCENE");
				IS_SINGLE_PIECE = s.Serialize<bool>(IS_SINGLE_PIECE, name: "IS_SINGLE_PIECE");
				ZFORCED = s.Serialize<bool>(ZFORCED, name: "ZFORCED");
				DIRECT_PICKING = s.Serialize<bool>(DIRECT_PICKING, name: "DIRECT_PICKING");
				viewType = s.Serialize<VIEWTYPE>(viewType, name: "viewType");
				if (s.HasFlags(SerializeFlags.Group_Data) && EMBED_SCENE) {
					SCENE = s.SerializeObject<Nullable<Scene>>(SCENE, name: "SCENE");
				}
				if (s.HasFlags(SerializeFlags.Editor)) {
					if (s.HasFlags(SerializeFlags.Instance)) {
						parentBind = s.SerializeObject<Nullable<Bind>>(parentBind, name: "parentBind"); // Serialized a second time
					}
					if (!s.HasFlags(SerializeFlags.Group_DataEditable)) {
						USERFRIENDLY = s.Serialize<string>(USERFRIENDLY, name: "USERFRIENDLY"); // Serialized a second time
					}
				}
			} else {
				RELATIVEPATH = s.SerializeObject<Path>(RELATIVEPATH, name: "RELATIVEPATH");
				EMBED_SCENE = s.Serialize<bool>(EMBED_SCENE, name: "EMBED_SCENE");
				IS_SINGLE_PIECE = s.Serialize<bool>(IS_SINGLE_PIECE, name: "IS_SINGLE_PIECE");
				ZFORCED = s.Serialize<bool>(ZFORCED, name: "ZFORCED");
				DIRECT_PICKING = s.Serialize<bool>(DIRECT_PICKING, name: "DIRECT_PICKING");
				viewType = s.Serialize<VIEWTYPE>(viewType, name: "viewType");
				if (s.HasFlags(SerializeFlags.Group_Data) && EMBED_SCENE) {
					SCENE = s.SerializeObject<Nullable<Scene>>(SCENE, name: "SCENE");
				}
				if (s.HasFlags(SerializeFlags.CSharp)) {
					SCENE = s.SerializeObject<Nullable<Scene>>(SCENE, name: "SCENE");
				}
			}
		}
		public enum VIEWTYPE {
			[Serialize("VIEWTYPE_MAIN"             )] MAIN = 0,
			[Serialize("VIEWTYPE_REMOTE"           )] REMOTE = 1,
			[Serialize("VIEWTYPE_ALL"              )] ALL = 2,
			[Serialize("VIEWTYPE_MAINONLY"         )] MAINONLY = 3,
			[Serialize("VIEWTYPE_REMOTEONLY"       )] REMOTEONLY = 4,
			[Serialize("VIEWTYPE_REMOTEASMAIN_ONLY")] REMOTEASMAIN_ONLY = 5,
		}
		public override uint? ClassCRC => 0x4FA40F09;
	}
}

