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
		var relativePoint = RelativeTransform.InverseTransformPoint(transform.position);
		if (ScaleTransform.localScale.x != 0 && ScaleTransform.localScale.y != 0 && ScaleTransform.localScale.z != 0) {
			relativePoint = Vector3.Scale(relativePoint, new Vector3(1f / ScaleTransform.localScale.x, 1f / ScaleTransform.localScale.y, 1f / ScaleTransform.localScale.z));
		}
		const float min = 0.005f;

		if (Mathf.Abs(relativePoint.x - (Data?.POS?.x ?? 0)) > min
			|| Mathf.Abs(relativePoint.y - (Data?.POS?.y ?? 0)) > min) {
			//Debug.Log($"Recompute ({relativePoint.x}, {relativePoint.y}) - ({Data.POS})");
			Data.POS = new UbiArt.Vec2d(relativePoint.x, relativePoint.y);
			Frise.frise.Recompute();
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
