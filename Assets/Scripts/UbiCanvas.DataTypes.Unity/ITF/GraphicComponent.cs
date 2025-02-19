using System.Collections.Generic;
using System.Linq;
using UbiArt.Animation;
using UbiCanvas;
using UnityEngine;
using UnityEngine.Rendering;

namespace UbiArt.ITF {
	public partial class GraphicComponent {
		protected MaterialPropertyBlock mpb;
		public void FillMaterialParams(Renderer r, int index = 0, float alpha = 1f) {
			if (mpb == null) mpb = new MaterialPropertyBlock();
			r.GetPropertyBlock(mpb, index);

			GFXPrimitiveParam param = null;
			if (UbiArtContext.Settings.EngineVersion > EngineVersion.RO)
				param = PrimitiveParameters;

			if (param != null) {
				param?.FillMaterialParams(UbiArtContext, mpb, alpha: alpha);
			} else {
				GFXPrimitiveParam.FillMaterialParamsDefault(UbiArtContext, mpb, alpha: alpha);
			}

			r.SetPropertyBlock(mpb, index);
		}

		public void SetMaterialTextures(GFXMaterialTexturePathSet textureSet, Renderer r, int index = 0) {
			if (mpb == null) mpb = new MaterialPropertyBlock();
			r.GetPropertyBlock(mpb, index);
			if (textureSet != null) {
				mpb.SetVector("_UseTextures", new Vector4(
					textureSet.tex_diffuse != null ? 1f : 0f,
					textureSet.tex_back_light != null ? 1f : 0f,
					0f,
					textureSet.tex_separateAlpha != null ? 1f : 0f));

				if (textureSet.tex_diffuse != null) {
					var t = textureSet.tex_diffuse.GetUnityTexture(UbiArtContext);
					mpb.SetTexture("_Diffuse", t.Texture);
					mpb.SetVector("_Diffuse_ST", new Vector4(1, t.HeightFactor, 0, 0));
				}
				if (textureSet.tex_back_light != null) {
					var t = textureSet.tex_back_light.GetUnityTexture(UbiArtContext);
					mpb.SetTexture("_BackLight", t.Texture);
					mpb.SetVector("_BackLight_ST", new Vector4(1, t.HeightFactor, 0, 0));
				}
				if (textureSet.tex_separateAlpha != null) {
					var t = textureSet.tex_separateAlpha.GetUnityTexture(UbiArtContext);
					mpb.SetTexture("_SeparateAlpha", t.Texture);
					mpb.SetVector("_SeparateAlpha_ST", new Vector4(1, t.HeightFactor, 0, 0));
				}
			}
			r.SetPropertyBlock(mpb, index);
		}
		public void SetMaterialTextureOrigins(TextureCooked tex, Renderer r, int index = 0) {
			if (r == null) return;
			if (mpb == null) mpb = new MaterialPropertyBlock();
			r.GetPropertyBlock(mpb, index);
			if (r != null && tex != null) {
				var t = tex.GetUnityTexture(UbiArtContext);
				mpb.SetVector("_UseTextures", new Vector4(1, 0, 0, 0));
				mpb.SetTexture("_Diffuse", t.Texture);
				mpb.SetVector("_Diffuse_ST", new Vector4(1, t.HeightFactor, 0, 0));
			}
			r.SetPropertyBlock(mpb, index);
		}
	}
}
