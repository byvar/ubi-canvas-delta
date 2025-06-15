using System.Collections.Generic;
using System.Linq;
using UbiCanvas.Helpers;
using UnityEngine;

namespace UbiArt.Animation {
	public static class AnimTemplateExtensions {

		public static UnityBone[] GetBones(this AnimTemplate atl, Context context, Mesh mesh, GameObject skeleton_gao, AnimSkeleton skeleton, UnityBone[] skeletonBones) {
			UnityBone[] unityBones = new UnityBone[atl.bones.Count];
			/*foreach (var b in skeletonBones) {
				b.boneLength = 0f;//atl.bonesDyn[i].boneLength;
				b.xScaleMultiplier = 1f;
				//b.xScaleMultiplier = 1f;
			}*/
			for (int i = 0; i < atl.bones.Count; i++) {
				int boneIndex = skeleton.GetBoneIndexFromTag(atl.bones[i].tag);
				if (boneIndex == -1) continue;
				unityBones[i] = skeletonBones[boneIndex];
				//Vector3 scale = bonesDyn[i].scale / skeleton.bonesDyn[boneIndex].scale;
				//unityBones[i].localScale = new Vector3(Mathf.Abs(scale.y), Mathf.Abs(scale.x), scale.z);
				unityBones[i].bindPosition = Vector3.zero;
				unityBones[i].bindRotation = 0;
				if (context.Settings.EngineVersion <= EngineVersion.RO) {
					unityBones[i].boneLength = atl.bonesDyn[i].boneLength;
				}
				//unityBones[i].bindRotation = bonesDyn[i].angle - unityBones[i].globalAngle;
				unityBones[i].bindScale = atl.bonesDyn[i].scale.GetUnityVector() / skeleton.bonesDyn[boneIndex].scale.GetUnityVector();
				unityBones[i].bindScale = new Vector2(
					unityBones[i].bindScale.y, // Why (y,x)?
					unityBones[i].bindScale.x);
				
				if (atl.bones[i].parentKey.stringID != 0) {
					AnimBone parent = atl.GetBoneFromLink(atl.bones[i].parentKey);
					int parentIndex = skeleton.GetBoneIndexFromTag(parent.tag);
					unityBones[i].Parent = skeletonBones[parentIndex];
				} else {
					unityBones[i].Parent = null;
				}
			}
			int[] updateOrder = atl.GetBonesUpdateOrder(null);
			for (int i = 0; i < updateOrder.Length; i++) {
				if (unityBones[updateOrder[i]] == null) continue;
				unityBones[updateOrder[i]].UpdateBone();
			}
			Matrix4x4[] bindPoses = new Matrix4x4[atl.bones.Count];
			for (int i = 0; i < atl.bones.Count; i++) {
				if (unityBones[i] == null) continue;
				bindPoses[i] = unityBones[i].transform.worldToLocalMatrix * skeleton_gao.transform.localToWorldMatrix;
			}
			skeleton.ResetBones(context, skeletonBones);
			if(mesh != null)
				mesh.bindposes = bindPoses;
			return unityBones;
		}


