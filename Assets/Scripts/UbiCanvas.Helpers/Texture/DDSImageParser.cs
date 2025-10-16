using System;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using BinarySerializer;

namespace UbiCanvas.Helpers {
	#region DDSImage Class
	public class DDSImage : IDisposable {
		#region Variables
		private bool m_makeNoLongerReadable = false;
		private bool m_isValid = false;
		private Texture2D m_bitmap = null;
		#endregion

		#region Constructor/Destructor
		public DDSImage(byte[] ddsImage, bool makeNoLongerReadable = true) {
			m_makeNoLongerReadable = makeNoLongerReadable;
			if (ddsImage == null) return;
			if (ddsImage.Length == 0) return;

			this.Parse(ddsImage);
		}

		public DDSImage(Stream ddsImage, bool makeNoLongerReadable = true) {
			m_makeNoLongerReadable = makeNoLongerReadable;
			if (ddsImage == null) return;
			if (!ddsImage.CanRead) return;

			using (BinaryReader reader = new BinaryReader(ddsImage)) {
				this.Parse(reader);
			}
		}

		private DDSImage(Texture2D bitmap, bool makeNoLongerReadable = true) {
			m_makeNoLongerReadable = makeNoLongerReadable;
			this.m_bitmap = bitmap;
		}
		#endregion

		#region Override Methods
		#endregion

		#region Private Methods
		private void Parse(BinaryReader reader) {
			DDSStruct header = new DDSStruct();
			PixelFormat pixelFormat = PixelFormat.UNKNOWN;
			byte[] data = null;

			if (this.ReadHeader(reader, ref header)) {
				this.m_isValid = true;
				// patches for stuff
				if (header.depth == 0) header.depth = 1;

				uint blocksize = 0;
				pixelFormat = this.GetFormat(header, ref blocksize);
				if (pixelFormat == PixelFormat.UNKNOWN) {
					throw new InvalidFileHeaderException();
				}

				data = this.ReadData(reader, header);
				if (data != null) {
					byte[] rawData = this.DecompressData(header, data, pixelFormat);
					this.m_bitmap = this.CreateBitmap((int)header.width, (int)header.height, rawData);
				}
			}
		}
		private void Parse(byte[] data) {
			DDSStruct header = new DDSStruct();
			PixelFormat pixelFormat = PixelFormat.UNKNOWN;
			bool goodHeader = false;

			using (MemoryStream stream = new MemoryStream(data)) {
				stream.Seek(0, SeekOrigin.Begin);

				using (BinaryReader reader = new BinaryReader(stream)) {
					goodHeader = this.ReadHeader(reader, ref header);
				}
			}
			if (goodHeader) {
				this.m_isValid = true;
				// patches for stuff
				if (header.depth == 0) header.depth = 1;

				uint blocksize = 0;
				pixelFormat = this.GetFormat(header, ref blocksize);
				if (pixelFormat == PixelFormat.UNKNOWN) {
					throw new InvalidFileHeaderException();
				}
				if (data != null) {
					byte[] rawData = this.DecompressData(header, data, pixelFormat);
					this.m_bitmap = this.CreateBitmap((int)header.width, (int)header.height, rawData);
				}
			}
		}

		private byte[] ReadData(BinaryReader reader, DDSStruct header) {
			byte[] compdata = null;
			uint compsize = 0;

			if ((header.flags & DDSD_LINEARSIZE) > 1) {
				compdata = reader.ReadBytes((int)header.sizeorpitch);
				compsize = (uint)compdata.Length;
			} else {
				uint bps = header.width * header.pixelformat.rgbbitcount / 8;
				compsize = bps * header.height * header.depth;
				compdata = reader.ReadBytes((int)compsize);
				/*compdata = new byte[compsize];

				MemoryStream mem = new MemoryStream((int)compsize);

				byte[] temp;
				for (int z = 0; z < header.depth; z++) {
					for (int y = 0; y < header.height; y++) {
						temp = reader.ReadBytes((int)bps);
						mem.Write(temp, 0, temp.Length);
					}
				}
				mem.Seek(0, SeekOrigin.Begin);

				mem.Read(compdata, 0, compdata.Length);
				mem.Close();*/
			}

			return compdata;
		}

		private Texture2D CreateBitmap(int width, int height, byte[] rawData) {
			Texture2D bitmap = new Texture2D(width, height, TextureFormat.RGBA32, false);
			bitmap.LoadRawTextureData(rawData);
			if (width == height) {
				bitmap.Apply(true, m_makeNoLongerReadable);
			} else {
				bitmap.Apply(true, false);
			}
			return bitmap;
		}

		private bool ReadHeader(BinaryReader reader, ref DDSStruct header) {
			byte[] signature = reader.ReadBytes(4);
			if (!(signature[0] == 'D' && signature[1] == 'D' && signature[2] == 'S' && signature[3] == ' '))
				return false;

			header.size = reader.ReadUInt32();
			if (header.size != 124)
				return false;

			//convert the data
			header.flags = reader.ReadUInt32();
			header.height = reader.ReadUInt32();
			header.width = reader.ReadUInt32();
			header.sizeorpitch = reader.ReadUInt32();
			header.depth = reader.ReadUInt32();
			header.mipmapcount = reader.ReadUInt32();
			header.alphabitdepth = reader.ReadUInt32();

			header.reserved = new uint[10];
			for (int i = 0; i < 10; i++) {
				header.reserved[i] = reader.ReadUInt32();
			}

			//pixelfromat
			header.pixelformat.size = reader.ReadUInt32();
			header.pixelformat.flags = reader.ReadUInt32();
			header.pixelformat.fourcc = reader.ReadUInt32();
			header.pixelformat.rgbbitcount = reader.ReadUInt32();
			header.pixelformat.rbitmask = reader.ReadUInt32();
			header.pixelformat.gbitmask = reader.ReadUInt32();
			header.pixelformat.bbitmask = reader.ReadUInt32();
			header.pixelformat.alphabitmask = reader.ReadUInt32();

			//caps
			header.ddscaps.caps1 = reader.ReadUInt32();
			header.ddscaps.caps2 = reader.ReadUInt32();
			header.ddscaps.caps3 = reader.ReadUInt32();
			header.ddscaps.caps4 = reader.ReadUInt32();
			header.texturestage = reader.ReadUInt32();

			return true;
		}

