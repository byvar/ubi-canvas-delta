﻿using System.Collections.Generic;
using System.Linq;
using UbiArt.Animation;
using UbiCanvas;
using UbiCanvas.Helpers;
using UnityEngine;
using UnityEngine.Rendering;

namespace UbiArt.ITF {
	public partial class AnimLightComponent {
		private AnimLightComponent_Template tpl;
		private UnityAnimation ua;

		public override void InitUnityComponent(Actor act, GameObject gao, ActorComponent_Template template, int index) {
			base.InitUnityComponent(act, gao, template, index);
			if (template != null && template is AnimLightComponent_Template) {
				tpl = template as AnimLightComponent_Template;
				CreateGameObjects(gao);

			}
		}

		private void CreateGameObjects(GameObject gao) {
			var context = UbiArtContext;
			if (!context.Loader.LoadAnimations) return;
			Material tex_mat = GFXMaterialShader_Template.GetShaderMaterial(shader: shader?.obj);
			if (context.Settings.EngineVersion > EngineVersion.RO) {
				CreateUnityAnimation(gao);
			} else {
				ProcessOrigins(gao, tex_mat);
			}
		}

		private void ProcessOrigins(GameObject gao, Material tex_mat) {
			var c = UbiArtContext;
			if (!UbiArtContext.Loader.LoadAnimations) return;

			// Create skeleton
			var animation_gao = new GameObject("AnimLightComponent - Animation");
			animation_gao.transform.SetParent(gao.transform, false);
			animation_gao.transform.localPosition = Vector3.zero;
			animation_gao.transform.localRotation = Quaternion.identity;
			animation_gao.transform.localScale = Vector3.one;

			// Collect resources
			ICSerializable[] resources = tpl.animSet.resources;
			var resourceList = tpl.animSet.resourceList;
			ICSerializable sklRes = resources.Where(res => res is AnimSkeleton).FirstOrDefault();
			AnimSkeleton skeleton = sklRes != null ? (AnimSkeleton)sklRes : null;
			if (skeleton == null) return;

			// Create list of patchbanks
			List<UnityAnimation.UnityPatchBank> pbksOrigins = new List<UnityAnimation.UnityPatchBank>();
			for(int i = 0; i < resources.Length; i++) {
				var res = resources[i];
				if (res is TextureCooked tex) {
					var texPath = tpl.animSet.resourceList[i];
					var pbkPathLower = $"{texPath.GetFilenameWithoutExtension(fullPath: true).ToLowerInvariant()}.pbk";
					for (int j = 0; j < resources.Length; j++) {
						if (resources[j] is AnimPatchBank pbk && resourceList[j].FullPath.ToLowerInvariant() == pbkPathLower) {
							var pbkPath = tpl.animSet.resourceList[j];
							pbksOrigins.Add(new UnityAnimation.UnityPatchBank() {
								PBK = pbk,
								TextureID = texPath.stringID
							});
						}
					}
				}
			}
			Dictionary<StringID, UnityAnimation.UnityPatchBank> unityPBKsOrigins = pbksOrigins.ToDictionary(tb => tb.TextureID);
			Dictionary<StringID, UnityAnimation.UnityPatchBank> unityPBKs = new Dictionary<StringID, UnityAnimation.UnityPatchBank>();
			// Now add the "bank change" entries
			foreach (var bankChange in tpl.animSet.banks) {
				var texturePath = bankChange.bankPath;
				var fullPath = bankChange.pbkPath;
				if(bankChange.state != uint.MaxValue) continue; // Only changed in specific cases

				if(bankChange.pbk != null) {
					unityPBKs[bankChange.bankName] = new UnityAnimation.UnityPatchBank() {
						PBK = bankChange.pbk,
						ID = bankChange.bankName,
						TexturePathOrigins = texturePath
					};
				}
			}

			// Create templates
			ua = animation_gao.AddComponent<UnityAnimation>();
			ua.InitSubObjects();
			var skeleton_gao = ua.skeletonGao;
			var bones = skeleton.CreateBones(c, skeleton_gao);
			ua.transform.localScale = new Vector3(tpl?.scale?.x ?? 1f, tpl?.scale?.y ?? 1f, 1f);
			ua.transform.localPosition = new Vector3(tpl?.posOffset?.x ?? 1f, tpl?.posOffset?.y ?? 1f, -(tpl?.depthOffset ?? 0f));
			ua.transform.localRotation = tpl?.angleOffset?.GetUnityQuaternion() ?? Quaternion.identity;
			ua.bones = bones;
			ua.skeleton = skeleton;
			ua.alc = this;
			ua.alc_tpl = tpl;
			if (tpl.patchLevel != 0) {
				ua.PatchHLevel = tpl.patchLevel;
				ua.PatchVLevel = tpl.patchLevel;
			} else {
				// These are only used if patchLevel is 0
				if (tpl.patchHLevel != 0) ua.PatchHLevel = tpl.patchHLevel;
				if (tpl.patchVLevel != 0) ua.PatchVLevel = tpl.patchVLevel;
			}
			ua.patchBanks = unityPBKs;
			ua.patchBanksOrigins = unityPBKsOrigins;
			ua.AllPatchBanks = unityPBKs.Concat(unityPBKsOrigins).Select(v => v.Value).Distinct().ToArray();
			foreach (var pbk in unityPBKs.Concat(unityPBKsOrigins)) {
				var bank = pbk.Value;
				bank.ResetPatches(ua);
			}
			skeleton.ResetBones(c, bones);
			/*List<Path> animPaths = new List<Path>();
			foreach (SubAnim_Template sat in tpl.animSet.animations) {
				animPaths.Add(sat.name);
			}*/

			List<UnityAnimation.UnityAnimationTrack> anims = new List<UnityAnimation.UnityAnimationTrack>();
			foreach (SubAnim_Template sat in tpl.animSet.animations) {
				var fullPath = new Path($"{tpl.animationPath?.FullPath ?? ""}{sat.name?.FullPath ?? ""}");

				var resourceIndex = resourceList.FindItemIndex(p => p == fullPath);

				if (resourceIndex != -1) {
					anims.Add(new UnityAnimation.UnityAnimationTrack() {
						ID = sat.friendlyName,
						Path = resourceList[resourceIndex],
						Track = resources[resourceIndex] as AnimTrack,
						SubAnim = sat,
						AABB = tpl.animSet.animAABB
					});
				}
			}
			ua.anims = anims.ToArray();
			for (int i = 0; i < ua.anims.Length; i++) {
				ua.anims[i].Index = i;
			}
			if (ua.anims.Length > 0) {
				ua.animIndex = 0;
			}
			var defaultAnim = tpl.defaultAnimation;
			if (defaultAnim != null && !defaultAnim.IsNull) {
				var newAnimIndex = ua.anims.FindItemIndex(anm => anm.ID == defaultAnim);
				if (newAnimIndex != -1) ua.animIndex = newAnimIndex;
			}
			ua.Init();
		}