		public static UnityBone[] CreateBonesLocal(this AnimTemplate atl, Context context, GameObject gao) {
			UnityBone[] unityBones = new UnityBone[atl.bones.Count];
			for (int i = 0; i < atl.bones.Count; i++) {
				StringID tag = atl.bones[i].tag;
				GameObject boneGao = new GameObject($"Bone {i} - {tag?.ToString(atl?.UbiArtContext, shortString: true)}");
				unityBones[i] = boneGao.AddComponent<UnityBone>();
				unityBones[i].bind = true;
				unityBones[i].transform.parent = gao.transform;
			}
			atl.ResetBonesLocal(unityBones.Select(b => b.transform).ToArray());
			return unityBones;
		}
		public static UnityBone[] GetBonesLocal(this AnimTemplate atl, Context context, Mesh mesh, GameObject parent_gao) {
			UnityBone[] unityBones = atl.CreateBonesLocal(context, parent_gao);
			/*foreach (var b in skeletonBones) {
				b.boneLength = 0f;//atl.bonesDyn[i].boneLength;
				b.xScaleMultiplier = 1f;
				//b.xScaleMultiplier = 1f;
			}*/
			atl.ResetBonesZeroLocal(unityBones.Select(b => b.transform).ToArray());
			for (int i = 0; i < atl.bones.Count; i++) {
				int boneIndex = i;
				//Vector3 scale = bonesDyn[i].scale / skeleton.bonesDyn[boneIndex].scale;
				//unityBones[i].localScale = new Vector3(Mathf.Abs(scale.y), Mathf.Abs(scale.x), scale.z);
				unityBones[i].bindPosition = Vector3.zero;
				unityBones[i].bindRotation = 0;
				if (context.Settings.EngineVersion <= EngineVersion.RO) {
					unityBones[i].boneLength = atl.bonesDyn[i].boneLength;
				}
				//unityBones[i].bindRotation = bonesDyn[i].angle - unityBones[i].globalAngle;
				unityBones[i].bindScale = Vector2.one;
				//unityBones[i].bindScale = atl.bonesDyn[i].scale.GetUnityVector();
				/*unityBones[i].bindScale = new Vector2(
					unityBones[i].bindScale.y, // Why (y,x)?
					unityBones[i].bindScale.x);*/
			}
			int[] updateOrder = atl.GetBonesUpdateOrder(null);
			for (int i = 0; i < updateOrder.Length; i++) {
				if (unityBones[updateOrder[i]] == null) continue;
				unityBones[updateOrder[i]].UpdateBone();
			}
			if (mesh != null) {
				Matrix4x4[] bindPoses = new Matrix4x4[atl.bones.Count];
				for (int i = 0; i < atl.bones.Count; i++) {
					if (unityBones[i] == null) continue;
					bindPoses[i] = unityBones[i].transform.worldToLocalMatrix * parent_gao.transform.localToWorldMatrix;
				}
				mesh.bindposes = bindPoses;
			}
			atl.ResetBonesLocal(unityBones.Select(b => b.transform).ToArray());
			return unityBones;
		}