		private PixelFormat GetFormat(DDSStruct header, ref uint blocksize) {
			PixelFormat format = PixelFormat.UNKNOWN;
			if ((header.pixelformat.flags & DDPF_FOURCC) == DDPF_FOURCC) {
				blocksize = ((header.width + 3) / 4) * ((header.height + 3) / 4) * header.depth;

				switch (header.pixelformat.fourcc) {
					case FOURCC_DXT1:
						format = PixelFormat.DXT1;
						blocksize *= 8;
						break;

					case FOURCC_DXT2:
						format = PixelFormat.DXT2;
						blocksize *= 16;
						break;

					case FOURCC_DXT3:
						format = PixelFormat.DXT3;
						blocksize *= 16;
						break;

					case FOURCC_DXT4:
						format = PixelFormat.DXT4;
						blocksize *= 16;
						break;

					case FOURCC_DXT5:
						format = PixelFormat.DXT5;
						blocksize *= 16;
						break;

					case FOURCC_ATI1:
						format = PixelFormat.ATI1N;
						blocksize *= 8;
						break;

					case FOURCC_ATI2:
						format = PixelFormat.THREEDC;
						blocksize *= 16;
						break;

					case FOURCC_RXGB:
						format = PixelFormat.RXGB;
						blocksize *= 16;
						break;

					case FOURCC_DOLLARNULL:
						format = PixelFormat.A16B16G16R16;
						blocksize = header.width * header.height * header.depth * 8;
						break;

					case FOURCC_oNULL:
						format = PixelFormat.R16F;
						blocksize = header.width * header.height * header.depth * 2;
						break;

					case FOURCC_pNULL:
						format = PixelFormat.G16R16F;
						blocksize = header.width * header.height * header.depth * 4;
						break;

					case FOURCC_qNULL:
						format = PixelFormat.A16B16G16R16F;
						blocksize = header.width * header.height * header.depth * 8;
						break;

					case FOURCC_rNULL:
						format = PixelFormat.R32F;
						blocksize = header.width * header.height * header.depth * 4;
						break;

					case FOURCC_sNULL:
						format = PixelFormat.G32R32F;
						blocksize = header.width * header.height * header.depth * 8;
						break;

					case FOURCC_tNULL:
						format = PixelFormat.A32B32G32R32F;
						blocksize = header.width * header.height * header.depth * 16;
						break;

					default:
						format = PixelFormat.UNKNOWN;
						blocksize *= 16;
						break;
				} // switch
			} else {
				// uncompressed image
				if ((header.pixelformat.flags & DDPF_LUMINANCE) == DDPF_LUMINANCE) {
					if ((header.pixelformat.flags & DDPF_ALPHAPIXELS) == DDPF_ALPHAPIXELS) {
						format = PixelFormat.LUMINANCE_ALPHA;
					} else {
						format = PixelFormat.LUMINANCE;
					}
				} else {
					if ((header.pixelformat.flags & DDPF_ALPHAPIXELS) == DDPF_ALPHAPIXELS) {
						format = PixelFormat.RGBA;
					} else {
						format = PixelFormat.RGB;
					}
				}

				blocksize = (header.width * header.height * header.depth * (header.pixelformat.rgbbitcount >> 3));
			}

			return format;
		}

		#region Helper Methods
		// iCompFormatToBpp
		private uint PixelFormatToBpp(PixelFormat pf, uint rgbbitcount) {
			switch (pf) {
				case PixelFormat.LUMINANCE:
				case PixelFormat.LUMINANCE_ALPHA:
				case PixelFormat.RGBA:
				case PixelFormat.RGB:
					return rgbbitcount / 8;

				case PixelFormat.THREEDC:
				case PixelFormat.RXGB:
					return 3;

				case PixelFormat.ATI1N:
					return 1;

				case PixelFormat.R16F:
					return 2;

				case PixelFormat.A16B16G16R16:
				case PixelFormat.A16B16G16R16F:
				case PixelFormat.G32R32F:
					return 8;

				case PixelFormat.A32B32G32R32F:
					return 16;

				default:
					return 4;
			}
		}

		// iCompFormatToBpc
		private uint PixelFormatToBpc(PixelFormat pf) {
			switch (pf) {
				case PixelFormat.R16F:
				case PixelFormat.G16R16F:
				case PixelFormat.A16B16G16R16F:
					return 4;

				case PixelFormat.R32F:
				case PixelFormat.G32R32F:
				case PixelFormat.A32B32G32R32F:
					return 4;

				case PixelFormat.A16B16G16R16:
					return 2;

				default:
					return 1;
			}
		}

		private bool Check16BitComponents(DDSStruct header) {
			if (header.pixelformat.rgbbitcount != 32)
				return false;
			// a2b10g10r10 format
			if (header.pixelformat.rbitmask == 0x3FF00000 && header.pixelformat.gbitmask == 0x000FFC00 && header.pixelformat.bbitmask == 0x000003FF
				&& header.pixelformat.alphabitmask == 0xC0000000)
				return true;
			// a2r10g10b10 format
			else if (header.pixelformat.rbitmask == 0x000003FF && header.pixelformat.gbitmask == 0x000FFC00 && header.pixelformat.bbitmask == 0x3FF00000
				&& header.pixelformat.alphabitmask == 0xC0000000)
				return true;

			return false;
		}

