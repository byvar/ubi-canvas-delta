using System;
using System.Linq;

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

		public Vec3d GetPoint(float time) {
			if (Points == null || Points.Count == 0) {
				return Vec3d.Zero;
			}
			var p0 = Points.LastOrDefault(p => p.Time <= time);
			var p1 = Points.FirstOrDefault(p => p.Time > time);

			if(p0 == null) return p1.Point;
			if(p1 == null) return p0.Point;
			
			var lerp = (time - p0.Time) / (p1.Time - p0.Time);
			float Lerp(float f1, float f2, float amount) => f1 + (f2 - f1) * amount;
			Vec3d LerpVec(Vec3d v1, Vec3d v2, float amount) =>
				new Vec3d(
					Lerp(v1.x, v2.x, amount),
					Lerp(v1.y, v2.y, amount),
					Lerp(v1.z, v2.z, amount));
			return LerpVec(p0.Point ?? Vec3d.Zero, p1.Point ?? Vec3d.Zero, lerp);
		}
	}
}

