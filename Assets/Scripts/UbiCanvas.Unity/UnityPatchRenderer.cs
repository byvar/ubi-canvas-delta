
using System;
using System.Collections.Generic;
using System.Linq;
using UbiArt;
using UbiArt.Animation;
using UbiCanvas.Helpers;
using UnityEngine;

public class UnityPatchRenderer : MonoBehaviour {
	MaterialPropertyBlock mpb;

	public AnimTemplate AnimTemplate { get; set; }
	//public GameObject PatchGameObject { get; set; }
	//public UnityAnimation Animation { get; set; }
	public AnimSkeleton Skeleton { get; set; }
	public UnityBone[] Bones { get; set; }

	public Dictionary<uint, PointData> Points { get; set; } = new Dictionary<uint, PointData>();
	public PatchData[] Patches { get; set; }

	public MeshCollider Collider { get; set; }
	public Mesh ColliderMesh { get; set; }
#if UNITY_EDITOR
	public MeshFilter ColliderFilter { get; set; }
	public MeshRenderer ColliderRenderer { get; set; }
#endif

	public bool IsPBKEditor { get; set; }
	public Mesh PatchMesh { get; set; }

	public class PointData {
		public AnimPatchPoint Point { get; set; }
		public int Index { get; set; }
		public Vec2d Position { get; set; }
		public Vec2d Normal { get; set; }
		public float Z { get; set; }
		public bool IsFlipped { get; set; }
		public float Alpha { get; set; }
		public float ZSkeleton { get; set; }
		public float ZLocal { get; set; }
		public float ZTemplate { get; set; }
		public bool BoneExists { get; set; }
	}
	public class PatchData {
		public AnimPatch Patch { get; set; }
		public int Index { get; set; }
		public float Z { get; set; }
		//public bool IsFlipped { get; set; }
		public bool DrawPatch { get; set; }
		//public float Alpha { get; set; }
		public PointData[] Points { get; set; }

		public GameObject GameObject { get; set; }
		public Renderer Renderer { get; set; }

		public Vector4[] patchPoints = new Vector4[8]; // 8 * numPatches control points (float4), e.g. 8 * 16 = 128
		public Vector4[] colorTable = new Vector4[4]; // 4 * numPatches colors (e.g. 4 * 16 = 64)
		public ComputeBuffer patchBuffer = new ComputeBuffer(8, sizeof(float) * 4);
		public ComputeBuffer colorBuffer = new ComputeBuffer(4, sizeof(float) * 4);

		public void OnDestroy() {
			patchBuffer?.Release();
			colorBuffer?.Release();
		}
		public void SetData() {
			patchBuffer?.SetData(patchPoints);
			colorBuffer?.SetData(colorTable);
		}
	}

	void Start() {
		//Init();
	}

	void OnDestroy() {
		if (Patches != null) {
			foreach (var p in Patches) {
				p.OnDestroy();
			}
		}
	}

	public void Init(int hLevel = 6, int vLevel = 6) {
		PatchMesh = GetPatchMesh(hLevel: hLevel, vLevel: vLevel);
		Points.Clear();
		for (int i = 0; i < AnimTemplate.patchPoints.Count; i++) {
			Points[AnimTemplate.patchPoints[i].key.stringID] = new PointData() {
				Point = AnimTemplate.patchPoints[i],
				Index = i
			};
		}

		if (Patches == null)
			Patches = new PatchData[AnimTemplate.patches.Count];
		for (int i = 0; i < AnimTemplate.patches.Count; i++) {
			var p = AnimTemplate.patches[i];
			Patches[i] = new PatchData() {
				Index = i,
				Patch = p,
				Z = 0,
				Points = p?.points.Select(p => Points.TryGetItem(p.stringID, null)).ToArray()
			};
			GameObject patch_gao = new GameObject($"Patch {i}");
			patch_gao.transform.SetParent(transform, false);
			patch_gao.transform.localPosition = Vector3.zero;
			patch_gao.transform.localRotation = Quaternion.identity;
			patch_gao.transform.localScale = Vector3.one;
			Patches[i].GameObject = patch_gao;
			patch_gao.hideFlags |= HideFlags.HideInHierarchy;
			MeshRenderer mr = patch_gao.AddComponent<MeshRenderer>();
			MeshFilter mf = patch_gao.AddComponent<MeshFilter>();
			mf.sharedMesh = PatchMesh;
			Patches[i].Renderer = mr;
		}
		CreateCollider();
		UpdateBuffer(updateUV: true);
		UpdateCollider();
	}

