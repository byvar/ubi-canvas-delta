using UnityEditor;
using UbiArt;
using UnityEngine;
using UbiCanvas.Helpers;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq;
using UbiArt.ITF;
using System.Collections.Generic;

public class UnityWindowActor : UnityWindow {
	[MenuItem("Ubi-Canvas/Actor Tools")]
	public static void ShowWindow() {
		GetWindow<UnityWindowActor>("Actor Tools");
	}
	private void OnEnable() {
		titleContent = EditorGUIUtility.IconContent("Favorite");
		titleContent.text = "Actor Tools";
	}
	protected override void UpdateEditorFields() {
		base.UpdateEditorFields();
		if (EditorApplication.isPlaying) {
			if (controller == null) controller = Controller.Obj;
			if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
				var c = Controller.MainContext;
				string ExtensionActor = c.Settings.EngineVersion == EngineVersion.RO ? "uca" : "act";
				string ExtensionActorTemplate = c.Settings.EngineVersion == EngineVersion.RO ? "act" : "tpl";
				string ExtensionTemplateScene = "tsc";
				string ExtensionScene = "isc";
				string ExtensionFrise = "frz";
				//string ExtensionFriseConfig = $"*.fcg{(c.Settings.Cooked ? ".ckd" : "")}";
				string ExtensionSubsceneActor = $"ucs";

				List<string> extensionsList = new List<string>();
				if(LoadActors) extensionsList.Add($"*.{ExtensionActor}{(c.Settings.Cooked ? ".ckd" : "")}");
				if(LoadTemplates) extensionsList.Add($"*.{ExtensionActorTemplate}{(c.Settings.Cooked ? ".ckd" : "")}");
				if(LoadTemplateScenes) extensionsList.Add($"*.{ExtensionTemplateScene}{(c.Settings.Cooked ? ".ckd" : "")}");
				if(LoadScenes) extensionsList.Add($"*.{ExtensionScene}{(c.Settings.Cooked ? ".ckd" : "")}");
				if(LoadFrise) extensionsList.Add($"*.{ExtensionFrise}{(c.Settings.Cooked ? ".ckd" : "")}");
				if(LoadSubsceneActors) extensionsList.Add($"*.{ExtensionSubsceneActor}{(c.Settings.Cooked ? ".ckd" : "")}");
				var extensions = extensionsList.ToArray();

				#region Add Actor
				DrawHeader("Import Actor");
				Rect rect = GetNextRect(vPaddingBottom: 4f);
				string buttonString = "No actor file selected";
				if (SelectedActorFile != null) {
					buttonString = System.IO.Path.GetFileName(SelectedActorFile);
					if (buttonString.EndsWith(".ckd")) buttonString = buttonString.Substring(0, buttonString.Length - 4);
				}
				rect = EditorGUI.PrefixLabel(rect, new GUIContent("Actor file"));
				if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
					string directory = (Controller.MainContext.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
					if (!directory.EndsWith("/")) directory += "/";
					while (directory.Contains("//")) directory = directory.Replace("//", "/");

					if (recheckFiles || Dropdown == null || Dropdown.directory != directory || Dropdown.extensions == null || !Enumerable.SequenceEqual(Dropdown.extensions, extensions) || Dropdown.mode != c.Settings.Mode
						|| Dropdown.displayFullPaths != UnitySettings.DisplayFullPathsInDropdowns) {
						Dropdown = new FileSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory, extensions) {
							name = "Actor files",
							mode = c.Settings.Mode,
							displayFullPaths = UnitySettings.DisplayFullPathsInDropdowns
						};
						recheckFiles = false;
					}
					Dropdown.Show(rect);
				}
				if (Dropdown != null && Dropdown.selection != null) {
					SelectedActorFile = Dropdown.selection;
					Dropdown.selection = null;
					Dirty = true;
				}
				if (!string.IsNullOrEmpty(SelectedActorFile)) {
					EditorGUI.TextArea(GetNextRect(), SelectedActorFile?.RemoveCKD());
				}
				EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(SelectedActorFile));
				UbiArt.ITF.Scene sc = Controller.Obj.MainScene.obj;
				UbiArt.ITF.SubSceneActor ssa = null;
				if (Selection.activeGameObject != null) {
					UnityScene us = Selection.activeGameObject.GetComponent<UnityScene>();
					if (us != null && us.scene != null) {
						sc = us.scene;
						ssa = us.subSceneActor;
					}
				}
				if (sc != null) {
					EditorGUI.LabelField(GetNextRect(), new GUIContent("Selected scene"), new GUIContent(ssa == null ? "Main scene" : ssa.USERFRIENDLY));
				}

