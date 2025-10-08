﻿using UnityEngine;

namespace UbiArt.Animation {
	public static class AnimSkeletonExtensions {

		public static UnityBone[] CreateBones(this AnimSkeleton skl, Context context, GameObject gao) {
			UnityBone[] unityBones = new UnityBone[skl.bones.Count];
			for (int i = 0; i < skl.bones.Count; i++) {
				StringID tag = skl.bones[i].tag;
				/*if (skl.boneTags?.Count > i) {
					tag = skl.boneTags[i];
				} else if (skl.boneTagsAdv?.Count > i) {
					tag = new StringID((uint)skl.boneTagsAdv[i]);
				}*/
				GameObject boneGao = new GameObject($"Bone {i} - {tag?.ToString(skl?.UbiArtContext, shortString: true)}");
				unityBones[i] = boneGao.AddComponent<UnityBone>();
				unityBones[i].transform.parent = gao.transform;
			}
			skl.ResetBones(context, unityBones);
			return unityBones;
		}

		public static void ResetBones(this AnimSkeleton skl, Context context, UnityBone[] unityBones) {
			for (int i = 0; i < skl.bones.Count; i++) {
				/*if (bones[i].parentKey.stringID != 0) {
					AnimBone parent = GetBoneFromLink(bones[i].parentKey);
					int parentIndex = bones.IndexOf(parent);
					unityBones[i].parent = unityBones[parentIndex];
				} else {
					unityBones[i].parent = gao.transform;
				}*/
				if (skl.bones[i].parentKey.stringID != 0) {
					AnimBone parent = skl.GetBoneFromLink(skl.bones[i].parentKey);
					int parentIndex = skl.bones.IndexOf(parent);
					unityBones[i].Parent = unityBones[parentIndex];
				} else {
					unityBones[i].Parent = null;
				}
				unityBones[i].bindPosition = skl.bonesDyn[i].position.GetUnityVector();
				unityBones[i].bindScale = skl.bonesDyn[i].scale.GetUnityVector();
				unityBones[i].bindRotation = skl.bonesDyn[i].angle;
				//unityBones[i].xOffset = bonesDyn[i].float1;
				unityBones[i].localPosition = Vector3.zero;
				unityBones[i].localScale = Vector2.one;
				unityBones[i].localRotation = 0;
				unityBones[i].bindZ = skl.bonesDyn[i].z;
				unityBones[i].bindAlpha = skl.bonesDyn[i].alpha;
				unityBones[i].localZ = 0;
				unityBones[i].localAlpha = 0;
				if (context.Settings.EngineVersion <= EngineVersion.RO) {
					unityBones[i].boneLength = skl.bonesDyn[i].boneLength;
				}
				//unityBones[i].UpdateBone();
			}

			// Calculate T Pose
			skl.UpdateBones(unityBones);
		}
		public static void UpdateBones(this AnimSkeleton skl, UnityBone[] unityBones) {
			int[] updateOrder = skl.GetBonesUpdateOrder();
			for (int i = 0; i < updateOrder.Length; i++) {
				unityBones[updateOrder[i]].UpdateBone();
			}
		}

		public static void ResetBonesZero(this AnimSkeleton skel, Transform[] unityBones) {
			for (int i = 0; i < skel.bones.Count; i++) {
				UnityBone b = unityBones[i].GetComponent<UnityBone>();
				b.Reset();
				/*b.bindPosition = bonesDyn[i].position;
				unityBones[i].localScale = bonesDyn[i].scale;
				b.bindRotation = Quaternion.Euler(new Vector3(0, 0, bonesDyn[i].angle.EulerAngle));
				b.localPosition = Vector3.zero;
				b.localRotation = Quaternion.identity;*/
			}
		}
	}
}