		private void CorrectPremult(uint pixnum, ref byte[] buffer) {
			for (uint i = 0; i < pixnum; i++) {
				byte alpha = buffer[(i * 4) + 3];
				if (alpha == 0) continue;
				int red = (buffer[(i * 4)] << 8) / alpha;
				int green = (buffer[(i * 4) + 1] << 8) / alpha;
				int blue = (buffer[(i * 4) + 2] << 8) / alpha;

				buffer[(i * 4)] = (byte)red;
				buffer[(i * 4) + 1] = (byte)green;
				buffer[(i * 4) + 2] = (byte)blue;
			}
		}

		private void ComputeMaskParams(uint mask, ref int shift1, ref int mul, ref int shift2) {
			shift1 = 0; mul = 1; shift2 = 0;
			while ((mask & 1) == 0) {
				mask >>= 1;
				shift1++;
			}
			uint bc = 0;
			while ((mask & (1 << (int)bc)) != 0) bc++;
			while ((mask * mul) < 255)
				mul = (mul << (int)bc) + 1;
			mask *= (uint)mul;

			while ((mask & ~0xff) != 0) {
				mask >>= 1;
				shift2++;
			}
		}

		private void DxtcReadColors(byte[] data, int pos, ref ColorFloat[] op) {
			ushort colour0 = ToUInt16(data, pos);
			ushort colour1 = ToUInt16(data, pos + 2);
			DxtcReadColor(colour0, ref op[0]);
			DxtcReadColor(colour1, ref op[1]);
		}

		private static void DxtcReadColor(ushort color, ref ColorFloat c) {
			// Extract 5/6/5 bits
			int r5 = BitHelpers.ExtractBits(color, 5, 11);
			int g6 = BitHelpers.ExtractBits(color, 6, 5);
			int b5 = BitHelpers.ExtractBits(color, 5, 0);

			/*c.red   = (byte)((r5 << 3) | (r5 >> 2));
			c.green = (byte)((g6 << 2) | (g6 >> 4));
			c.blue  = (byte)((b5 << 3) | (b5 >> 2));*/
			/*byte Conv(int v, float max) {
				return (byte)MathF.Round(((float)v * (255f/max)));
			}
			c.red = Conv(r5, 31);
			c.green = Conv(g6, 63);
			c.blue = Conv(b5, 31);*/
			float Conv(int v, float max) {
				return ((float)v * (255f / max));
			}
			c.red = Conv(r5, 31);
			c.green = Conv(g6, 63);
			c.blue = Conv(b5, 31);


			c.alpha = 255;
		}

		private void GetBitsFromMask(uint mask, ref uint shiftLeft, ref uint shiftRight) {
			uint temp, i;

			if (mask == 0) {
				shiftLeft = shiftRight = 0;
				return;
			}

			temp = mask;
			for (i = 0; i < 32; i++, temp >>= 1) {
				if ((temp & 1) != 0)
					break;
			}
			shiftRight = i;

			// Temp is preserved, so use it again:
			for (i = 0; i < 8; i++, temp >>= 1) {
				if ((temp & 1) == 0)
					break;
			}
			shiftLeft = 8 - i;
		}

		// This function simply counts how many contiguous bits are in the mask.
		private uint CountBitsFromMask(uint mask) {
			uint i, testBit = 0x01, count = 0;
			bool foundBit = false;

			for (i = 0; i < 32; i++, testBit <<= 1) {
				if ((mask & testBit) != 0) {
					if (!foundBit)
						foundBit = true;
					count++;
				} else if (foundBit)
					return count;
			}

			return count;
		}

		private uint HalfToFloat(ushort y) {
			int s = (y >> 15) & 0x00000001;
			int e = (y >> 10) & 0x0000001f;
			int m = y & 0x000003ff;

			if (e == 0) {
				if (m == 0) {
					//
					// Plus or minus zero
					//
					return (uint)(s << 31);
				} else {
					//
					// Denormalized number -- renormalize it
					//
					while ((m & 0x00000400) == 0) {
						m <<= 1;
						e -= 1;
					}

					e += 1;
					m &= ~0x00000400;
				}
			} else if (e == 31) {
				if (m == 0) {
					//
					// Positive or negative infinity
					//
					return (uint)((s << 31) | 0x7f800000);
				} else {
					//
					// Nan -- preserve sign and significand bits
					//
					return (uint)((s << 31) | 0x7f800000 | (m << 13));
				}
			}

			//
			// Normalized number
			//
			e = e + (127 - 15);
			m = m << 13;

			//
			// Assemble s, e and m.
			//
			return (uint)((s << 31) | (e << 23) | m);
		}

		/*private unsafe void ConvFloat16ToFloat32(uint* dest, ushort* src, uint size) {
			uint i;
			for (i = 0; i < size; ++i, ++dest, ++src) {
				//float: 1 sign bit, 8 exponent bits, 23 mantissa bits
				//half: 1 sign bit, 5 exponent bits, 10 mantissa bits
				*dest = HalfToFloat(*src);
			}
		}

		private unsafe void ConvG16R16ToFloat32(uint* dest, ushort* src, uint size) {
			uint i;
			for (i = 0; i < size; i += 3) {
				//float: 1 sign bit, 8 exponent bits, 23 mantissa bits
				//half: 1 sign bit, 5 exponent bits, 10 mantissa bits
				*dest++ = HalfToFloat(*src++);
				*dest++ = HalfToFloat(*src++);
				*((float*)dest++) = 1.0f;
			}
		}

		private unsafe void ConvR16ToFloat32(uint* dest, ushort* src, uint size) {
			uint i;
			for (i = 0; i < size; i += 3) {
				//float: 1 sign bit, 8 exponent bits, 23 mantissa bits
				//half: 1 sign bit, 5 exponent bits, 10 mantissa bits
				*dest++ = HalfToFloat(*src++);
				*((float*)dest++) = 1.0f;
				*((float*)dest++) = 1.0f;
			}
		}*/
		#endregion