		public static void ResetBones(this AnimTemplate atl, Transform[] unityBones, AnimSkeleton skeleton) {
			for (int i = 0; i < atl.bones.Count; i++) {
				int boneIndex = skeleton.GetBoneIndexFromTag(atl.bones[i].tag);
				UnityBone b = unityBones[boneIndex].GetComponent<UnityBone>();
				if (atl.bones[i].parentKey.stringID != 0) {
					AnimBone parent = atl.GetBoneFromLink(atl.bones[i].parentKey);
					int parentIndex = skeleton.GetBoneIndexFromTag(parent.tag);
					b.Parent = unityBones[parentIndex].GetComponent<UnityBone>();
				} else {
					b.Parent = null;
				}
				b.localPosition = atl.bonesDyn[i].position.GetUnityVector();
				b.localScale = atl.bonesDyn[i].scale.GetUnityVector();
				b.localRotation = atl.bonesDyn[i].angle;
				b.UpdateBone();
			}
		}
		public static void ResetBonesLocal(this AnimTemplate atl, Transform[] unityBones, bool invert = true) {
			int[] updateOrder = atl.GetBonesUpdateOrder(null);
			for (int u = 0; u < updateOrder.Length; u++) {
				var i = updateOrder[u];
				if (unityBones[i] == null) continue;
				UnityBone b = unityBones[i].GetComponent<UnityBone>();

				if (atl.bones[i].parentKey.stringID != 0) {
					AnimBone parent = atl.GetBoneFromLink(atl.bones[i].parentKey);
					if (parent != null) {
						b.Parent = unityBones[atl.bones.IndexOf(parent)].GetComponent<UnityBone>();
					} else {
						b.Parent = null;
					}
				} else {
					b.Parent = null;
				}
				if (b.Parent != null || !invert) {
					b.localPosition = atl.bonesDyn[i].position.GetUnityVector();
				} else {
					var posVector = atl.bonesDyn[i].position.GetUnityVector();
					b.localPosition = new Vector3(posVector.x, -posVector.y);
				}
				b.localScale = atl.bonesDyn[i].scale.GetUnityVector();
				b.localRotation = atl.bonesDyn[i].angle;

				b.UpdateBone();
			}
		}
		public static void ResetBonesZeroLocal(this AnimTemplate atl, Transform[] unityBones) {
			int[] updateOrder = atl.GetBonesUpdateOrder(null);
			for (int u = 0; u < updateOrder.Length; u++) {
				var i = updateOrder[u];
				UnityBone b = unityBones[i].GetComponent<UnityBone>();
				b.Reset();
			}
		}
		public static void ResetBonesZero(this AnimTemplate atl, Transform[] unityBones, AnimSkeleton skeleton) {
			for (int i = 0; i < atl.bones.Count; i++) {
				int boneIndex = skeleton.GetBoneIndexFromTag(atl.bones[i].tag);
				UnityBone b = unityBones[boneIndex].GetComponent<UnityBone>();
				b.Reset();
			}
		}
		private class TemplateMeshVertex {
			public Vector3 Vertex { get; set; }
			public Vector2 UV { get; set; }
			public BoneWeight Weight { get; set; }
			public int PatchIndex { get; set; }
		}
		public static Mesh CreateMesh(this AnimTemplate atl) {
			// Old method, use Bezier Patch Renderer instead
			Mesh meshUnity = new Mesh();
			List<Link> pointLinks = atl.patches.SelectMany(p => p.points).Distinct().ToList();
			List<TemplateMeshVertex> verts = new List<TemplateMeshVertex>();
			List<int> tris = new List<int>();
			for (int i = 0; i < pointLinks.Count; i++) {
				AnimPatchPoint pp = atl.GetPointFromLink(pointLinks[i]);
				var v = new TemplateMeshVertex() {
					Vertex = pp.local.pos.GetUnityVector(),
					UV = pp.uv.GetUnityVector(),
					PatchIndex = atl.patches.FindItemIndex(p => p?.points?.Any(po => po.stringID == pointLinks[i].stringID) ?? false)
				};
				AnimBone bone = atl.GetBoneFromLink(pp.local.boneId);
				if (bone != null) {
					v.Weight = new BoneWeight() {
						boneIndex0 = atl.bones.IndexOf(bone),
						weight0 = 1,
						weight1 = 0,
						weight2 = 0,
						weight3 = 0
					};
				}
				verts.Add(v);
			}
			for (int i = 0; i < atl.patches.Count; i++) {
				AnimPatch p = atl.patches[i];
				int[] trisPatch = new int[(p.numPoints - 2) * 3];
				List<int> pointIndices = p.points.Select(link => pointLinks.IndexOf(link)).ToList();
				for (int j = 0; j < p.numPoints - 2; j++) {
					if (j % 2 == 0) {
						trisPatch[(j * 3) + 1] = pointIndices[j];
						trisPatch[(j * 3)] = pointIndices[j + 1];
						trisPatch[(j * 3) + 2] = pointIndices[j + 2];
					} else {
						trisPatch[(j * 3)] = pointIndices[j];
						trisPatch[(j * 3) + 1] = pointIndices[j + 1];
						trisPatch[(j * 3) + 2] = pointIndices[j + 2];
					}
				}
				tris.AddRange(trisPatch);
			}
			if (verts.Count < 3) return null;
			Vector3[] normals = Enumerable.Repeat(Vector3.forward, verts.Count).ToArray();
			meshUnity.vertices = verts.Select(v => v.Vertex).ToArray();
			meshUnity.normals = normals;
			meshUnity.SetTriangles(tris, 0);
			meshUnity.uv = verts.Select(v => v.UV).ToArray();
			meshUnity.boneWeights = verts.Select(v => v.Weight).ToArray();
			meshUnity.SetUVs(4, verts.Select(v => new Vector4(v.PatchIndex, 0,0,0)).ToList());
			//meshUnity.SetUVs(4, Enumerable.Repeat(Vector4.one, 4).ToList());
			return meshUnity;
		}
	}
}
