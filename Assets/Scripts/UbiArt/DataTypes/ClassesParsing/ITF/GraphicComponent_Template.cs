using System;
using System.Collections.Generic;

namespace UbiArt.ITF {
	public partial class GraphicComponent_Template {
		public override ActorComponent_Template Convert(Context context, Actor_Template actor, Settings oldSettings, Settings newSettings) {
			base.Convert(context, actor, oldSettings, newSettings);
			if (newSettings.Game == Game.RL) {
				var firstComponent = actor.GetComponent<GraphicComponent_Template>();
				if (firstComponent != null && this != firstComponent) {
					//return null;
				}
				if (oldSettings.Game == Game.RA || oldSettings.Game == Game.RM) {
					materialtype3 = (GFX_MAT3)(int)materialtype;
				}
			}
			if (newSettings.EngineVersion >= EngineVersion.RL && oldSettings.EngineVersion <= EngineVersion.RO) {
				blendmode = (GFX_BLEND)(int)blendmode2;
				materialtype3 = (GFX_MAT3)(int)materialtype2;
			}
			return this;
		}
	}
}
