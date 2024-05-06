using Cysharp.Threading.Tasks;
using UbiCanvas.Helpers;
using UnityEngine;

namespace UbiArt.ITF {
	public partial class Pickable {
		protected GameObject gao;
		public async UniTask<GameObject> GetGameObject() {
			if (gao == null) {
				await InitGameObject();
			}
			return GetPrecreatedGameObject();
		}
		public GameObject GetPrecreatedGameObject() {
			return gao;
		}

		protected virtual async UniTask InitGameObject() {
			gao = new GameObject(USERFRIENDLY);
			UbiArtContext.Loader.LoadingState = $"Creating objects\n{USERFRIENDLY}";
			await TimeController.WaitIfNecessary();
			UnityPickable p = gao.AddComponent<UnityPickable>();
			p.transform.hideFlags = HideFlags.HideInInspector;
			p.pickable = this;
			p.ResetTransformFromData();
			await UniTask.CompletedTask;
			//MapLoader.Loader.controller.zListManager.Register(this);
			//if (ANGLE.angle != 0f) gao.name += " - " + ANGLE.angle;
		}
		public async UniTask SetGameObjectParent(GameObject gp) {
			if (gao == null) {
				await GetGameObject();
			}
			gao.transform.SetParent(gp.transform, false);
			gao.GetComponent<UnityPickable>().ResetTransformFromData();
		}
		public async UniTask SetContainingScene(Scene sc) {
			if (gao == null) {
				await GetGameObject();
			}
			var pickable = gao.GetComponent<UnityPickable>();
			pickable.ContainingScene = sc;
		}
	}
}
