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
	public AnimPatchPoint Point => pointIndex > -1 ?
		patch.Template.patchPoints[pointIndex] : null;
	public bool IsOrigins => Controller.MainContext.Settings.EngineVersion <= EngineVersion.RO;

	public (Vec2d, Vec2d, Vec2d, float) GetLocalToNormalStuff() {
		var isOrigins = IsOrigins;
		var isFlipped = (bone.globalScalePreLength.x * bone.globalScalePreLength.y) < 0;
		var dir = bone.XAxe;
		float boneDynLocalConvert = 1f;
		Vec2d dirNormalized = isOrigins ? dir : (bone.XAxe / bone.XAxeLength);
		Vec2d perpendicular = dirNormalized.Rotate90 * (isFlipped ? -1f : 1f);
		float ratio;

		if (isOrigins) {
			ratio = Math.Abs(patch.Template.SizeMultiplier * bone.globalScale.y / bone.globalScale.x);
		} else {
			ratio = Math.Abs(boneDynLocalConvert * bone.globalScale.y * (bone.XAxeLength / bone.globalScale.x));
		}
		return (dir, dirNormalized, perpendicular, ratio);
	}

	public Vec2d GlobalNormal {
		get {
			(Vec2d dir, Vec2d dirNormalized, Vec2d perpendicular, float ratio) = GetLocalToNormalStuff();
			var normalX = dirNormalized * Point.local.normal.x;
			var normalY = perpendicular * Point.local.normal.y;
			return (normalX + normalY).Normalize();
		}
		set {
			(Vec2d dir, Vec2d dirNormalized, Vec2d perpendicular, float ratio) = GetLocalToNormalStuff();

			float localNX = Vec2d.Dot(value, dirNormalized);
			float localNY = Vec2d.Dot(value, perpendicular);
			Point.local.normal = new Vec2d(localNX, localNY);
		}
	}
	public Vec2d GlobalPosition {
		get {
			(Vec2d dir, Vec2d dirNormalized, Vec2d perpendicular, float ratio) = GetLocalToNormalStuff();
			var vX = dir * Point.local.pos.x;
			var vY = perpendicular * ratio * Point.local.pos.y;
			return new Vec2d(bone.globalPosition.x, bone.globalPosition.y) + vX + vY;
		}
		set {
			(Vec2d dir, Vec2d dirNormalized, Vec2d perpendicular, float ratio) = GetLocalToNormalStuff();
			Vec2d toLocal = value - new Vec2d(bone.globalPosition.x, bone.globalPosition.y);
			float localX = Vec2d.Dot(toLocal, dir) / dir.SquareNorm;
			float localY = Vec2d.Dot(toLocal, perpendicular) / (ratio * perpendicular.SquareNorm);
			Point.local.pos = new Vec2d(localX, localY);
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
			patch.UpdateMesh();
		}
	}
	public void UpdatePosition(Vector3 newPos) {
		// Calculate new UV from position
		var uvScaleInverse = patch.UVScaleInverse.GetUnityVector();
		var newUV = Vector2.Scale(AverageTransformation.ApplyInverse(newPos), uvScaleInverse);

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
			//Debug.Log($"Updating {transform.parent.name}.{gameObject.name} position");

			transform.localPosition = unityPos;
			patch.SetChanged();
		}
	}
	public void UpdateNormalFromUV() {
		if (AverageTransformation == null) return;
		CheckUpdateLocalNormal();
	}
	public void CheckUpdateLocalNormal() {
		var uvNormal = Point.normal * patch.UVScale;
		var boneNormal = GlobalNormal;
		var rot = AverageTransformation.Rotation;
		var transformedUVNormal = uvNormal.Rotate(rot).NormalizeDouble();
		var oldAngle = boneNormal.Angle;
		var newAngle = transformedUVNormal.Angle;
		if (Mathf.Abs(oldAngle - newAngle) > 0.005f) {
			//Debug.Log($"Updating {transform.parent.name}.{gameObject.name} normal: {boneNormal} - {transformedUVNormal}");
			GlobalNormal = transformedUVNormal;
			patch.SetChanged();
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
				patch.SetChanged();
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
