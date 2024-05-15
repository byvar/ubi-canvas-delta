using System.Collections.Generic;

namespace UbiArt {
	public class TextureCookedHeader : ICSerializable {
		public static uint ExpectedSignature = 0x54455800; // TEX\0
		public uint Version { get; set; } // 0xD for legends, 0x11 for adventures
		public uint Signature { get; set; } = ExpectedSignature;
		public uint RawDataStartOffset { get; set; }
		public uint RawDataSize { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public ushort Depth { get; set; } = 1; // Always 1 in Legends? Maybe for animated textures?
		public byte BPP { get; set; } = 32;
		public TextureType Type { get; set; } = TextureType.Generic; // Unused on PC, but usually set to 0. Sometimes 1 for backlight, 2 for teensy_c10_back (?). On Android: 3 = PVRTC, 4 = ATC
		public byte V16_Byte0 { get; set; } // Only first byte seems used?
		public byte V16_Byte1 { get; set; }
		public byte V16_Byte2 { get; set; }
		public byte V16_Byte3 { get; set; }
		public uint MemorySize { get; set; }
		public uint UncompressedSize { get; set; } // Always 0 in Legends?
		public uint PixelsCountOpaque { get; set; } // Pixels where alpha == 1, before compression. Verified with uncompressed RL PS4 textures
		public uint PixelsCountHole { get; set; } // Pixels where alpha == 0, before compression. This and PixelsCountAlpha1 are divided by (Width * Height * ImagesCount) to calculate ratios.
		public uint TextureConfigCRC { get; set; } // Seems to be a CRC. Doesn't seem to be used at all by the game. Some kind of tag, only for the editor?
		public TextureAddressingMode WrapModeU { get; set; } = TextureAddressingMode.Wrap;
		public TextureAddressingMode WrapModeV { get; set; } = TextureAddressingMode.Wrap;
		public byte Padding1 { get; set; } // 0xCC in Legends, 0 in Adventures
		public byte Padding2 { get; set; }
		public uint Remap { get; set; } = 0x00010203; // 0 1 2 3

		public UV.UVAtlas atlas = null;

		//public string filename;

		public void Serialize(CSerializerObject s, string name) {
			//filename = s.CurrentPointer.File.DisplayName;
			Reinit(s.Settings);
			if (s.Settings.EngineVersion > EngineVersion.RO) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				s.DoEndian(() => {
					Signature = s.Serialize<uint>(Signature, name: nameof(Signature));
				}, Endian.Big);
				RawDataStartOffset = s.Serialize<uint>(RawDataStartOffset, name: nameof(RawDataStartOffset));
				RawDataSize = s.Serialize<uint>(RawDataSize, name: nameof(RawDataSize));
				Width = s.Serialize<ushort>(Width, name: nameof(Width));
				Height = s.Serialize<ushort>(Height, name: nameof(Height));
				Depth = s.Serialize<ushort>(Depth, name: nameof(Depth));
				BPP = s.Serialize<byte>(BPP, name: nameof(BPP));
				Type = (TextureType)s.Serialize<byte>((byte)Type, name: nameof(Type));
				if (Version >= 16) {
					V16_Byte0 = s.Serialize<byte>(V16_Byte0, name: nameof(V16_Byte0));
					V16_Byte1 = s.Serialize<byte>(V16_Byte1, name: nameof(V16_Byte1));
					V16_Byte2 = s.Serialize<byte>(V16_Byte2, name: nameof(V16_Byte2));
					V16_Byte3 = s.Serialize<byte>(V16_Byte3, name: nameof(V16_Byte3));
				}
				MemorySize = s.Serialize<uint>(MemorySize, name: nameof(MemorySize));
				UncompressedSize = s.Serialize<uint>(UncompressedSize, name: nameof(UncompressedSize));
				PixelsCountOpaque = s.Serialize<uint>(PixelsCountOpaque, name: nameof(PixelsCountOpaque));
				PixelsCountHole = s.Serialize<uint>(PixelsCountHole, name: nameof(PixelsCountHole));
				if (Version > 10) {
					TextureConfigCRC = s.Serialize<uint>(TextureConfigCRC, name: nameof(TextureConfigCRC));
				}
				WrapModeU = (TextureAddressingMode)s.Serialize<byte>((byte)WrapModeU, name: nameof(WrapModeU));
				WrapModeV = (TextureAddressingMode)s.Serialize<byte>((byte)WrapModeV, name: nameof(WrapModeV));
				Padding1 = s.Serialize<byte>(Padding1, name: nameof(Padding1));
				Padding2 = s.Serialize<byte>(Padding2, name: nameof(Padding2));
				if (Version > 10) {
					Remap = s.Serialize<uint>(Remap, name: nameof(Remap));
				}

				if (s.Settings.Platform == GamePlatform.Vita) {
					RawDataSize = (uint)(s.Length - (RawDataStartOffset * 2));
				}
			} else {
				RawDataSize = (uint)s.Length;
			}
		}

		public void Reinit(Settings settings) {
			if (settings.Game == Game.RL && Version >= 16) {
				Version = 13;
				RawDataStartOffset = 0x34;
			}
		}

		public TextureCookedHeader() { }
		public TextureCookedHeader(Context context) {
			if (context.Settings.Game == Game.RL) {
				Version = 13;
				RawDataStartOffset = 0x34;
				Padding1 = 0xCC;
				Padding2 = 0xCC;
			}
		}

		public enum TextureAddressingMode : byte {
			Wrap = 0,
			Mirror = 1,
			Clamp = 2,
			Border = 3,
		}
		public enum TextureType : byte {
			Generic = 0,
			Backlight = 1,
			BacklightEmmissive = 2,
			PVRTC = 3,
			ATC = 4,
		}
	}
}
