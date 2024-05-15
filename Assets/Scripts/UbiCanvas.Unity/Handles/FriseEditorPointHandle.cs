using UbiArt.ITF;
using UbiCanvas.Helpers;
using UnityEngine;

public class FriseEditorPointHandle : UnityHandle {
	private LineRenderer lr;
	public PolyLineEdge Data { get; set; }
	public UnityFrise Frise { get; set; }

	public override float HandleScale => 0.3f + Data.Scale;
	public Vector3 DataPosition => new Vector3(Data.POS?.x ?? 0, Data.POS?.y ?? 0, 0);

	public FriseEditorPointHandle target;
	public Transform RelativeTransform;
	public Transform ScaleTransform;
	public Vector3 PreviousScale { get; set; }

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

		if (target != null) {
			var relativePoint = RelativeTransform.TransformPoint(transform.localPosition);
			var relativeTarget = target.ScaleTransform.TransformPoint(target.DataPosition);

			lr.positionCount = 5;
			var lerp = Vector3.Lerp(relativePoint, relativeTarget, 0.5f);
			var perp = Vector2.Perpendicular(relativeTarget - relativePoint).normalized * 0.2f;
			lr.SetPositions(new Vector3[] {
				relativePoint,
				lerp,
				new Vector3(lerp.x + perp.x, lerp.y + perp.y, lerp.z),
				lerp,
				relativeTarget
			});
		}
	}
	void UpdateData() {
		var scale = ScaleTransform.lossyScale;
		if (scale.x != 0 && scale.y != 0 && scale.z != 0) {
			var relativePoint = RelativeTransform.InverseTransformPoint(transform.position);
			relativePoint = Vector3.Scale(relativePoint, new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z));

			const float min = 0.001f;
			var og = DataPosition;
			var diff = new UbiArt.Vec3d(relativePoint.x - og.x, relativePoint.y - og.y, relativePoint.z - og.z);
			diff.x = Mathf.Abs(diff.x);
			diff.y = Mathf.Abs(diff.y);
			diff.z = Mathf.Abs(diff.z);

			if (diff.x > min / Mathf.Abs(scale.x) || diff.y > min / Mathf.Abs(scale.y)) {
				Debug.Log($"Recomputing frise point: ({relativePoint.x}, {relativePoint.y}) - ({Data.POS})");
				Data.POS = new UbiArt.Vec2d(relativePoint.x, relativePoint.y);
				Frise.frise.Recompute();
			}
		}
	}
	public override void CreateMesh() {
		base.CreateMesh();
		if (target != null) {
			lr = gameObject.AddComponent<LineRenderer>();
			lr.material = new Material(manager.lineMaterial);
			lr.useWorldSpace = true;
			lr.widthMultiplier = 0.15f;
			lr.sortingLayerName = "Gizmo-Line";
			Color color = Data.HoleMode switch {
				PolyLineEdge.Hole.None => Color.white,
				PolyLineEdge.Hole.Visual => Color.yellow,
				PolyLineEdge.Hole.Collision => Color.blue,
				PolyLineEdge.Hole.Both => Color.black,
				_ => Color.white,
			};
			lr.material.color = color;
			lr.startColor = color;
			lr.endColor = color;
		}
	}
}
