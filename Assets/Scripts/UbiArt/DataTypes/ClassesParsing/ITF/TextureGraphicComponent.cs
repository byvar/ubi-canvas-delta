using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class TextureGraphicComponent {
		public override ActorComponent Convert(Context context, Actor actor, Settings oldSettings, Settings newSettings) {
			base.Convert(context, actor, oldSettings, newSettings);
			if (newSettings.EngineVersion == EngineVersion.RL && oldSettings.EngineVersion <= EngineVersion.RO) {
				var blend = actor?.template?.obj?.GetComponent<TextureGraphicComponent_Template>()?.blendmode2;
				string shaderPath = blend switch {
					GraphicComponent_Template.GFX_BLEND2.ADDALPHA => "world/common/matshader/addalpha.msh",
					GraphicComponent_Template.GFX_BLEND2.ADD => "world/common/matshader/add.msh",
					GraphicComponent_Template.GFX_BLEND2.MUL => "world/common/matshader/mul.msh",
					GraphicComponent_Template.GFX_BLEND2.MUL2X => "world/common/matshader/mul2x.msh",
					//_ => "world/common/matshader/default.msh",
					_ => "world/common/matshader/regularbuffer/backlighted.msh"

				};
				material = new GFXMaterialSerializable() {
					textureSet = new GFXMaterialTexturePathSet() {
						diffuse = texture
					},
					shaderPath = new Path(shaderPath)
				};
			}
			return this;
		}
	}
}