	public void UpdateBuffer(bool updateUV = true) {
		int patchCount = AnimTemplate.patches.Count;
		foreach (var p in Points) {
			UpdateGlobalPointData(p.Value);
		}
		foreach (var patch in Patches) {
			//patch.Z = patch.Points.FirstOrDefault()?.Z ?? 0f;//(p => p.Z);
			patch.Z = patch.Points.Average(p => p.Z);
			//patch.Alpha = patch.Points.Average(p => p.Alpha);
			//patch.IsFlipped = patch.Points.Any(p => p.IsFlipped);
			patch.DrawPatch = patch.Points.All(p => p.BoneExists);
			//patch.GameObject.name = $"{patch.Index} - {patch.Z} | S{patch.Points.Average(p => p.ZSkeleton)} | L{patch.Points.Average(p => p.ZLocal)} | T{patch.Points.Average(p => p.ZTemplate)}";
			if(patch.GameObject.activeSelf != patch.DrawPatch)
				patch.GameObject.SetActive(patch.DrawPatch);
		//}

			// Z-Sort patches
			//var patchesSorted = Patches.OrderBy(p => p.Z).ToArray();

			//for(int i = 0; i < patchesSorted.Length; i++) {
			//var patch = patchesSorted[i];
			if (!patch.DrawPatch || patch.Patch?.points?.Length != 4) continue;
			var points = patch.Points;
			patch.colorTable[0] = new Vector4(1, 1, 1, points[0].Alpha);
			patch.colorTable[1] = new Vector4(1, 1, 1, points[2].Alpha);
			patch.colorTable[2] = new Vector4(1, 1, 1, points[1].Alpha);
			patch.colorTable[3] = new Vector4(1, 1, 1, points[3].Alpha);
			// bezier: 0-2, 3-1

			void DoForIndices(int index1, int index2, int start, bool flip = false, bool flipUV = true) {
				var p0 = points[index1];
				var p1 = points[index2];
				var cpPos = BezierHelpers.GetControlPoints(
					new Vec2d(p0.Position.x, p0.Position.y),
					new Vec2d(p1.Position.x, p1.Position.y),
					new Vec2d(p0.Normal.x, p0.Normal.y),
					new Vec2d(p1.Normal.x, p1.Normal.y),
					flip0: p0.IsFlipped ^ flip,
					flip1: p1.IsFlipped ^ flip);
				if (updateUV) {
					var cpUV = BezierHelpers.GetControlPoints(
						p0.Point.uv, p1.Point.uv,
						p0.Point.normal, p1.Point.normal, flip0: flipUV, flip1: flipUV);
					for (int i = 0; i < 4; i++) {
						patch.patchPoints[start + i] = new Vector4(cpPos[i].x, cpPos[i].y, cpUV[i].x, cpUV[i].y);
					}
				} else {
					for (int i = 0; i < 4; i++) {
						patch.patchPoints[start + i] = new Vector4(cpPos[i].x, cpPos[i].y, patch.patchPoints[start + i].z, patch.patchPoints[start + i].w);
					}
				}
			}
			//bool patchFlipped = patch.IsFlipped;
			//bool flipPos = false;//points[0].IsFlipped != points[1].IsFlipped;
			DoForIndices(0, 2, 0, flip: true, flipUV: false);
			DoForIndices(1, 3, 4, flip: false, flipUV: true);
			patch.SetData();
			/*var cpPos = BezierHelpers.GetControlPoints(
				p0.local.pos, p1.local.pos,
				p0.local.normal, p1.local.normal);
			var cpUV = BezierHelpers.GetControlPoints(
				p0.uv, p1.uv,
				p0.normal, p1.normal);

			patchPoints[i * 4 + j] = ;
			colorTable[i * 4 + j] = Vector4.one;*/
		}
	}

