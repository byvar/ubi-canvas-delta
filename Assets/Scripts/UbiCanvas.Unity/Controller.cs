﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using UbiArt;
using System.Threading.Tasks;
using UbiArt.ITF;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UbiCanvas.Helpers;
using UbiCanvas;

public class Controller : MonoBehaviour {
	public Material baseMaterial;
	public Material baseTransparentMaterial;
	public Material baseLightMaterial;
	public Material collideMaterial;
	public Material collideTransparentMaterial;
	public LoadingScreen loadingScreen;
	public ZListManager zListManager;
	private Sprite[] icons;
	private Texture2D[] iconTextures;
	bool displayGizmos_ = false; public bool displayGizmos = false;
	bool displayBezier_ = false; public bool displayBezier = false;
	bool displayCollision_ = false; public bool displayCollision = false;
	bool playAnimations_ = true; public bool playAnimations = true;
	public PickableSelector selector;
	public UnityPickable SelectedObject => selector.selected;

	public static Controller Obj { get; protected set; }
	public static Context MainContext { get; protected set; }
	public ContainerFile<UbiArt.ITF.Scene> MainScene { get; protected set; }

	void Awake() {
		Application.logMessageReceived += Log;
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			UnityEngine.Debug.unityLogger.filterLogType = LogType.Assert;
		}

