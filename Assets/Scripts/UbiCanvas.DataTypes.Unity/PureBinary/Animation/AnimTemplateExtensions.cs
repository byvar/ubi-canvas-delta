using System.Collections.Generic;
using System.Linq;
using UbiCanvas.Helpers;
using UnityEngine;

namespace UbiArt.Animation {
	public static class AnimTemplateExtensions {
		public static UnityBone[] CreateBonesLocal(this AnimTemplate atl, Context context, GameObject gao) {
			UnityBone[] unityBones = new UnityBone[atl.bones.Count];
			for (int i = 0; i < atl.bones.Count; i++) {
				StringID tag = atl.bones[i].tag;
				GameObject boneGao = new GameObject($"Bone {i} - {tag?.ToString(atl?.UbiArtContext, shortString: true)}");
				unityBones[i] = boneGao.AddComponent<UnityBone>();
				unityBones[i].transform.parent = gao.transform;
			}
			atl.ResetBonesLocal(unityBones.Select(b => b.transform).ToArray());
			return unityBones;
		}
		public static UnityBone[] GetBonesLocal(this AnimTemplate atl, Context context, GameObject parent_gao) {
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
					unityBones[i].boneLength = atl.bonesDyn[i].boneLength * atl.SizeMultiplier;
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
			atl.ResetBonesLocal(unityBones.Select(b => b.transform).ToArray());
			return unityBones;
		}

		public static void ResetBonesLocal(this AnimTemplate atl, Transform[] unityBones) {
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
				b.localPosition = atl.bonesDyn[i].position.GetUnityVector();
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
	}
}
