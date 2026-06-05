using System;

/*
 * MIT License

Copyright (c) 2021 BinarySerializer

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

 * */

// Code written by RayCarrot
namespace BinarySerializer.UbiArt
{
    // TODO: This should be moved to some other library, like an Xbox one or a DirectX one

    // NOTE: A D3D texture, used on Xbox 360. I'm not 100% sure if all the enums are correctly matched
    //       with the properties. The struct comes from the Xbox 360 prototype of Rayman Legends which
    //       declares all bitfield values as integers, so I manually matched the enums.
    public class D3DTexture : BinarySerializable
    {
        #region Public Properties

        public int Common { get; set; }
        public int ReferenceCount { get; set; }
        public int Fence { get; set; }
        public int ReadFence { get; set; }
        public int Identifier { get; set; }
        public int BaseFlush { get; set; }
        public int MipFlush { get; set; }

        // Format
        public GPUCONSTANTTYPE Type { get; set; }
        public GPUSIGN SignX { get; set; }
        public GPUSIGN SignY { get; set; }
        public GPUSIGN SignZ { get; set; }
        public GPUSIGN SignW { get; set; }
        public GPUCLAMP ClampX { get; set; }
        public GPUCLAMP ClampY { get; set; }
        public GPUCLAMP ClampZ { get; set; }
        public int Unknown1 { get; set; } // TODO: Unknown name, probably ClampW?
        public int Pitch { get; set; }
        public bool Tiled { get; set; }
        public GPUTEXTUREFORMAT DataFormat { get; set; }
        public GPUENDIAN Endian { get; set; }
        public GPUREQUESTSIZE RequestSize { get; set; }
        public bool Stacked { get; set; }
        public GPUCLAMPPOLICY ClampPolicy { get; set; }
        public int BaseAddress { get; set; }
        public int ActualWidth => Width + 1;
        public int ActualHeight => Height + 1;
        public int Width { get; set; }
        public int Height { get; set; }
        public GPUNUMFORMAT NumFormat { get; set; }
        public GPUSWIZZLE SwizzleX { get; set; }
        public GPUSWIZZLE SwizzleY { get; set; }
        public GPUSWIZZLE SwizzleZ { get; set; }
        public GPUSWIZZLE SwizzleW { get; set; }
        public int ExpAdjust { get; set; }
        public GPUMINMAGFILTER MagFilter { get; set; }
        public GPUMINMAGFILTER MinFilter { get; set; }
        public GPUMIPFILTER MipFilter { get; set; }
        public GPUANISOFILTER AnisoFilter { get; set; }
        public int Unknown2 { get; set; } // TODO: Unknown name
        public int BorderSize { get; set; }
        public int VolMagFilter { get; set; }
        public int VolMinFilter { get; set; }
        public int MinMipLevel { get; set; }
        public int MaxMipLevel { get; set; }
        public int MagAnisoWalk { get; set; }
        public int MinAnisoWalk { get; set; }
        public int LODBias { get; set; }
        public int GradExpAdjustH { get; set; }
        public int GradExpAdjustV { get; set; }
        public GPUBORDERCOLOR BorderColor { get; set; }
        public bool ForceBCWToMax { get; set; }
        public GPUTRICLAMP TriClamp { get; set; }
        public int AnisoBias { get; set; }
        public GPUDIMENSION Dimension { get; set; }
        public bool PackedMips { get; set; }
        public int MipAddress { get; set; }

		public byte[] ImageData { get; set; }

        #endregion

        #region Private Methods

