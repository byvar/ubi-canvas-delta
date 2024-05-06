using System;
using UbiArt;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using UnityEngine;

public class PickableOutlineDisplay : MonoBehaviour {
	public Controller controller;

	public UnityPickable pickable;
	private LineRenderer lr;
	private Tooltip tooltip;

	public RectTransform UICanvas;
	public Tooltip TooltipPrefab;

	private LineRenderer[] linkRenderers;
	private LineRenderer bindRenderer;

	public float alpha = 1f;

	private void UpdateLinks(LinkComponent linkComponent, Bind parentBind) {
		bool activateLinks = false;
		bool activateBind = false;
		bool hasLinks = false;
		bool hasBind = false;
		if (linkComponent != null && linkComponent.Children?.Count > 0) {
			hasLinks = true;
			var linkCount = linkComponent.Children?.Count ?? 0;
			if (linkRenderers != null && linkCount != linkRenderers.Length) {
				foreach (var lnk in linkRenderers) {
					GameObject.Destroy(lnk.gameObject);
				}
				linkRenderers = null;
			}
			if (linkRenderers == null) {
				linkRenderers = new LineRenderer[linkCount];
				for (int i = 0; i < linkRenderers.Length; i++) {
					var gao = new GameObject($"Link {i}");
					gao.transform.SetParent(transform, false);
					linkRenderers[i] = gao.AddComponent<LineRenderer>();
					linkRenderers[i].material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
					linkRenderers[i].useWorldSpace = true;
					linkRenderers[i].widthMultiplier = 0.03f;
					linkRenderers[i].sortingLayerName = "Gizmo-Line";
					var color = new UnityEngine.Color(0f, 1f, 0f, alpha);
					linkRenderers[i].material.color = color;
					linkRenderers[i].startColor = color;
					linkRenderers[i].endColor = color;
					linkRenderers[i].positionCount = 2;
					linkRenderers[i].loop = false;
				}
			}
		}
		if (parentBind?.parentPath != null && !string.IsNullOrWhiteSpace(parentBind.parentPath.FullPath)) {
			hasBind = true;
			if (bindRenderer == null) {
				var gao = new GameObject($"Parent Bind");
				gao.transform.SetParent(transform, false);
				bindRenderer = gao.AddComponent<LineRenderer>();
				bindRenderer.material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
				bindRenderer.useWorldSpace = true;
				bindRenderer.widthMultiplier = 0.03f;
				bindRenderer.sortingLayerName = "Gizmo-Line";
				var color = new UnityEngine.Color(0f, 107 / 255f, 1f, alpha);
				bindRenderer.material.color = color;
				bindRenderer.startColor = color;
				bindRenderer.endColor = color;
				bindRenderer.positionCount = 2;
				bindRenderer.loop = false;
			}
		}
		if(hasLinks || hasBind) {
			var tree = new PickableTree(Controller.Obj.MainScene.obj);
			var path = Controller.Obj.MainScene.obj.FindPickable(p => p == pickable.pickable);

			if (path != null) {
				var curNode = tree.FollowObjectPath(path.Path, throwIfNotFound: false);
				if (curNode != null) {
					if (hasLinks) {
						activateLinks = true;
						for (int i = 0; i < linkRenderers.Length; i++) {
							var link = linkComponent.Children[i];
							var linkedNode = curNode.Parent.GetNodeWithObjectPath(link?.Path, throwIfNotFound: false);
							bool linkActive = linkedNode?.Pickable != null;
							SetLinkActive(i, linkActive);
							if (linkActive) {
								linkRenderers[i].SetPositions(new Vector3[] {
									pickable.transform.position,
									linkedNode.Pickable.GetPrecreatedGameObject().transform.position
								});
							}
						}
					}
					if (hasBind) {
						activateBind = true;
						var linkedNode = curNode.Parent.GetNodeWithObjectPath(parentBind.parentPath, throwIfNotFound: false);
						bool linkActive = linkedNode?.Pickable != null;
						SetBindActive(linkActive);
						if (linkActive) {
							bindRenderer.SetPositions(new Vector3[] {
								pickable.transform.position,
								linkedNode.Pickable.GetPrecreatedGameObject().transform.position
							});
						}
					}
				}
			}
		}
		void SetLinkActive(int i, bool active) {
			if (linkRenderers[i].gameObject.activeSelf != active) linkRenderers[i].gameObject.SetActive(active);
		}
		void SetBindActive(bool active) {
			if (bindRenderer.gameObject.activeSelf != active) bindRenderer.gameObject.SetActive(active);
		}

		if (!activateLinks) {
			if (linkRenderers != null) {
				for (int i = 0; i < linkRenderers.Length; i++) {
					SetLinkActive(i, false);
				}
			}
		}
		if (!activateBind) {
			if(bindRenderer != null) SetBindActive(false);
		}
	}

    private void UpdateHighlight() {
		if (lr == null) {
			lr = gameObject.AddComponent<LineRenderer>();
			lr.material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
			lr.useWorldSpace = true;
			lr.widthMultiplier = 0.03f;
			lr.sortingLayerName = "Gizmo-Line";
			var color = new UnityEngine.Color(1f,1f,1f, alpha);
			lr.material.color = color;
			lr.startColor = color;
			lr.endColor = color;
			lr.positionCount = 0;
		}
		if (tooltip == null) {
			tooltip = GameObject.Instantiate(TooltipPrefab);
			tooltip.transform.SetParent(UICanvas);
			tooltip.transform.localPosition = Vector3.zero;
			tooltip.canvas = UICanvas;
			tooltip.Hide();
		}
		if (pickable != null) {
			lr.enabled = true;
			lr.positionCount = 4;
			lr.loop = true;
			lr.SetPositions(new Vector3[] {
				pickable.transform.TransformPoint(new Vector3(-0.5f, -0.5f)),
				pickable.transform.TransformPoint(new Vector3(0.5f, -0.5f)),
				pickable.transform.TransformPoint(new Vector3(0.5f, 0.5f)),
				pickable.transform.TransformPoint(new Vector3(-0.5f, 0.5f))
			});
			tooltip.Show(pickable.pickable.USERFRIENDLY, alpha);
			tooltip.SetWorldPosition(pickable.transform.position);
			if (pickable?.pickable is Actor act) {
				var linkComponent = act.GetComponent<LinkComponent>();
				var bind = (act?.parentBind?.read ?? false) ? act?.parentBind?.value : null;
				UpdateLinks(linkComponent, bind);
			} else {
				UpdateLinks(null, null);
			}
		} else {
			lr.enabled = false;
			lr.positionCount = 0;
			tooltip.Hide();
			UpdateLinks(null, null);
		}
    }
    void Update() {
		if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
			UpdateHighlight();
		}
    }
}
