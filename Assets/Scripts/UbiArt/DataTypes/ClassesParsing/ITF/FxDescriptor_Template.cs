using System;
using System.Collections.Generic;

namespace UbiArt.ITF {
	public partial class FxDescriptor_Template {
		protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
			Reinit(s.Context, s.Settings);
		}

		Settings previousSettings = null;
		protected virtual void Reinit(Context c, Settings settings) {
			if (previousSettings != null) {
				if (previousSettings.Game != settings.Game) {
					if (previousSettings.EngineVersion <= EngineVersion.RO && settings.EngineVersion >= EngineVersion.RL) {
						var p = gen._params;
						var blend = p.blendMode;
						string shaderPath = blend switch {
							ParticleGeneratorParameters.GFX_BLEND.ADDALPHA => "world/common/matshader/addalpha.msh",
							ParticleGeneratorParameters.GFX_BLEND.ADD => "world/common/matshader/add.msh",
							ParticleGeneratorParameters.GFX_BLEND.MUL => "world/common/matshader/mul.msh",
							ParticleGeneratorParameters.GFX_BLEND.MUL2X => "world/common/matshader/mul2x.msh",
							_ => "world/common/matshader/default.msh",

						};
						material = new GFXMaterialSerializable() {
							textureSet = new GFXMaterialTexturePathSet() {
								diffuse = texture
							},
							shaderPath = new Path(shaderPath)
						};
						p.genGenType = (ParticleGeneratorParameters.PARGEN_GEN)(int)p.genGenType2;
						p.velNorm = (float)p.vel.Magnitude;
						p.genEmitMode = ParticleGeneratorParameters.PARGEN_EMITMODE.OVERTIME;
						//p.freqDelta = p.emitInterval;
						p.uniformscale = (float)p.uniformscale2;
					}
				}
			}
			previousSettings = settings;
		}
	}
}