        // Based on https://github.com/gildor2/UEViewer/blob/eaba2837228f9fe39134616d7bff734acd314ffb/Unreal/UnrealMaterial/UnTexture.cpp#L562
        private byte[] UntileTexture(byte[] srcData, int originalWidth, int originalHeight, int bytesPerBlock, int alignX, int alignY, int blockSizeX, int blockSizeY)
        {
            var dstData = new byte[srcData.Length];

            int alignedWidth = Align(originalWidth, alignX);
            int alignedHeight = Align(originalHeight, alignY);
			//originalWidth = alignedWidth;
			//originalHeight = alignedHeight;

            int tiledBlockWidth = (alignedWidth + blockSizeX - 1) / blockSizeX;       // width of image in blocks
            int originalBlockWidth = (originalWidth + blockSizeX - 1) / blockSizeX;   // width of image in blocks
            int tiledBlockHeight = (alignedHeight + blockSizeY - 1) / blockSizeY;     // height of image in blocks
            int originalBlockHeight = (originalHeight + blockSizeY - 1) / blockSizeY; // height of image in blocks
            int logBpp = AppLog2(bytesPerBlock);

            // XBox360 has packed multiple lower mip levels into a single tile - should use special code
            // to unpack it. Textures are aligned to bottom-right corder.
            // Packing looks like this:
            // ....CCCCBBBBBBBBAAAAAAAAAAAAAAAA
            // ....CCCCBBBBBBBBAAAAAAAAAAAAAAAA
            // E.......BBBBBBBBAAAAAAAAAAAAAAAA
            // ........BBBBBBBBAAAAAAAAAAAAAAAA
            // DD..............AAAAAAAAAAAAAAAA
            // ................AAAAAAAAAAAAAAAA
            // ................AAAAAAAAAAAAAAAA
            // ................AAAAAAAAAAAAAAAA
            // (Where mips are A,B,C,D,E - E is 1x1, D is 2x2 etc)
            // Force sxOffset=0 and enable DEBUG_MIPS in UnRender.cpp to visualize this layout.
            // So we should offset X coordinate when unpacking to the width of mip level.
            // Note: this doesn't work with non-square textures.
            var sxOffset = 0;
            var syOffset = 0;

			if (PackedMips) {
				if (originalWidth <= 16) sxOffset = originalBlockWidth;
				if (originalWidth <= 8) sxOffset = originalBlockWidth * 2;
			}

            int numImageBlocks = tiledBlockWidth * tiledBlockHeight;    // used for verification

            // Iterate over image blocks
            for (int dy = 0; dy < originalBlockHeight; dy++)
            {
                for (int dx = 0; dx < originalBlockWidth; dx++)
                {
                    // Unswizzle only once for a whole block
                    uint swzAddr = GetTiledOffset(dx + sxOffset, dy + syOffset, tiledBlockWidth, logBpp);

                    if (swzAddr >= numImageBlocks)
                        throw new Exception("Error in Xbox 360 texture parsing");

                    int sy = (int)(swzAddr / tiledBlockWidth);
                    int sx = (int)(swzAddr % tiledBlockWidth);

                    int dstStart = (dy * originalBlockWidth + dx) * bytesPerBlock;
                    int srcStart = (sy * tiledBlockWidth + sx) * bytesPerBlock;
                    Array.Copy(srcData, srcStart, dstData, dstStart, bytesPerBlock);
                }
            }

            return dstData;
        }

        private static uint GetTiledOffset(int x, int y, int width, int logBpb)
        {
            if (width > 8192)
                throw new Exception($"Xbox 360 texture: Width {width} too large");

            if (width <= x)
                throw new Exception($"Xbox 360 texture: X {x} too large for width {width}");

            int alignedWidth = Align(width, 32);
            // top bits of coordinates
            int macro = ((x >> 5) + (y >> 5) * (alignedWidth >> 5)) << (logBpb + 7);
            // lower bits of coordinates (result is 6-bit value)
            int micro = ((x & 7) + ((y & 0xE) << 2)) << logBpb;
            // mix micro/macro + add few remaining x/y bits
            int offset = macro + ((micro & ~0xF) << 1) + (micro & 0xF) + ((y & 1) << 4);
            // mix bits again
            return (uint)((((offset & ~0x1FF) << 3) +            // upper bits (offset bits [*-9])
                           ((y & 16) << 7) +                           // next 1 bit
                           ((offset & 0x1C0) << 2) +                   // next 3 bits (offset bits [8-6])
                           (((((y & 8) >> 2) + (x >> 3)) & 3) << 6) +  // next 2 bits
                           (offset & 0x3F)                             // lower 6 bits (offset bits [5-0])
                ) >> logBpb);
        }