		private void CreateUnityAnimation(GameObject gao) {
			var c = UbiArtContext;
			if (!UbiArtContext.Loader.LoadAnimations) return;

			AnimSkeleton skeleton = subAnimInfo?.animPackage?.skel ?? tpl.animSet?.animPackage?.skel;
			if(skeleton == null) return;

			// Create skeleton
			var animation_gao = new GameObject("AnimLightComponent - Animation");
			animation_gao.transform.SetParent(gao.transform, false);
			animation_gao.transform.localPosition = Vector3.zero;
			animation_gao.transform.localRotation = Quaternion.identity;
			animation_gao.transform.localScale = Vector3.one;

			// Create list of texture banks
			Dictionary<StringID, TextureBankPath> textureBanks = new Dictionary<StringID, TextureBankPath>();
			foreach (TextureBankPath bp in tpl.animSet.animPackage.textureBank) {
				if (bp?.pbk == null) continue;
				textureBanks[bp.id] = bp;
			}
			foreach (TextureBankPath bp in subAnimInfo?.animPackage?.textureBank) {
				if(bp?.pbk == null) continue;
				textureBanks[bp.id] = bp;
			}
			Dictionary<StringID, UnityAnimation.UnityPatchBank> unityPBKs = textureBanks
				.Select(tb => new UnityAnimation.UnityPatchBank() {
					ID = tb.Key,
					PBK = tb.Value.pbk,
					TextureBankPath = tb.Value
				}).ToDictionary(tb => tb.ID);

			// Create templates
			ua = animation_gao.AddComponent<UnityAnimation>();
			ua.InitSubObjects();
			var bones = skeleton.CreateBones(c, ua.skeletonGao);
			ua.transform.localScale = new Vector3(tpl?.scale?.x ?? 1f, tpl?.scale?.y ?? 1f, 1f);
			ua.transform.localPosition = new Vector3(tpl?.posOffset?.x ?? 1f, tpl?.posOffset?.y ?? 1f, -(tpl?.depthOffset ?? 0f));
			ua.transform.localRotation = tpl?.angleOffset?.GetUnityQuaternion() ?? Quaternion.identity;
			ua.bones = bones;
			ua.skeleton = skeleton;
			ua.patchBanks = unityPBKs;
			ua.AllPatchBanks = unityPBKs.Select(v => v.Value).Distinct().ToArray();
			ua.alc = this;
			ua.alc_tpl = tpl;
			if (tpl.patchLevel != 0) {
				ua.PatchHLevel = tpl.patchLevel;
				ua.PatchVLevel = tpl.patchLevel;
			} else {
				// These are only used if patchLevel is 0
				if (tpl.patchHLevel != 0) ua.PatchHLevel = tpl.patchHLevel;
				if (tpl.patchVLevel != 0) ua.PatchVLevel = tpl.patchVLevel;
			}
			foreach (var pbk in unityPBKs) {
				var bank = pbk.Value;
				bank.ResetPatches(ua);
			}
			skeleton.ResetBones(c, bones);


			List<UnityAnimation.UnityAnimationTrack> anims = new List<UnityAnimation.UnityAnimationTrack>();
			foreach (SubAnim_Template sat in subAnimInfo.animations) {
				if (sat.anim == null) continue;
				anims.Add(new UnityAnimation.UnityAnimationTrack() {
					ID = sat.friendlyName,
					Path = sat.name,
					Track = sat.anim,
					SubAnim = sat
				});
			}
			foreach (SubAnim_Template sat in tpl.animSet.animations) {
				if(sat.anim == null) continue;
				if(anims.Any(a => a.ID == sat.friendlyName)) continue;
				anims.Add(new UnityAnimation.UnityAnimationTrack() {
					ID = sat.friendlyName,
					Path = sat.name,
					Track = sat.anim,
					SubAnim = sat
				});
			}
			if (tpl?.animSet?.animPackage?.animPathAABB != null) {
				foreach (var animPath in tpl?.animSet?.animPackage?.animPathAABB) {
					if(animPath?.anim == null) continue;
					var existingAnims = anims.Where(a => a.Track == animPath.anim);
					foreach (var a in existingAnims) {
						a.AABB = animPath.aabb;
					}
					if (existingAnims.Any()) continue;
					anims.Add(new UnityAnimation.UnityAnimationTrack() {
						ID = animPath.name,
						Path = animPath.path,
						Track = animPath.anim,
						AABB = animPath.aabb,
						SubAnim = null
					});
				}
			}
			ua.anims = anims.ToArray();
			for (int i = 0; i < ua.anims.Length; i++) {
				ua.anims[i].Index = i;
			}
			// Set default animation if possible
			if (ua.anims.Length > 0) {
				ua.animIndex = 0;
			}
			var defaultAnim = this.defaultAnim;
			if(defaultAnim == null || defaultAnim.IsNull) defaultAnim = tpl.defaultAnimation;
			if (defaultAnim != null && !defaultAnim.IsNull) {
				var newAnimIndex = ua.anims.FindItemIndex(anm => anm.ID == defaultAnim);
				if(newAnimIndex != -1) ua.animIndex = newAnimIndex;
			}
			ua.Init();

		}