		#region Decompress Methods
		private byte[] DecompressData(DDSStruct header, byte[] data, PixelFormat pixelFormat) {
			//Debug.WriteLine(pixelFormat);
			// allocate bitmap
			byte[] rawData = null;

			int pos = (int)(header.size + 4); // 4 for "DDS "

			switch (pixelFormat) {
				case PixelFormat.RGBA:
					rawData = this.DecompressRGBA(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.RGB:
					rawData = this.DecompressRGB(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.LUMINANCE:
				case PixelFormat.LUMINANCE_ALPHA:
					rawData = this.DecompressLum(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.DXT1:
					rawData = this.DecompressDXT1(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.DXT2:
					rawData = this.DecompressDXT2(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.DXT3:
					rawData = this.DecompressDXT3(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.DXT4:
					rawData = this.DecompressDXT4(header, data, pixelFormat, pos: pos);
					break;

				case PixelFormat.DXT5:
					rawData = this.DecompressDXT5(header, data, pixelFormat, pos: pos);
					break;

				/*case PixelFormat.THREEDC:
					rawData = this.Decompress3Dc(header, data, pixelFormat);
					break;

				case PixelFormat.ATI1N:
					rawData = this.DecompressAti1n(header, data, pixelFormat);
					break;

				case PixelFormat.RXGB:
					rawData = this.DecompressRXGB(header, data, pixelFormat);
					break;

				case PixelFormat.R16F:
				case PixelFormat.G16R16F:
				case PixelFormat.A16B16G16R16F:
				case PixelFormat.R32F:
				case PixelFormat.G32R32F:
				case PixelFormat.A32B32G32R32F:
					rawData = this.DecompressFloat(header, data, pixelFormat);
					break;*/

				default:
					throw new Exception("Unknown format: " + pixelFormat);
			}

			return rawData;
		}

		private byte[] DecompressDXT1(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int bpp = (int)(PixelFormatToBpp(pixelFormat, header.pixelformat.rgbbitcount));
			int bps = (int)(header.width * bpp * PixelFormatToBpc(pixelFormat));
			int sizeofplane = (int)(bps * header.height);
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;



			// DXT1 decompressor
			byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

			ColorFloat[] colours = new ColorFloat[4];
			for (int i = 0; i < 4; i++) colours[i].alpha = 0xFF;

			int temp = pos;
			for (int z = 0; z < depth; z++) {
				for (int y = 0; y < height; y += 4) {
					for (int x = 0; x < width; x += 4) {
						ushort colour0 = ToUInt16(data, temp);
						ushort colour1 = ToUInt16(data, temp+2);
						DxtcReadColor(colour0, ref colours[0]);
						DxtcReadColor(colour1, ref colours[1]);

						uint bitmask = ToUInt32(data, temp+4);
						temp += 8;

						if (colour0 > colour1) {
							// Four-color block: derive the other two colors.
							// 00 = color_0, 01 = color_1, 10 = color_2, 11 = color_3
							// These 2-bit codes correspond to the 2-bit fields
							// stored in the 64-bit block.
							colours[2].red = ComputeColorMix(colours[0].red, colours[1].red);
							colours[2].green = ComputeColorMix(colours[0].green, colours[1].green);
							colours[2].blue = ComputeColorMix(colours[0].blue, colours[1].blue);
							colours[2].alpha = 255;

							colours[3].red = ComputeColorMixInv(colours[0].red, colours[1].red);
							colours[3].green = ComputeColorMixInv(colours[0].green, colours[1].green);
							colours[3].blue = ComputeColorMixInv(colours[0].blue, colours[1].blue);
							colours[3].alpha = 255;
						} else {
							// Three-color block: derive the other color.
							// 00 = color_0,  01 = color_1,  10 = color_2,
							// 11 = transparent.
							// These 2-bit codes correspond to the 2-bit fields 
							// stored in the 64-bit block. 
							colours[2].red = ComputeColorMixEqual(colours[0].red, colours[1].red);
							colours[2].green = ComputeColorMixEqual(colours[0].green, colours[1].green);
							colours[2].blue = ComputeColorMixEqual(colours[0].blue, colours[1].blue);
							colours[2].alpha = 255;

							colours[3].red = 0;
							colours[3].green = 0;
							colours[3].blue = 0;
							colours[3].alpha = 0;
						}

						for (int j = 0, k = 0; j < 4; j++) {
							for (int i = 0; i < 4; i++, k++) {
								int select = (int)((bitmask & (0x03 << k * 2)) >> k * 2);
								ColorFloat col = colours[select];
								if (((x + i) < width) && ((y + j) < height)) {
									uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp);
									rawData[offset + 0] = (byte)MathF.Round(col.red);
									rawData[offset + 1] = (byte)MathF.Round(col.green);
									rawData[offset + 2] = (byte)MathF.Round(col.blue);
									rawData[offset + 3] = (byte)MathF.Round(col.alpha);
								}
							}
						}
					}
				}
			}

			return rawData;
		}

		private byte[] DecompressDXT2(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			// Can do color & alpha same as dxt3, but color is pre-multiplied
			// so the result will be wrong unless corrected.
			byte[] rawData = DecompressDXT3(header, data, pixelFormat, pos: pos);
			CorrectPremult((uint)(width * height * depth), ref rawData);

			return rawData;
		}

		private ushort ToUInt16(byte[] data, int temp) {
			return (ushort)(((ushort)data[temp + 1] << 8) | (ushort)data[temp]);
		}

		private uint ToUInt32(byte[] data, int temp) {
			return (uint)(((uint)data[temp + 3] << 24) | ((uint)data[temp + 2] << 16) | ((uint)data[temp + 1] << 8) | (uint)data[temp]);
		}
		private float Lerp(float c0, float c1, float t)
			=> c0 + t * (c1 - c0);
		private float ComputeColorMix(float c0, float c1) {
			return Lerp(c0, c1, 1/3f);
			//return (((2/3f) * c0 + (1/3f) * c1));
			//return (byte)((5 * c0 + 3 * c1) >> 3);
			//return (byte)((2 * c0 + c1 + 2) / 3); // +1 for rounding
			// (byte)Math.Round((2 * colours[0].red + colours[1].red + 1) / 3f);
		}
		private float ComputeColorMixInv(float c0, float c1) {
			return Lerp(c0, c1, 2/3f);
			// => ComputeColorMix(c1, c0);
		}

		private float ComputeColorMixEqual(float c0, float c1) {
			return Lerp(c0, c1, 1/2f);
			//return (c0 + c1) / 2f;
			//(byte)Math.Round((colours[0].blue + colours[1].blue + 1) / 2f)
		}

		private byte[] DecompressDXT3(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int bpp = (int)(PixelFormatToBpp(pixelFormat, header.pixelformat.rgbbitcount));
			int bps = (int)(header.width * bpp * PixelFormatToBpc(pixelFormat));
			int sizeofplane = (int)(bps * header.height);
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			// DXT3 decompressor
			byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];
			ColorFloat[] colours = new ColorFloat[4];
			
			int temp = pos;
			int z, y, x, alpha, i, j, k, select;
			uint offset;
			ushort word;

			for (z = 0; z < depth; z++) {
				for (y = 0; y < height; y += 4) {
					for (x = 0; x < width; x += 4) {
						alpha = temp;
						temp += 8;
						
						ushort colour0 = ToUInt16(data, temp);
						ushort colour1 = ToUInt16(data, temp+2);
						temp += 4;

						DxtcReadColor(colour0, ref colours[0]);
						DxtcReadColor(colour1, ref colours[1]);
						
						uint bitmask;
						try {
							bitmask = ToUInt32(data, temp);
							temp += 4;
						} catch (Exception ex) {
							MonoBehaviour.print("StartIndex: " + (temp + 4) + " - Size: " + data.Length);
							throw ex;
						}

						// Four-color block: derive the other two colors.
						// 00 = color_0, 01 = color_1, 10 = color_2, 11 = color_3
						// These 2-bit codes correspond to the 2-bit fields
						// stored in the 64-bit block.
						colours[2].red = ComputeColorMix(colours[0].red, colours[1].red);
						colours[2].green = ComputeColorMix(colours[0].green, colours[1].green);
						colours[2].blue = ComputeColorMix(colours[0].blue, colours[1].blue);
						colours[2].alpha = 255;

						colours[3].red = ComputeColorMixInv(colours[0].red, colours[1].red);
						colours[3].green = ComputeColorMixInv(colours[0].green, colours[1].green);
						colours[3].blue = ComputeColorMixInv(colours[0].blue, colours[1].blue);
						colours[3].alpha = 255;

						for (j = 0, k = 0; j < 4; j++) {
							for (i = 0; i < 4; k++, i++) {
								select = (int)((bitmask & (0x03 << k * 2)) >> k * 2);

								if (((x + i) < width) && ((y + j) < height)) {
									offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp);
									rawData[offset + 0] = (byte)MathF.Round(colours[select].red);
									rawData[offset + 1] = (byte)MathF.Round(colours[select].green);
									rawData[offset + 2] = (byte)MathF.Round(colours[select].blue);
								}
							}
						}

						for (j = 0; j < 4; j++) {
							//ushort word = (ushort)(alpha[2 * j] + 256 * alpha[2 * j + 1]);
							word = ToUInt16(data, alpha + 2 * j);
							for (i = 0; i < 4; i++) {
								if (((x + i) < width) && ((y + j) < height)) {
									offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp + 3);
									// Convert 4-bit alpha to 8-bit by multiplying by 17 (0x11)
									rawData[offset] = (byte)((word & 0x0F) * 17);
								}
								word >>= 4;
							}
						}
					}
				}
			}
			return rawData;
		}

		private byte[] DecompressDXT4(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			// Can do color & alpha same as dxt5, but color is pre-multiplied
			// so the result will be wrong unless corrected.
			byte[] rawData = DecompressDXT5(header, data, pixelFormat, pos: pos);
			CorrectPremult((uint)(width * height * depth), ref rawData);

			return rawData;
		}

		private byte[] DecompressDXT5(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int bpp = (int)(PixelFormatToBpp(pixelFormat, header.pixelformat.rgbbitcount));
			int bps = (int)(header.width * bpp * PixelFormatToBpc(pixelFormat));
			int sizeofplane = (int)(bps * header.height);
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];
			ColorFloat[] colours = new ColorFloat[4];
			ushort[] alphas = new ushort[8];
			
			int temp = pos;
			for (int z = 0; z < depth; z++) {
				for (int y = 0; y < height; y += 4) {
					for (int x = 0; x < width; x += 4) {
						if (y >= height || x >= width)
							break;

						alphas[0] = data[temp+0];
						alphas[1] = data[temp+1];
						int alphamask = (temp + 2);
						temp += 8;

						DxtcReadColors(data, temp, ref colours);
						uint bitmask = ToUInt32(data, temp + 4);
						temp += 8;

						// Four-color block: derive the other two colors.
						// 00 = color_0, 01 = color_1, 10 = color_2, 11	= color_3
						// These 2-bit codes correspond to the 2-bit fields
						// stored in the 64-bit block.

						colours[2].red = ComputeColorMix(colours[0].red, colours[1].red);
						colours[2].green = ComputeColorMix(colours[0].green, colours[1].green);
						colours[2].blue = ComputeColorMix(colours[0].blue, colours[1].blue);
						//colours[2].alpha = 0xFF;

						colours[3].red = ComputeColorMixInv(colours[0].red, colours[1].red);
						colours[3].green = ComputeColorMixInv(colours[0].green, colours[1].green);
						colours[3].blue = ComputeColorMixInv(colours[0].blue, colours[1].blue);
						//colours[3].alpha = 0xFF;

						int k = 0;
						for (int j = 0; j < 4; j++) {
							for (int i = 0; i < 4; k++, i++) {
								int select = (int)((bitmask & (0x03 << k * 2)) >> k * 2);
								ColorFloat col = colours[select];
								// only put pixels out < width or height
								if (((x + i) < width) && ((y + j) < height)) {
									uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp);
									rawData[offset] = (byte)MathF.Round(col.red);
									rawData[offset + 1] = (byte)MathF.Round(col.green);
									rawData[offset + 2] = (byte)MathF.Round(col.blue);
								}
							}
						}

						// 8-alpha or 6-alpha block?
						if (alphas[0] > alphas[1]) {
							// 8-alpha block:  derive the other six alphas.
							// Bit code 000 = alpha_0, 001 = alpha_1, others are interpolated.
							alphas[2] = (ushort)((6 * alphas[0] + 1 * alphas[1] + 3) / 7); // bit code 010
							alphas[3] = (ushort)((5 * alphas[0] + 2 * alphas[1] + 3) / 7); // bit code 011
							alphas[4] = (ushort)((4 * alphas[0] + 3 * alphas[1] + 3) / 7); // bit code 100
							alphas[5] = (ushort)((3 * alphas[0] + 4 * alphas[1] + 3) / 7); // bit code 101
							alphas[6] = (ushort)((2 * alphas[0] + 5 * alphas[1] + 3) / 7); // bit code 110
							alphas[7] = (ushort)((1 * alphas[0] + 6 * alphas[1] + 3) / 7); // bit code 111
						} else {
							// 6-alpha block.
							// Bit code 000 = alpha_0, 001 = alpha_1, others are interpolated.
							alphas[2] = (ushort)((4 * alphas[0] + 1 * alphas[1] + 2) / 5); // Bit code 010
							alphas[3] = (ushort)((3 * alphas[0] + 2 * alphas[1] + 2) / 5); // Bit code 011
							alphas[4] = (ushort)((2 * alphas[0] + 3 * alphas[1] + 2) / 5); // Bit code 100
							alphas[5] = (ushort)((1 * alphas[0] + 4 * alphas[1] + 2) / 5); // Bit code 101
							alphas[6] = 0x00; // Bit code 110
							alphas[7] = 0xFF; // Bit code 111
						}

						// Note: Have to separate the next two loops,
						// it operates on a 6-byte system.

						// First three bytes
						//uint bits = (uint)(alphamask[0]);
						uint bits = (uint)((data[alphamask+0]) | (data[alphamask+1] << 8) | (data[alphamask+2] << 16));
						for (int j = 0; j < 2; j++) {
							for (int i = 0; i < 4; i++) {
								// only put pixels out < width or height
								if (((x + i) < width) && ((y + j) < height)) {
									uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp + 3);
									rawData[offset] = (byte)alphas[bits & 0x07];
								}
								bits >>= 3;
							}
						}

						// Last three bytes
						//bits = (uint)(alphamask[3]);
						bits = (uint)((data[alphamask + 3]) | (data[alphamask + 4] << 8) | (data[alphamask + 5] << 16));
						for (int j = 2; j < 4; j++) {
							for (int i = 0; i < 4; i++) {
								// only put pixels out < width or height
								if (((x + i) < width) && ((y + j) < height)) {
									uint offset = (uint)(z * sizeofplane + (y + j) * bps + (x + i) * bpp + 3);
									rawData[offset] = (byte)alphas[bits & 0x07];
								}
								bits >>= 3;
							}
						}
					}
				}
			}

