using System;
using System.Collections.Generic;
using System.Linq;
using UbiArt.ITF;

namespace UbiArt {
	public class TexturePathConversionSettings {
		public ConversionSettings Conversion { get; set; }

		public Dictionary<Path, Path> DiffuseConversion { get; set; } = new Dictionary<Path, Path>();
		public Dictionary<Path, Path> BackLightPaths { get; set; } = new Dictionary<Path, Path>();
		public Dictionary<Path, Path> ShaderPaths { get; set; } = new Dictionary<Path, Path>();

		public void Convert(TextureBankPath bank) {
			var diffusePath = bank?.textureSet?.diffuse;
			if (!Path.IsNull(diffusePath)) {
				if (!Conversion.IsPathLocked(diffusePath)) {
					diffusePath = new Path(diffusePath);
					diffusePath.ConvertPath(Conversion);
				}
				if (ShaderPaths.ContainsKey(diffusePath)) {
					bank.materialShader = ShaderPaths[diffusePath];
				}
			}
		}

		public void Convert(GFXMaterialTexturePathSet set) {
			if (!Path.IsNull(set?.diffuse)) {
				var diffusePath = set.diffuse;
				if (!Conversion.IsPathLocked(diffusePath)) {
					diffusePath = new Path(diffusePath);
					diffusePath.ConvertPath(Conversion);
				}
				if (DiffuseConversion.ContainsKey(diffusePath)) {
					set.diffuse = DiffuseConversion[diffusePath];
				}
			}
			if (Path.IsNull(set?.back_light) && !Path.IsNull(set?.diffuse)) {
				var diffusePath = set.diffuse;
				if (!Conversion.IsPathLocked(diffusePath)) {
					diffusePath = new Path(diffusePath);
					diffusePath.ConvertPath(Conversion);
				}
				if (BackLightPaths.ContainsKey(diffusePath)) {
					var backlight = BackLightPaths[diffusePath];
					set.back_light = backlight;
				}
			}
		}
	}
}
