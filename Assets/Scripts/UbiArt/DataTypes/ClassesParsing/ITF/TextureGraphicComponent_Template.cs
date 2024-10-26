using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class TextureGraphicComponent_Template {
		public override ActorComponent_Template Convert(Context context, Actor_Template actor, Settings oldSettings, Settings newSettings) {
			base.Convert(context, actor, oldSettings, newSettings);
			if (newSettings.EngineVersion == EngineVersion.RL && oldSettings.EngineVersion <= EngineVersion.RO) {
				var blend = blendmode; // Already converted by previous convert operation
				string shaderPath = blend switch {
					GraphicComponent_Template.GFX_BLEND.ADDALPHA => "world/common/matshader/addalpha.msh",
					GraphicComponent_Template.GFX_BLEND.ADD => "world/common/matshader/add.msh",
					GraphicComponent_Template.GFX_BLEND.MUL => "world/common/matshader/mul.msh",
					GraphicComponent_Template.GFX_BLEND.MUL2X => "world/common/matshader/mul2x.msh",
					//_ => "world/common/matshader/default.msh",
					_ => "world/common/matshader/regularbuffer/backlighted.msh"

				};
				if (texture?.FullPath != "[texture]") {
					material = new GFXMaterialSerializable() {
						textureSet = new GFXMaterialTexturePathSet() {
							diffuse = (texture?.FullPath != "[texture]") ? texture : null
						},
						shaderPath = new Path(shaderPath)
					};
				}
			}
			return this;
		}
	}
}