        private static int Align(int value, int align) => (value % align != 0) ? ((value / align) + 1) * (align) : value;

        private static int AppLog2(int n)
        {
            int r;
            for (r = -1; n != 0; n >>= 1, r++) { /*empty*/ }
            return r;
        }

		private byte[] ApplyEndian(byte[] data) {
			byte[] result = new byte[data.Length];

			switch (Endian) {
				case GPUENDIAN.GPUENDIAN_NONE:
					return data;

				case GPUENDIAN.GPUENDIAN_8IN16:
					for (int i = 0; i < data.Length; i += 2) {
						result[i] = data[i + 1];
						result[i + 1] = data[i];
					}
					break;

				case GPUENDIAN.GPUENDIAN_8IN32:
					for (int i = 0; i < data.Length; i += 4) {
						result[i] = data[i + 3];
						result[i + 1] = data[i + 2];
						result[i + 2] = data[i + 1];
						result[i + 3] = data[i];
					}
					break;

				case GPUENDIAN.GPUENDIAN_16IN32:
					for (int i = 0; i < data.Length; i += 4) {
						result[i] = data[i + 2];
						result[i + 3] = data[i + 1];
						result[i + 2] = data[i];
						result[i + 1] = data[i + 3];
					}
					break;

				default:
					throw new NotSupportedException();
			}

			return result;
		}

		#endregion

		#region Public Methods

		public byte[] Untile(byte[] imageData)
        {
			if (!Tiled)
				return imageData;

			byte[] imgData;

			bool isDXT = DataFormat is
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT1 or
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT2_3 or
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT4_5;

			if (isDXT) {
				imgData = new byte[imageData.Length];

				for (int i = 0; i < imageData.Length; i += 2) {
					imgData[i] = imageData[i + 1];
					imgData[i + 1] = imageData[i];
				}
			} else {
				imgData = ApplyEndian(imageData);
			}

			return DataFormat switch
            {
                GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8 => UntileTexture(imgData, ActualWidth, ActualHeight, 4, 32, 32, 1, 1),
                GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT1 => UntileTexture(imgData, ActualWidth, ActualHeight, 8, 128, 128, 4, 4),
                GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT2_3 => UntileTexture(imgData, ActualWidth, ActualHeight, 16, 128, 128, 4, 4),
                GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT4_5 => UntileTexture(imgData, ActualWidth, ActualHeight, 16, 128, 128, 4, 4),
				_ => throw new ArgumentOutOfRangeException(nameof(DataFormat), DataFormat, null)
            };
        }

		public (int, int) GetTiledWidthHeight() {
			var w = ActualWidth;
			var h = ActualHeight;
			if(!Tiled)
				return (w, h);
			(int alX, int alY) = DataFormat switch {
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8 => (32, 32),
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT1 => (128, 128),
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT2_3 => (128, 128),
				GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT4_5 => (128, 128),
				_ => throw new ArgumentOutOfRangeException(nameof(DataFormat), DataFormat, null)
			};
			w = Align(w, alX);
			h = Align(h, alY);
			return (w, h);
		}

