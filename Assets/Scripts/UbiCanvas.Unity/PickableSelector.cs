﻿using UbiArt.ITF;
using UbiCanvas.Helpers;
using UnityEngine;

public class PickableSelector : MonoBehaviour {
	public Controller controller;
	public CameraComponent cam;

	public UnityPickable highlighted;
	public UnityPickable selected;

	public PickableOutlineDisplay selectionDisplay;
	public PickableOutlineDisplay highlightDisplay;

	public bool IsSelecting { get; private set; } = false;

    private void HandleCollision() {
        /*int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Visual");*/
        int layerMask = -1;

        Ray ray = cam.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore);
        if (hits != null && hits.Length > 0) {
			float Get2DDistanceToOrigin(RaycastHit hit) {
				var tp = hit.transform.position;
				var hp = hit.point;
				return new Vector2(hp.x - tp.x, hp.y - tp.y).magnitude;
			}
            System.Array.Sort(hits, (x, y) => (x.distance.CompareTo(y.distance))); // Sort on distance to the camera
			System.Array.Sort(hits, (x, y) => (Get2DDistanceToOrigin(x).CompareTo(Get2DDistanceToOrigin(y)))); // After this, sort on distance of impact point to transform
			var friseEditor = FindObjectOfType<UnityHandleManager>();
			for (int i = 0; i < hits.Length; i++) {
				var patchRenderer = hits[i].transform.GetComponent<UnityPatchRenderer>();
				if (controller.displayGizmos || patchRenderer != null) {
					// the object identified by hit.transform was clicked
					UnityHandle handle = hits[i].transform.GetComponentInParent<UnityHandle>();
					if (handle != null) {
						if (UnityEngine.Input.GetMouseButtonDown(0)) {
							IsSelecting = true;
						}
						if (IsSelecting) {
							if (cam.IsPanningWithThreshold()) {
								IsSelecting = false;
							} else {
								friseEditor.SelectedPoint = handle;
							}
						}
						break;
					}
					UnityPickable pb = hits[i].transform.GetComponentInParent<UnityPickable>();
					if (pb != null) {
						highlighted = pb;
						if (UnityEngine.Input.GetMouseButtonDown(0)) {
							IsSelecting = true;
						}
						//UpdateHighlight();
						if (IsSelecting) {
							if (cam.IsPanningWithThreshold()) {
								IsSelecting = false;
							} else if (UnityEngine.Input.GetMouseButtonUp(0)) {
								Select(pb, view: true);
							}
						}
						break;
					}
				}
			}
        }
    }

    public void Select(UnityPickable p, bool view = false) {
        if (selected != p || view) {
            Deselect();
            selected = p;
            p.Select();
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = p.gameObject;
#endif
            cam.JumpTo(p.gameObject, frontView: true);
        }
    }

    public void Deselect() {
        if (selected != null) {
            selected.Deselect();
        }
        selected = null;
    }
    private void UpdateHighlight() {
        //outline.Highlight = highlightedPerso != null ? highlightedPerso.gameObject : null;
        //outline.selecting = IsSelecting;
    }
	private void UpdateOutlineDisplays() {
		if(selectionDisplay != null) selectionDisplay.pickable = selected;
		if(highlightDisplay != null) highlightDisplay.pickable = selected != highlighted ? highlighted : null;
	}
    void Update() {
        highlighted = null;
        UpdateHighlight();
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!cam.MouseLookEnabled && !cam.MouseLookRMBEnabled
            && GlobalLoadState.LoadState == GlobalLoadState.State.Finished
            && screenRect.Contains(UnityEngine.Input.mousePosition)) HandleCollision();
        if (IsSelecting && !UnityEngine.Input.GetMouseButton(0)) {
            IsSelecting = false;
        }
		UpdateOutlineDisplays();
    }
}
