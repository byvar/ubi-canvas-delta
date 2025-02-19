using UnityEngine;
using UbiArt;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using System.Linq;

public class UnityFrise : MonoBehaviour {
	public Frise frise;
	float prevZ = -99999;
	public LineRenderer[] CollisionRenderers;


	void InitRenderers() {
		if (CollisionRenderers == null && frise != null
			&& (frise?.collisionData?.value?.LocalCollisionList?.Any() ?? false)) {
			var colList = frise?.collisionData?.value?.LocalCollisionList;
			CollisionRenderers = new LineRenderer[colList.Count];
			for (int i = 0; i < CollisionRenderers.Length; i++) {
				var gao = new GameObject($"Link {i}");
				gao.transform.SetParent(transform, false);
				gao.transform.localPosition = Vector3.zero;
				gao.transform.localRotation = Quaternion.identity;
				gao.transform.localScale = Vector3.one;
				CollisionRenderers[i] = gao.AddComponent<LineRenderer>();
				CollisionRenderers[i].material = new Material(FindObjectOfType<UnityHandleManager>().lineMaterial);
				CollisionRenderers[i].useWorldSpace = false;
				CollisionRenderers[i].widthMultiplier = 0.03f;
				CollisionRenderers[i].sortingLayerName = "Gizmo-Line";
				var color = new UnityEngine.Color(0f, 1f, 0f, 0.7f);
				CollisionRenderers[i].material.color = color;
				CollisionRenderers[i].startColor = color;
				CollisionRenderers[i].endColor = color;
				var col = colList[i];
				var posCount = col.LocalPoints.Count + (col.Loop ? -1 : 0);
				CollisionRenderers[i].positionCount = posCount;
				CollisionRenderers[i].loop = col.Loop;
				Vector3[] positions = new Vector3[posCount];
				for (int j = 0; j < posCount; j++) {
					var pos = col.LocalPoints[j].POS;
					positions[j] = new Vector3(pos.x, pos.y, 0);
				}
				CollisionRenderers[i].SetPositions(positions);
			}
		}
	}

	void UpdateCollisionRenderers() {
		if (CollisionRenderers == null) InitRenderers();
		if (CollisionRenderers != null && CollisionRenderers.Any()) {
			bool isSelected = frise != null && Controller.Obj.SelectedObject?.pickable == frise;
			var color = isSelected ? UnityEngine.Color.red : new UnityEngine.Color(0f, 1f, 0f, 0.7f);
			var widthMultiplier = isSelected ? 0.075f: 0.03f;
			foreach (var r in CollisionRenderers) {
				if (r == null) continue;
				if (r.startColor != color) {
					r.material.color = color;
					r.startColor = color;
					r.endColor = color;
				}
				if (r.widthMultiplier != widthMultiplier) {
					r.widthMultiplier = widthMultiplier;
				}
			}
		}
		// TODO: Implement updating positions
		//SetPositions(AABBRenderer, CameraModifier.localAABB);
	}

	private void Update() {
		if (GlobalLoadState.LoadState != GlobalLoadState.State.Finished) return;
		if (frise != null) {
			bool showCollision = Controller.Obj.displayCollision;
			if (showCollision) {
				UpdateCollisionRenderers();
			}
			if (CollisionRenderers != null) {
				foreach (var r in CollisionRenderers) {
					if (r == null) continue;
					r.enabled = showCollision;
				}
			}

			frise.UpdateAllMaterialParams();

			float z = transform.position.z;
			if (z != prevZ) {
				prevZ = z;
				ZListManager zman = Controller.Obj.zListManager;
				if (frise.mr_static != null) {
					zman.zDict[frise.mr_static] = z;
					/*foreach (Material m in frise.mr_static.sharedMaterials) {
						zman.zDict[m] = z;
						z += 0.001f;
					}*/
				}
				if (frise.mr_overlay != null) {
					zman.zDict[frise.mr_overlay] = z;
					/*foreach (Material m in frise.mr_static.sharedMaterials) {
						zman.zDict[m] = z;
						z += 0.001f;
					}*/
				}
				if (frise.mr_anim != null) {
					zman.zDict[frise.mr_anim] = z;
					/*foreach (Material m in frise.mr_anim.sharedMaterials) {
						zman.zDict[m] = z;
						z += 0.001f;
					}*/
				}
				if (CollisionRenderers != null) {
					foreach (var r in CollisionRenderers) {
						if(r == null) continue;
						zman.zDict[r] = z;
					}
				}
			}
		}
	}
}
