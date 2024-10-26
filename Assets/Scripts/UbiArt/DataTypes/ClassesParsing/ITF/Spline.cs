namespace UbiArt.ITF {
	public partial class Spline  {
		public partial class SplinePoint {
			protected override void OnPreSerialize(CSerializerObject s) {
				base.OnPreSerialize(s);
				Reinit(s.Context, s.Settings);
			}

			Settings previousSettings = null;
			protected virtual void Reinit(Context c, Settings settings) {
				if (previousSettings != null) {
					if (previousSettings.Game != settings.Game) {
						if (previousSettings.EngineVersion <= EngineVersion.RO && settings.EngineVersion >= EngineVersion.RL) {
							Interpolation = (interp)(int)Interpolation_RO;
						}
					}
				}
				previousSettings = settings;
			}
		}
	}
}

