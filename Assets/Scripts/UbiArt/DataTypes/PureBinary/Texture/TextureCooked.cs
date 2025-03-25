namespace UbiArt {
	public class TextureCooked : ICSerializable {
		public TextureCookedHeader Header { get; set; }
		public TextureCookedHeader HeaderEditor { get; set; }
		public byte[] Data { get; set; }

		public UV.UVAtlas atlas = null;

		public void Serialize(CSerializerObject s, string name) {
			Header = s.SerializeObject<TextureCookedHeader>(Header, name: nameof(Header));
			var dataSize = (int)(Header?.RawDataSize ?? (s.Length - s.CurrentPosition));
			if(s.Settings.Platform == GamePlatform.iOS && (Header?.Type ?? 0) != 0 && dataSize == 0) {
				dataSize = (int)(s.Length - s.CurrentPosition);
			}

			Data = s.SerializeBytes(Data, dataSize);
			if (s.Settings.Platform == GamePlatform.Vita) {
				// Header is also appended to bottom of file, probably a leftover from their uncooked version.
				// Jade also has this where the header is at the top in the built version, but at the bottom in raw files.
				s.DoEndian(() => {
					HeaderEditor = s.SerializeObject<TextureCookedHeader>(HeaderEditor, name: nameof(HeaderEditor));
				}, Endian.Little);
			}
		}

		public TextureCooked() {
			Header = new TextureCookedHeader();
		}
		public TextureCooked(Context context) {
			Header = new TextureCookedHeader(context);
		}
	}
}
