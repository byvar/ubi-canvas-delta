using UnityEngine;

namespace UbiArt.ITF {
	public static class FriseTextureConfigExtensions {
		private static MaterialPropertyBlock mpb;

		public static void FillUnityMaterialPropertyBlock(this FriseTextureConfig ftg, Context c, Renderer r, int index = 0, GFXMaterialShader_Template shader = null) {
			ftg.material.FillUnityMaterialPropertyBlock(c, r, index: index, shader: shader);
			if (mpb == null) mpb = new MaterialPropertyBlock();
			r.GetPropertyBlock(mpb, index);
			mpb.SetVector("_TextureScrollParams", new Vector4(
				ftg.scrollUV?.x ?? 0f,
				ftg.scrollUV?.y ?? 0f,
				ftg.scrollAngle,
				ftg.useUV2 ? 1 : 0));
			if (ftg.useUV2) {
				mpb.SetVector("_TextureScrollParams2", new Vector4(
					ftg.scrollUV2?.x ?? 0f,
					ftg.scrollUV2?.y ?? 0f,
					ftg.scaleUV2?.x ?? 0f,
					ftg.scaleUV2?.y ?? 0f));
			}

			// Set property block
			r.SetPropertyBlock(mpb, index);
		}
	}
}
