
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbiArt;
using UbiArt.Animation;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnityAnimation : MonoBehaviour {
	public class UnityPatchBank {
		public StringID ID { get; set; }
		public StringID TextureID { get; set; }

		public AnimPatchBank PBK { get; set; }
		public UnityAnimTemplate[] Templates { get; set; }
		public TextureBankPath TextureBankPath { get; set; }
		public Path TexturePathOrigins { get; set; }

		public void ResetPatches(UnityAnimation unityAnimation) {
			if (Templates == null || Templates.Length != PBK.templates.Count) {
				if (Templates != null) {
					foreach (var p in Templates) {
						if (p?.Object != null)
							Destroy(p.Object);
					}
				}
				Templates = new UnityAnimation.UnityAnimTemplate[PBK.templates.Count];
			}

			for (int i = 0; i < PBK.templates.Count; i++) {
				ResetSinglePatch(i, unityAnimation);
			}
		}

		public void ResetSinglePatch(int i, UnityAnimation unityAnimation) {
			if (Templates != null && i < Templates.Length) {
				var p = Templates[i];
				if (p?.Object != null)
					Destroy(p.Object);

				//skeleton.ResetBones(Controller.MainContext, bones);
				AnimTemplate at = PBK.templates[i];
				Templates[i] = new UnityAnimation.UnityAnimTemplate() {
					Template = at
				};

				var gao = unityAnimation.templatesGao;
				GameObject patch_gao = new GameObject($"Anim Bank {(ID ?? TextureID)?.ToString(PBK.UbiArtContext, shortString: true)} - {i} [ {PBK.templateKeys.GetKey(i).ToString(PBK.UbiArtContext, shortString: true)} ]");
				patch_gao.transform.SetParent(gao.transform, false);
				patch_gao.transform.localPosition = Vector3.zero;
				patch_gao.transform.localRotation = Quaternion.identity;
				patch_gao.transform.localScale = Vector3.one;

				UnityPatchRenderer pr = patch_gao.AddComponent<UnityPatchRenderer>();
				pr.Skeleton = unityAnimation.skeleton;
				pr.Bones = unityAnimation.bones;
				pr.AnimTemplate = at;

				Templates[i].Object = patch_gao;
				Templates[i].PatchRenderer = pr;

				pr.Init(hLevel: (int)unityAnimation.PatchHLevel, vLevel: (int)unityAnimation.PatchVLevel);
				pr.ProcessRenderers(mr => unityAnimation.alc.SetPatchMaterial(mr, this));
				pr.FillUnityMaterialPropertyBlock();
			}
		}
	}
	public class UnityAnimTemplate {
		public AnimTemplate Template { get; set; }
		public GameObject Object { get; set; }
		public bool Active { get; set; }
		public int IndexInBML { get; set; }
		public UnityPatchRenderer PatchRenderer { get; set; }
	}
	public class UnityAnimationTrack {
		public StringID ID { get; set; }
		public Path Path { get; set; }
		public AnimTrack Track { get; set; }
		public SubAnim_Template SubAnim { get; set; }
		public Bounds Bounds { get; set; }
		public AABB AABB { get; set; }
		public int Index { get; set; }

		public Bounds GetBounds() {
			if (Bounds == default) {
				if (AABB == null) AABB = new AABB() { MIN = Vec2d.One * -10, MAX = Vec2d.One * 10 }; // Default AABB
				var center = (AABB.MIN + AABB.MAX) / 2f;
				var size = (AABB.MAX - AABB.MIN);
				Bounds = new Bounds(center.GetUnityVector(), new Vector3(size.x, size.y, 1f) * 2);
				Bounds.Expand(10f);
			}
			return Bounds;
		}

		public string ToString(bool single) {
			if (single) {
				return Path.GetFilenameWithoutExtension(removeCooked: true);
			} else {
				return $"{ID?.ToString(Track?.UbiArtContext, shortString: true)} - {Path.GetFilenameWithoutExtension(removeCooked: true)}";
			}
		}

		public class PathComparer : IEqualityComparer<UnityAnimationTrack> {
			public bool Equals(UnityAnimationTrack x, UnityAnimationTrack y) => x.Path == y.Path;
			public int GetHashCode(UnityAnimationTrack obj) => obj.Path.GetHashCode();
		}
	}
	public AnimTrack animTrack => Animation?.Track;
	public UnityAnimationTrack Animation {
		get {
			if (anims != null && animIndex >= 0 && animIndex < anims.Length) {
				return anims[animIndex];
			}
			return null;
		}
	}
	public int animIndex = -1;
	public float currentBMLFrame = -1;
	public UnityAnimationTrack[] anims;
	public AnimSkeleton skeleton;
	public UnityBone[] bones;
	public UnityPatchBank[] AllPatchBanks;
	public Dictionary<StringID, UnityPatchBank> patchBanks; // Index: bank ID
	public Dictionary<StringID, UnityPatchBank> patchBanksOrigins; // Index: texture path StringID
	public AnimLightComponent alc;
	public AnimLightComponent_Template alc_tpl;
	public bool playAnimation = true;
	public bool DisplayPolylines;
	public bool DisplayInactivePolylines;
	public bool DisplayBones;
	public float animationSpeed = 60f;
	public int zValue = 0;
	bool loaded = false;
	private Dictionary<StringID, LineRenderer> lines;
	public GameObject linesGao;
	public GameObject skeletonGao;
	public GameObject templatesGao;
	public uint PatchHLevel { get; set; } = 6;
	public uint PatchVLevel { get; set; } = 6;
	public bool ForceUpdatePatches { get; set; } = false;
	public bool PlayFullAnimation = true;
	public UnityAnimationTrack[] animsFull;

	//private float updateCounter = 0f;
	public float currentFrame = 0;

	public void Start() {
	}
	public void Init(float time = 0) {
		if (animsFull == null) {
			animsFull = anims.Distinct(new UnityAnimationTrack.PathComparer())
				//.OrderBy(o => o.Path?.filename ?? "")
				.ToArray();
		}
		Context l = Controller.MainContext;
		if (animIndex >= 0 && skeleton != null) {
			currentFrame = time;
			currentBMLFrame = -1;
			skeleton.ResetBones(l, bones);
			InitLines();
			//FixCircularInterpolation(animTrack);
			//WarnCircularInterpolation(animTrack);
		}
		loaded = true;
		if (animIndex >= 0) {
			UpdateAnimation(force: true);
		}
	}

	public void ResetPatches(Predicate<UnityPatchBank> pbkFilter = null, Predicate<AnimTemplate> templateFilter = null) {
		foreach (var pbk in AllPatchBanks) {
			if (templateFilter == null) {
				if (pbkFilter == null || pbkFilter(pbk))
					pbk.ResetPatches(this);
			} else {
				if (pbk.Templates != null) {
					for (int i = 0; i < pbk.Templates.Length; i++) {
						if(templateFilter(pbk.Templates[i].Template))
							pbk.ResetSinglePatch(i, this);
					}
				}
			}
		}
		// Resetting no longer changes bone positions 
		/*if (!(animIndex >= 0 && skeleton != null)) {
			skeleton.ResetBones(skeleton.UbiArtContext, bones);
		}*/
		Init(currentFrame);
		/*if (bones != null) {
			skeleton.UpdateBones(bones);
		}*/
	}
	/*public void ForceUpdatePatches(Predicate<UnityPatchBank> pbkFilter = null, Predicate<AnimTemplate> templateFilter = null) {
		if (Controller.Obj.playAnimations) return; // Happens automatically in this case
		foreach (var pbk in AllPatchBanks) {
			if (pbkFilter == null || pbkFilter(pbk)) {
				foreach (var p in pbk.Templates) {
					if (templateFilter == null || templateFilter(p.Template)) {
						// force update here and now
						if (p.PatchRenderer == null) continue;
						if (p.Active) {
							p.PatchRenderer.UpdateBuffer();
						}
					}
				}
			}
		}
	}*/

	void WarnCircularInterpolation(AnimTrack anim) {
		if (anim == null) return;
		for(int bi = 0; bi < anim.bonesLists.Count; bi++) {
			var bl = anim.bonesLists[bi];
			if (bl.amountPAS == 0) continue;

			// First pass: front to back
			Angle lastAngle = new Angle();
			int lastPasFrame = -1;
			for (int i = 0; i < bl.amountPAS; i++) {
				var pas = anim.bonePAS[bl.startPAS + i];
				var curAngle = new Angle(pas.Rotation * anim.multiplierA);
				if (pas.frame == lastPasFrame + 1) {
					lastAngle = curAngle;
					continue;
				}
				lastPasFrame = pas.frame;
				float curAngleFloat = curAngle.EulerAngle;
				float lastAngleFloat = lastAngle.EulerAngle;
				if (curAngleFloat == lastAngleFloat) continue;
				while (curAngleFloat - lastAngleFloat > 180) curAngleFloat -= 360f;
				while (curAngleFloat - lastAngleFloat <= -180) curAngleFloat += 360f;

				if (curAngleFloat != curAngle.EulerAngle) {
					UnityEngine.Debug.Log($"Circular interpolation: Bone {bi} - PAS {bl.startPAS + i} - Frame {pas.frame}");
					curAngle = new Angle(curAngleFloat, degrees: true);
					//pas.Rotation = curAngle / anim.multiplierA;
				}
				lastAngle = curAngle;
			}
		}
	}

	void FixCircularInterpolation(AnimTrack anim) {
		if(anim?.bonePAS == null) return;
		Angle[] angles = new Angle[anim.bonePAS.Count];
		bool madeChanges = false;

		for (int i = 0; i < anim.bonePAS.Count; i++) {
			var pas = anim.bonePAS[i];
			angles[i] = new Angle(pas.Rotation * anim.multiplierA);
		}

		foreach (var bl in anim.bonesLists) {
			if (bl.amountPAS == 0) continue;

			// First pass: front to back
			Angle lastAngle = new Angle();
			int lastPasFrame = -1;
			for (int pass = 0; pass < 2; pass++) {
				for (int i = 0; i < bl.amountPAS; i++) {
					var index = bl.startPAS + i;

					var pas = anim.bonePAS[index];
					var curAngle = angles[index];
					if (pas.frame == lastPasFrame + 1) {
						lastAngle = curAngle;
						continue;
					}
					lastPasFrame = pas.frame;
					float curAngleFloat = curAngle.EulerAngle;
					float lastAngleFloat = lastAngle.EulerAngle;
					if (curAngleFloat == lastAngleFloat) continue;
					while (curAngleFloat - lastAngleFloat > 180) curAngleFloat -= 360f;
					while (curAngleFloat - lastAngleFloat <= -180) curAngleFloat += 360f;

					if (curAngleFloat != curAngle.EulerAngle) {
						madeChanges = true;
						curAngle = new Angle(curAngleFloat, degrees: true);
						angles[index] = curAngle;
					}
					lastAngle = curAngle;
				}
			}

			// Second pass: back to front
			/*var lastPAS = anim.bonePAS[bl.startPAS];
			lastAngle = new Angle(lastPAS.Rotation * anim.multiplierA);
			lastPasFrame = -1;
			for (int i = 0; i < bl.amountPAS; i++) {
				var pas = anim.bonePAS[bl.startPAS + bl.amountPAS - 1 - i];
				var curAngle = new Angle(pas.Rotation * anim.multiplierA);
				if (pas.frame == lastPasFrame - 1) {
					break;
				}
				lastPasFrame = pas.frame;
				float curAngleFloat = curAngle.EulerAngle;
				float lastAngleFloat = lastAngle.EulerAngle;
				if (curAngleFloat == lastAngleFloat) continue;
				while (curAngleFloat - lastAngleFloat > 180) curAngleFloat -= 360f;
				while (curAngleFloat - lastAngleFloat <= -180) curAngleFloat += 360f;

				if (curAngleFloat != curAngle.EulerAngle) {
					curAngle = new Angle(curAngleFloat, degrees: true);
					pas.Rotation = curAngle / anim.multiplierA;
				}
				lastAngle = curAngle;
			}*/
		}
		if (madeChanges) {
			anim.multiplierA = angles.Max(a => MathF.Abs(a)) + 0.5f;
			for (int i = 0; i < anim.bonePAS.Count; i++) {
				var pas = anim.bonePAS[i];
				pas.Rotation = angles[i] / anim.multiplierA;
			}
		}
	}

	public void InitSubObjects() {
		if (skeletonGao == null) {
			skeletonGao = new GameObject("Skeleton");
			skeletonGao.transform.SetParent(transform, false);
			skeletonGao.transform.localPosition = Vector3.zero;
			skeletonGao.transform.localScale = Vector3.one;
			skeletonGao.transform.localRotation = Quaternion.identity;
		}
		if (templatesGao == null) {
			templatesGao = new GameObject("Templates");
			templatesGao.transform.SetParent(transform, false);
			templatesGao.transform.localPosition = Vector3.zero;
			templatesGao.transform.localScale = Vector3.one;
			templatesGao.transform.localRotation = Quaternion.identity;
		}
	}

	public void InitLines() {
		if(linesGao == null) linesGao = new GameObject("Polylines");
		linesGao.SetActive(DisplayPolylines);
		linesGao.transform.SetParent(transform, false);
		linesGao.transform.localPosition = Vector3.zero;
		linesGao.transform.localScale = Vector3.one;
		linesGao.transform.localRotation = Quaternion.identity;

		if(lines == null) lines = new Dictionary<StringID, LineRenderer>();
		if (lines.Any()) {
			foreach (var l in lines) {
				Destroy(l.Value.gameObject);
			}
		}
		lines.Clear();
		if ((skeleton?.bank?.value?.polylines?.Count ?? 0) > 0) {
			foreach (var l in skeleton?.bank?.value?.polylines) {
				var lr = new GameObject($"Polyline {l.name}").AddComponent<LineRenderer>();
				lr.material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
				lr.positionCount = l.points.Count;
				lr.loop = l.loop;

				lr.useWorldSpace = false;
				lr.widthMultiplier = 0.03f;
				lr.sortingLayerName = "Gizmo-Line";
				var color = UnityEngine.Color.green;
				lr.material.color = color;
				lr.startColor = color;
				lr.endColor = color;

				lr.transform.SetParent(linesGao.transform, false);
				lr.transform.localPosition = Vector3.zero;
				lr.transform.localRotation = Quaternion.identity;
				lr.transform.localScale = Vector3.one;
				lr.transform.gameObject.SetActive(false);
				lines[l.name] = lr;
			}
		}
	}
	public void UpdateLines() {
		if(linesGao != null) linesGao.SetActive(DisplayPolylines);
		if (DisplayPolylines) {
			if((skeleton?.bank?.value?.polylines?.Count ?? 0) > 0) {
				AnimTrackPolyline lastPolyLineList = null;
				if (animTrack?.polylines?.Count > 0) {
					lastPolyLineList = animTrack.polylines.LastOrDefault(p => p.frame <= currentFrame);
				}
				foreach (var l in lines) {
					var lr = l.Value;
					var gao = lr.gameObject;
					if (lastPolyLineList?.entries.Contains(l.Key) ?? false) {
						var color = UnityEngine.Color.green;
						lr.material.color = color;
						lr.startColor = color;
						lr.endColor = color;
						if (gao.activeSelf != true) gao.SetActive(true);
					} else {
						if (DisplayInactivePolylines) {
							var color = UnityEngine.Color.red;
							lr.material.color = color;
							lr.startColor = color;
							lr.endColor = color;
						}
						if (gao.activeSelf != DisplayInactivePolylines) gao.SetActive(DisplayInactivePolylines);
					}
					if (DisplayInactivePolylines || (lastPolyLineList?.entries.Contains(l.Key) ?? false)) {
						var polyline = skeleton?.bank?.value?.polylines.FirstOrDefault(pl => pl.name == l.Key);

						var positions = polyline.points.Select(point => {
							var boneName = point.name;
							var boneIndex = skeleton.GetBoneIndexFromTag2(boneName);
							if (boneIndex != -1) {
								var bone = bones[boneIndex];
								var pos = point.pos.GetUnityVector();
								return transform.InverseTransformPoint(bone.transform.TransformPoint(pos));
							}
							return Vector3.zero;
						});
						lr.SetPositions(positions.ToArray());
						lr.numCornerVertices = 5;
					}
				}
			}
		}
	}

	public void Update() {
		if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished && loaded && Controller.Obj.playAnimations) {
			if (animTrack != null) {
				if (playAnimation) currentFrame += Time.deltaTime * animationSpeed * (Animation?.SubAnim?.playRate ?? 1f);
			} else {
				currentFrame = 0f;
			}
			UpdateAnimation();
		} else if (ForceUpdatePatches) {
			UpdatePatches();
		}
		ForceUpdatePatches = false;
	}

	void UpdateAnimation(bool force = false) {
		if(!loaded || skeleton == null || bones == null) return;
		Context context = Controller.MainContext;

		foreach (var b in bones) {
			b.visualize = DisplayBones;
		}
		if (animTrack != null) {
			var subAnim = Animation.SubAnim;
			if (animTrack.length == 0) {
				currentFrame = 0;
			} else if(!PlayFullAnimation
				&& !(subAnim?.markerStart?.IsNull ?? true) && !(subAnim?.markerStop?.IsNull ?? true)) {
				// Play between markers only
				var start = subAnim.markerStart;
				var stop = subAnim.markerStop;
				var startFrame = animTrack.frameEvents?.FirstOrDefault(e => e.events?.Any(e => e.type == AnimTrackFrameEvents.AnimMarkerEvent.AnimMarkerEventType.AnimAnimationEvent && e.marker == start) ?? false)?.frame;
				var stopFrame = animTrack.frameEvents?.FirstOrDefault(e => e.events?.Any(e => e.type == AnimTrackFrameEvents.AnimMarkerEvent.AnimMarkerEventType.AnimAnimationEvent && e.marker == stop) ?? false)?.frame;
				if (startFrame.HasValue && stopFrame.HasValue) {
					var duration = stopFrame.Value - startFrame.Value;
					if (duration <= 0) {
						currentFrame = startFrame.Value;
					} else {
						var curFrameRel = currentFrame - startFrame.Value;
						if (curFrameRel < 0) curFrameRel = 0;
						curFrameRel %= duration;
						currentFrame = startFrame.Value + curFrameRel;
					}
				} else if (currentFrame >= animTrack.length) {
					currentFrame %= animTrack.length;
				}
			} else if (currentFrame >= animTrack.length) {
				currentFrame %= animTrack.length;
			}
			var bounds = Animation.GetBounds();
			bool isVisible = IsVisible(bounds);
			if (isVisible || force) {
				UpdateBonePASZAL();

				// Activate the correct patches
				AnimTrackBML bml = GetCurrentBML();
				UpdateBML(bml, context);
				UpdateBoneLength();
				UpdateBones();
				UpdatePatches(isVisible: isVisible);
				UpdateLines();

				// Configure Z for all patches
				/*if (bml == null) {
					bml = animTrack.bml.ToList().FindLast(b => b.frame == currentBMLFrame);
				}*/
				if (bml != null) {
					ZSortBones();
				}
			} else {
			}
		} else {
			// Bind pose
			foreach (var b in bones) {
				b.localPosition = Vector3.zero;
				b.localScale = Vector3.one;
				b.localRotation = 0;
			}

			UpdateBoneLength();
			UpdateBones();
			UpdatePatches();
			UpdateLines();
			ZSortBones();
		}
	}

	private void UpdateBonePASZAL() {
		int numBones = Math.Min(animTrack.bonesLists.Count, bones.Length);
		int rootIndex = skeleton.GetBoneIndexFromTag(new StringID("Root"));
		bool useRoot = (alc_tpl?.useRootBone) ?? true;
		bool useRootRotation = Animation?.SubAnim?.useRootRotation ?? true;
		bool defaultFlip = Animation?.SubAnim?.defaultFlip ?? false;

		for (int i = 0; i < numBones; i++) {
			bool isRoot = i == rootIndex;
			/*if (((!alc_tpl?.useRootBone) ?? false) && isRoot) {
				bones[i].localPosition = Vector3.zero;
				bones[i].localScale = Vector3.one;
				bones[i].localRotation = 0f;
			} else {*/
			AnimTrackBonesList bl = animTrack.bonesLists[i];
			if (bl.amountPAS > 0) { // Position Angle Scale
				for (int p = 0; p < bl.amountPAS; p++) {
					AnimTrackBonePAS pas = animTrack.bonePAS[bl.startPAS + p];
					AnimTrackBonePAS next = p < bl.amountPAS - 1 ? animTrack.bonePAS[bl.startPAS + ((p + 1) % bl.amountPAS)] : null; // Don't interpolate with start frame in loops
					if (p == bl.amountPAS - 1 || (currentFrame >= pas.frame && currentFrame < next.frame)) {
						Vector2 pos = pas.Position.GetUnityVector();
						Angle rot = pas.Rotation;
						Vector2 scl = pas.Scale.GetUnityVector();
						if (next != null && next != pas) {
							float nextFrame = next.frame < pas.frame ? next.frame + animTrack.length : next.frame;
							float lerp = (Mathf.Floor(currentFrame) - pas.frame) / (Mathf.Floor(nextFrame) - pas.frame); // TODO: maybe change to Math.Floor(currentFrame) if animations can't be interpolated. This fixed jittery feet for Rayman
							pos = Vector2.Lerp(pos, next.Position.GetUnityVector(), lerp);
							//rot = Mathf.LerpAngle(rot * animTrack.multiplierA, next.Rotation * animTrack.multiplierA, lerp) / animTrack.multiplierA;
							rot = Mathf.Lerp(rot, next.Rotation, lerp);
							scl = Vector2.Lerp(scl, next.Scale.GetUnityVector(), lerp);
						}
						pos *= animTrack.multiplierP;
						rot *= animTrack.multiplierA;
						scl *= animTrack.multiplierS;

						if (isRoot) {
							if (!useRoot) {
								pos = Vector2.zero;
								scl = Vector2.one;
								if (!useRootRotation) rot = 0f;
							}
							if (defaultFlip) scl = new Vector2(-scl.x, scl.y);
						}

						bones[i].localPosition = pos;
						bones[i].localScale = scl;
						bones[i].localRotation = rot;
						break;
					}
				}
				if (bl.amountZAL > 0) { // Z ALpha
					for (int p = 0; p < bl.amountZAL; p++) {
						AnimTrackBoneZAL zal = animTrack.boneZAL[bl.startZAL + p];
						AnimTrackBoneZAL next = p < bl.amountZAL - 1 ? animTrack.boneZAL[bl.startZAL + ((p + 1) % bl.amountZAL)] : null; // Don't interpolate with start frame
						if (p == bl.amountZAL - 1 || (currentFrame >= zal.frame && currentFrame < next.frame)) {
							float z = zal.z;
							float alpha = zal.alpha / 255f;
							if (next != null && next != zal) {
								float nextFrame = next.frame < zal.frame ? next.frame + animTrack.length : next.frame;
								float lerp = (currentFrame - zal.frame) / (nextFrame - zal.frame);
								z = Mathf.Lerp(z, next.z, lerp);
								alpha = Mathf.Lerp(alpha, next.alpha / 255f, lerp);
							}
							bones[i].localZ = z;
							bones[i].localAlpha = alpha;
							break;
						}
					}
				}
			}
		}
	}

	private AnimTrackBML GetCurrentBML() {
		AnimTrackBML bml = null;
		if (animTrack.bml.Count > 0) {
			// Reset BML at end of animation - except if current BML frame is the first frame
			if (currentBMLFrame > currentFrame && currentBMLFrame != animTrack.bml[0].frame) {
				currentBMLFrame = -1;
			}
			// Find last index that matches current BML
			int currentBMLIndex = currentBMLFrame == -1 ? 0 : (animTrack.bml.ToList().FindLastIndex(b => b.frame == currentBMLFrame) % animTrack.bml.Count);

			for (int i = currentBMLIndex; i < animTrack.bml.Count; i++) {
				AnimTrackBML currentBML = animTrack.bml[i];
				if (currentBML.frame > currentFrame) break;
				//if ((curB.frame > currentFrame) && !(checkHigher && curB.frame > lastBmlFrame)) break;
				bml = currentBML;
			}
		}
		return bml;
	}
	private void UpdateBML(AnimTrackBML bml, Context context) {
		if (bml != null && bml.frame != currentBMLFrame) {
			currentBMLFrame = bml.frame;
			foreach (var pbk in AllPatchBanks) {
				foreach (var p in pbk.Templates) p.Active = false;
			}
			int indexInBML = 0;
			foreach (AnimTrackBML.Entry entry in bml.entries) {
				StringID templateId = entry.templateId;
				var bank = LookupTextureBankId(entry.textureBankId);
				if (bank == null) continue;
				int ind = bank.PBK.templateKeys.GetKeyIndex(templateId);
				if (ind != -1) {
					bank.Templates[ind].IndexInBML = indexInBML;
					bank.Templates[ind].Active = true;
					if (context.Settings.EngineVersion == EngineVersion.RO) {
						var texPath = GetTexturePathOrigins(entry.textureBankId);
						if (texPath != null) {
							if (context.Loader.tex.ContainsKey(texPath.stringID)) {
								bank.Templates[ind].PatchRenderer.ProcessRenderers(mr => {
									alc.FillUnityMaterialPropertyBlock(mr);
									alc.SetMaterialTextureOrigins((TextureCooked)context.Loader.tex[texPath.stringID], mr);
								});
							}
						}
					}
				}
				indexInBML++;
			}
			foreach (var pbk in AllPatchBanks) {
				foreach (var p in pbk.Templates) {
					if (p?.Object == null) continue;
					p.Object.SetActive(p.Active);
				}
			}
		} else if (bml == null) {
			foreach (var pbk in AllPatchBanks) {
				foreach (var p in pbk.Templates) {
					p.Active = false;
					if (p?.Object == null) continue;
					p.Object.SetActive(p.Active);
				}
			}
		}
	}

	private void UpdateBones() {
		var updateOrder = skeleton.GetBonesUpdateOrder();
		foreach (var boneIndex in updateOrder) {
			bones[boneIndex].UpdateBone(controlTransform: true, updateRecursive: false);
		}
	}

	private void UpdatePatches(bool isVisible = true) {
		foreach (var pbk in AllPatchBanks) {
			foreach (var p in pbk.Templates) {
				if(p?.PatchRenderer == null) continue;
				if (p.Active) {
					p.PatchRenderer.UpdateBuffer();
					if(isVisible) p.PatchRenderer.UpdateCollider();
				}
			}
		}
	}

	private void UpdateBoneLength() {
		var context = Controller.MainContext;
		if (context.Settings.EngineVersion <= EngineVersion.RO) {
			for (int i = 0; i < bones.Length; i++) {
				AnimBoneDyn selectedDyn = null;
				var b = bones[i];
				foreach (var pbk in AllPatchBanks) {
					foreach (var p in pbk.Templates) {
						if (p.Active) {
							var atl = p.Template;
							for (int j = 0; j < atl.bones.Count; j++) {
								var boneIndex = skeleton.GetBoneIndexFromTag(atl.bones[j].tag);
								if (boneIndex == i) {
									selectedDyn = atl.bonesDyn[j];
								}
								if (selectedDyn != null) break;
							}
						}
						if (selectedDyn != null) break;
					}
					if (selectedDyn != null) break;
				}
				if (selectedDyn == null) selectedDyn = skeleton.bonesDyn[i];
				//b.xScaleMultiplier = selectedDyn.boneLength;
				b.boneLength = selectedDyn.boneLength;
			}
		}
	}

	private void ZSortBones() {
		ZListManager zman = Controller.Obj.zListManager;
		var activePatches = AllPatchBanks
			.SelectMany(pbk => pbk.Templates.Where(p => p.PatchRenderer != null && p.Active))
			.OrderBy(p => p.IndexInBML)
			.SelectMany(p => p.PatchRenderer.Patches)
			.OrderBy(p => p.Z);
		// Back to front
		int i = 0;
		foreach (var patch in activePatches) {
			if(patch?.Renderer == null) continue;
			alc.FillMaterialParams(patch.Renderer, alpha: patch.Alpha);
			zman.zDict[patch.Renderer] = transform.position.z - (i / 10000f);
			if (Animation != null) {
				patch.Renderer.localBounds = Animation.GetBounds();
			}
			i++;
		}
		var inactivePatches = AllPatchBanks
			.SelectMany(pbk => pbk.Templates.Where(p => p.PatchRenderer != null && !p.Active))
			.SelectMany(p => p.PatchRenderer.Patches);
		foreach (var patch in inactivePatches) {
			if (patch?.Renderer == null) continue;
			if (zman.zDict.ContainsKey(patch.Renderer)) {
				zman.zDict.Remove(patch.Renderer);
			}
		}
	}

	UnityPatchBank LookupTextureBankId(StringID id) {
		if(patchBanks.ContainsKey(id)) return patchBanks[id];

		// Origins specific
		Context l = Controller.MainContext;
		if (l.Settings.EngineVersion <= EngineVersion.RO) {
			int texInd = animTrack.texturePathKeysOrigins.GetKeyIndex(id);
			if (texInd != -1) {
				pair<StringID, CString> texPath = animTrack.texturePathsOrigins[texInd];
				var key = texPath.Item1;
				if(patchBanksOrigins.ContainsKey(key)) return patchBanksOrigins[key];
			}
		}
		return null;
	}
	Path GetTexturePathOrigins(StringID id) {
		if (patchBanks.ContainsKey(id)) return patchBanks[id].TexturePathOrigins;

		// Origins specific
		int texInd = animTrack.texturePathKeysOrigins.GetKeyIndex(id);
		if (texInd != -1) {
			pair<StringID, CString> texPath = animTrack.texturePathsOrigins[texInd];
			return new Path(texPath.Item2.str);
		}
		return null;
	}

	public bool IsVisible(Bounds bounds) {
		bool isVisible = false;
		bounds = TransformBounds(bounds);

		// Game View
		Camera gameCam = Camera.main;
		if (gameCam != null) {
			Plane[] gamePlanes = GeometryUtility.CalculateFrustumPlanes(gameCam);
			if (GeometryUtility.TestPlanesAABB(gamePlanes, bounds)) {
				isVisible = true;
				//Debug.Log("Visible: game view");
			}
		}

#if UNITY_EDITOR
		// Scene View
		SceneView sceneView = SceneView.lastActiveSceneView;
		if (sceneView != null && sceneView.camera != null) {
			Plane[] scenePlanes = GeometryUtility.CalculateFrustumPlanes(sceneView.camera);
			if (GeometryUtility.TestPlanesAABB(scenePlanes, bounds)) {
				isVisible = true;
				//Debug.Log("Visible: scene view");
			}
		}
#endif
		return isVisible;
	}
	public Bounds TransformBounds(Bounds localBounds) {
		Vector3 center = transform.TransformPoint(localBounds.center);

		// Transform the extents to world space axes
		Vector3 extents = localBounds.extents;
		Vector3 axisX = transform.TransformVector(extents.x, 0, 0);
		Vector3 axisY = transform.TransformVector(0, extents.y, 0);
		Vector3 axisZ = transform.TransformVector(0, 0, extents.z);

		// Sum their absolute values to get world extents
		Vector3 worldExtents =
			new Vector3(
				Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
				Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
				Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z)
			);

		return new Bounds(center, worldExtents * 2);
	}
}