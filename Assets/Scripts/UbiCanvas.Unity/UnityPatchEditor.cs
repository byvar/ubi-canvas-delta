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

public class UnityPatchEditor : MonoBehaviour {
	public AnimPatchBank pbk;
	public int templateIndex;
	public TextureCooked texture;
	public UnityBone[] bones;
	public int[] updateOrder;
	public List<UnityPatchPointEditor> points = new List<UnityPatchPointEditor>();
	public float AspectRatio { get; set; }
	public Vec2d UVScale => new Vec2d(1, -1);
	public Vec2d UVScaleInverse => Vec2d.One / UVScale;
	public AnimTemplate Template => pbk.templates[templateIndex];
	private Dictionary<PatchPointLine, LineRenderer> lines = new Dictionary<PatchPointLine, LineRenderer>();
	private bool changed = false;
	public UnityPatchRenderer PatchRenderer;

	private void Start() {
		CreateMesh();
		CreatePoints();
		//CreateLines();
	}

	private void Update() {
		if (bones != null && updateOrder != null) {
			foreach (var u in updateOrder) {
				bones[u].UpdateBone(controlTransform: false);
			}
		}
		foreach (var l in lines) {
			UpdateLinePositions(l.Value, l.Key);
		}
		if (changed) {
			changed = false;
			Sync();
		}
	}

	void Sync() {
		if (Controller.Obj.playAnimations) return; // Happens automatically in this case
		var unityAnimations = FindObjectsOfType<UnityAnimation>().Where(a => a.AllPatchBanks != null && a.AllPatchBanks.Any(p => p.PBK == pbk));
		if (unityAnimations.Any()) {
			var tpl = Template;
			foreach (var ua in unityAnimations) {
				ua.ForceUpdatePatches = true; //.ForceUpdatePatches(pbkFilter: p => p.PBK == pbk, templateFilter: p => p == tpl);
			}
		}
	}

	void CreateMesh() {
		var tpl = Template;
		
		var patch_gao = gameObject;
		var mesh_bones = tpl.GetBonesLocal(tpl.UbiArtContext, null, patch_gao);
		updateOrder = tpl.GetBonesUpdateOrder(null);
		for(int i = 0; i < mesh_bones.Length; i++) {
			var bone = mesh_bones[i];
			bone.PBKBone = tpl.bonesDyn[i];
			bone.IsPBKEditor = true;
			bone.visualize = true;
		}
		bones = mesh_bones;
		Material tex_mat = UbiArt.ITF.GFXMaterialShader_Template.GetShaderMaterial();

		UnityPatchRenderer pr = patch_gao.AddComponent<UnityPatchRenderer>();
		pr.Skeleton = null;
		pr.Bones = bones;
		pr.AnimTemplate = tpl;
		pr.IsPBKEditor = true;
		PatchRenderer = pr;

		var bounds = new Bounds(Vector3.zero, Vector3.one * 20);

		pr.Init(hLevel: 15, vLevel: 15); // Higher res to match lines from Atlas Editor
		var mpb = new MaterialPropertyBlock();
		pr.ProcessRenderers(mr => {
			mr.localBounds = bounds;
			mr.sharedMaterial = tex_mat;
			mr.GetPropertyBlock(mpb, 0);

			UbiArt.ITF.GFXPrimitiveParam.FillMaterialParamsDefault(tpl.UbiArtContext, mpb, alpha: 1f);

			mpb.SetVector("_UseTextures", new Vector4(1, 0, 0, 0));
			mpb.SetVector("_UseTextures2", new Vector4(0, 0, 0, 0));
			var t = texture.GetUnityTexture(Controller.MainContext);
			mpb.SetTexture("_Diffuse", t.Texture);
			mpb.SetVector("_Diffuse_ST", new Vector4(1, t.HeightFactor, 0, 0));
			mr.SetPropertyBlock(mpb, 0);
		});
		pr.FillUnityMaterialPropertyBlock();

		//FillMaterialParams(mr);
		//SetMaterialTextures(bank.TextureBankPath.textureSet, mr);
		//FillUnityMaterialPropertyBlock(mr, shader: bank?.TextureBankPath?.shader?.obj);
	}

	void CreatePoints() {
		var atl = Template;
		points = new List<UnityPatchPointEditor>();
		for(int i = 0; i < atl.patchPoints.Count; i++) {
			var p = atl.patchPoints[i];
			var gao = new GameObject($"Point {i}");
			gao.transform.SetParent(transform, false);
			gao.transform.localPosition = Vector3.zero;
			gao.transform.localRotation = Quaternion.identity;
			gao.transform.localScale = Vector2.one;
			var ppe = gao.AddComponent<UnityPatchPointEditor>();
			ppe.patch = this;
			ppe.pointIndex = i;

			if (p.local.boneId != null) {
				AnimBone bone = atl.GetBoneFromLink(p.local.boneId);
				if (bone != null) {
					ppe.InitBone(bones[atl.bones.IndexOf(bone)]);
				}
			}
			points.Add(ppe);
		}
		List<Link> pointLinks = atl.patches.SelectMany(p => p.points).Distinct().ToList();

		for (int i = 0; i < pointLinks.Count; i++) {
			//var p = atl.GetPointFromLink(pointLinks[i]);
			var p = atl.patchPoints.FindItemIndex(pp => pp.key == pointLinks[i]);
			if(p == -1) continue;
			var unityPoint = points[p];
			var globalPos = unityPoint.GlobalPosition;
			var unityPos = new Vector3(globalPos.x, globalPos.y, 0f);
			unityPoint.transform.localPosition = unityPos;
		}
		RecalculatePointTransformations();
	}

