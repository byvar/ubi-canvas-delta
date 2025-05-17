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
					//unityBones[i].boneLength = atl.bonesDyn[i].boneLength;
					//unityBones[i].xScaleMultiplier = atl.bonesDyn[i].boneLength;
					//unityBones[i].xScaleMultiplier = 1f;
					unityBones[i].xScaleMultiplier = skeleton.bonesDyn[boneIndex].boneLength / atl.bonesDyn[i].boneLength;
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
					//unityBones[i].boneLength = atl.bonesDyn[i].boneLength;
					//unityBones[i].xScaleMultiplier = atl.bonesDyn[i].boneLength;
					//unityBones[i].xScaleMultiplier = 1f;
					unityBones[i].xScaleMultiplier = atl.bonesDyn[i].boneLength;
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
			Matrix4x4[] bindPoses = new Matrix4x4[atl.bones.Count];
			for (int i = 0; i < atl.bones.Count; i++) {
				if (unityBones[i] == null) continue;
				bindPoses[i] = unityBones[i].transform.worldToLocalMatrix * parent_gao.transform.localToWorldMatrix;
			}
			atl.ResetBonesLocal(unityBones.Select(b => b.transform).ToArray());
			mesh.bindposes = bindPoses;
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
		}
		public static Mesh CreateMesh(this AnimTemplate atl, int interpCount = 1) {
			// TODO: Create 1 mesh per patch. Then pass the bezier control points as a buffer to the shader?
			// Refer to default_PCT_Patch_VS (in Rayman Origins PC: bundle_PC/shaders/unified/renderoverdraw.fx) for shader implementation
			Mesh meshUnity = new Mesh();
			List<Link> pointLinks = atl.patches.SelectMany(p => p.points).Distinct().ToList();
			List<TemplateMeshVertex> verts = new List<TemplateMeshVertex>();
			List<int> tris = new List<int>();
			for (int i = 0; i < pointLinks.Count; i++) {
				AnimPatchPoint pp = atl.GetPointFromLink(pointLinks[i]);
				var v = new TemplateMeshVertex() {
					Vertex = pp.local.pos.GetUnityVector(),
					UV = pp.uv.GetUnityVector(),
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
				int curInterpCount = interpCount;
				if(p.numPoints != 4) curInterpCount = 1;

				if (curInterpCount > 1) {
					int AddInterpolatedVertex(AnimPatchPoint p0, AnimPatchPoint p1, int defaultResult, float lerp) {
						if (lerp == 0 || lerp == 1) return defaultResult;
						//return defaultResult;
						/*var p0i = patchPoints.FindItemIndex(p => p == p0);
						var p1i = patchPoints.FindItemIndex(p => p == p1);

						// Points have to be on different side of the patch
						if (p0i / 2 == p1i / 2 || p0i % 2 != p1i % 2) {
							return defaultResult;
						}*/
						BoneWeight lerpWeight = new BoneWeight();
						if (p0.local.boneId != p1.local.boneId) {
							AnimBone bone0 = atl.GetBoneFromLink(p0.local.boneId);
							AnimBone bone1 = atl.GetBoneFromLink(p1.local.boneId);
							if (bone0 != null && bone1 != null) {
								lerpWeight = new BoneWeight() {
									boneIndex0 = atl.bones.IndexOf(bone0),
									boneIndex1 = atl.bones.IndexOf(bone1),
									weight0 = lerp,
									weight1 = 1 - lerp,
									weight2 = 0,
									weight3 = 0
								};
							}
						} else {
							AnimBone bone0 = atl.GetBoneFromLink(p0.local.boneId);
							lerpWeight = new BoneWeight() {
								boneIndex0 = atl.bones.IndexOf(bone0),
								weight0 = 1,
								weight1 = 0,
								weight2 = 0,
								weight3 = 0
							};
						}

						Vec2d lerpPos, lerpUV;
						var cpPos = BezierHelpers.GetControlPoints(
							p0.local.pos, p1.local.pos,
							p0.local.normal, p1.local.normal);
						var cpUV = BezierHelpers.GetControlPoints(
							p0.uv, p1.uv,
							p0.normal, p1.normal);
						lerpPos = BezierHelpers.CalculateCubicBezierPoint(lerp, cpPos);
						lerpUV = BezierHelpers.CalculateCubicBezierPoint(lerp, cpUV);
						//lerpPos = p0.local.pos + (p1.local.pos - p0.local.pos) * lerp;
						//lerpUV = p0.uv + (p1.uv - p0.uv) * lerp;

						verts.Add(new TemplateMeshVertex() {
							Vertex = lerpPos.GetUnityVector(),
							UV = lerpUV.GetUnityVector(),
							Weight = lerpWeight
						});

						return verts.Count - 1;
					}

					int[] trisPatch = new int[(p.numPoints - 2) * 3 * curInterpCount];
					List<int> pointIndices = p.points.Select(link => pointLinks.IndexOf(link)).ToList();
					for (int j = 0; j < p.numPoints - 2; j++) {
						int[] inds = new int[] { pointIndices[j + 0], pointIndices[j + 1], pointIndices[j + 2] };

						int p0i = 0;
						int p1i = 2;
						/*if (j % 2 != 0) {
							p0i = 1;
							p1i = 2;
						}*/
						var p0 = atl.GetPointFromLink(p.points[j + p0i]);
						var p1 = atl.GetPointFromLink(p.points[j + p1i]);

						for (int k = 0; k < curInterpCount; k++) {
							//var interp0 = k > 0 ? (verts.Count - 1) : AddInterpolatedVertex(p0, p1, pointIndices[j + 0], (float)k / interpCount);
							var interp0 = AddInterpolatedVertex(p0, p1, pointIndices[j + p0i], (float)k / interpCount);
							var interp1 = AddInterpolatedVertex(p0, p1, pointIndices[j + p1i], (float)(k + 1) / curInterpCount);
							inds[p0i] = interp0;
							inds[p1i] = interp1;
							
							if (j % 2 == 0) {
								trisPatch[(j * 3 * curInterpCount) + k * 3 + 1] = inds[0];
								trisPatch[(j * 3 * curInterpCount) + k * 3 + 0] = inds[1];
								trisPatch[(j * 3 * curInterpCount) + k * 3 + 2] = inds[2];
							} else {
								trisPatch[(j * 3 * curInterpCount) + k * 3 + 0] = inds[0];
								trisPatch[(j * 3 * curInterpCount) + k * 3 + 1] = inds[1];
								trisPatch[(j * 3 * curInterpCount) + k * 3 + 2] = inds[2];
							}
						}
					}
					tris.AddRange(trisPatch);
				} else {
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
			}
			if (verts.Count < 3) return null;
			Vector3[] normals = Enumerable.Repeat(Vector3.forward, verts.Count).ToArray();
			meshUnity.vertices = verts.Select(v => v.Vertex).ToArray();
			meshUnity.normals = normals;
			meshUnity.SetTriangles(tris, 0);
			meshUnity.uv = verts.Select(v => v.UV).ToArray();
			meshUnity.boneWeights = verts.Select(v => v.Weight).ToArray();
			//meshUnity.SetUVs(4, Enumerable.Repeat(Vector4.one, 4).ToList());
			return meshUnity;
		}
	}
}
