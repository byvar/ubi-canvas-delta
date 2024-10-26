using UbiArt;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using UnityEngine;

public class UnityBezierPointHandle : UnityHandle {
	private LineRenderer lr;
	public RO2_BezierNode Node;

	public override float HandleScale => 0.3f + Node.scale;

	public int PointIndex;
	public UnityBezierRenderer UnityBezier;
	public Transform RelativeTransform;
	public Transform ScaleTransform;
	public Vector3 PreviousScale { get; set; }
	public UnityBezierPointHandle MainPoint;
	public Vector3 DataPosition => MainPoint != null
		? UnityBezier.GetTangent(PointIndex, addPosition: true, applyInverseScale: true).GetUnityVector(invertZ: true)
		: UnityBezier.GetPosition(PointIndex, applyInverseScale: true).GetUnityVector(invertZ: true);
	// Use this for initialization
	void Start() {
	}

	// Update is called once per frame
	public override void ManualUpdate() {
		if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
			UpdateLine();
			UpdateData();
		}
	}

	void UpdateLine() {
		if (PreviousScale != ScaleTransform.lossyScale) {
			PreviousScale = ScaleTransform.lossyScale;
			transform.localPosition = RelativeTransform.InverseTransformPoint(ScaleTransform.TransformPoint(DataPosition));
		}
		if (MainPoint != null) {
			lr.positionCount = 2;
			lr.SetPositions(new Vector3[] {
				transform.position,
				MainPoint.transform.position
			});
		}
	}
	void UpdateData() {
		var scale = ScaleTransform.lossyScale;
		if (scale.x != 0 && scale.y != 0 && scale.z != 0) {
			var relativePoint = RelativeTransform.TransformPoint(transform.localPosition);
			relativePoint = ScaleTransform.InverseTransformPoint(relativePoint);

			const float min = 0.001f;
			var og = DataPosition;
			var diff = new UbiArt.Vec3d(relativePoint.x - og.x, relativePoint.y - og.y, relativePoint.z - og.z);
			diff.x = Mathf.Abs(diff.x);
			diff.y = Mathf.Abs(diff.y);
			diff.z = Mathf.Abs(diff.z);

			if (MainPoint != null) {
				// This is a tangent line
				if (diff.x > min / Mathf.Abs(scale.x) || diff.y > min / Mathf.Abs(scale.y)) {
					var localTangent = relativePoint;
					UnityBezier.SetTangent(PointIndex, new Vec2d(localTangent.x, localTangent.y), addPosition: true, applyInverseScale: true);
				}
			} else {
				if (diff.x > min / Mathf.Abs(scale.x) || diff.y > min / Mathf.Abs(scale.y) || diff.z > min / Mathf.Abs(scale.z)) {
					//Node.pos = new UbiArt.Vec3d(relativePoint.x, relativePoint.y, -relativePoint.z);
					var pos = new Vec3d(relativePoint.x, relativePoint.y, -relativePoint.z);
					UnityBezier.SetPosition(PointIndex, pos, addPosition: true, applyInverseScale: true);
				}
			}
		}
	}
	public override void CreateMesh() {
		base.CreateMesh();
		if (MainPoint != null) {
			lr = gameObject.AddComponent<LineRenderer>();
			lr.material = new Material(manager.lineMaterial);
			lr.useWorldSpace = true;
			lr.widthMultiplier = 0.05f;
			lr.sortingLayerName = "Gizmo-Line";
			var color = UnityEngine.Color.green;
			lr.material.color = color;
			lr.startColor = color;
			lr.endColor = color;
		}
	}
}