	public void RecalculatePointTransformations() {
		var uvScale = UVScale;
		var atl = Template;
		foreach (var patch in atl.patches) {
			var patchPoints = patch.points.Select(pointLink =>
				atl.patchPoints.FindItemIndex(patchPoint => patchPoint.key == pointLink))
				.Select(pointIndex => points.FirstOrDefault(p => p.pointIndex == pointIndex)).ToArray();
			var globalPos = patchPoints.Select(pp => pp.GlobalPosition.GetUnityVector()).ToArray();
			var localPos = patchPoints.Select(pp => (pp.Point.uv * uvScale).GetUnityVector()).ToArray();
			VectorHelpers.GetTransformation(localPos, globalPos, out var translation, out var rotation, out var scale);
			//Debug.Log($"{translation} - {rotation} - {scale}");
			UnityPatchPointEditor.Transformation t = new UnityPatchPointEditor.Transformation() {
				Translation = translation,
				Rotation = rotation,
				Scale = scale
			};
			foreach (var p in patchPoints) {
				p.Transformations.Add(t);
			}
		}
		foreach (var p in points) {
			if (!p.Transformations.Any()) continue;
			UnityPatchPointEditor.Transformation t = new UnityPatchPointEditor.Transformation() {
				Translation = new Vector2(p.Transformations.Average(p => p.Translation.x), p.Transformations.Average(p => p.Translation.y)),
				Rotation = p.Transformations.Average(p => p.Rotation),
				Scale = p.Transformations.Average(p => p.Scale)
			};
			//Debug.Log($"{t.Translation} - {t.Rotation} - {t.Scale}");
			p.AverageTransformation = t;
			p.Transformations.Clear();
		}
	}
	public void ResetUVs() {
		SetChanged();
	}

	public void SetChanged() {
		changed = true;
		UpdateMesh();
	}
	public void UpdateMesh() {
		PatchRenderer.UpdateBuffer(updateUV: true);
	}

	void CreateLines() {
		var tpl = Template;
		Dictionary<Link, AnimPatchPoint> points = tpl.patchPoints.ToDictionary(p => p.key, p => p);
		Dictionary<PatchPointLine, int> patchboundaries = new Dictionary<PatchPointLine, int>();
		void AddBoundary(Link link0, Link link1) {
			var boundpt0 = link0.stringID;
			var boundpt1 = link1.stringID;
			var pto0 = points[link0];
			var pto1 = points[link1];
			var ptu0 = this.points.FirstOrDefault(p => p.Point == pto0);
			var ptu1 = this.points.FirstOrDefault(p => p.Point == pto1);

			var bound = new PatchPointLine() { point0 = ptu0.pointIndex, point1 = ptu1.pointIndex };
			if (!patchboundaries.ContainsKey(bound)) {
				patchboundaries[bound] = 1;
			} else {
				patchboundaries[bound] = patchboundaries[bound] + 1;
			}
		}


		foreach (var patch in tpl.patches) {
			var count = patch.points.Length;
			if (count != 4) throw new System.Exception("Unexpected patch points count!");

			AddBoundary(patch.points[0], patch.points[1]);
			AddBoundary(patch.points[2], patch.points[3]);
		}
		foreach (var bound in patchboundaries) {
			if (bound.Value == 1) {
				var lineGao = new GameObject("Line");
				lineGao.hideFlags |= HideFlags.HideInInspector;
				lineGao.transform.SetParent(transform, false);
				lineGao.transform.localPosition = Vector3.zero;
				lineGao.transform.localRotation = Quaternion.identity;
				lineGao.transform.localScale = Vector3.one;
				var lr = lineGao.AddComponent<LineRenderer>();
				lr.material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
				lr.useWorldSpace = false;
				lr.widthMultiplier = 0.02f;
				lr.sortingLayerName = "Gizmo-Line";
				var color = UnityEngine.Color.yellow;
				lr.material.color = color;
				lr.startColor = color;
				lr.endColor = color;
				UpdateLinePositions(lr, bound.Key);
				lines[bound.Key] = lr;
			}
		}
	}
	void UpdateLinePositions(LineRenderer lr, PatchPointLine line) {
		var pt1 = line.point0;
		var pt2 = line.point1;
		lr.positionCount = 2;
		lr.loop = false;
		lr.SetPositions(new Vector3[] {
			points[pt1].transform.localPosition,
			points[pt2].transform.localPosition
		});

	}
	private class PatchPointLine {
		public int point0;
		public int point1;

		public override bool Equals(object obj) {
			var line = obj as PatchPointLine;
			if (line == default) return false;

			return (point0 == line.point0 && point1 == line.point1) || (point0 == line.point1 || point1 == line.point0);
		}

		public override int GetHashCode() {
			var pt0 = point0 > point1 ? point1 : point0;
			var pt1 = point0 > point1 ? point0 : point1;
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 23 + pt0.GetHashCode();
				hash = hash * 23 + pt1.GetHashCode();
				return hash;
			}
		}
	}
}
