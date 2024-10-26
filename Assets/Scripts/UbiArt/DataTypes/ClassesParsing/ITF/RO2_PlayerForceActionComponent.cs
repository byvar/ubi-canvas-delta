namespace UbiArt.ITF {
	public partial class RO2_PlayerForceActionComponent {
		/*protected override void OnPreSerialize(CSerializerObject s) {
			base.OnPreSerialize(s);
			Reinit(s.Context, s.Settings);
		}

		Settings previousSettings = null;
		protected virtual void Reinit(Context c, Settings settings) {
			if (previousSettings != null) {
				if (previousSettings.Game != settings.Game) {
					if (settings.Game == Game.RL && previousSettings.Game != Game.RL) {
						if (Action != PlayerForcedAction.Win) {
							Action2 = (PlayerForcedAction2)(int)Action;
						}
					}
				}
			}
			previousSettings = settings;
		}*/
	}
}