        public override void SerializeImpl(SerializerObject s)
        {
            Common = s.Serialize<int>(Common, name: nameof(Common));
            ReferenceCount = s.Serialize<int>(ReferenceCount, name: nameof(ReferenceCount));
            Fence = s.Serialize<int>(Fence, name: nameof(Fence));
            ReadFence = s.Serialize<int>(ReadFence, name: nameof(ReadFence));
            Identifier = s.Serialize<int>(Identifier, name: nameof(Identifier));
            BaseFlush = s.Serialize<int>(BaseFlush, name: nameof(BaseFlush));
            MipFlush = s.Serialize<int>(MipFlush, name: nameof(MipFlush));

            s.DoBits<int>(b =>
            {
                Type = b.SerializeBits<GPUCONSTANTTYPE>(Type, 2, name: nameof(Type));
                SignX = b.SerializeBits<GPUSIGN>(SignX, 2, name: nameof(SignX));
                SignY = b.SerializeBits<GPUSIGN>(SignY, 2, name: nameof(SignY));
                SignZ = b.SerializeBits<GPUSIGN>(SignZ, 2, name: nameof(SignZ));
                SignW = b.SerializeBits<GPUSIGN>(SignW, 2, name: nameof(SignW));
                ClampX = b.SerializeBits<GPUCLAMP>(ClampX, 3, name: nameof(ClampX));
                ClampY = b.SerializeBits<GPUCLAMP>(ClampY, 3, name: nameof(ClampY));
                ClampZ = b.SerializeBits<GPUCLAMP>(ClampZ, 3, name: nameof(ClampZ));
                Unknown1 = b.SerializeBits<int>(Unknown1, 3, name: nameof(Unknown1));
                Pitch = b.SerializeBits<int>(Pitch, 9, name: nameof(Pitch));
                Tiled = b.SerializeBits<bool>(Tiled, 1, name: nameof(Tiled));
            });
            s.DoBits<int>(b =>
            {
                DataFormat = b.SerializeBits<GPUTEXTUREFORMAT>(DataFormat, 6, name: nameof(DataFormat));
                Endian = b.SerializeBits<GPUENDIAN>(Endian, 2, name: nameof(Endian));
                RequestSize = b.SerializeBits<GPUREQUESTSIZE>(RequestSize, 2, name: nameof(RequestSize));
                Stacked = b.SerializeBits<bool>(Stacked, 1, name: nameof(Stacked));
                ClampPolicy = b.SerializeBits<GPUCLAMPPOLICY>(ClampPolicy, 1, name: nameof(ClampPolicy));
                BaseAddress = b.SerializeBits<int>(BaseAddress, 20, name: nameof(BaseAddress));
            });
            s.DoBits<int>(b =>
            {
                Width = b.SerializeBits<int>(Width, 13, name: nameof(Width));
                Height = b.SerializeBits<int>(Height, 13, name: nameof(Height));
                b.SerializePadding(6, logIfNotNull: true);
            });
            s.DoBits<int>(b =>
            {
                NumFormat = b.SerializeBits<GPUNUMFORMAT>(NumFormat, 1, name: nameof(NumFormat));
                SwizzleX = b.SerializeBits<GPUSWIZZLE>(SwizzleX, 3, name: nameof(SwizzleX));
                SwizzleY = b.SerializeBits<GPUSWIZZLE>(SwizzleY, 3, name: nameof(SwizzleY));
                SwizzleZ = b.SerializeBits<GPUSWIZZLE>(SwizzleZ, 3, name: nameof(SwizzleZ));
                SwizzleW = b.SerializeBits<GPUSWIZZLE>(SwizzleW, 3, name: nameof(SwizzleW));
                ExpAdjust = b.SerializeBits<int>(ExpAdjust, 6, name: nameof(ExpAdjust)); // TODO: Signed?
                MagFilter = b.SerializeBits<GPUMINMAGFILTER>(MagFilter, 2, name: nameof(MagFilter));
                MinFilter = b.SerializeBits<GPUMINMAGFILTER>(MinFilter, 2, name: nameof(MinFilter));
                MipFilter = b.SerializeBits<GPUMIPFILTER>(MipFilter, 2, name: nameof(MipFilter));
                AnisoFilter = b.SerializeBits<GPUANISOFILTER>(AnisoFilter, 3, name: nameof(AnisoFilter));
                Unknown2 = b.SerializeBits<int>(Unknown2, 3, name: nameof(Unknown2));
                BorderSize = b.SerializeBits<int>(BorderSize, 1, name: nameof(BorderSize));
            });
            s.DoBits<int>(b =>
            {
                VolMagFilter = b.SerializeBits<int>(VolMagFilter, 1, name: nameof(VolMagFilter));
                VolMinFilter = b.SerializeBits<int>(VolMinFilter, 1, name: nameof(VolMinFilter));
                MinMipLevel = b.SerializeBits<int>(MinMipLevel, 4, name: nameof(MinMipLevel));
                MaxMipLevel = b.SerializeBits<int>(MaxMipLevel, 4, name: nameof(MaxMipLevel));
                MagAnisoWalk = b.SerializeBits<int>(MagAnisoWalk, 1, name: nameof(MagAnisoWalk));
                MinAnisoWalk = b.SerializeBits<int>(MinAnisoWalk, 1, name: nameof(MinAnisoWalk));
                LODBias = b.SerializeBits<int>(LODBias, 10, name: nameof(LODBias)); // TODO: Signed?
                GradExpAdjustH = b.SerializeBits<int>(GradExpAdjustH, 5, name: nameof(GradExpAdjustH)); // TODO: Signed?
                GradExpAdjustV = b.SerializeBits<int>(GradExpAdjustV, 5, name: nameof(GradExpAdjustV)); // TODO: Signed?
            });
            s.DoBits<int>(b =>
            {
                BorderColor = b.SerializeBits<GPUBORDERCOLOR>(BorderColor, 2, name: nameof(BorderColor));
                ForceBCWToMax = b.SerializeBits<bool>(ForceBCWToMax, 1, name: nameof(ForceBCWToMax));
                TriClamp = b.SerializeBits<GPUTRICLAMP>(TriClamp, 2, name: nameof(TriClamp));
                AnisoBias = b.SerializeBits<int>(AnisoBias, 4, name: nameof(AnisoBias)); // TODO: Signed?
                Dimension = b.SerializeBits<GPUDIMENSION>(Dimension, 2, name: nameof(Dimension));
                PackedMips = b.SerializeBits<bool>(PackedMips, 1, name: nameof(PackedMips));
                MipAddress = b.SerializeBits<int>(MipAddress, 20, name: nameof(MipAddress));
            });
			ImageData = s.SerializeArray<byte>(ImageData, s.CurrentLength - s.CurrentFileOffset, name: nameof(ImageData));
		}

