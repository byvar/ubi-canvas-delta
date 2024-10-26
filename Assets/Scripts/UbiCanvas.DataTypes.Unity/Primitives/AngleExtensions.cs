using UnityEngine;

namespace UbiArt {
	public static class AngleExtensions {
		public static Quaternion GetUnityQuaternion(this Angle angle, bool flip = false) => Quaternion.Euler(0, 0, flip ? -angle.EulerAngle : angle.EulerAngle);
		public static void SetUnityQuaternion(this Angle angle, Quaternion quaternion) => angle.EulerAngle = quaternion.eulerAngles.z;
	}
}
