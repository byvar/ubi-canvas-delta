using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UbiCanvas.Helpers {
	public static class VectorHelpers {

		public static void GetTransformation(Vector2[] src, Vector2[] dst, out Vector2 translation, out float rotation, out float scale) {
			// Step 1: Compute centroids
			Vector2 srcCentroid = ComputeCentroid(src);
			Vector2 dstCentroid = ComputeCentroid(dst);

			// Step 2: Center the points
			Vector2[] srcCentered = CenterPoints(src, srcCentroid);
			Vector2[] dstCentered = CenterPoints(dst, dstCentroid);

			// Step 3: Compute scale
			double srcNorm = ComputeFrobeniusNorm(srcCentered);
			double dstNorm = ComputeFrobeniusNorm(dstCentered);
			scale = (float)(dstNorm / srcNorm);

			// Step 4: Compute rotation
			double sumCos = 0.0;
			double sumSin = 0.0;
			for (int i = 0; i < src.Length; i++) {
				Vector2 s = srcCentered[i].normalized;
				Vector2 d = dstCentered[i].normalized;

				float dot = Vector2.Dot(s, d);
				double cross = s.x * d.y - s.y * d.x;

				sumCos += dot;
				sumSin += cross;
			}

			double angleRad = Math.Atan2(sumSin, sumCos);
			rotation = (float)angleRad;

			// Step 5: Compute translation
			double cos = Math.Cos(angleRad);
			double sin = Math.Sin(angleRad);
			Vector2 rotated = new Vector2(
				(float)(cos * srcCentroid.x - sin * srcCentroid.y),
				(float)(sin * srcCentroid.x + cos * srcCentroid.y)
			);
			translation = dstCentroid - scale * rotated;


			Vector2 ComputeCentroid(Vector2[] points) {
				Vector2 sum = Vector2.zero;
				foreach (var p in points)
					sum += p;
				return sum / points.Length;
			}

			Vector2[] CenterPoints(Vector2[] points, Vector2 centroid) {
				Vector2[] centered = new Vector2[points.Length];
				for (int i = 0; i < points.Length; i++)
					centered[i] = points[i] - centroid;
				return centered;
			}

			double ComputeFrobeniusNorm(Vector2[] points) {
				double sum = 0.0;
				foreach (var p in points)
					sum += p.sqrMagnitude;
				return Math.Sqrt(sum);
			}
		}
	}
}
