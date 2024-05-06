using System;
using System.Collections.Generic;

namespace UbiArt.ITF {
	public partial class FXControl {
		protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
			Reinit(s.Context, s.Settings);
		}

		Settings previousSettings = null;
		protected virtual void Reinit(Context c, Settings settings) {
			if (previousSettings != null) {
				if (previousSettings.Game != settings.Game) {
					if (previousSettings.EngineVersion <= EngineVersion.RO && settings.EngineVersion >= EngineVersion.RL) {
						if(!sound.IsNull) sounds.Add(sound);
						if(!particle.IsNull) particles.Add(particle);
					}
					//sounds = null;
					//music = null;
				}
			}
			previousSettings = settings;
		}
	}
}