	private void UpdateGlobalPointData(PointData point) {
		point.BoneExists = false;
		var localBone = AnimTemplate.GetBoneFromLink(point.Point.local.boneId);
		if(localBone == null) return;
		var localBoneIndex = AnimTemplate.bones.IndexOf(localBone);
		var boneDynLocal = AnimTemplate.bonesDyn[localBoneIndex];

		int boneIndex = -1;
		if (IsPBKEditor) {
			boneIndex = localBoneIndex;
		} else {
			boneIndex = Skeleton.GetBoneIndexFromTag(localBone.tag);
			if (boneIndex == -1) return;
		}
		point.BoneExists = true;
		var bone = Bones[boneIndex];

		var boneAngle = (bone?.globalAngle ?? 0f);
		var bonePos = (bone?.globalPosition.GetUbiArtVector() ?? Vec3d.Zero);
		var boneScale = (bone?.globalScale.GetUbiArtVector() ?? Vec2d.One);

		if (point.Point.UbiArtContext.Settings.EngineVersion <= EngineVersion.RO && !IsPBKEditor) {
			/*var boneLength = bone?.boneLength ?? 1f;
			var boneLengthLocal = boneDynLocal?.boneLength ?? 1f;
			var tplBoneScale = AnimTemplate.boneScaleY;*/
			//boneScale *= new Vec2d(boneLength, tplBoneScale);
			var tplBoneScale = AnimTemplate.boneScaleY;
			boneScale *= new Vec2d(1, tplBoneScale);
		}

		Vec2d tplScale = null;
		if (IsPBKEditor) {
			tplScale = Vec2d.One;
		} else {
			var boneDyn = Skeleton.bonesDyn[boneIndex];
			tplScale = boneDynLocal.scale / boneDyn.scale;
			tplScale = new Vec2d(
				tplScale.y, // Why (y,x)?
				tplScale.x);
		}
		Vec2d localBoneScale = boneDynLocal.scale;

		var scaled = (point.Point.local.pos) * boneScale / tplScale;
		var rotated = scaled.Rotate(boneAngle);
		var translated = rotated + new Vec2d(bonePos.x, bonePos.y);

		var scaleSign = Mathf.Sign(boneScale.x * boneScale.y) * Mathf.Sign(localBoneScale.x * localBoneScale.y);
		//var scaleSign = Mathf.Sign(boneScale.y);

		var globalNormal = (point.Point.local.normal * new Vec2d(scaleSign, 1)).Rotate(boneAngle);

		point.Position = translated;
		point.Normal = globalNormal;
		point.Z = bone.globalZ; // It doesn't seem to use the template Z value! :(
		point.IsFlipped = (boneScale.x * localBoneScale.x) < 0;
		point.Alpha = bone.bindAlpha + bone.localAlpha;

		point.ZLocal = bone.localZ;
		point.ZSkeleton = bone.bindZ;
		point.ZTemplate = boneDynLocal.z;
	}
	public void ProcessRenderers(Action<Renderer> action) {
		foreach (var p in Patches) {
			var r = p.Renderer;
			action?.Invoke(r);
		}
	}
	public void FillUnityMaterialPropertyBlock() {
		int index = 0;
		foreach (var p in Patches) {
			var r = p.Renderer;
			if (r == null) continue;
			if (p.patchBuffer == null) continue;
			if (mpb == null) mpb = new MaterialPropertyBlock();
			r.GetPropertyBlock(mpb, index);
			float z = 0;
			mpb.SetVector("_BezierParams", new Vector4(1, 0, z, 0));
			mpb.SetBuffer("_BezierControlPoints", p.patchBuffer);
			mpb.SetBuffer("_BezierColors", p.colorBuffer);
			// Set property block
			r.SetPropertyBlock(mpb, index);
		}
	}