			return rawData;
		}

		private byte[] DecompressRGB(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int bpp = (int)(this.PixelFormatToBpp(pixelFormat, header.pixelformat.rgbbitcount));
			int bps = (int)(header.width * bpp * this.PixelFormatToBpc(pixelFormat));
			int sizeofplane = (int)(bps * header.height);
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

			uint valMask = (uint)((header.pixelformat.rgbbitcount == 32) ? ~0 : (1 << (int)header.pixelformat.rgbbitcount) - 1);
			uint pixSize = (uint)(((int)header.pixelformat.rgbbitcount + 7) / 8);
			int rShift1 = 0; int rMul = 0; int rShift2 = 0;
			ComputeMaskParams(header.pixelformat.rbitmask, ref rShift1, ref rMul, ref rShift2);
			int gShift1 = 0; int gMul = 0; int gShift2 = 0;
			ComputeMaskParams(header.pixelformat.gbitmask, ref gShift1, ref gMul, ref gShift2);
			int bShift1 = 0; int bMul = 0; int bShift2 = 0;
			ComputeMaskParams(header.pixelformat.bbitmask, ref bShift1, ref bMul, ref bShift2);

			int offset = 0;
			int pixnum = width * height * depth;
			int temp = pos;
			while (pixnum-- > 0) {
				uint px = ToUInt32(data,temp) & valMask;
				temp += (int)pixSize;
				uint pxc = px & header.pixelformat.rbitmask;
				rawData[offset + 0] = (byte)(((pxc >> rShift1) * rMul) >> rShift2);
				pxc = px & header.pixelformat.gbitmask;
				rawData[offset + 1] = (byte)(((pxc >> gShift1) * gMul) >> gShift2);
				pxc = px & header.pixelformat.bbitmask;
				rawData[offset + 2] = (byte)(((pxc >> bShift1) * bMul) >> bShift2);
				rawData[offset + 3] = 0xff;
				offset += 4;
			}
			return rawData;
		}

