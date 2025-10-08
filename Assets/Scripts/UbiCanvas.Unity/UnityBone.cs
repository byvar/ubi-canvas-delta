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
	public Vector3 globalPosition = Vector3.zero;
	public Vector3 globalPositionPreInvert = Vector3.zero;
	public float globalAngle;
	public Vector2 globalScalePreLength = Vector2.one;
	public Vector2 globalScale = Vector2.one;
	public float boneLength = 0f;
	public float localZ = 0;
	public float bindZ = 0;
	public float globalZ = 0;
	public float bindAlpha = 1f;
	public float localAlpha = 0f;

	public Vec2d XAxe = Vec2d.Right;
	public float XAxeLength = 1f;
	
	public bool visualize = false;
	public GameObject Visual { get; set; }

	public bool IsPBKEditor { get; set; }
	public AnimBoneDyn PBKBone { get; set; }


	public List<UnityBone> Children { get; set; } = new List<UnityBone>();
	private UnityBone _parent;
	[UnityEngine.SerializeField]
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

	public void Reset(bool resetParent = true, bool update = true, bool invert = true) {
		localPosition = Vector3.zero;
		localScale = Vector2.one;
		localRotation = 0; //new Angle(0);
		if (resetParent) Parent = null;
		if (update) UpdateBone(invert: invert);
	}
	void Update() {
		UpdateVisual();
	}

	public void ComputeGlobalPos() {
		var c = Controller.MainContext;
		if (Parent != null) {
			Vector3 PosLocal = localPosition + bindPosition;
			Vec2d PosWorld = new Vec2d(
				(PosLocal.x + Parent.boneLength) * Parent.globalScalePreLength.x,
				PosLocal.y * Parent.globalScalePreLength.y).Rotate(-Parent.globalAngle);

			globalPosition = Parent.globalPositionPreInvert + new Vector3(PosWorld.x, PosWorld.y, 0f);
			globalAngle = Parent.globalAngle + bindRotation + localRotation;
		} else {
			// Check ITF::AnimInfo::ComputeBonesFromLocalToWorld
			globalPosition = localPosition;
			globalAngle = bindRotation + localRotation;
		}
		globalPositionPreInvert = globalPosition;
		globalScalePreLength = Vector2.Scale(localScale, bindScale);

		if (c.Settings.EngineVersion == EngineVersion.RO) {
			globalScale = new Vector2(globalScalePreLength.x * boneLength, globalScalePreLength.y);
		} else {
			globalScale = globalScalePreLength;
		}

		globalZ = bindZ + localZ;// + Parent?.globalZ ?? 0;
	}
	public void ComputeBoneEnd(bool processInvert = true) {
		XAxe = (Vec2d.Right * globalScale.x).Rotate(-globalAngle); // bonelength included
		XAxeLength = System.Math.Abs(globalScale.x);

		if (processInvert) {
			XAxe.y = -XAxe.y;
			globalPosition = new Vector3(globalPosition.x, -globalPosition.y, globalPosition.z);
		}
	}
	public void ComputeBoneEnd_Inverted(bool processInvert = true) {
		if (processInvert) {
			globalPosition = new Vector3(globalPosition.x, -globalPosition.y, globalPosition.z);
		}
		// All derived variables like XAxe and XAxeLength are recalculated later
	}
	public void ComputeGlobalPos_Inverted() {
		var c = Controller.MainContext;
		//globalZ = bindZ + localZ;// + Parent?.globalZ ?? 0;

		// Invert scale
		if (c.Settings.EngineVersion <= EngineVersion.RO && boneLength != 0) {
			globalScalePreLength = new Vector2(globalScale.x / boneLength, globalScalePreLength.y);
		} else {
			globalScalePreLength = globalScale;
		}
		globalPositionPreInvert = globalPosition;

		localScale = Vector2.Scale(globalScalePreLength, new Vector2(1 / bindScale.x, 1 / bindScale.y));

		if (Parent != null) {
			// Invert rotation
			localRotation = globalAngle - Parent.globalAngle - bindRotation;

			// Inverted
			Vector3 posWorld3d = globalPosition - Parent.globalPositionPreInvert;
			Vec2d PosWorld = new Vec2d(posWorld3d.x, posWorld3d.y);
			var posScaled = PosWorld.Rotate(Parent.globalAngle);
			var endPos = posScaled / Parent.globalScalePreLength.GetUbiArtVector();
			Vector3 PosLocal = new Vector3(endPos.x - Parent.boneLength, endPos.y, 0);
			localPosition = PosLocal - bindPosition;
		} else {
			// Check ITF::AnimInfo::ComputeBonesFromLocalToWorld
			localPosition = globalPosition;
			localRotation = globalAngle - bindRotation;
		}
	}

	public void UpdateBone(bool controlTransform = true, bool updateRecursive = false, bool invert = true) {
		var c = Controller.MainContext;

		Vector3 position;
		Vector3 scale;
		Quaternion rotation;

		ComputeGlobalPos();
		ComputeBoneEnd(processInvert: invert);

		position = globalPosition;
		rotation = new Angle(globalAngle).GetUnityQuaternion();
		scale = new Vector3(globalScale.x, globalScale.y, 1f);
		if (controlTransform) {
			transform.localScale = scale;
			transform.localRotation = rotation;
			transform.localPosition = position;
			//transform.localPosition = new Vector3(position.x, -position.y, position.z);

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
				globalAngle = new Angle().SetUnityQuaternion(transform.localRotation, previous: globalAngle);
				globalPosition = transform.localPosition;

				// Reverse calculations
				ComputeBoneEnd_Inverted(processInvert: invert);
				ComputeGlobalPos_Inverted();

				if (IsPBKEditor) {
					if (PBKBone != null) {
						// Update PBK bone
						PBKBone.scale = localScale.GetUbiArtVector();
						PBKBone.position = new Vec2d(localPosition.x, localPosition.y);
						PBKBone.angle = new Angle(localRotation);
					}
				}

				// Recompute bone variables
				ComputeGlobalPos();
				ComputeBoneEnd(processInvert: invert);

				foreach (var child in Children)
					child.UpdateBone(controlTransform: true, updateRecursive: true, invert: invert);
			}
		}


		if (updateRecursive) {
			foreach (var child in Children)
				child.UpdateBone(controlTransform: controlTransform, updateRecursive: updateRecursive, invert: invert);
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
