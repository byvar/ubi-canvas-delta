using UnityEngine;
using UbiArt;
using UbiArt.ITF;
using System.Linq;
using UbiCanvas.Helpers;

[ExecuteInEditMode]
public class UnityPickable : MonoBehaviour {
	public Pickable pickable;
	private SpriteRenderer sr;
	private SphereCollider sc;
	public Scene ContainingScene;
	private bool inited = false;

	[SerializeField]
	[HideInInspector]
	private int originalInstanceID = -1;

	private void Init() {
		inited = true;
		ProcessCopyPaste();
		CreateMesh();
		UpdateGizmo();
	}

	private async void ProcessCopyPaste() {
		if (originalInstanceID != -1) {
			// This is a copy made through CTRL+C
			Destroy(gameObject);
			
			var parentPickable = transform.parent?.GetComponentInParent<UnityPickable>(includeInactive: true);
			if (parentPickable != null) {
				if (parentPickable.pickable == null) {
					return;
				}
			}

			var allPickables = FindObjectsOfType<UnityPickable>(includeInactive: true);
			var original = allPickables.FirstOrDefault(p => p.GetInstanceID() == originalInstanceID);
			if(original == null) return;
			var newPickable = (Pickable)original.pickable.Clone("isc");

			UnityScene containingScene = null;
			if (transform.parent?.gameObject != null) {
				containingScene = transform.GetComponentInParent<UnityScene>(includeInactive: true);
			}
			if (containingScene?.scene == null) {
				containingScene = original.transform.GetComponentInParent<UnityScene>(includeInactive: true);
			}
			if (containingScene?.scene != null) {
				if (newPickable is Actor a) { // Frises are also actors
					containingScene.scene.AddActor(a);
					await a.SetGameObjectParent(containingScene.gameObject);
					await a.SetContainingScene(containingScene.scene);
#if UNITY_EDITOR
					UnityEditor.Selection.activeGameObject = a.GetPrecreatedGameObject();
#endif
				}
			}
		}
		originalInstanceID = GetInstanceID();
	}

	private void Update() {
		if (GlobalLoadState.LoadState != GlobalLoadState.State.Finished) return;
		if (!inited) Init();
		if (sr != null) {
			sr.enabled = Controller.Obj.displayGizmos;
			sc.enabled = sr.enabled;
			if (sr.enabled) {
				sr.size = new Vector2(
					transform.lossyScale.x != 0 ? (1f / transform.lossyScale.x) : 1f,
					transform.lossyScale.y != 0 ? (1f / transform.lossyScale.y) : 1f);
				sc.radius = sr.size.magnitude * 0.2f;
			}
		}
		UpdateDataFromTransform();
	}

	private bool IsRO => Controller.MainContext.Settings.EngineVersion <= EngineVersion.RO;

