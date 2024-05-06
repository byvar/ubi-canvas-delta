namespace UbiArt.UV {
	public class UVAtlasManager : CSerializable {
		public uint version;
		public CMap<StringID, UVAtlas> atlas = new CMap<StringID, UVAtlas>();

		protected override void SerializeImpl(CSerializerObject s) {
			version = s.Serialize<uint>(version, name: "version");
			atlas = s.SerializeObject<CMap<StringID, UVAtlas>>(atlas, name: "atlas");
		}

		public UVAtlas GetAtlasIfExists(Path path) {
			if (path != null && !path.IsNull && atlas != null && atlas.ContainsKey(path.stringID)) {
				return atlas[path.stringID];
			}
			return null;
		}

		public void Reinit(Settings settings) {
			if (atlas != null) {
				foreach (var at in atlas)
					at.Value?.Reinit(settings);
			}
		}
	}
}