        #endregion

        #region Enums

        public enum GPUCONSTANTTYPE
        {
            GPUCONSTANTTYPE_INVALID_TEXTURE = 0x0,
            GPUCONSTANTTYPE_INVALID_VERTEX = 0x1,
            GPUCONSTANTTYPE_TEXTURE = 0x2,
            GPUCONSTANTTYPE_VERTEX = 0x3,
        };

        public enum GPUSIGN
        {
            GPUSIGN_UNSIGNED = 0x0,
            GPUSIGN_SIGNED = 0x1,
            GPUSIGN_BIAS = 0x2,
            GPUSIGN_GAMMA = 0x3,
        };

        public enum GPUCLAMP
        {
            GPUCLAMP_WRAP = 0x0,
            GPUCLAMP_MIRROR = 0x1,
            GPUCLAMP_CLAMP_TO_LAST = 0x2,
            GPUCLAMP_MIRROR_ONCE_TO_LAST = 0x3,
            GPUCLAMP_CLAMP_HALFWAY = 0x4,
            GPUCLAMP_MIRROR_ONCE_HALFWAY = 0x5,
            GPUCLAMP_CLAMP_TO_BORDER = 0x6,
            GPUCLAMP_MIRROR_TO_BORDER = 0x7,
        };

        public enum GPUTEXTUREFORMAT
        {
            GPUTEXTUREFORMAT_1_REVERSE = 0x0,
            GPUTEXTUREFORMAT_1 = 0x1,
            GPUTEXTUREFORMAT_8 = 0x2,
            GPUTEXTUREFORMAT_1_5_5_5 = 0x3,
            GPUTEXTUREFORMAT_5_6_5 = 0x4,
            GPUTEXTUREFORMAT_6_5_5 = 0x5,
            GPUTEXTUREFORMAT_8_8_8_8 = 0x6,
            GPUTEXTUREFORMAT_2_10_10_10 = 0x7,
            GPUTEXTUREFORMAT_8_A = 0x8,
            GPUTEXTUREFORMAT_8_B = 0x9,
            GPUTEXTUREFORMAT_8_8 = 0xA,
            GPUTEXTUREFORMAT_Cr_Y1_Cb_Y0_REP = 0xB,
            GPUTEXTUREFORMAT_Y1_Cr_Y0_Cb_REP = 0xC,
            GPUTEXTUREFORMAT_16_16_EDRAM = 0xD,
            GPUTEXTUREFORMAT_8_8_8_8_A = 0xE,
            GPUTEXTUREFORMAT_4_4_4_4 = 0xF,
            GPUTEXTUREFORMAT_10_11_11 = 0x10,
            GPUTEXTUREFORMAT_11_11_10 = 0x11,
            GPUTEXTUREFORMAT_DXT1 = 0x12,
            GPUTEXTUREFORMAT_DXT2_3 = 0x13,
            GPUTEXTUREFORMAT_DXT4_5 = 0x14,
            GPUTEXTUREFORMAT_16_16_16_16_EDRAM = 0x15,
            GPUTEXTUREFORMAT_24_8 = 0x16,
            GPUTEXTUREFORMAT_24_8_FLOAT = 0x17,
            GPUTEXTUREFORMAT_16 = 0x18,
            GPUTEXTUREFORMAT_16_16 = 0x19,
            GPUTEXTUREFORMAT_16_16_16_16 = 0x1A,
            GPUTEXTUREFORMAT_16_EXPAND = 0x1B,
            GPUTEXTUREFORMAT_16_16_EXPAND = 0x1C,
            GPUTEXTUREFORMAT_16_16_16_16_EXPAND = 0x1D,
            GPUTEXTUREFORMAT_16_FLOAT = 0x1E,
            GPUTEXTUREFORMAT_16_16_FLOAT = 0x1F,
            GPUTEXTUREFORMAT_16_16_16_16_FLOAT = 0x20,
            GPUTEXTUREFORMAT_32 = 0x21,
            GPUTEXTUREFORMAT_32_32 = 0x22,
            GPUTEXTUREFORMAT_32_32_32_32 = 0x23,
            GPUTEXTUREFORMAT_32_FLOAT = 0x24,
            GPUTEXTUREFORMAT_32_32_FLOAT = 0x25,
            GPUTEXTUREFORMAT_32_32_32_32_FLOAT = 0x26,
            GPUTEXTUREFORMAT_32_AS_8 = 0x27,
            GPUTEXTUREFORMAT_32_AS_8_8 = 0x28,
            GPUTEXTUREFORMAT_16_MPEG = 0x29,
            GPUTEXTUREFORMAT_16_16_MPEG = 0x2A,
            GPUTEXTUREFORMAT_8_INTERLACED = 0x2B,
            GPUTEXTUREFORMAT_32_AS_8_INTERLACED = 0x2C,
            GPUTEXTUREFORMAT_32_AS_8_8_INTERLACED = 0x2D,
            GPUTEXTUREFORMAT_16_INTERLACED = 0x2E,
            GPUTEXTUREFORMAT_16_MPEG_INTERLACED = 0x2F,
            GPUTEXTUREFORMAT_16_16_MPEG_INTERLACED = 0x30,
            GPUTEXTUREFORMAT_DXN = 0x31,
            GPUTEXTUREFORMAT_8_8_8_8_AS_16_16_16_16 = 0x32,
            GPUTEXTUREFORMAT_DXT1_AS_16_16_16_16 = 0x33,
            GPUTEXTUREFORMAT_DXT2_3_AS_16_16_16_16 = 0x34,
            GPUTEXTUREFORMAT_DXT4_5_AS_16_16_16_16 = 0x35,
            GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16 = 0x36,
            GPUTEXTUREFORMAT_10_11_11_AS_16_16_16_16 = 0x37,
            GPUTEXTUREFORMAT_11_11_10_AS_16_16_16_16 = 0x38,
            GPUTEXTUREFORMAT_32_32_32_FLOAT = 0x39,
            GPUTEXTUREFORMAT_DXT3A = 0x3A,
            GPUTEXTUREFORMAT_DXT5A = 0x3B,
            GPUTEXTUREFORMAT_CTX1 = 0x3C,
            GPUTEXTUREFORMAT_DXT3A_AS_1_1_1_1 = 0x3D,
            GPUTEXTUREFORMAT_8_8_8_8_GAMMA_EDRAM = 0x3E,
            GPUTEXTUREFORMAT_2_10_10_10_FLOAT_EDRAM = 0x3F,
        };

