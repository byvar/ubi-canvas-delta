namespace UbiArt.ITF {
	public partial class RO2_CheckpointComponent_Template {
		public override ActorComponent_Template Convert(Context context, Actor_Template actor, Settings oldSettings, Settings newSettings) {
			var baseResult = base.Convert(context, actor, oldSettings, newSettings);
			if (oldSettings.Game != newSettings.Game) {
				if ((newSettings.Game == Game.RL || newSettings.Game == Game.COL)) {
					return Merger.Merge<CheckpointComponent_Template>(this);
				}
			}
			return baseResult;
		}
	}
}
