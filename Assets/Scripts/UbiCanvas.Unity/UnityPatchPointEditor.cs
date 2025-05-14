using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Animation;
using UbiCanvas.Helpers;
using UnityEngine;

public class UnityPatchPointEditor : MonoBehaviour {
	public UnityPatchEditor patch;

	public int pointIndex = -1;
	public List<int> vertices = new List<int>();
	public AnimPatchPoint Point => pointIndex > -1 ?
		patch.Template.patchPoints[pointIndex] : null;
	public Vec2d GlobalNormal {
		get {
			var globalAngle = (bone?.globalAngle ?? 0f);
			return Point.local.normal.Rotate(globalAngle);
		}
		set {
			var globalAngle = (bone?.globalAngle ?? 0f);
			Point.local.normal = value.Rotate(-globalAngle);
		}
	}
	public Vec2d GlobalPosition {
		get {
			var boneAngle = (bone?.globalAngle ?? 0f);
			var bonePos = (bone?.globalPosition.GetUbiArtVector() ?? Vec3d.Zero);
			var boneScale = (bone?.globalScale.GetUbiArtVector() ?? Vec2d.One) / (bone?.bindScale.GetUbiArtVector() ?? Vec2d.One);
			var boneLength = 0f; //(bone?.boneLength ?? 0f);
			
			var scaled = (Point.local.pos + new Vec2d(boneLength, 0)) * boneScale;
			var rotated = scaled.Rotate(boneAngle);
			var translated = rotated + new Vec2d(bonePos.x, bonePos.y);
			return translated;
		}
		set {
			var boneAngle = (bone?.globalAngle ?? 0f);
			var bonePos = (bone?.globalPosition.GetUbiArtVector() ?? Vec3d.Zero);
			var boneScale = (bone?.globalScale.GetUbiArtVector() ?? Vec2d.One) / (bone?.bindScale.GetUbiArtVector() ?? Vec2d.One);
			var boneLength = 0f; //(bone?.boneLength ?? 0f);

			var translated = value;
			var rotated = translated - new Vec2d(bonePos.x, bonePos.y);
			var scaled = rotated.Rotate(-boneAngle);
			var local = (scaled / boneScale) - new Vec2d(boneLength, 0);
			Point.local.pos = local;
		}
	}
	public List<Transformation> Transformations = new List<Transformation>();
	public Transformation AverageTransformation { get; set; }

	public UnityBone bone;
	private UnityBone _bone;

	private void Start() {
	}

	private void Update() {
		if (AverageTransformation != null) {
			CheckUpdateBone();
			CheckUpdateLocalPositionFromTransform();
			CheckUpdateLocalNormal();
		}
	}

	public void CheckUpdateLocalPositionFromTransform() {
		var oldPos = GlobalPosition;
		var newPos = transform.localPosition;
		Vector3 oldPosConv = new Vector3(oldPos.x, oldPos.y, 0f);
		if (newPos != oldPosConv) {
			UpdatePosition(newPos);
		}
	}
	public void UpdatePosition(Vector3 newPos) {
		// Calculate new UV from position
		var uvScaleInverse = patch.UVScaleInverse.GetUnityVector();
		var newUV = Vector2.Scale(AverageTransformation.ApplyInverse(newPos), uvScaleInverse);

		UpdateMesh(newPos, newUV);

		// Update data
		GlobalPosition = new Vec2d(newPos.x, newPos.y);
		Point.uv = newUV.GetUbiArtVector();
	}
	public void UpdatePositionFromUV() {
		if(AverageTransformation == null) return;

		var uv = Point.uv;
		var pos = AverageTransformation.Apply((uv * patch.UVScale).GetUnityVector());
		var unityPos = new Vector3(pos.x, pos.y, 0f);
		if (transform.localPosition != unityPos) {
			transform.localPosition = unityPos;
			UpdateMesh(unityPos, uv.GetUnityVector());
		}
	}
	public void UpdateMesh(Vector3 newPos, Vector2 newUV) {
		var vs = patch.bakedMesh.vertices;
		var uv = patch.bakedMesh.uv;
		foreach (var v in vertices) {
			vs[v] = newPos;
			uv[v] = newUV;
		}
		patch.bakedMesh.vertices = vs;
		patch.bakedMesh.uv = uv;
	}
	public void CheckUpdateLocalNormal() {
		var uvNormal = Point.normal * patch.UVScale;
		var boneNormal = GlobalNormal;
		var rot = AverageTransformation.Rotation;
		var transformedUVNormal = uvNormal.Rotate(rot).Normalize();
		var oldAngle = boneNormal.Angle();
		var newAngle = transformedUVNormal.Angle();
		if (Mathf.Abs(oldAngle - newAngle) > 0.005f) {
			//Debug.Log($"Updating {transform.parent.name}.{gameObject.name} normal: {boneNormal} - {transformedUVNormal}");
			GlobalNormal = transformedUVNormal;
		}
	}
	public void InitBone(UnityBone bone) {
		this.bone = bone;
		this._bone = bone;
	}
	public void CheckUpdateBone() {
		if (bone == null && _bone != null) {
			bone = _bone;
		} else if (bone != _bone ) {
			var boneIndex = patch.bones.FindItemIndex(b => b == bone);
			if (boneIndex != -1) {
				_bone = bone;
				var pbkBone = patch.Template.bones[boneIndex];
				Point.local.boneId = new Link(pbkBone.key.stringID);
			} else {
				bone = _bone;
			}
		}
	}
	public class Transformation {
		public Vector2 Translation { get; set; }
		public float Rotation { get; set; }
		public float Scale { get; set; }

		// UV -> Global pos
		public Vector2 Apply(Vector2 pos) {
			var rotated = pos.GetUbiArtVector().Rotate(Rotation);
			var scaled = rotated * Scale;
			var translated = scaled + Translation.GetUbiArtVector();
			return translated.GetUnityVector();
		}
		// Global pos -> UV
		public Vector2 ApplyInverse(Vector2 pos) {
			var scaled = (pos - Translation).GetUbiArtVector();
			var rotated = scaled / Scale;
			var untransformed = rotated.Rotate(-Rotation);
			return untransformed.GetUnityVector();
		}
	}
}