	public bool UpdateDataFromTransform() {
		bool updatedData = false;
		if (pickable != null && GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
			var z = pickable.RELATIVEZ;
			bool zForced = false;
			bool scaleForced = false;
			Vec2d scale = pickable.SCALE;
			if (IsRO) {
				if (pickable.templatePickable != null && pickable.templatePickable is Actor_Template atpl) {
					if (atpl.useZForced != 0) {
						zForced = true;
						z = atpl.zForced;
					}
					if (atpl.scaleForced != Vec2d.Zero) {
						scaleForced = true;
						scale = atpl.scaleForced;
					}
				} else if (pickable is Frise f && f.configOrigins != null) {
					if (f.configOrigins.isZForced != 0) {
						zForced = true;
						z = f.configOrigins.forcedZ;
					}
					scaleForced = true;
					scale = Vec2d.One;
				}
			}
			if (transform.localPosition != new Vector3(pickable.POS2D.x, pickable.POS2D.y, -z)) {
				pickable.POS2D = new Vec2d(transform.localPosition.x, transform.localPosition.y);

				if (z != -transform.localPosition.z) {
					if (!zForced) {
						pickable.RELATIVEZ = -transform.localPosition.z;
						Controller.Obj.zListManager.Sort(printMessages: false);
					} else {
						transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -z);
					}
				}
				updatedData = true;
			}
			var curAngle = pickable.ANGLE?.EulerAngle ?? 0;
			var angleDifference = NormalizeEulerAngle(transform.localEulerAngles.z) - NormalizeEulerAngle(curAngle);
			if (Mathf.Abs(angleDifference) > 0.00005) {
				pickable.ANGLE.EulerAngle = transform.localEulerAngles.z;
				updatedData = true;
			}
			var expectedScale = new Vector2((pickable.xFLIPPED ? -1f : 1f) * scale.x, scale.y);
			if (new Vector2(transform.localScale.x, transform.localScale.y) != expectedScale) {
				if (!scaleForced) {
					pickable.SCALE = new Vec2d((pickable.xFLIPPED ? -1f : 1f) * transform.localScale.x, transform.localScale.y);
					updatedData = true;
				} else {
					transform.localScale = new Vector3(expectedScale.x, expectedScale.y, 1f);
				}
			}
			if (transform.localScale.z != 1f) {
				transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
			}
			if (transform.localEulerAngles.x != 0f || transform.localEulerAngles.y != 0f) {
				transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
			}
			if (ContainingScene != null && transform.parent.gameObject != null) {
				var parentScene = transform.parent.gameObject.GetComponent<UnityScene>();
				if (parentScene != null && parentScene.scene != ContainingScene) {
					RemoveFromContainingScene();
					ContainingScene = parentScene.scene;
					AddToContainingScene();
					updatedData = true;
				}
			}
			if (pickable.USERFRIENDLY != gameObject.name) {
				var newName = gameObject.name;
				if (ContainingScene != null) {
					var objectWithName = ContainingScene.FindByName(newName);
					if (objectWithName == null || objectWithName == pickable) {
						pickable.USERFRIENDLY = newName;
					} else {
						UnityEngine.Debug.Log($"An object with the name {newName} already exists in this scene. Reverting name to {pickable.USERFRIENDLY}.");
						gameObject.name = pickable.USERFRIENDLY;
					}
				} else {
					pickable.USERFRIENDLY = newName;
				}
				updatedData = true;
			}
		}
		if(updatedData) Dirty = true;
		return updatedData;
	}

	public bool Dirty { get; set; }

	public float NormalizeEulerAngle(float angle, float targetAngle = 180) {
		while (angle < targetAngle - 180) angle += 360;
		while (angle >= targetAngle + 180) angle -= 360;
		return angle;
	}

	public void ResetTransformFromData() {
		var p = pickable;


		var z = p?.RELATIVEZ ?? 0;
		//bool zForced = false;
		//bool scaleForced = false;
		Vec2d scale = p?.SCALE ?? Vec2d.One;
		if (IsRO && p != null) {
			if (pickable.templatePickable != null && pickable.templatePickable is Actor_Template atpl) {
				if (atpl.useZForced != 0) {
					//zForced = true;
					z = atpl.zForced;
				}
				if (atpl.scaleForced != Vec2d.Zero) {
					//scaleForced = true;
					scale = atpl.scaleForced;
				}
			} else if (pickable is Frise f && f.configOrigins != null) {
				if (f.configOrigins.isZForced != 0) {
					//zForced = true;
					z = f.configOrigins.forcedZ;
				}
				//scaleForced = true;
				scale = Vec2d.One;
			}
		}


		transform.localPosition = new Vector3(p?.POS2D?.x ?? 0, p?.POS2D?.y ?? 0, -z);
		transform.localScale = new Vector3((p.xFLIPPED ? -1f : 1f) * scale.x, scale.y, 1f);
		transform.localEulerAngles = new Vector3(0, 0, p?.ANGLE?.EulerAngle ?? 0);
	}

	void UpdateGizmo(bool selected = false) {
		Sprite spr = null;
		if (pickable != null) {
			Controller c = Controller.Obj;
			TemplatePickable t = pickable.templatePickable;
			if (pickable is Frise) {
				if (t != null && t.TAGS != null) {
					foreach (string tag in t.TAGS) {
						Sprite sprLoc = c.GetIcon(tag, selected);
						if (sprLoc == null) {
							print($"Frieze {pickable.USERFRIENDLY}: Untreated tag " + tag);
						}
						if (spr == null)
							spr = sprLoc;
					}
				}
				if (spr == null) {
					spr = c.GetIcon("frieze", selected);
				}
			} else if (pickable is Actor a) {
				if (pickable is SubSceneActor ssa) {
					if (t != null && t.TAGS != null && t.TAGS.Count > 0) {
						foreach (string tag in t.TAGS) {
							Sprite sprLoc = c.GetIcon(tag, selected);
							if (sprLoc == null) {
								print($"SubSceneActor {a.USERFRIENDLY}: Untreated tag " + tag);
							}
							if (spr == null)
								spr = sprLoc;
						}
					}
					if (spr == null && !Path.IsNull(ssa.RELATIVEPATH)) {
						if (ssa.RELATIVEPATH.filename.EndsWith("_graph.isc")) {
							spr = c.GetIcon("isc_graph", selected);
						} else if (ssa.RELATIVEPATH.filename.EndsWith("_fx.isc")) {
							spr = c.GetIcon("isc_fx", selected);
						} else if (ssa.RELATIVEPATH.filename.EndsWith("_ld.isc")) {
							spr = c.GetIcon("isc_ld", selected);
						} else if (ssa.RELATIVEPATH.filename.EndsWith("_cine.isc")) {
							spr = c.GetIcon("isc_cine", selected);
						} else if (ssa.RELATIVEPATH.filename.EndsWith("_sound.isc")) {
							spr = c.GetIcon("isc_sound", selected);
						}
					}
					if (spr == null) {
						spr = c.GetIcon("isc", selected);
					}
				} else {
					if (t != null && t.TAGS != null && t.TAGS.Count > 0) {
						foreach (string tag in t.TAGS) {
							if(string.IsNullOrWhiteSpace(tag)) continue;
							Sprite sprLoc = c.GetIcon(tag, selected);
							if (sprLoc == null) {
								print($"Actor {a.USERFRIENDLY}: Untreated tag " + tag);
							}
							if(spr == null)
								spr = sprLoc;
						}
					}
					if (spr == null) {
						if (a.GetComponent<CameraModifierComponent>() != null) {
							spr = c.GetIcon("cameramodifier", selected);
						} else if (a.GetComponent<RelayEventComponent>() != null) {
							spr = c.GetIcon("relay", selected);
						}
					}
					if (spr == null) {
						spr = c.GetIcon("actor", selected);
					}
				}
			}
		}
		if (spr != null) {
			sr.sprite = spr;
#if UNITY_EDITOR
			var tex = Controller.Obj.GetTextureForIcon(spr);
			if (tex != null) {
				UnityEditor.EditorGUIUtility.SetIconForObject(gameObject, tex);
			}
#endif
		}
	}

	public void Deselect() {
		UpdateGizmo(false);
	}
	public void Select() {
		UpdateGizmo(true);
	}

	public void OnDestroy() {
		RemoveFromContainingScene();
	}

	void RemoveFromContainingScene() {
		if (ContainingScene != null && pickable != null) {
			ContainingScene.DeletePickable(pickable);
		}
	}

	void AddToContainingScene() {
		if (ContainingScene != null && pickable != null) {
			if (pickable is Actor a) { // Frises are also actors
				ContainingScene.AddActor(a);
			}
		}
	}

	void CreateMesh() {
		if (gameObject.GetComponent<SpriteRenderer>() != null) {
			sr = gameObject.GetComponent<SpriteRenderer>();
		} else {
			sr = gameObject.AddComponent<SpriteRenderer>();
		}
		sr.sortingLayerName = "Gizmo";
		sr.drawMode = SpriteDrawMode.Sliced;
		if (gameObject.GetComponent<SphereCollider>() != null) {
			sc = gameObject.GetComponent<SphereCollider>();
		} else {
			sc = gameObject.AddComponent<SphereCollider>();
		}
		sc.radius = 0.2f;
		//sr.sharedMaterial = new Material(Shader.Find("Custom/Gizmo"));
		/*mr.sharedMaterial = unityMat;
		Mesh meshUnity = new Mesh();
		Vector3[] vertices = new Vector3[4];
		vertices[0] = new Vector3(0, -1, -1);
		vertices[1] = new Vector3(0, -1, 1);
		vertices[2] = new Vector3(0, 1, -1);
		vertices[3] = new Vector3(0, 1, 1);
		Vector3[] normals = new Vector3[4];
		normals[0] = Vector3.forward;
		normals[1] = Vector3.forward;
		normals[2] = Vector3.forward;
		normals[3] = Vector3.forward;
		Vector2[] uvs = new Vector2[4];
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(1, 0);
		uvs[2] = new Vector2(0, 1);
		uvs[3] = new Vector2(1, 1);
		int[] triangles = new int[] { 0, 2, 1, 1, 2, 3 };

		meshUnity.vertices = vertices;
		meshUnity.normals = normals;
		meshUnity.triangles = triangles;
		meshUnity.uv = uvs;


		mf.mesh = meshUnity;*/
	}
}
