﻿using ImageMagick;
using UnityEngine;

namespace UbiCanvas.Helpers 
{
	public static class TextureHelpers 
	{
		/// <summary>
		/// Creates a new <see cref="Texture2D"/>
		/// </summary>
		/// <param name="width">The texture width</param>
		/// <param name="height">The texture height</param>
		/// <param name="clear">Indicates if the image should start as fully transparent</param>
		/// <param name="applyClear">Indicates if the clear transparent pixels should be applied</param>
		/// <returns>The texture</returns>
		public static Texture2D CreateTexture2D(int width, int height, bool clear = false, bool applyClear = false) {
			var tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};

			if (clear) {
				tex.SetPixels(new Color[width * height]);

				if (applyClear)
					tex.Apply();
			}

			return tex;
		}

		public static Texture2D Crop(this Texture2D tex, RectInt rect, bool destroyTex, bool flipY = true) {
			var newTex = CreateTexture2D(rect.width, rect.height);

			if (flipY)
				rect.y = tex.height - rect.height - rect.y;

			newTex.SetPixels(tex.GetPixels(rect.x, rect.y, rect.width, rect.height));

			newTex.Apply();

			if (destroyTex)
				Object.DestroyImmediate(tex);

			return newTex;
		}

		public static void ResizeImageData(this Texture2D texture2D, int targetX, int targetY, bool mipmap = true, FilterMode filter = FilterMode.Bilinear) {
			//create a temporary RenderTexture with the target size
			RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

			//set the active RenderTexture to the temporary texture so we can read from it
			RenderTexture.active = rt;

			//Copy the texture data on the GPU - this is where the magic happens [(;]
			Graphics.Blit(texture2D, rt);
			//resize the texture to the target values (this sets the pixel data as undefined)
			texture2D.Reinitialize(targetX, targetY, texture2D.format, mipmap);
			texture2D.filterMode = filter;

			try {
				//reads the pixel values from the temporary RenderTexture onto the resized texture
				texture2D.ReadPixels(new Rect(0.0f, 0.0f, targetX, targetY), 0, 0);
				//actually upload the changed pixels to the graphics card
				texture2D.Apply();
			} catch {
				Debug.LogError("Read/Write is not enabled on texture " + texture2D.name);
			}


			RenderTexture.ReleaseTemporary(rt);
		}

		public static Texture2D Decompress(this Texture2D source)
		{
			RenderTexture renderTex = RenderTexture.GetTemporary(
				source.width,
				source.height,
				0,
				RenderTextureFormat.Default,
				RenderTextureReadWrite.Linear);

			Graphics.Blit(source, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D readableText = new Texture2D(source.width, source.height);
			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			return readableText;
		}

		public static void Export(this Texture2D texture2D, string filePath, bool includesExt = false)
		{
			if (!includesExt)
				filePath = $"{filePath}.png";

			Util.ByteArrayToFile(filePath, texture2D.EncodeToPNG());
		}

		public static Texture2D Copy(this Texture2D texture2D,
			bool removeTransparency = false, bool alphaChannelOnly = false, bool flipY = false) {
			if (texture2D.format == TextureFormat.RGBA32) {
				var rawData = texture2D.GetRawTextureData();
				Texture2D newTex = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGBA32, false);
				var pixelCount = texture2D.width * texture2D.height;
				if (alphaChannelOnly) {
					for (int i = 0; i < pixelCount; i++) {
						var a = rawData[i * 4 + 3];
						rawData[i * 4 + 0] = a;
						rawData[i * 4 + 1] = a;
						rawData[i * 4 + 2] = a;
						rawData[i * 4 + 3] = 255;
					}
				} else {
					if (removeTransparency) {
						for (int i = 0; i < pixelCount; i++) {
							rawData[i * 4 + 3] = 255;
						}
					}
				}
				if (flipY) {
					var width = texture2D.width;
					var height = texture2D.height;
					var rd = new byte[rawData.Length];
					for (var x = 0; x < width; x++) {
						for (var y = 0; y < height; y++) {
							var i = x + y * width;
							var ogi = x + (height - y - 1) * width;
							rd[i * 4 + 0] = rawData[ogi * 4 + 0];
							rd[i * 4 + 1] = rawData[ogi * 4 + 1];
							rd[i * 4 + 2] = rawData[ogi * 4 + 2];
							rd[i * 4 + 3] = rawData[ogi * 4 + 3];
						}
					}
					rawData = rd;
				}

				newTex.LoadRawTextureData(rawData);
				newTex.Apply();
				return newTex;
			} else {
				var pixels = texture2D.GetPixels();
				Texture2D newTex = new Texture2D(texture2D.width, texture2D.height);
				if (alphaChannelOnly) {
					for (int i = 0; i < pixels.Length; i++) {
						pixels[i] = new Color(pixels[i].a, pixels[i].a, pixels[i].a, 1f);
					}
				} else {
					if (removeTransparency) {
						for (int i = 0; i < pixels.Length; i++) {
							pixels[i] = new Color(pixels[i].r, pixels[i].g, pixels[i].b, 1f);
						}
					}
				}

				Color[] newPixels = pixels;

				if (flipY) {
					var width = texture2D.width;
					var height = texture2D.height;
					newPixels = new Color[pixels.Length];
					for (var x = 0; x < width; x++) {
						for (var y = 0; y < height; y++) {
							newPixels[x + y * width] = pixels[x + (height - y - 1) * width];
						}
					}
				}

				newTex.SetPixels(newPixels);
				newTex.Apply();
				return newTex;
			}
		}