		private byte[] DecompressRGBA(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int bpp = (int)(this.PixelFormatToBpp(pixelFormat, header.pixelformat.rgbbitcount));
			int bps = (int)(header.width * bpp * this.PixelFormatToBpc(pixelFormat));
			int sizeofplane = (int)(bps * header.height);
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

			uint valMask = (uint)((header.pixelformat.rgbbitcount == 32) ? ~0 : (1 << (int)header.pixelformat.rgbbitcount) - 1);
			// Funny x86s, make 1 << 32 == 1
			uint pixSize = (header.pixelformat.rgbbitcount + 7) / 8;
			int rShift1 = 0; int rMul = 0; int rShift2 = 0;
			ComputeMaskParams(header.pixelformat.rbitmask, ref rShift1, ref rMul, ref rShift2);
			int gShift1 = 0; int gMul = 0; int gShift2 = 0;
			ComputeMaskParams(header.pixelformat.gbitmask, ref gShift1, ref gMul, ref gShift2);
			int bShift1 = 0; int bMul = 0; int bShift2 = 0;
			ComputeMaskParams(header.pixelformat.bbitmask, ref bShift1, ref bMul, ref bShift2);
			int aShift1 = 0; int aMul = 0; int aShift2 = 0;
			ComputeMaskParams(header.pixelformat.alphabitmask, ref aShift1, ref aMul, ref aShift2);

			int offset = 0;
			int pixnum = width * height * depth;
			int temp = pos;

			while (pixnum-- > 0) {
				uint px = ToUInt32(data,temp) & valMask;
				temp += (int)pixSize;
				uint pxc = px & header.pixelformat.rbitmask;
				rawData[offset + 0] = (byte)(((pxc >> rShift1) * rMul) >> rShift2);
				pxc = px & header.pixelformat.gbitmask;
				rawData[offset + 1] = (byte)(((pxc >> gShift1) * gMul) >> gShift2);
				pxc = px & header.pixelformat.bbitmask;
				rawData[offset + 2] = (byte)(((pxc >> bShift1) * bMul) >> bShift2);
				pxc = px & header.pixelformat.alphabitmask;
				rawData[offset + 3] = (byte)(((pxc >> aShift1) * aMul) >> aShift2);
				offset += 4;
			}
			return rawData;
		}