	private void CreateCollider() {
		if (ColliderMesh == null) {
			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> indices = new List<int>();
			
			int currentVertexCount = 0;
			foreach (var patch in Patches) {
				if(patch.Points == null || patch.Points.Length != 4) continue;
				foreach (var point in patch.Points) {
					var pos = point.Position?.GetUnityVector() ?? Vector2.zero;
					vertices.Add(pos);
				}

				indices.Add(currentVertexCount + 1);
				indices.Add(currentVertexCount);
				indices.Add(currentVertexCount + 2);

				indices.Add(currentVertexCount + 1);
				indices.Add(currentVertexCount + 2);
				indices.Add(currentVertexCount + 3);

				indices.Add(currentVertexCount);
				indices.Add(currentVertexCount + 1);
				indices.Add(currentVertexCount + 2);

				indices.Add(currentVertexCount + 2);
				indices.Add(currentVertexCount + 1);
				indices.Add(currentVertexCount + 3);
				currentVertexCount += 4;
			}

			Mesh mesh = new Mesh();
			mesh.name = "Collider";
			mesh.SetVertices(vertices);
			mesh.SetTriangles(indices, 0);
			mesh.RecalculateNormals();
			ColliderMesh = mesh;
		}
		if (Collider == null) Collider = gameObject.AddComponent<MeshCollider>();
#if UNITY_EDITOR
		if (ColliderFilter == null) ColliderFilter = gameObject.AddComponent<MeshFilter>();
		if (ColliderRenderer == null) {
			ColliderRenderer = gameObject.AddComponent<MeshRenderer>();
			gameObject.layer |= LayerMask.NameToLayer("DoNotRender");
			ColliderRenderer.sharedMaterial = Controller.Obj.hiddenMaterial;
		}
#endif
	}
	public void UpdateCollider() {
		if (ColliderMesh != null) {
			var vertices = ColliderMesh.vertices;
			int i = 0;
			foreach (var patch in Patches) {
				if (patch.Points == null || patch.Points.Length != 4) continue;
				foreach (var point in patch.Points) {
					var pos = point.Position?.GetUnityVector() ?? Vector2.zero;
					vertices[i] = pos;
					i++;
				}
			}
			if (vertices.Distinct().Count() >= 3) {
				ColliderMesh.vertices = vertices;
				ColliderMesh.RecalculateBounds();

				if (Collider != null)
					Collider.sharedMesh = ColliderMesh;
#if UNITY_EDITOR
				if (ColliderFilter != null)
					ColliderFilter.sharedMesh = ColliderMesh;
#endif
			}
		}
	}

	private static Dictionary<(int,int), Mesh> PatchMeshes = new Dictionary<(int Width, int Height), Mesh>();
	private static Mesh GetPatchMesh(int hLevel = 8, int vLevel = 8) {
		if (UnitySettings.UseHighResolutionBezierPatches) {
			hLevel = 32;
			vLevel = 32;
		}
		var size = (hLevel, vLevel);
		if (PatchMeshes.ContainsKey(size)) {
			return PatchMeshes[size];
		}
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> indices = new List<int>();

		const int baseIndex = 0;
		const int patch = 0;

		for (int y = 0; y <= vLevel; y++) {
			for (int x = 0; x <= hLevel; x++) {
				float u = (float)x / hLevel;
				float v = (float)y / vLevel;

				vertices.Add(new Vector3(0, 0, patch)); // z = patch index
				uvs.Add(new Vector2(u, v));
			}
		}

		int vertsPerRow = hLevel + 1;

		for (int y = 0; y < vLevel; y++) {
			for (int x = 0; x < hLevel; x++) {
				int i0 = baseIndex + y * vertsPerRow + x;
				int i1 = baseIndex + y * vertsPerRow + x + 1;
				int i2 = baseIndex + (y + 1) * vertsPerRow + x;
				int i3 = baseIndex + (y + 1) * vertsPerRow + x + 1;

				indices.Add(i0);
				indices.Add(i2);
				indices.Add(i1);

				indices.Add(i1);
				indices.Add(i2);
				indices.Add(i3);
			}
		}

		Mesh mesh = new Mesh();
		mesh.name = $"BezierPatchGrid[{hLevel}x{vLevel}]";
		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uvs);
		mesh.SetTriangles(indices, 0);
		mesh.RecalculateNormals();
		//mesh.bounds = new Bounds(Vector3.zero, new Vector3(10,10,0.1f));
		
		PatchMeshes[size] = mesh;

		return mesh;
	}
}