		private Mesh CreateMesh(TextureCooked tex) {
			Mesh meshUnity = new Mesh();
			/* CArray<Vector2> uvsArr = null;
			if (spriteIndex != 0xFFFFFFFF && tex.atlas != null) {
				if (tex.atlas.uvData.ContainsKey((int)spriteIndex)) {
					uvsArr = tex.atlas.uvData[(int)spriteIndex].uvs;
					MapLoader.Loader.print("Texture path with UV count:" + material.textureSet.diffuse + " - " + uvsArr.Count);
				}
			}*/
			Vector3[] vertices = new Vector3[4];
			vertices[0] = new Vector3(-0.5f, -0.5f, 0f);
			vertices[1] = new Vector3(-0.5f, 0.5f, 0f);
			vertices[2] = new Vector3(0.5f, -0.5f, 0f);
			vertices[3] = new Vector3(0.5f, 0.5f, 0f);
			Vector3[] normals = new Vector3[4];
			normals[0] = Vector3.forward;
			normals[1] = Vector3.forward;
			normals[2] = Vector3.forward;
			normals[3] = Vector3.forward;
			Vector3[] uvs = new Vector3[4];
			uvs[0] = new Vector3(0, 1);
			uvs[1] = new Vector3(0, 0);
			uvs[2] = new Vector3(1, 1);
			uvs[3] = new Vector3(1, 0);
			int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
			meshUnity.vertices = vertices;
			meshUnity.normals = normals;
			meshUnity.triangles = triangles;
			meshUnity.SetUVs(0, uvs.ToList());
			//meshUnity.SetUVs(4, Enumerable.Repeat(Vector4.one, 4).ToList());
			return meshUnity;
		}