		private byte[] DecompressLum(DDSStruct header, byte[] data, PixelFormat pixelFormat, int pos = 0) {
			// allocate bitmap
			int bpp = (int)(this.PixelFormatToBpp(pixelFormat, header.pixelformat.rgbbitcount));
			int bps = (int)(header.width * bpp * this.PixelFormatToBpc(pixelFormat));
			int sizeofplane = (int)(bps * header.height);
			int width = (int)header.width;
			int height = (int)header.height;
			int depth = (int)header.depth;

			byte[] rawData = new byte[depth * sizeofplane + height * bps + width * bpp];

			int lShift1 = 0; int lMul = 0; int lShift2 = 0;
			ComputeMaskParams(header.pixelformat.rbitmask, ref lShift1, ref lMul, ref lShift2);

			int offset = 0;
			int pixnum = width * height * depth;
			int temp = pos;
			while (pixnum-- > 0) {
				byte px = data[temp++];
				rawData[offset + 0] = (byte)(((px >> lShift1) * lMul) >> lShift2);
				rawData[offset + 1] = (byte)(((px >> lShift1) * lMul) >> lShift2);
				rawData[offset + 2] = (byte)(((px >> lShift1) * lMul) >> lShift2);
				rawData[offset + 3] = (byte)(((px >> lShift1) * lMul) >> lShift2);
				offset += 4;
			}
			return rawData;
		}

		#endregion

		#endregion

