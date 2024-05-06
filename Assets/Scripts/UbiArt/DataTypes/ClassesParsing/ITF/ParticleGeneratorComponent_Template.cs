using System;
using System.Collections.Generic;
using System.Linq;

namespace UbiArt.ITF {
	public partial class ParticleGeneratorComponent_Template {
		public override ActorComponent_Template Convert(Context context, Actor_Template actor, Settings oldSettings, Settings newSettings) {
			base.Convert(context, actor, oldSettings, newSettings);
			if (newSettings.EngineVersion == EngineVersion.RL && oldSettings.EngineVersion <= EngineVersion.RO) {
				var p = ParticleGeneratorParams._params;
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
				p.freqDelta = p.emitInterval;
				p.uniformscale = (float)p.uniformscale2;
			}
			return this;
		}
	}
}
