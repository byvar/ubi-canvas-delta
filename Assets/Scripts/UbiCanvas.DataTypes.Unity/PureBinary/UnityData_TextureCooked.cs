using PVRImageWrapper;
using System;
using System.IO;
using UbiArt;
using UbiCanvas.Helpers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UbiCanvas {
	public class UnityData_TextureCooked : UnityData<TextureCooked> {
		private Texture2D texture;
		//private Texture2D[] subtextures;
		public Texture2D Texture {
			get {
				if (texture == null) texture = GetTexture();
				return texture;
			}
		}

		public Texture2D TextureForExport {
			get => GetTexture(forExport: true);
		}

		private Texture2D GetTexture(bool forExport = false) {
			bool MakeNoLongerReadable = false;
			if(!forExport)
				MakeNoLongerReadable = true;
			Texture2D texture = null;
			if (LinkedObject.Data != null) {
				var data = LinkedObject.Data;
				if (Context.Settings.Platform == GamePlatform.Vita) {
					//texture = new Texture2D(LinkedObject.Width, LinkedObject.Height);
					using MemoryStream ms = new MemoryStream(data);
					var gxt = new GXTConvert.FileFormat.GxtBinary(ms, makeNoLongerReadable: MakeNoLongerReadable);
					texture = gxt.Textures[0];
					//texture = TextureHelpers.CreateDummyTexture();
					return texture;
				} else if (Context.Settings.Platform == GamePlatform.iOS) {
					if (Context.Settings.EngineVersion <= EngineVersion.RO) {
						texture = CreateTextureFromPNG(data, makeNoLongerReadable: MakeNoLongerReadable);
						return texture;
					} else {
						if (LinkedObject.Header.Type == TextureCookedHeader.TextureType.BacklightEmmissive
							|| LinkedObject.Header.Type == TextureCookedHeader.TextureType.PVRTC) {
							// This is a PVR!
							var pvr = new PVRImage(data);
							texture = pvr.LoadIntoTexture();
							texture.wrapMode = TextureWrapMode.Repeat;
							texture.Apply(true, makeNoLongerReadable: MakeNoLongerReadable);
							return texture;
						}
					}
				} else if (Context.Settings.Platform == GamePlatform.Android) {
					if (Context.Settings.EngineVersion <= EngineVersion.RO) {
						texture = CreateTextureFromPNG(data, makeNoLongerReadable: MakeNoLongerReadable);
						return texture;
					}
				}
				// For jpg & png:
				/*texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
				texture.LoadImage(texData);
				texture.Apply();*/
				// Other formats:
				/*using (var img = new MagickImage(texData, MagickFormat.Dds)) {
					var pixels = img.GetPixels().GetValues();
					texture = new Texture2D(img.Width, img.Height, TextureFormat.ARGB32, false);
					texture.LoadImage(img.ToByteArray(MagickFormat.Png32));
					texture.Apply();
				}*/
				//UnityEngine.Debug.Log($"Texture - {texData.Length} - {width} - {height}");
				if (forExport) {
					// DDSImageParser is accurate to DirectXTeX
					using (DDSImage dds = new DDSImage(data, makeNoLongerReadable: MakeNoLongerReadable)) {
						texture = dds.BitmapImage;
					}
					if (texture == null) {
						texture = LoadTextureDXT(data, makeNoLongerReadable: MakeNoLongerReadable);
					}
				} else {
					texture = LoadTextureDXT(data, makeNoLongerReadable: MakeNoLongerReadable);
					if (texture == null) {
						using (DDSImage dds = new DDSImage(data, makeNoLongerReadable: MakeNoLongerReadable)) {
							texture = dds.BitmapImage;
						}
					}
				}
				if (texture != null) {
					texture.wrapModeU = GetWrapMode(LinkedObject.Header?.WrapModeU ?? TextureCookedHeader.TextureAddressingMode.Wrap);
					texture.wrapModeV = GetWrapMode(LinkedObject.Header?.WrapModeV ?? TextureCookedHeader.TextureAddressingMode.Wrap);
				}
				//texture = LoadDDS(texData);
				//if (FileSystem.mode == FileSystem.Mode.Web) LinkedObject.texData = null;
			}
			return texture;
		}

		public float HeightFactor {
			get {
				Texture2D tex = Texture;
				if (tex != null) {
					return (float)tex.width / (float)tex.height;
				} else {
					return (float)LinkedObject.Header.Width / (float)LinkedObject.Header.Height;
				}
			}
		}

		private static Texture2D CreateTextureFromPNG(byte[] data, bool makeNoLongerReadable = true) {
			var pngtex = new Texture2D(2, 2);
			pngtex.LoadImage(data);
			var texture = TextureHelpers.CreateTexture2D(pngtex.width, pngtex.height);
			var w = texture.width;
			var h = texture.height;
			for (int y = 0; y < h; y++) {
				var row = pngtex.GetPixels(0, h - 1 - y, w, 1);
				texture.SetPixels(0, y, w, 1, row);
			}
			texture.wrapMode = TextureWrapMode.Repeat;
			texture.Apply(true, makeNoLongerReadable);
			return texture;
		}

		private static Texture2D LoadTextureDXT(byte[] ddsBytes, bool makeNoLongerReadable = true) {
			GraphicsFormat format = GraphicsFormat.RGBA_DXT5_UNorm;
			switch (ddsBytes[87]) { // 84 - 87: DXT1 or DXT5 in ASCII
				case 0x31: // DXT1
					format = GraphicsFormat.RGBA_DXT1_UNorm;
					break;
				case 0x33:
					format = GraphicsFormat.RGBA_DXT3_UNorm; // Unity doesn't handle these well unfortunately
					break;
				case 0x35: // DXT5
					format = GraphicsFormat.RGBA_DXT5_UNorm;
					break;
				default:
					return null;
					/*Debug.Log("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");
					throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");*/
			}

			byte ddsSizeCheck = ddsBytes[4];
			if (ddsSizeCheck != 124)
				throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

			int height = ddsBytes[13] << 8 | ddsBytes[12];
			int width = ddsBytes[17] << 8 | ddsBytes[16];
			if(format == GraphicsFormat.RGBA_DXT1_UNorm && (width % 4 != 0 || height % 4 != 0))
				return null; // Can't load in Unity if dimensions aren't multiple of 4

			int DDS_HEADER_SIZE = 128;
			byte[] dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
			Array.Copy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

			try {
				Texture2D texture = new Texture2D(width, height, format, TextureCreationFlags.None);
				texture.LoadRawTextureData(dxtBytes);
				texture.Apply(true, makeNoLongerReadable);
				return (texture);
			} catch (Exception ex) {
				Debug.LogWarning(ex);
				return null;
			}
		}

		private static TextureWrapMode GetWrapMode(TextureCookedHeader.TextureAddressingMode m) {
			return m switch {
				TextureCookedHeader.TextureAddressingMode.Wrap => TextureWrapMode.Repeat,
				TextureCookedHeader.TextureAddressingMode.Mirror => TextureWrapMode.Mirror,
				TextureCookedHeader.TextureAddressingMode.Clamp => TextureWrapMode.Clamp,
				_ => TextureWrapMode.Repeat
			};
		}
	}
}