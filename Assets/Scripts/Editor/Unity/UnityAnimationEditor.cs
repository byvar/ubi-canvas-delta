using UnityEditor;
using UbiArt;
using UnityEngine;
using System.Linq;
using UbiCanvas.Helpers;
using System;
using System.Threading.Tasks;
using ImageMagick;
using System.Collections.Generic;
using UbiArt.Animation;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(UnityAnimation))]
public class UnityAnimationEditor : Editor {

	public override void OnInspectorGUI() {
		if (GlobalLoadState.LoadState != GlobalLoadState.State.Finished) {
			EditorGUILayout.HelpBox("Loading...\nTo use this feature, please wait until everything has loaded.", MessageType.Warning);
			return;
		} else if (Screenshot_IsTakingScreenshots) {
			EditorGUILayout.HelpBox("Please wait while ubi-canvas records this animation.", MessageType.Info);
			return;
		}
		DrawDefaultInspector();

		UnityAnimation ua = target as UnityAnimation;
		if (ua != null && ua.anims != null && ua.animsFull != null) {
			var anims = ua.PlayFullAnimation ? ua.animsFull : ua.anims;
			var newInd = ua.animIndex;
			string locIdPreview = "Bind Pose";
			if (newInd >= 0 && newInd < (ua.anims?.Length ?? 0))
				locIdPreview = ua.anims[newInd].ToString(ua.PlayFullAnimation);

			EditorGUI.indentLevel = 0;
			Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));
			if (EditorGUI.DropdownButton(rect, new GUIContent(locIdPreview), FocusType.Passive)) {
				if (Dropdown == null || Dropdown.PlayFullAnimation != ua.PlayFullAnimation || refreshAnimationsList) {
					Dropdown = new AnimationsDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), anims) {
						name = "Animations",
						PlayFullAnimation = ua.PlayFullAnimation
					};
					refreshAnimationsList = false;
				}
				Dropdown.rect = rect;
				Dropdown.Show(rect);
			}
			if (Dropdown != null && Dropdown.selection != null && Dropdown.rect == rect) {
				newInd = Dropdown.selection.Value;
				Dropdown.selection = null;
			}

			if (ua.PlayFullAnimation && newInd != -1) {
				var fullInd = anims.FindItemIndex(i => i.Index == newInd);
				if (fullInd == -1)
					fullInd = anims.FindItemIndex(i => i.Path == ua.anims[newInd].Path);
				if (fullInd != -1) {
					newInd = fullInd;
				}
			}

			// Next/previous buttons
			GUILayout.BeginHorizontal();
			GUI.enabled = newInd >= 0; // Allow -1 = bind pose
			if (GUILayout.Button("Previous")) newInd--;
			GUI.enabled = (newInd < anims.Length - 1);
			if (GUILayout.Button("Next")) newInd++;
			GUI.enabled = true;
			GUILayout.EndHorizontal();

			// Update anim index
			if (newInd == -1) {
				if (ua.animIndex != -1) {
					ua.animIndex = -1;
					ua.Init();
				}
			} else {
				var anim = anims[newInd];
				if (ua.animIndex != anim.Index) {
					ua.animIndex = anim.Index;
					ua.Init();
				}
			}

			IsExtraAnimationsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(IsExtraAnimationsOpen, "Extra animations");
			if (IsExtraAnimationsOpen) {
				EditorGUI.indentLevel++;
				string buttonString = "No animation file selected";
				if (SelectedAnimationFile != null) {
					buttonString = System.IO.Path.GetFileName(SelectedAnimationFile);
					if (buttonString.EndsWith(".ckd")) buttonString = buttonString.Substring(0, buttonString.Length - 4);
				}
				rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
				rect = EditorGUI.PrefixLabel(rect, new GUIContent("Animation file"));
				if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
					var c = Controller.MainContext;

					List<string> extensionsList = new List<string>();
					extensionsList.Add($"*.anm{(c.Settings.Cooked ? ".ckd" : "")}");
					var extensions = extensionsList.ToArray();

					string directory = (c.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
					if (!directory.EndsWith("/")) directory += "/";
					while (directory.Contains("//")) directory = directory.Replace("//", "/");

					if (ExtraAnimationDropdown == null || ExtraAnimationDropdown.directory != directory || ExtraAnimationDropdown.extensions == null || !Enumerable.SequenceEqual(ExtraAnimationDropdown.extensions, extensions) || ExtraAnimationDropdown.mode != c.Settings.Mode
						|| ExtraAnimationDropdown.displayFullPaths != UnitySettings.DisplayFullPathsInDropdowns) {
						ExtraAnimationDropdown = new FileSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory, extensions) {
							name = "Animation files",
							mode = c.Settings.Mode,
							allowDirectorySelection = true,
							displayFullPaths = UnitySettings.DisplayFullPathsInDropdowns
						};
					}
					ExtraAnimationDropdown.Show(rect);
				}
				if (ExtraAnimationDropdown != null && ExtraAnimationDropdown.selection != null) {
					SelectedAnimationFile = ExtraAnimationDropdown.selection;
					ExtraAnimationDropdown.selection = null;
					// Dirty = true;
				}
				if (!string.IsNullOrEmpty(SelectedAnimationFile)) {
					rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
					EditorGUI.TextArea(rect, SelectedAnimationFile?.RemoveCKD());

					if (GUILayout.Button("Load")) {
						var c = Controller.MainContext;
						string directory = (c.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
						if (!directory.EndsWith("/")) directory += "/";
						while (directory.Contains("//")) directory = directory.Replace("//", "/");

						var animPath = SelectedAnimationFile;
						SelectedAnimationFile = null;

						async Task Load(string basePath, string animPath) {
							Debug.Log($"Loading animation(s): {animPath.RemoveCKD()}");
							if (animPath.EndsWith("/*.*")) {
								animPath = animPath.Substring(0, animPath.Length - 4);
								var directory = basePath;
								if (!string.IsNullOrEmpty(animPath)) {
									directory = System.IO.Path.Combine(directory, animPath)
										.Replace(System.IO.Path.DirectorySeparatorChar, '/');
									if (!directory.EndsWith("/")) directory += "/";
									while (directory.Contains("//")) directory = directory.Replace("//", "/");
								}
								var paths =
									(from file in System.IO.Directory.EnumerateFiles(directory, $"*.anm{(c.Settings.Cooked ? ".ckd" : "")}", System.IO.SearchOption.AllDirectories)
									select new Path(file.Substring(basePath.Length).Replace(System.IO.Path.DirectorySeparatorChar, '/').RemoveCKD())).ToArray();

								if (paths.Any()) {
									foreach (var p in paths) {
										p.LoadObject(c, removeCooked: true);
									}
									await c.Loader.LoadLoop();

									var newAnims = paths
										.Where(p => p.Object != null && p.Object is AnimTrack at && !ua.animsFull.Any(a => a.Path == p))
										.Select(p => new UnityAnimation.UnityAnimationTrack() {
											Path = p,
											Track = p.GetObject<AnimTrack>(),
											ID = p.GetFilenameWithoutExtension()
										});
									if (newAnims.Any()) {
										foreach (var a in newAnims) {
											Debug.Log($"Added animation: {a.Path.FullPath}");
										}
										ua.anims = ua.anims.Concat(newAnims).ToArray();
										for (int i = 0; i < ua.anims.Length; i++) {
											ua.anims[i].Index = i;
										}
										ua.animsFull = null;
										ua.Init();
										refreshAnimationsList = true;
									}
								}
							} else {
								var p = new Path(animPath);
								p.LoadObject(c, removeCooked: true);
								await c.Loader.LoadLoop();

								if (p.Object != null && p.Object is AnimTrack at) {
									if (!ua.animsFull.Any(a => a.Path == p)) {
										Debug.Log($"Added animation: {p.FullPath}");
										ua.anims = ua.anims.Append(new UnityAnimation.UnityAnimationTrack() {
											Path = p,
											Track = at,
											ID = p.GetFilenameWithoutExtension()											
										}).ToArray();
										for (int i = 0; i < ua.anims.Length; i++) {
											ua.anims[i].Index = i;
										}
										ua.animsFull = null;
										ua.Init();
										refreshAnimationsList = true;
									}
								}
							}
						}
						UnityWindow.ExecuteTask(Controller.Obj.AdditionalLoad(Load(directory, animPath)));
					}
				}

				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			IsScreenshotToolsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(IsScreenshotToolsOpen, "Screenshot tools");
			if (IsScreenshotToolsOpen) {
				EditorGUI.indentLevel++;
				EditorGUILayout.HelpBox("This tool makes multiple screenshots of an animation, one per frame. Make sure to position the (game view) camera correctly before using this.", MessageType.Info);
				var pb = Controller.Obj.selector.cam.GetComponent<TransparencyCaptureBehaviour>();

				Screenshot_IsTransparent = EditorGUILayout.Toggle("Transparent", Screenshot_IsTransparent);
				Screenshot_Scale = EditorGUILayout.FloatField("Scale", Screenshot_Scale);



				async Task TakeScreenshots(bool animate, int frameskip = 1, string animFormat = "gif") {
					try {
						Screenshot_IsTakingScreenshots = true;
						var defaultName = "";
						var objectName = ua.transform.parent.gameObject.name;
						foreach (char c in System.IO.Path.GetInvalidFileNameChars())
							objectName = objectName.Replace(System.Char.ToString(c), "");
						var animName = $"{ua.Animation.Path?.GetFilenameWithoutExtension()}";
						if (animate)
							defaultName = $"{objectName}_{animName}";
						string savePath;
						List<byte[]> screenshots = new List<byte[]>();
						if (animate)
							savePath = EditorUtility.SaveFilePanel("Output file", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), defaultName, animFormat);
						else
							savePath = EditorUtility.SaveFolderPanel("Output directory", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), defaultName);
						if (!string.IsNullOrWhiteSpace(savePath)) {
							Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
							var startStopFrame = ua.GetStartStopFrame();

							if (startStopFrame != null) {
								var isPlay = ua.playAnimation;
								var frame = ua.currentFrame;
								for (int i = startStopFrame.Value.Item1; i <= startStopFrame.Value.Item2; i += frameskip) {
									var index = i - startStopFrame.Value.Item1;
									ua.playAnimation = false;
									ua.currentFrame = i;
									ua.ForceUpdatePatches = true;
									ua.Update();
									byte[] screenshotBytes = await pb.Capture((int)(res.width * Screenshot_Scale), (int)(res.height * Screenshot_Scale), true);
									if (animate) {
										screenshots.Add(screenshotBytes);
										Debug.Log($"Screenshot progress: {(index / frameskip) + 1}/{(startStopFrame.Value.Item2 - startStopFrame.Value.Item1) / frameskip + 1}");
									} else {
										UbiCanvas.Helpers.Util.ByteArrayToFile(System.IO.Path.Combine(savePath, objectName, animName, $"{i}.png"), screenshotBytes);
										Debug.Log($"Screenshot saved: {index + 1}/{startStopFrame.Value.Item2 - startStopFrame.Value.Item1 + 1}");
									}
								}

								if (animate && screenshots.Any()) {
									using (var collection = new MagickImageCollection()) {
										foreach (var pngBytes in screenshots) {
											// Load PNG from byte[]
											var png = new MagickImage(pngBytes);
											if (animFormat == "gif") {
												png.AnimationDelay = frameskip switch {
													1 => 2,
													2 => 3,
													3 => 7,
													_ => 1
												}; // Set frame delay (1/100s units, so "10" = 100ms per frame)
												png.Format = MagickFormat.Gif;
											} else if (animFormat == "webp") {
												png.AnimationDelay = frameskip switch {
													1 => 2,
													2 => 3,
													3 => 7,
													_ => 1
												}; // Set frame delay (1/100s units, so "10" = 100ms per frame)
												png.Format = MagickFormat.WebP;
												png.Quality = 100;
												png.Settings.SetDefine(MagickFormat.WebP, "lossless", true);
											}
											png.GifDisposeMethod = GifDisposeMethod.Background;

											collection.Add(png);
										}
										if (animFormat == "gif") {
											collection.OptimizeTransparency();
											var gifBytes = collection.ToByteArray(MagickFormat.Gif);
											UbiCanvas.Helpers.Util.ByteArrayToFile(savePath, gifBytes);

											Debug.Log($"GIF saved.");
										} else if (animFormat == "webp") {
											var webpBytes = collection.ToByteArray(MagickFormat.WebP);
											UbiCanvas.Helpers.Util.ByteArrayToFile(savePath, webpBytes);
											Debug.Log($"WebP saved.");
										}

									}

									ua.currentFrame = frame;
									ua.playAnimation = isPlay;
								}
							}
						}
					} finally {
						Screenshot_IsTakingScreenshots = false;
					}
				}

				if (GUILayout.Button("Take animation screenshots")) {
					UnityWindow.ExecuteTask(TakeScreenshots(false));
				}
				rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
				rect = EditorGUI.PrefixLabel(rect, new GUIContent("Record GIF"));
				var buttonRects = UnityWindow.DivideRectHorizontally(rect, 3);
				if (GUI.Button(buttonRects[0], "60fps")) {
					UnityWindow.ExecuteTask(TakeScreenshots(true, 1));
				}
				if (GUI.Button(buttonRects[1], "30fps")) {
					UnityWindow.ExecuteTask(TakeScreenshots(true, 2));
				}
				if (GUI.Button(buttonRects[2], "15fps")) {
					UnityWindow.ExecuteTask(TakeScreenshots(true, 3));
				}
				rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
				rect = EditorGUI.PrefixLabel(rect, new GUIContent("Record WebP"));
				buttonRects = UnityWindow.DivideRectHorizontally(rect, 3);
				if (GUI.Button(buttonRects[0], "60fps")) {
					UnityWindow.ExecuteTask(TakeScreenshots(true, 1, "webp"));
				}
				if (GUI.Button(buttonRects[1], "30fps")) {
					UnityWindow.ExecuteTask(TakeScreenshots(true, 2, "webp"));
				}
				if (GUI.Button(buttonRects[2], "15fps")) {
					UnityWindow.ExecuteTask(TakeScreenshots(true, 3, "webp"));
				}

				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}

	private AnimationsDropdown Dropdown { get; set; }
	public bool IsScreenshotToolsOpen { get; set; }
	public bool IsExtraAnimationsOpen { get; set; }
	public float Screenshot_Scale = 1f;
	public bool Screenshot_IsTransparent = true;
	public bool Screenshot_IsTakingScreenshots = false;

	private FileSelectionDropdown ExtraAnimationDropdown { get; set; }
	private string SelectedAnimationFile { get; set; }
	private bool refreshAnimationsList = false;
}