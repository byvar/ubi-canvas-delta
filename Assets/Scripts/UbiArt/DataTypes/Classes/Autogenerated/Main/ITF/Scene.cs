namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion | GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class Scene : BaseObject {
		public uint ENGINE_VERSION;
		public CArrayO<Path> DEPENDENCIES;
		public CRefList<Frise> FRISE;
		public CRefList<MetaFrieze> METAFRIEZE;
		public CArrayO<Generic<Actor>> ACTORS;
		public CListO<FriezeConnectionResult> friezeConnections;
		public SceneConfigs sceneConfigs;
		public CListO<Path> TABS;
		public float GRIDUNIT;
		public TargetFilterList PLATFORM_FILTER;
		public Path MUSIC_THEME_PATH;
		public StringID MUSIC_THEME;
		public int DEPTH_SEPARATOR;
		public Matrix44 NEAR_SEPARATOR;
		public Matrix44 FAR_SEPARATOR;
		public CArrayO<Generic<Pickable>> FRISE_ORIGINS;
		public CArrayO<Generic<Pickable>> ACTORS_ORIGINS;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RJR || s.Settings.Game == Game.RFR) {
				if (s.HasFlags(SerializeFlags.Data_Save)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasFlags(SerializeFlags.Data_Load)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasProperties(SerializerProperties.Binary)) {
					DEPENDENCIES = s.SerializeObject<CArrayO<Path>>(DEPENDENCIES, name: "DEPENDENCIES");
					MUSIC_THEME_PATH = s.SerializeObject<Path>(MUSIC_THEME_PATH, name: "MUSIC_THEME_PATH");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					FRISE_ORIGINS = s.SerializeObject<CArrayO<Generic<Pickable>>>(FRISE_ORIGINS, name: "FRISE");
					ACTORS_ORIGINS = s.SerializeObject<CArrayO<Generic<Pickable>>>(ACTORS_ORIGINS, name: "ACTORS");
					friezeConnections = s.SerializeObject<CListO<FriezeConnectionResult>>(friezeConnections, name: "friezeConnections");
					MUSIC_THEME = s.SerializeObject<StringID>(MUSIC_THEME, name: "MUSIC_THEME");
				}
			} else if (s.Settings.Game == Game.RO) {
				if (s.HasFlags(SerializeFlags.Data_Save)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasFlags(SerializeFlags.Data_Load)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasProperties(SerializerProperties.Binary)) {
					DEPENDENCIES = s.SerializeObject<CArrayO<Path>>(DEPENDENCIES, name: "DEPENDENCIES");
					MUSIC_THEME_PATH = s.SerializeObject<Path>(MUSIC_THEME_PATH, name: "MUSIC_THEME_PATH");
				} else if (s.HasFlags(SerializeFlags.Group_Data)) {
					TABS = s.SerializeObject<CListO<Path>>(TABS, name: "TABS");
					GRIDUNIT = s.Serialize<float>(GRIDUNIT, name: "GRIDUNIT");
					PLATFORM_FILTER = s.SerializeObject<TargetFilterList>(PLATFORM_FILTER, name: "PLATFORM_FILTER");
				}
				if (s.HasFlags(SerializeFlags.Group_Data)) {
					FRISE_ORIGINS = s.SerializeObject<CArrayO<Generic<Pickable>>>(FRISE_ORIGINS, name: "FRISE");
					ACTORS_ORIGINS = s.SerializeObject<CArrayO<Generic<Pickable>>>(ACTORS_ORIGINS, name: "ACTORS");
					friezeConnections = s.SerializeObject<CListO<FriezeConnectionResult>>(friezeConnections, name: "friezeConnections");
					MUSIC_THEME = s.SerializeObject<StringID>(MUSIC_THEME, name: "MUSIC_THEME");
				}
			} else if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
				if (s.HasFlags(SerializeFlags.Data_Save)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasFlags(SerializeFlags.Data_Load)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasFlags(SerializeFlags.DataRaw)) {
					TABS = s.SerializeObject<CListO<Path>>(TABS, name: "TABS");
					GRIDUNIT = s.Serialize<float>(GRIDUNIT, name: "GRIDUNIT");
					DEPTH_SEPARATOR = s.Serialize<int>(DEPTH_SEPARATOR, name: "DEPTH_SEPARATOR");
					NEAR_SEPARATOR = s.SerializeObject<Matrix44>(NEAR_SEPARATOR, name: "NEAR_SEPARATOR");
					FAR_SEPARATOR = s.SerializeObject<Matrix44>(FAR_SEPARATOR, name: "FAR_SEPARATOR");
					PLATFORM_FILTER = s.SerializeObject<TargetFilterList>(PLATFORM_FILTER, name: "PLATFORM_FILTER");
				}
				if (s.HasFlags(SerializeFlags.DataBin)) {
					DEPENDENCIES = s.SerializeObject<CArrayO<Path>>(DEPENDENCIES, name: "DEPENDENCIES");
				}
				FRISE = s.SerializeObject<CRefList<Frise>>(FRISE, name: "FRISE");
				METAFRIEZE = s.SerializeObject<CRefList<MetaFrieze>>(METAFRIEZE, name: "METAFRIEZE");
				ACTORS = s.SerializeObject<CArrayO<Generic<Actor>>>(ACTORS, name: "ACTORS");
				friezeConnections = s.SerializeObject<CListO<FriezeConnectionResult>>(friezeConnections, name: "friezeConnections");
				sceneConfigs = s.SerializeObject<SceneConfigs>(sceneConfigs, name: "sceneConfigs");
			} else {
				if (s.HasFlags(SerializeFlags.Data_Save)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasFlags(SerializeFlags.Data_Load)) {
					ENGINE_VERSION = s.Serialize<uint>(ENGINE_VERSION, name: "ENGINE_VERSION");
				}
				if (s.HasFlags(SerializeFlags.DataBin)) {
					DEPENDENCIES = s.SerializeObject<CArrayO<Path>>(DEPENDENCIES, name: "DEPENDENCIES");
				}
				FRISE = s.SerializeObject<CRefList<Frise>>(FRISE, name: "FRISE");
				METAFRIEZE = s.SerializeObject<CRefList<MetaFrieze>>(METAFRIEZE, name: "METAFRIEZE");
				ACTORS = s.SerializeObject<CArrayO<Generic<Actor>>>(ACTORS, name: "ACTORS");
				friezeConnections = s.SerializeObject<CListO<FriezeConnectionResult>>(friezeConnections, name: "friezeConnections");
				sceneConfigs = s.SerializeObject<SceneConfigs>(sceneConfigs, name: "sceneConfigs");
			}
		}
		public override uint? ClassCRC => 0x0C75B172;
	}
}