        public enum GPUENDIAN
        {
            GPUENDIAN_NONE = 0x0,
            GPUENDIAN_8IN16 = 0x1,
            GPUENDIAN_8IN32 = 0x2,
            GPUENDIAN_16IN32 = 0x3
        }

        public enum GPUREQUESTSIZE
        {
            GPUREQUESTSIZE_256BIT = 0x0,
            GPUREQUESTSIZE_512BIT = 0x1,
        };

        public enum GPUCLAMPPOLICY
        {
            GPUCLAMPPOLICY_D3D = 0x0,
            GPUCLAMPPOLICY_OGL = 0x1,
        };

        public enum GPUNUMFORMAT
        {
            GPUNUMFORMAT_FRACTION = 0x0,
            GPUNUMFORMAT_INTEGER = 0x1,
        };

        public enum GPUSWIZZLE
        {
            GPUSWIZZLE_X = 0x0,
            GPUSWIZZLE_Y = 0x1,
            GPUSWIZZLE_Z = 0x2,
            GPUSWIZZLE_W = 0x3,
            GPUSWIZZLE_0 = 0x4,
            GPUSWIZZLE_1 = 0x5,
            GPUSWIZZLE_KEEP = 0x7,
        };

        public enum GPUMINMAGFILTER
        {
            GPUMINMAGFILTER_POINT = 0x0,
            GPUMINMAGFILTER_LINEAR = 0x1,
            GPUMINMAGFILTER_KEEP = 0x3,
        };