		#region Public Methods
		public void Dispose() {
			if (this.m_bitmap != null) {
				this.m_bitmap = null;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns a System.Imaging.Bitmap containing the DDS image.
		/// </summary>
		public Texture2D BitmapImage {
			get { return this.m_bitmap; }
		}

		/// <summary>
		/// Returns the DDS image is valid format.
		/// </summary>
		public bool IsValid {
			get { return this.m_isValid; }
		}
		#endregion

		#region Operators
		public static implicit operator DDSImage(Texture2D value) {
			return new DDSImage(value);
		}

		public static explicit operator Texture2D(DDSImage value) {
			return value.BitmapImage;
		}
		#endregion

		#region Nested Types

		#region Colors
		[StructLayout(LayoutKind.Sequential)]
		private struct Color8888 {
			public byte red;
			public byte green;
			public byte blue;
			public byte alpha;
		}
		private struct ColorFloat {
			public float red;
			public float green;
			public float blue;
			public float alpha;
		}
		#endregion

		#region DDSStruct
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct DDSStruct {
			public uint size;       // equals size of struct (which is part of the data file!)
			public uint flags;
			public uint height;
			public uint width;
			public uint sizeorpitch;
			public uint depth;
			public uint mipmapcount;
			public uint alphabitdepth;
			//[MarshalAs(UnmanagedType.U4, SizeConst = 11)]
			public uint[] reserved;//[11];

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
			public struct pixelformatstruct {
				public uint size;   // equals size of struct (which is part of the data file!)
				public uint flags;
				public uint fourcc;
				public uint rgbbitcount;
				public uint rbitmask;
				public uint gbitmask;
				public uint bbitmask;
				public uint alphabitmask;
			}
			public pixelformatstruct pixelformat;

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
			public struct ddscapsstruct {
				public uint caps1;
				public uint caps2;
				public uint caps3;
				public uint caps4;
			}
			public ddscapsstruct ddscaps;
			public uint texturestage;

			//#ifndef __i386__
			//void to_little_endian()
			//{
			//	size_t size = sizeof(DDSStruct);
			//	assert(size % 4 == 0);
			//	size /= 4;
			//	for (size_t i=0; i<size; i++)
			//	{
			//		((int32_t*) this)[i] = little_endian(((int32_t*) this)[i]);
			//	}
			//}
			//#endif
		}
		#endregion

		#region DDSStruct Flags
		private const int DDSD_CAPS = 0x00000001;
		private const int DDSD_HEIGHT = 0x00000002;
		private const int DDSD_WIDTH = 0x00000004;
		private const int DDSD_PITCH = 0x00000008;
		private const int DDSD_PIXELFORMAT = 0x00001000;
		private const int DDSD_MIPMAPCOUNT = 0x00020000;
		private const int DDSD_LINEARSIZE = 0x00080000;
		private const int DDSD_DEPTH = 0x00800000;
		#endregion

		#region pixelformat values
		private const int DDPF_ALPHAPIXELS = 0x00000001;
		private const int DDPF_FOURCC = 0x00000004;
		private const int DDPF_RGB = 0x00000040;
		private const int DDPF_LUMINANCE = 0x00020000;
		#endregion

		#region ddscaps
		// caps1
		private const int DDSCAPS_COMPLEX = 0x00000008;
		private const int DDSCAPS_TEXTURE = 0x00001000;
		private const int DDSCAPS_MIPMAP = 0x00400000;
		// caps2
		private const int DDSCAPS2_CUBEMAP = 0x00000200;
		private const int DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400;
		private const int DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800;
		private const int DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000;
		private const int DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000;
		private const int DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000;
		private const int DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000;
		private const int DDSCAPS2_VOLUME = 0x00200000;
		#endregion

		#region fourccs
		private const uint FOURCC_DXT1 = 0x31545844;
		private const uint FOURCC_DXT2 = 0x32545844;
		private const uint FOURCC_DXT3 = 0x33545844;
		private const uint FOURCC_DXT4 = 0x34545844;
		private const uint FOURCC_DXT5 = 0x35545844;
		private const uint FOURCC_ATI1 = 0x31495441;
		private const uint FOURCC_ATI2 = 0x32495441;
		private const uint FOURCC_RXGB = 0x42475852;
		private const uint FOURCC_DOLLARNULL = 0x24;
		private const uint FOURCC_oNULL = 0x6f;
		private const uint FOURCC_pNULL = 0x70;
		private const uint FOURCC_qNULL = 0x71;
		private const uint FOURCC_rNULL = 0x72;
		private const uint FOURCC_sNULL = 0x73;
		private const uint FOURCC_tNULL = 0x74;
		#endregion

		#region PixelFormat
		/// <summary>
		/// Various pixel formats/compressors used by the DDS image.
		/// </summary>
		private enum PixelFormat {
			/// <summary>
			/// 32-bit image, with 8-bit red, green, blue and alpha.
			/// </summary>
			RGBA,
			/// <summary>
			/// 24-bit image with 8-bit red, green, blue.
			/// </summary>
			RGB,
			/// <summary>
			/// 16-bit DXT-1 compression, 1-bit alpha.
			/// </summary>
			DXT1,
			/// <summary>
			/// DXT-2 Compression
			/// </summary>
			DXT2,
			/// <summary>
			/// DXT-3 Compression
			/// </summary>
			DXT3,
			/// <summary>
			/// DXT-4 Compression
			/// </summary>
			DXT4,
			/// <summary>
			/// DXT-5 Compression
			/// </summary>
			DXT5,
			/// <summary>
			/// 3DC Compression
			/// </summary>
			THREEDC,
			/// <summary>
			/// ATI1n Compression
			/// </summary>
			ATI1N,
			LUMINANCE,
			LUMINANCE_ALPHA,
			RXGB,
			A16B16G16R16,
			R16F,
			G16R16F,
			A16B16G16R16F,
			R32F,
			G32R32F,
			A32B32G32R32F,
			/// <summary>
			/// Unknown pixel format.
			/// </summary>
			UNKNOWN
		}
		#endregion

		#endregion
	}
	#endregion

	#region Exceptions Class
	/// <summary>
	/// Thrown when an invalid file header has been encountered.
	/// </summary>
	public class InvalidFileHeaderException : Exception {
	}

	/// <summary>
	/// Thrown when the data does not contain a DDS image.
	/// </summary>
	public class NotADDSImageException : Exception {

	}

	/// <summary>
	/// Thrown when there is an unknown compressor used in the DDS file.
	/// </summary>
	public class UnknownFileFormatException : Exception {
	}
	#endregion
}