		public void SetPatchMaterial(Renderer renderer, UnityAnimation.UnityPatchBank bank) {
			Material tex_mat = GFXMaterialShader_Template.GetShaderMaterial(shader: shader?.obj);
			renderer.sharedMaterial = tex_mat;
			FillMaterialParams(renderer);
			if (UbiArtContext.Settings.EngineVersion > EngineVersion.RO) {
				SetMaterialTextures(bank?.TextureBankPath.textureSet, renderer);
				FillUnityMaterialPropertyBlock(renderer, shader: bank?.TextureBankPath?.shader?.obj);
			}
		}

		public void FillUnityMaterialPropertyBlock(Renderer r, int index = 0, GFXMaterialShader_Template shader = null) {

			if (r == null) return;
			if (shader == null) shader = (this.shader != null ? this.shader.obj : null);
			if (mpb == null) mpb = new MaterialPropertyBlock();
			if (shader == null) {
				r.GetPropertyBlock(mpb, index);
				mpb.SetVector("_ShaderParams", new Vector4(1,0,0,0));
				mpb.SetVector("_ShaderParams2", new Vector4(0,2,0,0));
			} else {
				r.GetPropertyBlock(mpb, index);
				mpb.SetVector("_ShaderParams", new Vector4(
					shader.renderRegular ? 1f : 0f,
					shader.renderFrontLight ? 1f : 0f,
					shader.renderBackLight ? 1f : 0f,
					0f));
				mpb.SetVector("_ShaderParams2", new Vector4(
					(int)shader.materialtype2,
					(int)shader.blendmode,
					(int)shader.lightingType,
					0f));
			}

			// Set property block
			r.SetPropertyBlock(mpb, index);
		}
	}
}