        public enum GPUMIPFILTER
        {
            GPUMIPFILTER_POINT = 0x0,
            GPUMIPFILTER_LINEAR = 0x1,
            GPUMIPFILTER_BASEMAP = 0x2,
            GPUMIPFILTER_KEEP = 0x3,
        };

        public enum GPUANISOFILTER
        {
            GPUANISOFILTER_DISABLED = 0x0,
            GPUANISOFILTER_MAX1TO1 = 0x1,
            GPUANISOFILTER_MAX2TO1 = 0x2,
            GPUANISOFILTER_MAX4TO1 = 0x3,
            GPUANISOFILTER_MAX8TO1 = 0x4,
            GPUANISOFILTER_MAX16TO1 = 0x5,
            GPUANISOFILTER_KEEP = 0x7,
        };

        public enum GPUBORDERCOLOR
        {
            GPUBORDERCOLOR_ABGR_BLACK = 0x0,
            GPUBORDERCOLOR_ABGR_WHITE = 0x1,
            GPUBORDERCOLOR_ACBYCR_BLACK = 0x2,
            GPUBORDERCOLOR_ACBCRY_BLACK = 0x3,
        };

        public enum GPUTRICLAMP
        {
            GPUTRICLAMP_NORMAL = 0x0,
            GPUTRICLAMP_ONE_SIXTH = 0x1,
            GPUTRICLAMP_ONE_FOURTH = 0x2,
            GPUTRICLAMP_THREE_EIGHTHS = 0x3,
        };

        public enum GPUDIMENSION
        {
            GPUDIMENSION_1D = 0x0,
            GPUDIMENSION_2D = 0x1,
            GPUDIMENSION_3D = 0x2,
            GPUDIMENSION_CUBEMAP = 0x3,
        };

        #endregion
    }
}