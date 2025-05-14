using UnityEngine;
using UbiArt;
using System.Collections.Generic;
using UbiArt.Animation;

public class UnityBone : MonoBehaviour {
	public Vector3 localPosition;
	public float localRotation;
	public Vector2 localScale = Vector2.one;
	public Vector3 bindPosition;
	public float bindRotation;
	public Vector2 bindScale = Vector2.one;
	public bool bind = false;
	public Vector3 globalPosition = Vector3.zero;
	public float globalAngle;
	public Vector2 computedScale = Vector2.one;
	public Vector2 globalScale = Vector2.one;
	public float boneLength = 0f;
	public float xScaleMultiplier = 1f;
	public float localZ = 0;
	public float bindZ = 0;
	public float bindAlpha = 1f;
	public float localAlpha = 0f;
	
	public bool visualize = false;
	public GameObject Visual { get; set; }

	public bool IsPBKEditor { get; set; }
	public AnimBoneDyn PBKBone { get; set; }


	public List<UnityBone> Children { get; set; } = new List<UnityBone>();
	private UnityBone _parent;
	public UnityBone Parent {
		get => _parent;
		set {
			if(_parent != null)
				_parent.Children.Remove(this);
			_parent = value;
			if(_parent != null)
				_parent.Children.Add(this);
		}
	}
	private void Start() {		
	}

	public void Reset(bool resetParent = true, bool update = true) {
		localPosition = Vector3.zero;
		localScale = Vector2.one;
		localRotation = 0; //new Angle(0);
		if (resetParent) Parent = null;
		if (update) UpdateBone();
	}
	void Update() {
		if (Controller.Obj.playAnimations && !IsPBKEditor)
			UpdateBone();
		UpdateVisual();
	}

	public void UpdateBone(bool controlTransform = true, bool updateRecursive = false) {
		var c = Controller.MainContext;

		Vector3 position;
		Vector3 scale;
		Quaternion rotation;

		if (bind) {
			if (Parent != null) {
				globalAngle = Parent.globalAngle + bindRotation + localRotation;
				float xPos = (Parent.computedScale.x) * (bindPosition.x + localPosition.x + Parent.boneLength);
				float yPos = -(Parent.computedScale.y) * (bindPosition.y + localPosition.y);
				var rotatedPos = new Vec2d(xPos, yPos).Rotate(Parent.globalAngle);
				globalPosition = Parent.globalPosition + new Vector3(rotatedPos.x, rotatedPos.y, 0f);
			} else {
				// Check ITF::AnimInfo::ComputeBonesFromLocalToWorld
				globalPosition = localPosition;
				globalAngle = bindRotation + localRotation;
			}
			computedScale = Vector2.Scale(localScale, bindScale);

			if (c.Settings.EngineVersion == EngineVersion.RO) {
				globalScale = new Vector2(computedScale.x * xScaleMultiplier, computedScale.y);
			} else {
				globalScale = computedScale;
			}
			rotation = new Angle(globalAngle).GetUnityQuaternion();
			position = globalPosition;
		} else {
			globalScale = localScale;
			if (Parent != null) {
				globalPosition = Parent.transform.localPosition + Parent.transform.localRotation * localPosition;
				rotation = Parent.transform.localRotation * new Angle(localRotation).GetUnityQuaternion();
			} else {
				globalPosition = localPosition;
				rotation = new Angle(localRotation).GetUnityQuaternion();
			}
			position = globalPosition;
		}
		scale = new Vector3(globalScale.x, globalScale.y, 1f);
		if (controlTransform) {
			transform.localScale = scale;
			transform.localRotation = rotation;
			transform.localPosition = position;
		} else {
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
			var euler = transform.localRotation.eulerAngles;
			if (euler.x != 0 || euler.y != 0) {
				euler = new Vector3(0,0,euler.z);
				transform.localEulerAngles = euler;
			}
			if (transform.localPosition.z != position.z) {
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, position.z);
			}

			if (transform.localScale != scale || transform.localRotation != rotation || transform.localPosition != position) {
				//Debug.Log($"{position} - {rotation} - {scale}");
				globalScale = new Vector2(transform.localScale.x, transform.localScale.y);
				if (bind) {
					if (c.Settings.EngineVersion == EngineVersion.RO) {
						computedScale = new Vector2(globalScale.x / xScaleMultiplier,
							globalScale.y);
					} else {
						computedScale = globalScale;
					}
					globalAngle = new Angle().SetUnityQuaternion(transform.localRotation, previous: globalAngle);
					globalPosition = transform.localPosition;

					// Now reverse the other calculations
					if (IsPBKEditor) {
						localScale = new Vector2(computedScale.x / localScale.y, localScale.y);
						bindScale = new Vector2(localScale.y, localScale.x);
					} else {
						localScale = Vector2.Scale(computedScale, new Vector3(1 / bindScale.x, 1 / bindScale.y));
					}

					if (Parent != null) {
						var rotatedPos3d = globalPosition - Parent.globalPosition;
						var rotatedPos = new Vec2d(rotatedPos3d.x, rotatedPos3d.y);
						var pos = rotatedPos.Rotate(-Parent.globalAngle) / new Vec2d(Parent.computedScale.x, -Parent.computedScale.y)
							- new Vec2d(bindPosition.x + Parent.boneLength, bindPosition.y);
						localPosition = new Vector3(pos.x, pos.y, 0f);

						localRotation = globalAngle - Parent.globalAngle - bindRotation;
					} else {
						// Check ITF::AnimInfo::ComputeBonesFromLocalToWorld
						localPosition = globalPosition;
						localRotation = globalAngle - bindRotation;
					}
				} else {
					if (Parent != null) {
						localPosition = Quaternion.Inverse(Parent.transform.localRotation) * (transform.localPosition - Parent.transform.localPosition);
						localRotation = new Angle().SetUnityQuaternion(Quaternion.Inverse(Parent.transform.localRotation) * transform.localRotation, previous: localRotation);
					} else {
						localPosition = transform.localPosition;
						localRotation = new Angle().SetUnityQuaternion(transform.localRotation, previous: localRotation);
					}
					localScale = globalScale;
					if (IsPBKEditor) {
						bindScale = new Vector2(localScale.y, localScale.x);
					}
				}

				if (IsPBKEditor) {
					if (PBKBone != null) {
						// Update PBK bone
						PBKBone.scale = localScale.GetUbiArtVector();
						PBKBone.position = new Vec2d(localPosition.x, localPosition.y);
						PBKBone.angle = new Angle(localRotation);
					}
				}

				foreach (var child in Children)
					child.UpdateBone(controlTransform: true, updateRecursive: updateRecursive);
			}
		}


		if (updateRecursive) {
			foreach (var child in Children)
				child.UpdateBone(controlTransform: controlTransform, updateRecursive: updateRecursive);
		}
	}

	public void UpdateVisual() {
		if (visualize && Visual == null) {
			var subobj = new GameObject("SR");
			subobj.transform.SetParent(transform, false);
			subobj.transform.localPosition = Vector3.zero;
			subobj.transform.localRotation = Quaternion.Euler(0, 0, 0);
			subobj.transform.localScale = new Vector3(1f, 0.1f, 0.1f);

			SpriteRenderer sr = subobj.AddComponent<SpriteRenderer>();
			sr.sprite = Controller.Obj.GetIcon("bone");
			sr.sortingLayerName = "Gizmo";
			//subobj.hideFlags = HideFlags.HideInHierarchy;

			Visual = subobj;
		}
		if (Visual != null && Visual.activeSelf != visualize) {
			Visual.SetActive(visualize);
		}
	}
}
