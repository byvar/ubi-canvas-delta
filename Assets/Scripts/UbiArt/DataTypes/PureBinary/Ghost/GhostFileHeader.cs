namespace UbiArt.Ghost {
	public class GhostFileHeader : CSerializable {
		public const uint MagicNumber = 0xFEEDBAAF;
		public const uint HeaderSize = 0x400;

		public byte version = 12;
		public uint signature = MagicNumber;
		public Path scenePath;
		public float score;
		public uint seed;
		public Vec2d endPos;
		public uint playersCount;
		public StringID[] skins = new StringID[4];
		public uint unk1;
		public bool isCompressed;
		public uint uncompressedSize;

		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			version = s.Serialize<byte>(version, name: nameof(version));
			signature = s.Serialize<uint>(signature, name: nameof(signature));
			scenePath = s.SerializeObject<Path>(scenePath, name: nameof(scenePath));
			score = s.Serialize<float>(score, name: nameof(score));
			seed = s.Serialize<uint>(seed, name: nameof(seed));
			endPos = s.SerializeObject<Vec2d>(endPos, name: nameof(endPos));
			playersCount = s.Serialize<uint>(playersCount, name: nameof(playersCount));
			for (int i = 0; i < 4; i++) {
				skins[i] = s.SerializeObject<StringID>(skins[i], name: $"{nameof(skins)}[{i}]");
			}
			unk1 = s.Serialize<uint>(unk1, name: nameof(unk1));
			isCompressed = s.Serialize<bool>(isCompressed, name: nameof(isCompressed));
			uncompressedSize = s.Serialize<uint>(uncompressedSize, name: nameof(uncompressedSize));
		}
	}
}
