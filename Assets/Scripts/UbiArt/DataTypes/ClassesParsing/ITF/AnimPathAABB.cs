using UbiArt.Animation;

namespace UbiArt.ITF {
	public partial class AnimPathAABB {
		public AnimTrack anim;

		protected override void OnPostSerialize(CSerializerObject s) {
			base.OnPostSerialize(s);
			if (path != null && IsFirstLoad) {
				Loader l = s.Context.Loader;
				if (l.LoadAnimations) {
					l.LoadFile<AnimTrack>(path, result => anim = result);
				}
				//AddToStringCache(s);
			}
		}
	}
}