				if (EditorButton("Load")) {
					string pathFolder = System.IO.Path.GetDirectoryName(SelectedActorFile);
					string pathFile = System.IO.Path.GetFileName(SelectedActorFile);
					UbiArt.Path path = new Path(pathFolder, pathFile);
					var extension = path.GetExtension(removeCooked: true).ToLowerInvariant();
					if (sc != null) {
						if (extension == ExtensionActorTemplate) {
							ExecuteTask(
								controller.AdditionalLoad(controller.LoadTemplateAndCreateActor(sc, path).AsTask())
							);
						} else if (extension == ExtensionActor) {
							ExecuteTask(controller.AdditionalLoad(controller.LoadActorContainer<Actor>(sc, path).AsTask()));
						} else if (extension == ExtensionFrise) {
							ExecuteTask(controller.AdditionalLoad(controller.LoadActorContainer<Frise>(sc, path).AsTask()));
						} else if (extension == ExtensionSubsceneActor) {
							ExecuteTask(controller.AdditionalLoad(controller.LoadActorContainer<SubSceneActor>(sc, path).AsTask()));
						} else if (extension == ExtensionTemplateScene || extension == ExtensionScene) {
							ExecuteTask(controller.AdditionalLoad(controller.LoadActorContainer<Scene>(sc, path).AsTask()));
						}
					}
				}
				EditorGUI.EndDisabledGroup();

				LoadActors = EditorField($"Actors ({ExtensionActor})", LoadActors);
				LoadTemplates = EditorField($"Actor Templates ({ExtensionActorTemplate})", LoadTemplates);
				LoadTemplateScenes = EditorField($"Template Scenes ({ExtensionTemplateScene})", LoadTemplateScenes);
				LoadScenes = EditorField($"Scenes ({ExtensionScene})", LoadScenes);
				LoadFrise = EditorField($"Frises ({ExtensionFrise})", LoadFrise);
				LoadSubsceneActors = EditorField($"Subscene Actors ({ExtensionSubsceneActor})", LoadSubsceneActors);
				#endregion

				#region Export Actor
				DrawHeader("Export Actor");
				UbiArt.ITF.Actor a = null;
				UbiArt.ITF.Scene scn = null;
				if (Selection.activeGameObject != null) {
					UnityPickable up = Selection.activeGameObject.GetComponent<UnityPickable>();
					if (up?.pickable != null && up.pickable is Actor act) {
						a = act;
					} else {
						UnityScene uscn = Selection.activeGameObject.GetComponent<UnityScene>();
						if (uscn?.scene != null) {
							scn = uscn.scene;
						}
					}
				}
				EditorGUI.LabelField(GetNextRect(), new GUIContent("Selected actor"), new GUIContent(a == null ? (scn == null ? "None" : "Scene") : a.USERFRIENDLY));
				var chosenExtension = ExtensionActor;
				if (a is Frise f) {
					chosenExtension = ExtensionFrise;
				} else if (a is SubSceneActor s) {
					chosenExtension = ExtensionSubsceneActor;
				} else if (scn != null) {
					chosenExtension = ExtensionScene;
				}
				if (c.Settings.Cooked) {
					chosenExtension += ".ckd";
				}
				ActorPath = FileField(GetNextRect(), "Actor path", ActorPath, true, chosenExtension);

				EditorGUI.BeginDisabledGroup((a == null && scn == null) || string.IsNullOrEmpty(ActorPath));
				if (EditorButton("Export")) {
					async UniTask exportActor() {
						if (scn != null) {
							await controller.ExportSceneContainer(scn, ActorPath);
						} else {
							await controller.ExportActorContainer(a, ActorPath);
						}
						recheckFiles = true;
					};
					ExecuteTask(exportActor());
				}
				EditorGUI.EndDisabledGroup();
				#endregion
			} else {
				EditorHelpBox("Loading...\nTo use this window, please wait until everything has loaded.", MessageType.Warning);
			}
		} else {
			EditorHelpBox("Please start the scene to use this window.", MessageType.Info);
		}
	}


	private Controller controller;

	/// <summary>
	/// The file selection dropdown
	/// </summary>
	private FileSelectionDropdown Dropdown { get; set; }

	/// <summary>
	/// The selected actor file
	/// </summary>
	private string SelectedActorFile { get; set; }

	/// <summary>
	/// The selected actor file
	/// </summary>
	private string ActorPath { get; set; }

	private bool recheckFiles = false;

	private bool LoadActors { get; set; } = true;
	private bool LoadTemplates { get; set; } = true;
	private bool LoadScenes { get; set; }
	private bool LoadTemplateScenes { get; set; } = true;
	private bool LoadSubsceneActors { get; set; } = true;
	private bool LoadFrise { get; set; } = true;
}