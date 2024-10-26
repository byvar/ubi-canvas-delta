﻿using UnityEngine;
using UbiArt;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using Color = UnityEngine.Color;
using System.Linq;
using System.Collections.Generic;

public class UnityBezierRenderer : MonoBehaviour {
	public LineRenderer Renderer;
	public RO2_BezierBranch Branch;
	public Pickable PickableForSelection;
	float prevZ = -99999;

	void InitRenderer() {
		Renderer = gameObject.AddComponent<LineRenderer>();
		Renderer.material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
		Renderer.useWorldSpace = false;
		Renderer.widthMultiplier = 0.10f;
		Renderer.sortingLayerName = "Gizmo-Line";
		Color color = Color.white;
		Renderer.material.color = color;
		Renderer.startColor = color;
		Renderer.endColor = color;
	}

	void UpdateRenderer() {
		if(Renderer == null) InitRenderer();

		Vec3d currentPos = new Vec3d();
		List<Vec3d> points = new List<Vec3d>();
		for (int i = 0; i < Branch.nodes.Count - 1; i++) {
			var n = Branch.nodes[i];
			currentPos += n.pos;
			points.AddRange(GetPointsForSegment(currentPos, n, Branch.nodes[i+1]));
		}
		if (Branch.nodes.Count > 0) {
			currentPos += Branch.nodes[Branch.nodes.Count-1].pos;
			points.Add(currentPos);
		}
		Renderer.positionCount = points.Count;
		var scale = PickableForSelection.SCALE ?? Vec2d.One;
		var inverseScale = (scale.x != 0 && scale.y != 0) ? new Vec3d(1f / scale.x, 1f / scale.y, 1f) : Vec3d.One;
		Renderer.SetPositions(points.Select(s => (s * inverseScale).GetUnityVector(invertZ: true)).ToArray());
		Renderer.startWidth = scale.y * 0.10f;
		Renderer.endWidth = scale.x * 0.10f;
	}

	Vec3d[] GetPointsForSegment(Vec3d currentPos, RO2_BezierNode node1, RO2_BezierNode node2) {
		var numPoints = 16;
		var p0 = new Vec2d(0f, 0f);
		var p3 = new Vec2d(node2.pos.x, node2.pos.y);
		var p1 = p0 + node1.tangent;
		var p2 = p3 - node2.tangent;
		var z0 = 0f;
		var z1 = node2.pos.z;
		//return Enumerable.Range(0, numPoints).Select(i => CalculateCubicBezierPoint(i / (float)(numPoints - 1), p0,p1,p2,p3, z0, z1)).ToArray();
		return Enumerable.Range(0, numPoints).Select(i => currentPos + CalculateCubicBezierPoint(i / (float)numPoints, p0, p1, p2, p3, z0, z1)).ToArray();
	}
	Vec3d CalculateCubicBezierPoint(float t, Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3, float z0, float z1) {
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vec2d p = p0 * uuu;
		p += p1 * 3f * uu * t;
		p += p2 * 3f * u * tt;
		p += p3 * ttt;

		return new Vec3d(p.x, p.y, Mathf.Lerp(z0, z1, t));
	}

	void UpdateSelectionDisplay() {

	}

	public IEnumerable<Vec3d> GetPositions(bool applyInverseScale = false) {
		var scale = applyInverseScale ? (PickableForSelection.SCALE ?? Vec2d.One) : Vec2d.One;
		var inverseScale = (applyInverseScale && scale.x != 0 && scale.y != 0) ? new Vec3d(1f / scale.x, 1f / scale.y, 1f) : Vec3d.One;

		Vec3d currentPos = new Vec3d();
		for (int i = 0; i < Branch.nodes.Count; i++) {
			var n = Branch.nodes[i];
			currentPos += n.pos;
			yield return currentPos * inverseScale;
			yield return (currentPos + new Vec3d(n.tangent?.x ?? 0f, n.tangent?.y ?? 0f, 0f)) * inverseScale;
		}
	}
	public Vec3d GetPosition(int index, bool applyInverseScale = false) {
		var scale = applyInverseScale ? (PickableForSelection.SCALE ?? Vec2d.One) : Vec2d.One;
		var inverseScale = (applyInverseScale && scale.x != 0 && scale.y != 0) ? new Vec3d(1f / scale.x, 1f / scale.y, 1f) : Vec3d.One;

		Vec3d currentPos = new Vec3d();
		for (int i = 0; i <= index; i++) {
			var n = Branch.nodes[i];
			currentPos += n.pos;
		}
		return currentPos * inverseScale;
	}
	public Vec3d GetTangent(int index, bool addPosition = false, bool applyInverseScale = false) {
		var scale = applyInverseScale ? (PickableForSelection.SCALE ?? Vec2d.One) : Vec2d.One;
		var inverseScale = (applyInverseScale && scale.x != 0 && scale.y != 0) ? new Vec3d(1f / scale.x, 1f / scale.y, 1f) : Vec3d.One;


		var n = Branch.nodes[index];
		var tangent = new Vec3d(n.tangent?.x ?? 0f, n.tangent?.y ?? 0f, 0f);
		if (addPosition) {
			return GetPosition(index, applyInverseScale: applyInverseScale) + tangent * inverseScale;
		} else {
			return tangent * inverseScale;
		}
	}
	public void SetTangent(int index, Vec2d vec, bool addPosition = false, bool applyInverseScale = false) {
		var scale = applyInverseScale ? (PickableForSelection.SCALE ?? Vec2d.One) : Vec2d.One;
		var inverseScale = (applyInverseScale && scale.x != 0 && scale.y != 0) ? new Vec3d(1f / scale.x, 1f / scale.y, 1f) : Vec3d.One;

		var pos = addPosition ? GetPosition(index, applyInverseScale: applyInverseScale) : Vec3d.Zero;
		var tang = vec - new Vec2d(pos.x, pos.y);
		var n = Branch.nodes[index];
		n.tangent = tang * scale;
	}
	public void SetPosition(int index, Vec3d vec, bool addPosition = false, bool applyInverseScale = false) {
		var scale = applyInverseScale ? (PickableForSelection.SCALE ?? Vec2d.One) : Vec2d.One;
		var inverseScale = (applyInverseScale && scale.x != 0 && scale.y != 0) ? new Vec3d(1f / scale.x, 1f / scale.y, 1f) : Vec3d.One;

		var pos = addPosition ? GetPosition(index, applyInverseScale: applyInverseScale) : Vec3d.Zero;
		var posDiff = vec - pos;
		var n = Branch.nodes[index];
		n.pos = n.pos + (posDiff * new Vec3d(scale.x, scale.y, 1f));
	}


	private void Update() {
		if (GlobalLoadState.LoadState != GlobalLoadState.State.Finished) return;
		bool isSelected = PickableForSelection != null && Controller.Obj.SelectedObject?.pickable == PickableForSelection;
		if (Controller.Obj.displayBezier || isSelected) {
			UpdateRenderer();
			if (Renderer != null) {
				float z = transform.position.z;
				if (z != prevZ) {
					prevZ = z;
					ZListManager zman = Controller.Obj.zListManager;
					zman.zDict[Renderer] = z;
				}
			}
			if (isSelected) {
				UpdateSelectionDisplay();
			}
		}
		if (Renderer != null) {
			Renderer.enabled = Controller.Obj.displayBezier || isSelected;
		}
	}
}
