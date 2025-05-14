using UnityEngine;

namespace UbiArt {
	public static class AngleExtensions {
		public static Quaternion GetUnityQuaternion(this Angle angle, bool flip = false) => Quaternion.Euler(0, 0, flip ? -angle.EulerAngle : angle.EulerAngle);
		public static Angle SetUnityQuaternion(this Angle angle, Quaternion quaternion, float? previous = null) {
			angle.EulerAngle = quaternion.eulerAngles.z;
			if (previous.HasValue) {
				float numWraps = Mathf.Round((previous.Value - angle.angle) / (2 * Mathf.PI));
				angle.angle += numWraps * 2 * Mathf.PI;
			}
			return angle;
		}
	}
}