		public static byte[] EncodeToPNGRaw(this Texture2D texture2D) {
			if (texture2D.format == TextureFormat.RGBA32) {
				var rawData = texture2D.GetRawTextureData();

				// Create a MagickImage from raw pixel data
				var settings = new MagickReadSettings {
					Width = texture2D.width,
					Height = texture2D.height,
					Depth = 8,
					ColorType = ColorType.TrueColorAlpha,
					Format = MagickFormat.Rgba,
					ColorSpace = ImageMagick.ColorSpace.RGB
				};

				// Magick.NET expects pixels in row-major order
				using (var image = new MagickImage(rawData, settings)) {
					return image.ToByteArray(MagickFormat.Png);
				}
			} else
				return texture2D.EncodeToPNG();
		}

		public static bool IsTransparent(this Texture2D texture2D) {
			Color[] cols = texture2D.GetPixels();
			foreach (Color col in cols) {
				if (col.a != 1f) {
					return true;
				}
			}
			return false;
		}

		public static Texture2D CreateDummyTexture() {
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f));
			texture.Apply();
			return texture;
		}

		public static Texture2D WhiteTexture() {
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, Color.white);
			tex.Apply();
			return tex;
		}
		public static Texture2D GrayTexture() {
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, Color.gray);
			tex.Apply();
			return tex;
		}

		public static Texture2D CreateDummyCheckerTexture() {
			Texture2D texture = new Texture2D(2, 2);
			Color col1 = Color.white;
			Color col2 = new Color(0.9f, 0.9f, 0.9f, 1f); // very light grey
			texture.SetPixel(0, 0, col1);
			texture.SetPixel(1, 1, col1);
			texture.SetPixel(0, 1, col2);
			texture.SetPixel(1, 0, col2);
			texture.filterMode = FilterMode.Point;
			texture.Apply();
			return texture;
		}

		public static Texture2D CreateDummyLineTexture() {
			Texture2D texture = new Texture2D(2, 2);
			Color col1 = Color.white;
			Color col2 = new Color(0.9f, 0.9f, 0.9f, 1f); // very light grey
			texture.SetPixel(0, 0, col1);
			texture.SetPixel(1, 1, col2);
			texture.SetPixel(0, 1, col1);
			texture.SetPixel(1, 0, col2);
			texture.filterMode = FilterMode.Point;
			texture.Apply();
			return texture;
		}
	}
}