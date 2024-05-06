using System;
using System.Collections.Generic;

namespace UbiArt.ITF {
	public partial class FXControllerComponent_Template {
		public override ActorComponent_Template Convert(Context context, Actor_Template actor, Settings oldSettings, Settings newSettings) {
			base.Convert(context, actor, oldSettings, newSettings);
			if (oldSettings.Game != newSettings.Game) {
				/*feedbackTags = null;
				fxControlList = null;
				triggerFx = null;
				defaultFx = null;
				FXSwitch = null;*/
				if (oldSettings.EngineVersion <= EngineVersion.RO && newSettings.EngineVersion >= EngineVersion.RL) {
					feedbackTags = new CListO<StringID>(new List<StringID>() { new StringID(0x112D8806) }); // Needs at least one feedback tag
				}
			}
			return this;
		}
	}
}