		// Make sure filesystem is set before checking here
		UnitySettings.ConfigureFileSystem();
	}

	// Use this for initialization
	async UniTaskVoid Start() {
		Obj = this;

		Mode mode = UnitySettings.GameMode;
		string gameDataBinFolder = UnitySettings.GameDirs.ContainsKey(mode) ? UnitySettings.GameDirs[mode] : "";

		if (FileSystem.mode == FileSystem.Mode.Web) {
			gameDataBinFolder = UnitySettings.GameDirsWeb.ContainsKey(mode) ? UnitySettings.GameDirsWeb[mode] : "";
		}

		icons = Resources.LoadAll<Sprite>("tagicons");
		iconTextures = icons.Select(spr => spr.GetTexture()).ToArray();

		loadingScreen.Active = true;
		var settings = Settings.FromMode(mode);
		MainContext = new Context(gameDataBinFolder, settings,
			serializerLogger: new MapViewerSerializerLogger(),
			fileManager: new MapViewerFileManager(),
			systemLogger: new UnitySystemLogger(),
			asyncController: new UniTaskAsyncController());

		// Optional, add lines
		if (MainContext.Loader.FileManager.FileExists("strings.txt")) {
			using (var str = MainContext.Loader.FileManager.GetFileReadStream("strings.txt")) {
				using (StreamReader r = new StreamReader(str)) {
					while (!r.EndOfStream) {
						var line = r.ReadLine();
						MainContext.AddToStringCache(line);
					}
				}
			}
		}

		MainContext.Loader.LoadAnimations = UnitySettings.LoadAnimations;
		MainContext.Loader.LoadAllPaths = UnitySettings.LoadAllPaths;

		await Init();
	}

	async UniTask Init() {
		Stopwatch w = new Stopwatch();
		w.Start();
		GlobalLoadState.LoadState = GlobalLoadState.State.Loading;
		await TimeController.WaitFrame();

		using (MainContext) {
			MainContext.Loader.LoadingState = "Loading initial files";
			await MainContext.Loader.LoadInitial();

			// Load main scene
			string lvlPath = UnitySettings.SelectedLevelFile;
			string pathFolder = System.IO.Path.GetDirectoryName(lvlPath);
			string pathFile = System.IO.Path.GetFileName(lvlPath);
			MainContext.Loader.LoadingState = $"Loading scene: {pathFile}";
			MainScene = await MainContext.Loader.LoadScene(new UbiArt.Path(pathFolder, pathFile));
		}
		if (MainScene != null && MainScene.obj != null) {
			GameObject sceneGao = await MainScene.obj.GetGameObject();
		}

		await TimeController.WaitFrame();
		if (GlobalLoadState.LoadState == GlobalLoadState.State.Error) return;
		GlobalLoadState.LoadState = GlobalLoadState.State.Initializing;
		zListManager.Sort();
		print("Loading finished after " + w.ElapsedMilliseconds + "ms.");
		w.Stop();
		await TimeController.WaitFrame();
		GlobalLoadState.DetailedState = "Finished";
		GlobalLoadState.LoadState = GlobalLoadState.State.Finished;
		loadingScreen.Active = false;
	}

	public async UniTask LoadActorContainer<T>(Scene scene, UbiArt.Path path) where T : CSerializable, new() {
		var ext = path.GetExtension(removeCooked: true);
		var name = path.GetFilenameWithoutExtension(removeCooked: true);
		ContainerFile<T> act = await MainContext.Loader.LoadExtra<ContainerFile<T>>(path);
		if (act != null && act.obj != null) {
			await TimeController.WaitFrame();
			CSerializable c = await MainContext.Loader.Clone(act.obj, ext);
			GlobalLoadState.LoadState = GlobalLoadState.State.Initializing;

			Actor actorToAdd = null;
			if (c is Actor a) {
				actorToAdd = a;
			} else if (c is Scene s) {
				actorToAdd = new SubSceneActor() {
					USERFRIENDLY = name,
					LUA = new UbiArt.Path("enginedata/actortemplates/subscene.tpl"),

					EMBED_SCENE = true,
					ZFORCED = true,
					SCENE = new UbiArt.Nullable<Scene>(s),
					parentBind = new Nullable<Bind>()
				};
				var tpl = await MainContext.Loader.LoadExtra<GenericFile<Actor_Template>>(actorToAdd.LUA);
				actorToAdd.template = tpl;
				actorToAdd.templatePickable = tpl.obj;
				MainContext.Loader.AddLoadedActor(actorToAdd);
				actorToAdd.InitContext(MainContext);
			}
			if (actorToAdd != null) {
				bool isAdded = scene.AddActor(actorToAdd, name);
				if (isAdded) {
					var sceneGao = await scene.GetGameObject();
					await actorToAdd.SetGameObjectParent(sceneGao);
					await actorToAdd.SetContainingScene(scene);
				}
			}
			Controller.Obj.zListManager.Sort();
			await TimeController.WaitFrame();
		}
	}
	public async UniTask ExportActorContainer(Actor actor, string path) {
		string extension = MainContext?.Settings?.EngineVersion == EngineVersion.RO ? "uca" : "act";
		if (actor is Frise f) {
			extension = "frz";
		} else if (actor is SubSceneActor ssa) {
			extension = "ucs";
		}
		GlobalLoadState.LoadState = GlobalLoadState.State.Loading;
		loadingScreen.Active = true;
		byte[] actorBytes = await MainContext.Loader.WriteActorFile(actor, extension);
		if (actorBytes != null)
			Util.ByteArrayToFile(path, actorBytes);

		GlobalLoadState.DetailedState = "Finished";
		GlobalLoadState.LoadState = GlobalLoadState.State.Finished;
		loadingScreen.Active = false;
	}
	public async UniTask ExportSceneContainer(Scene scene, string path) {
		const string extension = "isc";
		
		GlobalLoadState.LoadState = GlobalLoadState.State.Loading;
		loadingScreen.Active = true;
		byte[] actorBytes = await MainContext.Loader.WriteSceneFile(scene, extension);
		if (actorBytes != null)
			Util.ByteArrayToFile(path, actorBytes);

		GlobalLoadState.DetailedState = "Finished";
		GlobalLoadState.LoadState = GlobalLoadState.State.Finished;
		loadingScreen.Active = false;
	}

	public async UniTask LoadTemplateAndCreateActor(Scene scene, UbiArt.Path path) {
		path.LoadObject(MainContext, removeCooked: true);
		var name = path.GetFilenameWithoutExtension(removeCooked: true);
		await MainContext.Loader.LoadLoop();
		var tpl = path.GetObject<GenericFile<Actor_Template>>();

		if (tpl?.obj != null) {
			await TimeController.WaitFrame();
			Actor a = tpl.obj.Instantiate(path);
			MainContext.Loader.AddLoadedActor(a);
			bool isAdded = scene.AddActor(a, name);
			if (isAdded) {
				var sceneGao = await scene.GetGameObject();
				await a.SetGameObjectParent(sceneGao);
				await a.SetContainingScene(scene);
			}
			Controller.Obj.zListManager.Sort();
			await TimeController.WaitFrame();
		}
	}

	public async UniTask AdditionalLoad(Task task) {
		GlobalLoadState.LoadState = GlobalLoadState.State.Loading;
		loadingScreen.Active = true;

		using (MainContext) {
			await task;
		}

		await TimeController.WaitFrame();
		GlobalLoadState.DetailedState = "Finished";
		GlobalLoadState.LoadState = GlobalLoadState.State.Finished;
		loadingScreen.Active = false;

	}

	// Update is called once per frame
	void Update() {
		if (loadingScreen.Active) {
			if (GlobalLoadState.LoadState == GlobalLoadState.State.Error) {
				loadingScreen.LoadingText = GlobalLoadState.DetailedState;
				loadingScreen.LoadingtextColor = UnityEngine.Color.red;
			} else {
				if (GlobalLoadState.LoadState == GlobalLoadState.State.Loading) {
					loadingScreen.LoadingText = MainContext.Loader.LoadingState;
				} else {
					loadingScreen.LoadingText = GlobalLoadState.DetailedState;
				}
				loadingScreen.LoadingtextColor = UnityEngine.Color.white;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.G)) {
			displayGizmos = !displayGizmos;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.C)) {
			displayCollision = !displayCollision;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.B)) {
			displayBezier = !displayBezier;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.P)) {
			playAnimations = !playAnimations;
		}
		bool updatedSettings = false;
		if (MainContext != null) {
			if (displayGizmos != displayGizmos_) {
				updatedSettings = true;
				displayGizmos_ = displayGizmos;
			}
			if (displayBezier != displayBezier_) {
				updatedSettings = true;
				displayBezier_ = displayBezier;
			}
			if (displayCollision != displayCollision_) {
				updatedSettings = true;
				displayCollision_ = displayCollision;
			}
			if (playAnimations != playAnimations_) {
				updatedSettings = true;
				playAnimations_ = playAnimations;
			}
		}
		if (updatedSettings) {
			//communicator.SendSettings();
		}
	}

	public void Log(string condition, string stacktrace, LogType type) {
		switch (type) {
			case LogType.Exception:
			case LogType.Error:
				if (GlobalLoadState.LoadState != GlobalLoadState.State.Finished) {
					// Allowed exceptions
					if (condition.Contains("cleaning the mesh failed")) break;

					// Go to error state
					GlobalLoadState.LoadState = GlobalLoadState.State.Error;
					if (loadingScreen.Active) {
						GlobalLoadState.DetailedState = condition;
					}
				}
				break;
		}
	}

	public Sprite GetIcon(string name, bool selected = false) {
		if (selected) {
			return icons.FirstOrDefault(s => s.name == $"{name}_s") ?? icons.FirstOrDefault(s => s.name == name);
		} else {
			return icons.FirstOrDefault(s => s.name == name);
		}
	}
	public Texture2D GetTextureForIcon(Sprite sprite) {
		var index = icons.FindItemIndex(i => i == sprite);
		if (index != -1) {
			return iconTextures[index];
		}
		return null;
	}
}
