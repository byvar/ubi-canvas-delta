using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json;
using UbiArt;
using UbiArt.Animation;
using UbiArt.UV;
using UbiCanvas.Helpers;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

public class UnityWindowAtlasEditor : UnityWindow {
	[MenuItem("Ubi-Canvas/Atlas Editor")]
	public static void ShowWindow() {
		GetWindow<UnityWindowAtlasEditor>("Atlas Editor");
	}
	private void OnEnable() {
		titleContent = EditorGUIUtility.IconContent("Image Icon");
		titleContent.text = "Atlas Editor";

		wantsMouseMove = true;
		SceneView.duringSceneGui += OnSceneGUI;
	}
	private void OnDisable() {
		SceneView.duringSceneGui -= OnSceneGUI;

	}
	#region Scene view
	public void OnSceneGUI(SceneView sceneView) {
		if (!string.IsNullOrWhiteSpace(SelectedPBKFile)) {
			Path pbkPath = new Path(SelectedPBKFile);

			var cache = Controller.MainContext?.Cache?.Structs;
			if (cache != null && cache.ContainsKey(typeof(AnimPatchBank)) && cache[typeof(AnimPatchBank)].ContainsKey(pbkPath.stringID)) {
				var pbk = cache[typeof(AnimPatchBank)][pbkPath.stringID] as AnimPatchBank;
				if (pbk != null) {
					var texture = SelectedTexture;
					if (texture != null) {
						// Scan for patch editors
						var patchEditors = FindObjectsOfType<UnityPatchEditor>().Where(p => p.texture == SelectedTexture && p.pbk == pbk);
						if (patchEditors.Any()) {
							foreach (var pe in patchEditors) {
								DrawPatchEditor(pe);
							}
						}
					}
				}
			}
		}
	}
	public void DrawPatchEditor(UnityPatchEditor patchEditor) {
		var pbk = patchEditor.pbk;
		var tpl = patchEditor.Template;

		float patchEditorRot = -patchEditor.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

		void DrawPoint(Vector3 center, Color fillColor) {
			if(!Display_Points) return;
			var size = 0.1f;
			float handleSize = HandleUtility.GetHandleSize(center) * size;

			Vector3 right = SceneView.lastActiveSceneView.camera.transform.right * handleSize * 0.5f;
			Vector3 up = SceneView.lastActiveSceneView.camera.transform.up * handleSize * 0.5f;

			Vector3[] verts = new Vector3[4];
			var outlineColor = Color.white;

			// Create a small square (quad) around the center point
			verts[0] = center - right - up;
			verts[1] = center - right + up;
			verts[2] = center + right + up;
			verts[3] = center + right - up;

			Handles.DrawSolidRectangleWithOutline(verts, fillColor, outlineColor);
		}
		void DrawCubicBezier(float handleSize, float z, Vec2d[] controlPoints) {
			if(!Display_Lines) return;
			var numPoints = 16;
			var lineSize = LineSize * handleSize * 2;

			Vec2d[] points = Enumerable.Range(0, numPoints).Select(i => BezierHelpers.CalculateCubicBezierPoint(i / (float)(numPoints - 1), controlPoints)).ToArray();
			Vector3[] pointsConv = points.Select(p => new Vector3(p.x, p.y, z)).ToArray();
			for (int i = 0; i < points.Length - 1; i++) {
				Handles.DrawDottedLine(pointsConv[i], pointsConv[i + 1], lineSize);
			}
		}
		void DrawNormal(Vector3 startPos, Vector3 endPos) {
			if(!Display_Normals) return;
			float handleSize = HandleUtility.GetHandleSize(startPos);
			var lineSize = LineSize * handleSize * 2;
			Handles.DrawLine(startPos, endPos, lineSize);
		}
		UnityEngine.Random.InitState(12675);
		UnityEngine.Color pointColor;

		for (int tpl_i = 0; tpl_i < patchEditor.templateIndex; tpl_i++) {
			var tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
			bool IsDisplayingSingleTemplate = tpl_i == CurrentPBKTemplate;
			if (CurrentPBKTemplate == -1) {
				if (!ShowTemplate.ContainsKey(tplKey)) {
					ShowTemplate[tplKey] = true;
				}
				if (!ShowTemplate[tplKey]) continue;
			} else if (!IsDisplayingSingleTemplate) continue;
			pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
		}
		pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
		foreach (var point in patchEditor.points) {
			DrawPoint(point.transform.position, pointColor);
			var norm = point.GlobalNormal.Rotate(-patchEditorRot).GetUnityVector() * 0.05f;
			DrawNormal(point.transform.position, point.transform.position + new Vector3(norm.x, norm.y, 0f));
		}
		Dictionary<Link, AnimPatchPoint> points = tpl.patchPoints.ToDictionary(p => p.key, p => p);
		UnityPatchPointEditor GetUnityPoint(AnimPatchPoint pt)
			=> patchEditor.points.FirstOrDefault(p => p.Point == pt);

		Dictionary<PatchPointLine, int> patchboundaries = new Dictionary<PatchPointLine, int>();
		void AddBoundary(Link link0, Link link1) {
			var boundpt0 = link0.stringID;
			var boundpt1 = link1.stringID;
			var pto0 = points[link0];
			var pto1 = points[link1];
			var bound = new PatchPointLine() { point0 = pto0, point1 = pto1 };
			if (!patchboundaries.ContainsKey(bound)) {
				patchboundaries[bound] = 1;
			} else {
				patchboundaries[bound] = patchboundaries[bound] + 1;
			}
		}

		if (Display_Lines) {
			foreach (var patch in tpl.patches) {
				using (new Handles.DrawingScope(pointColor)) {
					var count = patch.points.Length;
					if (count != 4) throw new System.Exception("Unexpected patch points count!");

					var unityPoints = patch.points.Select(
						p => GetUnityPoint(points[p])).ToArray();
					var handleSize = HandleUtility.GetHandleSize(unityPoints[0].transform.position);
					var z = unityPoints[0].transform.position.z;
					DrawCubicBezier(handleSize, z, BezierHelpers.GetControlPoints<UnityPatchPointEditor>(
						unityPoints[0],
						unityPoints[2],
						p => new Vec2d(p.transform.position.x, p.transform.position.y), p => p.GlobalNormal.Rotate(-patchEditorRot), flip: true));
					DrawCubicBezier(handleSize, z, BezierHelpers.GetControlPoints<UnityPatchPointEditor>(
						unityPoints[3],
						unityPoints[1],
						p => new Vec2d(p.transform.position.x, p.transform.position.y), p => p.GlobalNormal.Rotate(-patchEditorRot), flip: true));
					AddBoundary(patch.points[0], patch.points[1]);
					AddBoundary(patch.points[2], patch.points[3]);
				}
			}

			foreach (var bound in patchboundaries) {
				if (bound.Value == 1) {
					using (new Handles.DrawingScope(pointColor)) {
						var pt1 = bound.Key.point0;
						var pt2 = bound.Key.point1;
						var upt1 = GetUnityPoint(pt1).transform.position;
						var upt2 = GetUnityPoint(pt2).transform.position;
						var handleSize = HandleUtility.GetHandleSize(upt1);
						Handles.DrawDottedLine(upt1, upt2, LineSize * handleSize * 2);
					}
				}
			}
		}
	}
	#endregion

	#region UI
	protected override void UpdateEditorFields() {
		base.UpdateEditorFields();
		DrawGUI = Event.current.type == EventType.Repaint;

		if (EditorApplication.isPlaying) {
			if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
				var c = Controller.MainContext;

				DrawHeader("Select texture");
				DrawUI_TextureSelection();

				if (!string.IsNullOrWhiteSpace(SelectedTextureFile)) {
					Path texPath = new Path(SelectedTextureFile);
					var tex = SelectedTexture;

					if (tex != null) {
						var t = tex.GetUnityTexture(Controller.MainContext);
						var heightFactor = t.HeightFactor;
						var unityTex = t.Texture;
						if (unityTex != null) {
							ZoomFactor = EditorGUI.Slider(GetNextRect(height: 25f), ZoomFactor, ZoomMin, ZoomMax);
							var height = CanvasHeight;
							if (Display_UseFullWidth) {
								var padding = 4f;
								var indent = 0f;
								var width = Mathf.Max(0f, EditorGUIUtility.currentViewWidth - padding * 2f - indent - (scrollbarShown ? scrollbarWidth : 0f));
								height = width * (unityTex.height / (float)unityTex.width);
							}
							var canvas = GetNextRect(height: height, vPadding: 4);
							Event e = Event.current;
							Rect subcanvas = Display_UseFullWidth
								? canvas
								: AdjustAspectRatio(canvas, unityTex.width, unityTex.height, centerVertically: false);
							YPos -= canvas.height - subcanvas.height;

							if(DrawGUI)
								EditorGUI.DrawRect(subcanvas, BackgroundColor);
							/*GUI.DrawTextureWithTexCoords(subcanvas,
								tex.GetUnityTexture(Controller.MainContext).Texture,
								new Rect(0, 0, 1, -1));*/

							// Calculate texture coordinates and limit zoom center
							var coord1 = ZoomCenter - new Vector2(0.5f / ZoomFactor, 0.5f / ZoomFactor);
							var coord2 = ZoomCenter + new Vector2(0.5f / ZoomFactor, 0.5f / ZoomFactor);
							if (coord1.x < 0f) ZoomCenter = ZoomCenter - new Vector2(coord1.x, 0f);
							if (coord1.y < 0f) ZoomCenter = ZoomCenter - new Vector2(0f, coord1.y);
							if (coord2.x > 1f) ZoomCenter = ZoomCenter - new Vector2(coord2.x - 1f, 0f);
							if (coord2.y > 1f) ZoomCenter = ZoomCenter - new Vector2(0f, coord2.y - 1f);
							coord1 = ZoomCenter - new Vector2(0.5f / ZoomFactor, 0.5f / ZoomFactor);
							coord2 = ZoomCenter + new Vector2(0.5f / ZoomFactor, 0.5f / ZoomFactor);

							var texCoords = new Rect(coord1.x, -coord1.y, coord2.x - coord1.x, -(coord2.y - coord1.y));
							if(DrawGUI)
								GUI.DrawTextureWithTexCoords(subcanvas,
								t.Texture,
								texCoords, AlphaBlending);

							var rects = DivideRectHorizontally(GetNextRect(), 4);
							AlphaBlending = EditorField("Alpha Blending", AlphaBlending, rect: rects[0]);
							if (EditorButton("Export as PNG", rect: rects[1])) {
								var defaultName = $"{texPath.GetFilenameWithoutExtension()}.png";
								string filePath = EditorUtility.SaveFilePanel("Output PNG file", "", defaultName, "png");

								if (!string.IsNullOrWhiteSpace(filePath))
									SaveAsPNG(tex, filePath, hasTransparency: true);
							}
							if (EditorButton("Export (No Transparency)", rect: rects[2])) {
								var defaultName = $"{texPath.GetFilenameWithoutExtension()}.rgb.png";
								string filePath = EditorUtility.SaveFilePanel("Output PNG file", "", defaultName, "png");

								if (!string.IsNullOrWhiteSpace(filePath))
									SaveAsPNG(tex, filePath, hasTransparency: false);
							}
							if (EditorButton("Export (Alpha Channel)", rect: rects[3])) {
								var defaultName = $"{texPath.GetFilenameWithoutExtension()}.a.png";
								string filePath = EditorUtility.SaveFilePanel("Output PNG file", "", defaultName, "png");

								if (!string.IsNullOrWhiteSpace(filePath))
									SaveAsPNG(tex, filePath, alphaChannelOnly: true);
							}

							/*if (t?.LinkedObject?.Header != null) {
								var header = t.LinkedObject.Header;
								rects = DivideRectHorizontally(GetNextRect(), 2);
								EditorField("WrapModeU", header.WrapModeU, rect: rects[0]);
								EditorField("WrapModeV", header.WrapModeV, rect: rects[1]);
							}*/

							var rect = GetNextRect();
							rect = EditorGUI.PrefixLabel(rect, new GUIContent("UV Source"));
							CurrentTab = (Tab)GUI.Toolbar(rect, (int)CurrentTab, new string[] { "Atlas Container", "Patch Bank" });
							switch (CurrentTab) {
								case Tab.AtlasContainer:
									DrawUI_Atlas(tex, subcanvas);
									break;
								case Tab.PatchBank:
									DrawUI_PatchBank(tex, subcanvas);
									break;
							}

							HandleEventsBatchMove(subcanvas);
							HandleEvents();
						}
					} else {
						if (EditorButton("Load")) {
							async Task LoadTex() {
								Controller.MainContext.Loader.LoadTexture(texPath, t => { });
								await Controller.MainContext.Loader.LoadLoop();
							}
							ExecuteTask(
								Controller.Obj.AdditionalLoad(
									LoadTex()
								)
							);
						}
					}
				}
			} else {
				EditorHelpBox("Loading...\nTo use this window, please wait until everything has loaded.", MessageType.Warning);
			}
		} else {
			EditorHelpBox("Please start the scene to use this window.", MessageType.Info);
		}
	}

	void DrawUI_Atlas(TextureCooked tex, Rect rect) {
		if (ResetAtlas) {
			ResetAtlas = false;
			ShowAtlas.Clear();
		}
		DrawAtlas(tex, rect);

		var atlas = tex.atlas;
		if (atlas != null) {
			EditorField("GridX", atlas.gridX);
			EditorField("GridY", atlas.gridY);

			var buttonsRect = GetNextRect();
			buttonsRect = EditorGUI.PrefixLabel(buttonsRect, new GUIContent("Show atlas"));
			var initialX = buttonsRect.xMin;
			bool MiniButton(string label, bool pressed) {
				var buttonRect = RowEntryRect(ref buttonsRect, buttonsRect.height * 3, getNextRect: () => {
					var r = GetNextRect();
					r.xMin = initialX;
					return r;
				});
				var oldColor = GUI.backgroundColor;
				if (pressed) GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
				if (GUI.Button(buttonRect, new GUIContent(label), EditorStyles.miniButton)) {
					if (pressed) GUI.backgroundColor = oldColor;
					return true;
				}
				if (pressed) GUI.backgroundColor = oldColor;
				return false;
			}
			bool allShown = ShowAtlas.All(st => st.Value == true);
			var showTemplateCopy = ShowAtlas.ToArray();
			if (MiniButton("All", allShown)) {
				foreach (var entry in showTemplateCopy) ShowAtlas[entry.Key] = !allShown;
				showTemplateCopy = ShowAtlas.ToArray();
			}
			int index = 0;
			foreach (var entry in showTemplateCopy) {
				if (MiniButton($"{entry.Key}", entry.Value)) {
					ShowAtlas[entry.Key] = !entry.Value;
				}
				index++;
			}

			DrawHeader("Atlas Editing");
			DrawUI_EditHelp(tex, atlas: atlas);
			var uvToolsRect = GetNextRect();
			var rectsRect = EditorGUI.PrefixLabel(uvToolsRect, new GUIContent("Quick UV Operations"));

			var rects = DivideRectHorizontally(rectsRect, 8);
			if (EditorButton("X x 2", rect: rects[0])) {
				ApplyUVOperation(atlas, new Vec2d(2, 1), Vec2d.Zero);
			}
			if (EditorButton("X / 2", rect: rects[1])) {
				ApplyUVOperation(atlas, new Vec2d(0.5f, 1), Vec2d.Zero);
			}
			if (EditorButton("Y x 2", rect: rects[2])) {
				ApplyUVOperation(atlas, new Vec2d(1, 2), Vec2d.Zero);
			}
			if (EditorButton("Y / 2", rect: rects[3])) {
				ApplyUVOperation(atlas, new Vec2d(1, 0.5f), Vec2d.Zero);
			}
			if (EditorButton("X + 0.5", rect: rects[4])) {
				ApplyUVOperation(atlas, Vec2d.One, new Vec2d(0.5f, 0f));
			}
			if (EditorButton("X - 0.5", rect: rects[5])) {
				ApplyUVOperation(atlas, Vec2d.One, new Vec2d(-0.5f, 0f));
			}
			if (EditorButton("Y + 0.5", rect: rects[6])) {
				ApplyUVOperation(atlas, Vec2d.One, new Vec2d(0, 0.5f));
			}
			if (EditorButton("Y - 0.5", rect: rects[7])) {
				ApplyUVOperation(atlas, Vec2d.One, new Vec2d(0, -0.5f));
			}
			uvToolsRect = GetNextRect();
			rectsRect = EditorGUI.PrefixLabel(uvToolsRect, new GUIContent("Custom UV operation"));
			rects = DivideRectHorizontally(rectsRect, 4);
			CustomUVOperationMultiply = Vec2dField(rects[0], CustomUVOperationMultiply, name: "Mul", labelWidth: 32);
			CustomUVOperationAdd = Vec2dField(rects[1], CustomUVOperationAdd, name: "Add", labelWidth: 32);
			CustomUVOperationUnit = (UVUnit)GUI.Toolbar(rects[2], (int)CustomUVOperationUnit, new string[] { "UV", "Pixels" });
			if (EditorButton("Apply", rect: rects[3])) {
				switch (CustomUVOperationUnit) {
					case UVUnit.UV:
						ApplyUVOperation(atlas, CustomUVOperationMultiply, CustomUVOperationAdd);
						break;
					case UVUnit.Pixels:
						var uTex = tex.GetUnityTexture(Controller.MainContext).Texture;
						var add = CustomUVOperationAdd / new Vec2d(uTex.width, uTex.height);
						ApplyUVOperation(atlas, CustomUVOperationMultiply, add);
						break;
				}
			}

			DrawUI_DisplayOptions(tex, atlas: atlas);
			DrawUI_Export(tex, atlas: atlas);

			DrawHeader("Other");
			if (GUI.Button(GetNextRect(), "Export to JSON (copy to clipboard)")) {
				var path = new Path(SelectedTextureFile).UncookedPath(Controller.MainContext).FullPath;
				var curUVEntries = new Dictionary<string, UbiArt.UV.UVAtlas>() {
					[path] = atlas
				};
				var atlasString = JsonConvert.SerializeObject(curUVEntries, Formatting.Indented);
				atlasString.CopyToClipboard();
				Debug.Log("Copied to clipboard.");
			}
		} else {
			DrawUI_DisplayOptions(tex);
		}
	}

	void DrawUI_PatchBank(TextureCooked tex, Rect rect) {
		DrawUI_PatchBankSelection();
		bool drewDisplayOptions = false;
		if (!string.IsNullOrWhiteSpace(SelectedPBKFile)) {
			Path pbkPath = new Path(SelectedPBKFile);

			var cache = Controller.MainContext?.Cache?.Structs;
			if (cache != null && cache.ContainsKey(typeof(AnimPatchBank)) && cache[typeof(AnimPatchBank)].ContainsKey(pbkPath.stringID)) {
				var pbk = cache[typeof(AnimPatchBank)][pbkPath.stringID] as AnimPatchBank;
				if (pbk != null) {
					DrawUI_PatchBankTemplateSelection(pbk);

					DrawPatchBank(tex, rect, pbk);

					if (CurrentPBKTemplate == -1) {
						var buttonsRect = GetNextRect();
						buttonsRect = EditorGUI.PrefixLabel(buttonsRect, new GUIContent("Show template"));
						var initialX = buttonsRect.xMin;
						bool MiniButton(string label, bool pressed) {
							var buttonRect = RowEntryRect(ref buttonsRect, buttonsRect.height * 7, getNextRect: () => {
								var r = GetNextRect();
								r.xMin = initialX;
								return r;
							});
							var oldColor = GUI.backgroundColor;
							if (pressed) GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
							if (GUI.Button(buttonRect, new GUIContent(label), EditorStyles.miniButton)) {
								if (pressed) GUI.backgroundColor = oldColor;
								return true;
							}
							if (pressed) GUI.backgroundColor = oldColor;
							return false;
						}
						bool allShown = ShowTemplate.All(st => st.Value == true);
						var showTemplateCopy = ShowTemplate.ToArray();
						if (MiniButton("Show all", allShown)) {
							foreach (var entry in showTemplateCopy) ShowTemplate[entry.Key] = !allShown;
							showTemplateCopy = ShowTemplate.ToArray();
						}
						IsShowTemplateOpen = EditorGUI.BeginFoldoutHeaderGroup(GetNextRect(), IsShowTemplateOpen, "Templates");
						if (IsShowTemplateOpen) {
							buttonsRect = GetNextRect();
							initialX = buttonsRect.xMin;
							int index = 0;
							foreach (var entry in showTemplateCopy) {
								string entryName = $"{index} - {entry.Key:X8}";
								try {
									var keySID = new StringID((uint)entry.Key);
									if (Controller.MainContext.StringCache.ContainsKey(keySID)) {
										entryName = keySID.ToString(Controller.MainContext, shortString: true);
									}
								} catch (Exception) { }
								if (MiniButton(entryName, entry.Value)) {
									ShowTemplate[entry.Key] = !entry.Value;
								}
								index++;
							}
						}
						EditorGUI.EndFoldoutHeaderGroup();
						DrawHeader("Patch Bank Editing");
						DrawUI_EditHelp(tex, pbk: pbk);
						if (EditorButton("Create Template Meshes (Not recommended for multiple templates)")) {
							for (int tpl_i = 0; tpl_i < pbk.templates.Count; tpl_i++) {
								var tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
								bool IsDisplayingSingleTemplate = tpl_i == CurrentPBKTemplate;
								if (CurrentPBKTemplate == -1) {
									if (!ShowTemplate.ContainsKey(tplKey)) {
										ShowTemplate[tplKey] = true;
									}
									if (!ShowTemplate[tplKey]) continue;
								} else if (!IsDisplayingSingleTemplate) continue;

								CreateTemplateGameObject(pbk, tpl_i);
							}
						}
					} else {
						DrawHeader("Patch Bank Editing");
						DrawUI_EditHelp(tex, pbk: pbk);
						if (EditorButton("Create Template Meshes (Start editing)")) {
							CreateTemplateGameObject(pbk, CurrentPBKTemplate);
						}
					}

					var uvToolsRect = GetNextRect();
					var rectsRect = EditorGUI.PrefixLabel(uvToolsRect, new GUIContent("Quick UV Operations"));

					var rects = DivideRectHorizontally(rectsRect, 8);
					if (EditorButton("x 2", rect: rects[0])) {
						ApplyUVOperation(pbk, new Vec2d(2, 2), Vec2d.Zero);
					}
					if (EditorButton("/ 2", rect: rects[1])) {
						ApplyUVOperation(pbk, new Vec2d(0.5f, 0.5f), Vec2d.Zero);
					}
					if (EditorButton("X + 0.5", rect: rects[2])) {
						ApplyUVOperation(pbk, Vec2d.One, new Vec2d(0.5f, 0f));
					}
					if (EditorButton("X - 0.5", rect: rects[3])) {
						ApplyUVOperation(pbk, Vec2d.One, new Vec2d(-0.5f, 0f));
					}
					if (EditorButton("Y + 0.5", rect: rects[4])) {
						ApplyUVOperation(pbk, Vec2d.One, new Vec2d(0, 0.5f));
					}
					if (EditorButton("Y - 0.5", rect: rects[5])) {
						ApplyUVOperation(pbk, Vec2d.One, new Vec2d(0, -0.5f));
					}
					if (EditorButton("90° CW", rect: rects[6])) {
						ApplyUVOperation(pbk, Vec2d.One, Vec2d.Zero, rot: 90);
					}
					if (EditorButton("90° CCW", rect: rects[7])) {
						ApplyUVOperation(pbk, Vec2d.One, Vec2d.Zero, rot: -90);
					}
					uvToolsRect = GetNextRect();
					rectsRect = EditorGUI.PrefixLabel(uvToolsRect, new GUIContent("Custom UV operation"));
					rects = DivideRectHorizontally(rectsRect, 4);
					CustomUVOperationMultiplyUniform = CustomFloatField("Mul", CustomUVOperationMultiplyUniform, rectToUse: rects[0], labelWidth: 32, limitLower: 0.001f, limitUpper: 100f);
					CustomUVOperationAdd = Vec2dField(rects[1], CustomUVOperationAdd, name: "Add", labelWidth: 32);
					CustomUVOperationUnit = (UVUnit)GUI.Toolbar(rects[2], (int)CustomUVOperationUnit, new string[] { "UV", "Pixels" });
					if (EditorButton("Apply", rect: rects[3])) {
						var limit = Mathf.Clamp(CustomUVOperationMultiplyUniform, 0.001f, 100f);
						var mul = new Vec2d(limit, limit);
						switch (CustomUVOperationUnit) {
							case UVUnit.UV:
								ApplyUVOperation(pbk, mul, CustomUVOperationAdd);
								break;
							case UVUnit.Pixels:
								var uTex = tex.GetUnityTexture(Controller.MainContext).Texture;
								var add = CustomUVOperationAdd / new Vec2d(uTex.width, uTex.width);
								ApplyUVOperation(pbk, mul, add);
								break;
						}
					}
					
					DrawUI_DisplayOptions(tex, pbk: pbk);
					drewDisplayOptions = true;
					DrawUI_Export(tex, pbk: pbk);
				}
			} else {
				if (EditorButton("Load")) {
					async Task LoadPBK() {
						Controller.MainContext.Loader.LoadFile<AnimPatchBank>(pbkPath, t => { });
						await Controller.MainContext.Loader.LoadLoop();
					}
					ExecuteTask(
						Controller.Obj.AdditionalLoad(
							LoadPBK()
						)
					);
				}
			}
		}
		if (!drewDisplayOptions) {
			DrawUI_DisplayOptions(tex);
		}
	}

	void DrawUI_TextureSelection() {
		var c = Controller.MainContext;
		string[] extensions = new string[] {
			$"*.tga{((c.Settings.Cooked && !c.Settings.PastaStructure) ? ".ckd" : "")}",
			$"*.png{((c.Settings.Cooked && !c.Settings.PastaStructure) ? ".ckd" : "")}"
		};
		Rect rect = GetNextRect(vPaddingBottom: 4f);
		string buttonString = "No texture selected";
		if (SelectedTextureFile != null) {
			buttonString = System.IO.Path.GetFileName(SelectedTextureFile);
			if (buttonString.EndsWith(".ckd")) buttonString = buttonString.Substring(0, buttonString.Length - 4);
		}
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Texture"));
		if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
			string directory = (Controller.MainContext.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
			if (!directory.EndsWith("/")) directory += "/";
			while (directory.Contains("//")) directory = directory.Replace("//", "/");

			if (recheckFiles || Dropdown == null || Dropdown.directory != directory || Dropdown.extensions == null || !Enumerable.SequenceEqual(Dropdown.extensions, extensions) || Dropdown.mode != c.Settings.Mode || Dropdown.displayFullPaths != UnitySettings.DisplayFullPathsInDropdowns) {
				Dropdown = new FileSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory, extensions) {
					name = "Texture files",
					mode = c.Settings.Mode,
					displayFullPaths = UnitySettings.DisplayFullPathsInDropdowns
				};
				recheckFiles = false;
			}
			Dropdown.Show(rect);
		}
		if (Dropdown != null && Dropdown.selection != null) {
			SelectedTextureFile = Dropdown.selection;
			ZoomFactor = 1f;
			ResetPBK = true;
			ResetAtlas = true;
			Dropdown.selection = null;
			Dirty = true;
		}
		EditorGUI.TextArea(GetNextRect(), SelectedTextureFile?.RemoveCKD());
	}

	void DrawUI_PatchBankSelection() {
		var c = Controller.MainContext;
		string[] PBKextensions = new string[] {
			$"*.pbk{(c.Settings.Cooked ? ".ckd" : "")}"
		};
		Rect rect = GetNextRect(vPaddingBottom: 4f);
		if (ResetPBK) {
			SelectedPBKFile = null;
			CurrentPBKTemplate = -1;
			if (PBKDropdown != null)
				PBKDropdown.selection = null;
			ResetPBK = false;
			ShowTemplate.Clear();
		}
		string buttonString = "No patch bank selected";
		if (SelectedPBKFile != null) {
			buttonString = System.IO.Path.GetFileName(SelectedPBKFile);
			if (buttonString.EndsWith(".ckd")) buttonString = buttonString.Substring(0, buttonString.Length - 4);
		}
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Patch Bank"));
		if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
			string directory = (Controller.MainContext.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
			if (!directory.EndsWith("/")) directory += "/";
			while (directory.Contains("//")) directory = directory.Replace("//", "/");

			if (recheckFiles || PBKDropdown == null || PBKDropdown.directory != directory || PBKDropdown.extensions == null || !Enumerable.SequenceEqual(PBKDropdown.extensions, PBKextensions) || PBKDropdown.mode != c.Settings.Mode || Dropdown.displayFullPaths != UnitySettings.DisplayFullPathsInDropdowns) {
				PBKDropdown = new FileSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory, PBKextensions) {
					name = "Patch bank files",
					mode = c.Settings.Mode,
					displayFullPaths = UnitySettings.DisplayFullPathsInDropdowns
				};
				recheckFiles = false;
			}
			PBKDropdown.Show(rect);
		}
		if (PBKDropdown != null && PBKDropdown.selection != null) {
			SelectedPBKFile = PBKDropdown.selection;
			PBKDropdown.selection = null;
			Dirty = true;
			CurrentPBKTemplate = -1;
		}
		EditorGUI.TextArea(GetNextRect(), SelectedPBKFile?.RemoveCKD());
	}

	void DrawUI_PatchBankTemplateSelection(AnimPatchBank pbk) {
		var c = Controller.MainContext;

		Rect rect = GetNextRect(vPaddingBottom: 4f);
		if (ResetPBK) {
			SelectedPBKFile = null;
			if (PBKDropdown != null)
				PBKDropdown.selection = null;
			ResetPBK = false;
			ShowTemplate.Clear();
		}
		string buttonString = "All";
		if (CurrentPBKTemplate != -1) {
			var tpl = pbk.templates[CurrentPBKTemplate];
			var tplKey = pbk.templateKeys.GetKeyFromValue(CurrentPBKTemplate);
			var sid = new StringID((uint)tplKey);
			var name = $"{CurrentPBKTemplate} - {sid.ToString(Controller.MainContext)}";
			buttonString = name;
		}
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Template"));
		if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
			if (recheckFiles || TemplateDropdown == null || TemplateDropdown.pbk != pbk) {
				TemplateDropdown = new PatchBankTemplateDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), pbk) {
					name = "Patch bank templates"
				};
			}
			TemplateDropdown.Show(rect);
		}
		if (TemplateDropdown != null && TemplateDropdown.selection != null) {
			CurrentPBKTemplate = TemplateDropdown.selection.Value;
			TemplateDropdown.selection = null;
			Dirty = true;
		}

		/*if (CurrentPBKTemplate != -1) {
			patchHLevel = EditorField("Patch H Level", patchHLevel);
			patchVLevel = EditorField("Patch V Level", patchVLevel);
		}*/
	}

	void DrawUI_EditHelp(TextureCooked tex, AnimPatchBank pbk = null, UVAtlas atlas = null) {
		if (pbk != null) {
			if (Controller.MainContext.Settings.EngineVersion < EngineVersion.RL) {
				EditorHelpBox("PBK editing is not fully supported for Rayman Origins and Jungle/Fiesta Run.\nUse this feature at your own risk.", MessageType.Warning);
			}
			if (CurrentPBKTemplate == -1) {
				EditorHelpBox("It is not recommended to use the editing functionality with multiple templates active.\nPlease select only the template you want to edit before using Create Template Meshes.", MessageType.Warning);
			}
			var patchEditors = FindObjectsOfType<UnityPatchEditor>().Where(p => p.texture == SelectedTexture && p.pbk == pbk && (CurrentPBKTemplate == -1 || p.templateIndex == CurrentPBKTemplate));
			if (!patchEditors.Any()) {
				EditorHelpBox("To be able to modify bone transforms and sprite meshes, first use the Create Template Meshes function for this PBK and texture.", MessageType.Info);
			}
			IsEditHelpOpen = EditorGUI.BeginFoldoutHeaderGroup(GetNextRect(), IsEditHelpOpen, "Help & controls");
			if(IsEditHelpOpen) {
				var controlsRect = GetNextRect(height: 160, padding: 4, vPadding: 4);
				EditorGUI.LabelField(controlsRect, "Single point controls: LMB: Move point - RMB: Rotate normal\n" +
					"Multi-point controls: Shift+LMB: Move all - Shift+RMB: Rotate all - Ctrl+LMB: Scale all\n" +
					"Single point operations affect both the UVs (texture positions) and mesh (physical area in animations).\n" +
					"Multi-point operations only affect the UVs, but you can hold ALT to also affect the mesh.\n\n" +
					"After creating the template meshes, you can edit the rig of this sprite mesh by editing the bone objects' transforms in Scene View. On point objects, you can also change which bone each point is deformed by during animations. Editing rigs is tricky, so it is recommended to keep an animation playing and to use \"Sync changes with animations\" to test your changes.", EditorStyles.wordWrappedLabel);
			}
			EditorGUI.EndFoldoutHeaderGroup();
		} else if (atlas != null) {
			IsEditHelpOpen = EditorGUI.BeginFoldoutHeaderGroup(GetNextRect(), IsEditHelpOpen, "Help & controls");
			if (IsEditHelpOpen) {
				var controlsRect = GetNextRect(height: 100, padding: 4, vPadding: 4);
				EditorGUI.LabelField(controlsRect, "Single point controls: LMB: Move point\n" +
					"Multi-point controls: Shift+LMB: Move all - Shift+RMB: Rotate all - Ctrl+LMB: Scale all\n" +
					"Multi-point operations affect every visible sprite, so it is important to only display the one you want to edit.\n\n" +
					"Be careful to avoid moving the pivot point (marked by a +), as this is the center point of the atlas in the game. Usually, this is the average of all points, so you will need to move different points to keep the pivot in place.", EditorStyles.wordWrappedLabel);
			}
			EditorGUI.EndFoldoutHeaderGroup();
		}
	}

	void DrawUI_DisplayOptions(TextureCooked tex, AnimPatchBank pbk = null, UVAtlas atlas = null) {
		DrawHeader("Display Options");
		if (pbk != null || atlas != null) {
			Display_Points = EditorField("Display points", Display_Points);
			Display_Lines = EditorField("Display lines", Display_Lines);
			if (pbk != null) {
				Display_Normals = EditorField("Display normals", Display_Normals);
			} else if (atlas != null) {
				Display_Pivots = EditorField("Display pivots", Display_Pivots);
			}
		}
		Display_UseFullWidth = EditorField("Use full width", Display_UseFullWidth);
	}
	void DrawUI_Export(TextureCooked tex, AnimPatchBank pbk = null, UVAtlas atlas = null) {
		DrawHeader(pbk != null ? "Patch Bank PNG Export" : "Atlas PNG Export");
		if (EditorButton("Export as PNG")) {
			var defaultName = $"{new Path(SelectedTextureFile).GetFilenameWithoutExtension()}.png";
			string filePath = EditorUtility.SaveFilePanel("Output PNG file", "", defaultName, "png");

			if (!string.IsNullOrWhiteSpace(filePath)) {
				if (pbk != null)
					SavePatchBankAsPNG(tex, filePath, pbk);
				else if (atlas != null)
					SaveAtlasAsPNG(tex, filePath, atlas);
			}
		}

		PatchBankExport_IncludeImage = EditorField("Include image", PatchBankExport_IncludeImage);
		PatchBankExport_Scale = EditorField("Scale", PatchBankExport_Scale);
		PatchBankExport_Padding = EditorField("Padding", PatchBankExport_Padding);
		PatchBankExport_PointSize = EditorField("Point size", PatchBankExport_PointSize);
		PatchBankExport_LineSize = EditorField("Line size", PatchBankExport_LineSize);
		if (pbk != null) {
			PatchBankExport_IncludeNormals = EditorField("Include normals", PatchBankExport_IncludeNormals);
		} else if (atlas != null) {
			PatchBankExport_PivotSize = EditorField("Pivot size", PatchBankExport_PivotSize);
		}
	}
	#endregion

	#region Image export
	void SavePatchBankAsPNG(TextureCooked tex, string outputPath, AnimPatchBank pbk) {
		Texture2D tex2d = tex.GetUnityTexture(Controller.MainContext).Texture;
		using MagickImage img = PatchBankExport_IncludeImage
			? new MagickImage(tex2d.Decompress().EncodeToPNG(), MagickFormat.Png)
			: new MagickImage(MagickColors.Transparent, tex2d.width, tex2d.height);

		// Make sure it's not vertically flipped!
		img.Flip();

		// Optionally resize
		if (PatchBankExport_Scale != 1) {
			//img.Interpolate = PixelInterpolateMethod.Integer;
			//img.FilterType = FilterType.Point;
			img.Resize(new Percentage(PatchBankExport_Scale * 100));
		}

		// Optionally add padding
		if (PatchBankExport_Padding != 0) {
			img.Extent(img.Width + PatchBankExport_Padding * 2, img.Height + PatchBankExport_Padding * 2, Gravity.Center, MagickColors.Transparent);
		}

		// Add UVs
		DrawPatchBankToImage(img, new Rect(PatchBankExport_Padding, PatchBankExport_Padding, img.Width - PatchBankExport_Padding * 2, img.Height - PatchBankExport_Padding * 2), pbk);

		// Save
		img.Write(outputPath);
	}

	// Re-implemented from DrawPatchBank
	void DrawPatchBankToImage(MagickImage img, Rect rect, AnimPatchBank pbk) {
		DrawableStrokeWidth lineSize = new(PatchBankExport_LineSize);

		Vector2 GetRectPos(Vector2 pos) => rect.position + pos * rect.width;
		void DrawPoint(Vector2 uvPos, DrawableFillColor fillColor) {
			img.Draw(fillColor, new DrawableRectangle(
				upperLeftX: uvPos.x - PatchBankExport_PointSize / 2,
				upperLeftY: uvPos.y - PatchBankExport_PointSize / 2,
				lowerRightX: uvPos.x + PatchBankExport_PointSize / 2,
				lowerRightY: uvPos.y + PatchBankExport_PointSize / 2));
		}
		void DrawLine(Vector2 pos1, Vector2 pos2, DrawableStrokeColor strokeColor, float? width = null) {
			DrawableStrokeWidth lineThickness = lineSize;
			if (width.HasValue) lineThickness = new DrawableStrokeWidth(width.Value);
			img.Draw(strokeColor, lineThickness, new DrawableLine(pos1.x, pos1.y, pos2.x, pos2.y));
		}
		MagickColor GetMagickColor(UnityEngine.Color unityColor) {
			return new MagickColor(
				(byte)(unityColor.r * 255),
				(byte)(unityColor.g * 255),
				(byte)(unityColor.b * 255));
		}
		void DrawCubicBezier(Vec2d[] controlPoints, DrawableStrokeColor strokeColor) {
			const int numPoints = 16;

			Vec2d[] bezierPoints = Enumerable.Range(0, numPoints).
				Select(i => BezierHelpers.CalculateCubicBezierPoint(i / (float)(numPoints - 1), controlPoints)).
				ToArray();
			Vector2[] pointsConv = bezierPoints.
				Select(p => GetRectPos(p.GetUnityVector())).
				ToArray();

			for (int i = 0; i < bezierPoints.Length - 1; i++) {
				DrawLine(pointsConv[i], pointsConv[i+1], strokeColor);
			}
		}

		UnityEngine.Random.InitState(12675);
		for (int tpl_i = 0; tpl_i < pbk.templates.Count; tpl_i++) {
			AnimTemplate tpl = pbk.templates[tpl_i];
			ulong tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
			if (CurrentPBKTemplate == -1) {
				ShowTemplate.TryAdd(tplKey, true);
				if (!ShowTemplate[tplKey])
					continue;
			} else if (tpl_i != CurrentPBKTemplate) {
				continue;
			}

			// Create a random color
			var pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
			var fillColor = new DrawableFillColor(GetMagickColor(pointColor));
			var strokeColor = new DrawableStrokeColor(GetMagickColor(pointColor));

			// Draw points
			foreach (var point in tpl.patchPoints) {
				var uvPos = GetRectPos(point.uv.GetUnityVector());

				DrawPoint(uvPos, fillColor);

				// Draw normal
				if (PatchBankExport_IncludeNormals) {
					Vec2d normalTest = point.uv + point.normal * 0.01f;
					Vector2 normalTestPos = GetRectPos(normalTest.GetUnityVector());
					DrawLine(uvPos, normalTestPos, new DrawableStrokeColor(MagickColors.White));
				}
			}

			int patchesCount = tpl.patches.Count;
			int curPatch = 0;


			Dictionary<Link, AnimPatchPoint> points = tpl.patchPoints.ToDictionary(p => p.key, p => p);

			Dictionary<PatchPointLine, int> patchboundaries = new Dictionary<PatchPointLine, int>();
			void AddBoundary(Link link0, Link link1) {
				var boundpt0 = link0.stringID;
				var boundpt1 = link1.stringID;
				var pto0 = points[link0];
				var pto1 = points[link1];
				var bound = new PatchPointLine() { point0 = pto0, point1 = pto1 };
				if (!patchboundaries.ContainsKey(bound)) {
					patchboundaries[bound] = 1;
				} else {
					patchboundaries[bound] = patchboundaries[bound] + 1;
				}
			}

			foreach (var patch in tpl.patches) {
				var count = patch.points.Length;
				if (count != 4) throw new System.Exception("Unexpected patch points count!");

				DrawCubicBezier(BezierHelpers.GetControlPoints<AnimPatchPoint>(
					points[patch.points[0]],
					points[patch.points[2]],
					p => p.uv, p => p.normal), strokeColor);
				DrawCubicBezier(BezierHelpers.GetControlPoints<AnimPatchPoint>(
					points[patch.points[3]],
					points[patch.points[1]],
					p => p.uv, p => p.normal), strokeColor);
				AddBoundary(patch.points[0], patch.points[1]);
				AddBoundary(patch.points[2], patch.points[3]);
				curPatch++;
			}

			foreach (var bound in patchboundaries) {
				if (bound.Value == 1) {
					var pt1 = bound.Key.point0;
					var pt2 = bound.Key.point1;
					var uv1 = pt1.uv;
					var uv2 = pt2.uv;
					var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
					var uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
					DrawLine(uvPos1, uvPos2, strokeColor);
				}
			}
		}
	}

	void SaveAtlasAsPNG(TextureCooked tex, string outputPath, UVAtlas atlas) {
		Texture2D tex2d = tex.GetUnityTexture(Controller.MainContext).Texture;
		using MagickImage img = PatchBankExport_IncludeImage
			? new MagickImage(tex2d.Decompress().EncodeToPNG(), MagickFormat.Png)
			: new MagickImage(MagickColors.Transparent, tex2d.width, tex2d.height);

		// Make sure it's not vertically flipped!
		img.Flip();

		// Optionally resize
		if (PatchBankExport_Scale != 1) {
			//img.Interpolate = PixelInterpolateMethod.Integer;
			//img.FilterType = FilterType.Point;
			img.Resize(new Percentage(PatchBankExport_Scale * 100));
		}

		// Optionally add padding
		if (PatchBankExport_Padding != 0) {
			img.Extent(img.Width + PatchBankExport_Padding * 2, img.Height + PatchBankExport_Padding * 2, Gravity.Center, MagickColors.Transparent);
		}

		// Add UVs
		DrawAtlasToImage(img, new Rect(PatchBankExport_Padding, PatchBankExport_Padding, img.Width - PatchBankExport_Padding * 2, img.Height - PatchBankExport_Padding * 2), atlas);

		// Save
		img.Write(outputPath);
	}

	void DrawAtlasToImage(MagickImage img, Rect rect, UVAtlas atlas) {
		DrawableStrokeWidth lineSize = new(PatchBankExport_LineSize);

		Vector2 GetRectPos(Vector2 pos) => rect.position + Vector2.Scale(pos, rect.size);
		void DrawPoint(Vector2 uvPos, DrawableFillColor fillColor) {
			img.Draw(fillColor, new DrawableRectangle(
				upperLeftX: uvPos.x - PatchBankExport_PointSize / 2,
				upperLeftY: uvPos.y - PatchBankExport_PointSize / 2,
				lowerRightX: uvPos.x + PatchBankExport_PointSize / 2,
				lowerRightY: uvPos.y + PatchBankExport_PointSize / 2));
		}
		void DrawLine(Vector2 pos1, Vector2 pos2, DrawableStrokeColor strokeColor, float? width = null) {
			DrawableStrokeWidth lineThickness = lineSize;
			if(width.HasValue) lineThickness = new DrawableStrokeWidth(width.Value);
			img.Draw(strokeColor, lineThickness, new DrawableLine(pos1.x, pos1.y, pos2.x, pos2.y));
		}
		void DrawPivot(Vector2 uvPos, float pivotSize, DrawableStrokeColor strokeColor) {
			DrawLine(uvPos - new Vector2(pivotSize / 2f, 0f), uvPos + new Vector2(pivotSize / 2f, 0f), strokeColor);//, width: pivotThickness);
			DrawLine(uvPos - new Vector2(0f, pivotSize / 2f), uvPos + new Vector2(0f, pivotSize / 2f), strokeColor);//, width: pivotThickness);
		}
		MagickColor GetMagickColor(UnityEngine.Color unityColor) {
			return new MagickColor(
				(byte)(unityColor.r * 255),
				(byte)(unityColor.g * 255),
				(byte)(unityColor.b * 255));
		}

		UnityEngine.Random.InitState(12675);

		// Draw grid
		if (atlas.gridX > 1f) {
			var col = new DrawableStrokeColor(GetMagickColor(Color.grey));
			float step = 1f / atlas.gridX;
			for (float i = step; i < 1f; i += step) {
				var uvPos1 = GetRectPos(new Vector2(i, 0));
				var uvPos2 = GetRectPos(new Vector2(i, 1));
				DrawLine(uvPos1, uvPos2, col);
			}
		}
		if (atlas.gridY > 1f) {
			var col = new DrawableStrokeColor(GetMagickColor(Color.grey));
			float step = 1f / atlas.gridY;
			for (float i = step; i < 1f; i += step) {
				var uvPos1 = GetRectPos(new Vector2(0, i));
				var uvPos2 = GetRectPos(new Vector2(1, i));
				DrawLine(uvPos1, uvPos2, col);
			}
		}

		// Draw UVs
		foreach (var uvPair in atlas.uvData) {
			var tplKey = uvPair.Key;
			if (!ShowAtlas.ContainsKey(tplKey)) {
				ShowAtlas[tplKey] = true;
			}
			if (!ShowAtlas[tplKey]) continue;

			UVdata uvdata = uvPair.Value;

			var pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
			var fillColor = new DrawableFillColor(GetMagickColor(pointColor));
			var strokeColor = new DrawableStrokeColor(GetMagickColor(pointColor));

			// Draw pivot
			Vec2d pivotUv = null;
			if (atlas.pivots != null && atlas.pivots.ContainsKey(uvPair.Key)) {
				var pivot = atlas.pivots[uvPair.Key];
				pivotUv = new Vec2d(pivot.x, pivot.y);
			} else {
				// Draw cross in the center
				var pivot = Vec2d.Zero;
				if (uvdata.uvs?.Any() ?? false) {
					pivot = new Vec2d(uvdata.uvs.Average(u => u.x), uvdata.uvs.Average(u => u.y));
				} else if (uvdata.uv0 != null && uvdata.uv1 != null) {
					pivot = (uvdata.uv0 + uvdata.uv1) / 2f; 
				}
				pivotUv = pivot;
			}
			if (pivotUv != null) {
				var uvPos = GetRectPos(pivotUv.GetUnityVector());
				DragUVCenter = uvPos;
				DrawPivot(uvPos, PatchBankExport_PivotSize, strokeColor);
			}

			// Draw points
			void DrawUV(Vec2d uv) {
				var uvPos = GetRectPos(new Vector2(uv.x, uv.y));
				DrawPoint(uvPos, fillColor);
			}
			if (uvdata.uvs != null) {
				for (int i = 0; i < uvdata.uvs.Count; i++) {
					DrawUV(uvdata.uvs[i]);
				}
			} else {
				if(uvdata.uv0 != null) DrawUV(uvdata.uv0);
				if(uvdata.uv1 != null) DrawUV(uvdata.uv1);
			}

			// Connect points
			int count = uvdata.uvs?.Count ?? 0;
			if (count == 2 || uvdata.uvs == null) {
				var uv1 = uvdata.uvs?[0] ?? uvdata.uv0;
				var uv2 = uvdata.uvs?[1] ?? uvdata.uv1;
				var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
				var uvPos2 = GetRectPos(new Vector2(uv1.x, uv2.y));
				DrawLine(uvPos1, uvPos2, strokeColor);
				uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
				uvPos2 = GetRectPos(new Vector2(uv2.x, uv1.y));
				DrawLine(uvPos1, uvPos2, strokeColor);
				uvPos1 = GetRectPos(new Vector2(uv2.x, uv1.y));
				uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
				DrawLine(uvPos1, uvPos2, strokeColor);
				uvPos1 = GetRectPos(new Vector2(uv1.x, uv2.y));
				uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
				DrawLine(uvPos1, uvPos2, strokeColor);
			} else {
				bool drawn = false;
				if (atlas.uvParams != null && atlas.uvParams.ContainsKey(uvPair.Key)) {
					var uvp = atlas.uvParams[uvPair.Key];
					if (uvp.triangles != null && uvp.triangles.Any()) {
						drawn = true;
						foreach (var tri in uvp.triangles) {
							var uv1 = uvdata.uvs[tri.vertex1];
							var uv2 = uvdata.uvs[tri.vertex2];
							var uv3 = uvdata.uvs[tri.vertex3];
							var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
							var uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
							var uvPos3 = GetRectPos(new Vector2(uv3.x, uv3.y));
							DrawLine(uvPos1, uvPos2, strokeColor);
							DrawLine(uvPos2, uvPos3, strokeColor);
							DrawLine(uvPos3, uvPos1, strokeColor);
						}
					}
				}
				if (!drawn) {
					for (int i = 0; i < count; i++) {
						var uv1 = uvdata.uvs[i];
						var uv2 = uvdata.uvs[(i + 1) % count];
						var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
						var uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
						DrawLine(uvPos1, uvPos2, strokeColor);
					}
				}
			}
		}
	}

	void SaveAsPNG(TextureCooked tex, string outputPath, bool hasTransparency = true, bool alphaChannelOnly = false) {
		var tex2d = tex.GetUnityTexture(Controller.MainContext).Texture;
		byte[] png;
		if (alphaChannelOnly) {
			png = tex2d.Copy(alphaChannelOnly: true, flipY: true).EncodeToPNG();
		} else {
			if (hasTransparency) {
				png = tex2d.Copy(flipY: true).EncodeToPNG();
			} else {
				png = tex2d.Copy(removeTransparency: true, flipY: true).EncodeToPNG();
			}
		}

		Util.ByteArrayToFile(outputPath, png);
	}
	#endregion

	void DrawPatchBank(TextureCooked tex, Rect rect, AnimPatchBank pbk) {
		var t = tex?.GetUnityTexture(Controller.MainContext);
		var heightFactor = t?.HeightFactor;
		GUI.BeginGroup(rect);
		Vector2 GetRectPos(Vector2 pos)
			=> ToRectPosition(rect, pos, atlas: false);
		Vector2 GetTexturePos(Vector2 pos)
			=> ToTexturePosition(rect, pos, atlas: false);
		if (pbk != null) {
			var lineSize = LineSize;
			Handles.BeginGUI();
			UnityEngine.Random.InitState(12675);
			int pointIndex = 0;
			var patchEditorsAll = FindObjectsOfType<UnityPatchEditor>().Where(p => p.texture == SelectedTexture && p.pbk == pbk);
			for (int tpl_i = 0; tpl_i < pbk.templates.Count; tpl_i++) {
				var tpl = pbk.templates[tpl_i];
				var tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
				bool IsDisplayingSingleTemplate = tpl_i == CurrentPBKTemplate;
				if (CurrentPBKTemplate == -1) {
					if (!ShowTemplate.ContainsKey(tplKey)) {
						ShowTemplate[tplKey] = true;
					}
					if (!ShowTemplate[tplKey]) continue;
				} else if (!IsDisplayingSingleTemplate) continue;

				Dictionary<Link, AnimPatchPoint> points = tpl.patchPoints.ToDictionary(p => p.key, p => p);
				var pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);

				var center = tpl.patchPoints.Any() ? new Vec2d(
					tpl.patchPoints.Average(p => p.uv.x),
					tpl.patchPoints.Average(p => p.uv.y)) : Vec2d.Zero;
				var centerRectPos = GetRectPos(center.GetUnityVector());
				DragUVCenter = centerRectPos;


				var patchEditors = patchEditorsAll.Where(p => p.templateIndex == tpl_i);
				bool hasPatchEditors = patchEditors?.Any() ?? false;
				bool isBatchDragUVOnly = (CurrentUVEditAction != UVEditAction.None && DragOnlyUV);
				bool isEditable = hasPatchEditors || isBatchDragUVOnly;

				bool recalculatePatchEditors = false;
				bool syncWithAnimations = false;

				// Draw points
				foreach (var point in tpl.patchPoints) {
					var uvPos = GetRectPos(point.uv.GetUnityVector());
					var newPos = DrawPoint(pointIndex, uvPos, pointColor, editable: isEditable);
					if (newPos != uvPos) {
						var newUVposTex = GetTexturePos(newPos);
						point.uv = newUVposTex.GetUbiArtVector();

						// Update patch editors
						if (hasPatchEditors) {
							if (isBatchDragUVOnly) {
								recalculatePatchEditors = true;
							} else {
								foreach (var pe in patchEditors) {
									var unityPoint = pe.points.FirstOrDefault(p => p.Point == point);
									unityPoint.UpdatePositionFromUV();
								}
							}
						} else {
							syncWithAnimations = true;
						}
					}
					var normalTest = point.uv + (point.normal * NormalLength);
					//Debug.Log(point.normal.MagnitudeFloat);
					var endPos = GetRectPos(normalTest.GetUnityVector());
					var newEndPos = DrawNormal(newPos, endPos, editable: isEditable);
					if (newEndPos != endPos) {
						var newEndposTex = GetTexturePos(newEndPos).GetUbiArtVector();
						var newNormal = (newEndposTex - point.uv) / NormalLength;
						var normalized = newNormal.Normalize();
						point.normal = normalized;
						if (hasPatchEditors) {
							if (isBatchDragUVOnly) {
								recalculatePatchEditors = true;
							} else {
								foreach (var pe in patchEditors) {
									var unityPoint = pe.points.FirstOrDefault(p => p.Point == point);
									unityPoint.UpdateNormalFromUV();
								}
							}
						} else {
							syncWithAnimations = true;
						}
					}

					pointIndex++;

				}
				if (syncWithAnimations) {
					SyncChangesWithAnimations(pbk, tpl);
				}
				if (recalculatePatchEditors) {
					foreach (var pe in patchEditors) {
						pe.RecalculatePointTransformations();
						pe.ResetUVs();
					}
				}
				int patchesCount = tpl.patches.Count;
				int curPatch = 0;

				Dictionary<PatchPointLine, int> patchboundaries = new Dictionary<PatchPointLine, int>();
				void AddBoundary(Link link0, Link link1) {
					var boundpt0 = link0.stringID;
					var boundpt1 = link1.stringID;
					var pto0 = points[link0];
					var pto1 = points[link1];
					var bound = new PatchPointLine() { point0 = pto0, point1 = pto1 };
					if (!patchboundaries.ContainsKey(bound)) {
						patchboundaries[bound] = 1;
					} else {
						patchboundaries[bound] = patchboundaries[bound] + 1;
					}
				}

				if (Display_Lines && DrawGUI) {
					foreach (var patch in tpl.patches) {
						using (new Handles.DrawingScope(pointColor)) {
							var count = patch.points.Length;
							if (count != 4) throw new System.Exception("Unexpected patch points count!");

							DrawCubicBezier(rect, BezierHelpers.GetControlPoints<AnimPatchPoint>(
								points[patch.points[0]],
								points[patch.points[2]],
								p => p.uv, p => p.normal), GetRectPos);
							DrawCubicBezier(rect, BezierHelpers.GetControlPoints<AnimPatchPoint>(
								points[patch.points[3]],
								points[patch.points[1]],
								p => p.uv, p => p.normal), GetRectPos);
							AddBoundary(patch.points[0], patch.points[1]);
							AddBoundary(patch.points[2], patch.points[3]);

							curPatch++;
						}
					}

					foreach (var bound in patchboundaries) {
						if (bound.Value == 1) {
							using (new Handles.DrawingScope(pointColor)) {
								var pt1 = bound.Key.point0;
								var pt2 = bound.Key.point1;
								var uv1 = pt1.uv;
								var uv2 = pt2.uv;
								var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
								var uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
								Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
							}
						}
					}
				}

				/*if (tpl_i == CurrentPBKTemplate) {
					Vec2d[] bonePositions = new Vec2d[tpl.bones.Count];
					Angle[] boneAngles = new Angle[tpl.bones.Count];
					for (int i = 0; i < tpl.bones.Count; i++) {
						var boneDyn = tpl.bonesDyn[i];
						var bonePos = boneDyn.position;
						var boneAngle = boneDyn.angle;
						var currentParent = tpl.bones[i].parentKey;
						var parentBone = tpl.GetBoneFromLink(currentParent);
						while (parentBone != null) {
							var parentBoneIndex = tpl.bones.IndexOf(parentBone);
							boneDyn = tpl.bonesDyn[parentBoneIndex];

							bonePos = boneDyn.position + bonePos.Rotate(boneDyn.angle) * boneDyn.scale.x;
							boneAngle = new Angle(boneAngle + boneDyn.angle);

							currentParent = parentBone.parentKey;
							parentBone = tpl.GetBoneFromLink(currentParent);
						}
						bonePositions[i] = bonePos;
						boneAngles[i] = boneAngle;
					}
					// Draw bones
					for(int i = 0; i < tpl.bonesDyn.Count; i++) {
						var boneDyn = tpl.bonesDyn[i];
						//var boneLocalPos = boneDyn.position;
						var boneGlobalPos = bonePositions[i] * new Vec2d(1f / patchHLevel, 1f / patchVLevel);

						pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
						pointSize = 10;

						var uvPos = GetTexturePositionOnRect(boneGlobalPos.GetUnityVector());
						Handles.DrawSolidRectangleWithOutline(
							new Rect(
								uvPos.x - pointSize / 2, uvPos.y - pointSize / 2, pointSize, pointSize), pointColor, Color.white);
					}
				}*/

			}
			Handles.EndGUI();
		}
		GUI.EndGroup();
	}

	void DrawAtlas(TextureCooked tex, Rect rect) {
		GUI.BeginGroup(rect);
		Vector2 GetRectPos(Vector2 pos)
			=> ToRectPosition(rect, pos, atlas: true);
		Vector2 GetTexturePos(Vector2 pos)
			=> ToTexturePosition(rect, pos, atlas: true);
		int pointIndex = 0;
		if (tex.atlas != null) {
			var pivotSize = 10;
			var pivotThickness = 0f;
			var lineSize = LineSize;
			Handles.BeginGUI();
			var atlas = tex.atlas;
			UnityEngine.Random.InitState(12675);

			// Draw grid
			if (atlas.gridX > 1f) {
				var col = Color.grey;
				float step = 1f / atlas.gridX;
				for (float i = step; i < 1f; i += step) {
					using (new Handles.DrawingScope(col)) {
						var uvPos1 = GetRectPos(new Vector2(i, 0));
						var uvPos2 = GetRectPos(new Vector2(i, 1));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
					}
				}
			}
			if (atlas.gridY > 1f) {
				var col = Color.grey;
				float step = 1f / atlas.gridY;
				for (float i = step; i < 1f; i += step) {
					using (new Handles.DrawingScope(col)) {
						var uvPos1 = GetRectPos(new Vector2(0, i));
						var uvPos2 = GetRectPos(new Vector2(1, i));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
					}
				}
			}

			// Draw UVs
			foreach (var uvPair in atlas.uvData) {
				var tplKey = uvPair.Key;
				if (!ShowAtlas.ContainsKey(tplKey)) {
					ShowAtlas[tplKey] = true;
				}
				if (!ShowAtlas[tplKey]) continue;

				UVdata uvdata = uvPair.Value;

				var pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);



				// Draw pivot
				Vec2d pivotUv = null;
				if (atlas.pivots != null && atlas.pivots.ContainsKey(uvPair.Key)) {
					var pivot = atlas.pivots[uvPair.Key];
					pivotUv = new Vec2d(pivot.x, pivot.y);
				} else {
					// Draw cross in the center
					var pivot = Vec2d.Zero;
					if (uvdata.uvs?.Any() ?? false) {
						pivot = new Vec2d(uvdata.uvs.Average(u => u.x), uvdata.uvs.Average(u => u.y));
					} else if (uvdata.uv0 != null && uvdata.uv1 != null) {
						pivot = (uvdata.uv0 + uvdata.uv1) / 2f;
					}
					pivotUv = pivot;
				}
				if (pivotUv != null) {
					var uvPos = GetRectPos(pivotUv.GetUnityVector());
					DragUVCenter = uvPos;
					if (Display_Pivots) {
						using (new Handles.DrawingScope(pointColor)) {
							Handles.DrawLine(uvPos - new Vector2(pivotSize / 2f, 0f), uvPos + new Vector2(pivotSize / 2f, 0f), pivotThickness);
							Handles.DrawLine(uvPos - new Vector2(0f, pivotSize / 2f), uvPos + new Vector2(0f, pivotSize / 2f), pivotThickness);
						}
					}
				}


				// Draw points
				void DrawUV(ref Vec2d uv) {
					var uvPos = GetRectPos(new Vector2(uv.x, uv.y));
					var newPos = DrawPoint(pointIndex, uvPos, pointColor, editable: true);

					if (newPos != uvPos) {
						var newUVposTex = GetTexturePos(newPos);
						uv = newUVposTex.GetUbiArtVector();
					}
				}
				if (uvdata.uvs != null) {
					for (int i = 0; i < uvdata.uvs.Count; i++) {
						var uv = uvdata.uvs[i];
						DrawUV(ref uv);
						uvdata.uvs[i] = uv;
					}
				} else {
					if (uvdata.uv0 != null)
						DrawUV(ref uvdata.uv0);
					if (uvdata.uv1 != null)
						DrawUV(ref uvdata.uv1);
				}

				// Connect points
				if (Display_Lines && DrawGUI) {
					int count = uvdata.uvs?.Count ?? 0;
					if (count == 2 || uvdata.uvs == null) {
						using (new Handles.DrawingScope(pointColor)) {
							var uv1 = uvdata.uvs?[0] ?? uvdata.uv0;
							var uv2 = uvdata.uvs?[1] ?? uvdata.uv1;
							var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
							var uvPos2 = GetRectPos(new Vector2(uv1.x, uv2.y));
							Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
							uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
							uvPos2 = GetRectPos(new Vector2(uv2.x, uv1.y));
							Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
							uvPos1 = GetRectPos(new Vector2(uv2.x, uv1.y));
							uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
							Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
							uvPos1 = GetRectPos(new Vector2(uv1.x, uv2.y));
							uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
							Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
						}
					} else {
						bool drawn = false;
						if (atlas.uvParams != null && atlas.uvParams.ContainsKey(uvPair.Key)) {
							var uvp = atlas.uvParams[uvPair.Key];
							if (uvp.triangles != null && uvp.triangles.Any()) {
								drawn = true;
								using (new Handles.DrawingScope(pointColor)) {
									foreach (var tri in uvp.triangles) {
										var uv1 = uvdata.uvs[tri.vertex1];
										var uv2 = uvdata.uvs[tri.vertex2];
										var uv3 = uvdata.uvs[tri.vertex3];
										var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
										var uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
										var uvPos3 = GetRectPos(new Vector2(uv3.x, uv3.y));
										Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
										Handles.DrawDottedLine(uvPos2, uvPos3, lineSize);
										Handles.DrawDottedLine(uvPos3, uvPos1, lineSize);
									}
								}
							}
						}
						if (!drawn) {
							using (new Handles.DrawingScope(pointColor)) {
								for (int i = 0; i < count; i++) {
									var uv1 = uvdata.uvs[i];
									var uv2 = uvdata.uvs[(i + 1) % count];
									var uvPos1 = GetRectPos(new Vector2(uv1.x, uv1.y));
									var uvPos2 = GetRectPos(new Vector2(uv2.x, uv2.y));
									Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
								}
							}
						}
					}
				}
			}
			Handles.EndGUI();
		}
		GUI.EndGroup();
	}

	void CreateTemplateGameObject(AnimPatchBank pbk, int i) {
		GameObject patch_gao = new GameObject($"{i} [ {pbk.templateKeys.GetKey(i).ToString(pbk.UbiArtContext, shortString: true)} ]");
		//patch_gao.transform.SetParent(gao.transform, false);
		patch_gao.transform.localPosition = Vector3.zero;
		patch_gao.transform.localRotation = Quaternion.identity;
		patch_gao.transform.localScale = Vector3.one;
		var patchEditor = patch_gao.AddComponent<UnityPatchEditor>();
		patchEditor.pbk = pbk;
		patchEditor.templateIndex = i;
		patchEditor.texture = SelectedTexture;
		var unityTex = SelectedTexture.GetUnityTexture(Controller.MainContext);
		patchEditor.AspectRatio = (float)unityTex.Texture.width / unityTex.Texture.height;
	}
	void SyncChangesWithAnimations(AnimPatchBank pbk, AnimTemplate tpl = null) {
		if (Controller.Obj.playAnimations) return; // Happens automatically in this case
		var unityAnimations = FindObjectsOfType<UnityAnimation>().Where(a => a.AllPatchBanks != null && a.AllPatchBanks.Any(p => p.PBK == pbk));
		if (unityAnimations.Any()) {
			foreach (var ua in unityAnimations) {
				ua.ForceUpdatePatches = true; //.ForceUpdatePatches(pbkFilter: p => p.PBK == pbk, templateFilter: tpl != null ? (t => t == tpl) : null);
			}
		}
	}
	void ApplyUVOperation(AnimPatchBank pbk, Vec2d multiplier, Vec2d add, float rot = 0) {
		for (int tpl_i = 0; tpl_i < pbk.templates.Count; tpl_i++) {
			var tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
			bool IsDisplayingSingleTemplate = tpl_i == CurrentPBKTemplate;
			if (CurrentPBKTemplate == -1) {
				if (!ShowTemplate.ContainsKey(tplKey)) {
					ShowTemplate[tplKey] = true;
				}
				if (!ShowTemplate[tplKey]) continue;
			} else if (!IsDisplayingSingleTemplate) continue;

			var template = pbk.templates[tpl_i];

			var center = (rot != 0 && template.patchPoints.Any()) ? new Vec2d(
				template.patchPoints.Average(p => p.uv.x),
				template.patchPoints.Average(p => p.uv.y)) : Vec2d.Zero;

			foreach (var point in template.patchPoints) {
				if (rot != 0) {
					point.uv = center + ((point.uv - center).Rotate(Mathf.Deg2Rad * rot));
					point.normal = point.normal.Rotate(Mathf.Deg2Rad * rot).Normalize();
				}
				point.uv = point.uv * multiplier + add;
			}
		}
		var patchEditors = FindObjectsOfType<UnityPatchEditor>().Where(p => p.texture == SelectedTexture && p.pbk == pbk);
		if (patchEditors.Any()) {
			foreach (var pe in patchEditors) {
				pe.RecalculatePointTransformations();
				pe.ResetUVs();
			}
		}
		SyncChangesWithAnimations(pbk);
	}
	void ApplyUVOperation(UVAtlas atlas, Vec2d multiplier, Vec2d add) {
		if (atlas.uvData != null) {
			foreach (var at in atlas.uvData) {
				var tplKey = at.Key;
				if (!ShowAtlas.ContainsKey(tplKey)) {
					ShowAtlas[tplKey] = true;
				}
				if (!ShowAtlas[tplKey]) continue;

				var uvd = at.Value;
				if(uvd == null) continue;
				if (uvd.uvs != null) {
					for (int i = 0; i < uvd.uvs.Count; i++) {
						uvd.uvs[i] = uvd.uvs[i] * multiplier + add;
					}
				}
				if(uvd.uv0 != null)
					uvd.uv0 = uvd.uv0 * multiplier + add;
				if (uvd.uv1 != null)
					uvd.uv1 = uvd.uv1 * multiplier + add;
			}
		}
		if (atlas.pivots != null) {
			var pivots = atlas.pivots.ToArray();
			foreach (var p in pivots) {
				var tplKey = p.Key;
				if (!ShowAtlas.ContainsKey(tplKey)) {
					ShowAtlas[tplKey] = true;
				}
				if (!ShowAtlas[tplKey]) continue;

				var p2d = new Vec2d(p.Value.x, p.Value.y);
				p2d = p2d * multiplier + add;
				atlas.pivots[p.Key] = new Vec3d(p2d.x, p2d.y, p.Value.z);
			}
		}
		// TODO: Params?
		/*if (atlas.uvParams != null) {
			foreach (var p in atlas.uvParams) {
				p.Value.
			}
		}*/
	}

	private void HandleEvents() {
		if(CurrentUVEditAction != UVEditAction.None) return;
		// Allow adjusting the zoom with the mouse wheel as well. In this case, use the mouse coordinates
		// as the zoom center instead of the top left corner of the zoom area. This is achieved by
		// maintaining an origin that is used as offset when drawing any GUI elements in the zoom area.
		if (Event.current.type == EventType.ScrollWheel && Event.current.modifiers == EventModifiers.Alt) {
			Vector2 delta = Event.current.delta;
			float zoomDelta = -delta.y / 25.0f;
			ZoomFactor += zoomDelta;
			ZoomFactor = Mathf.Clamp(ZoomFactor, ZoomMin, ZoomMax);
			Event.current.Use();
		}
		// Allow moving the zoom area's origin by dragging with the middle mouse button or dragging
		// with the left mouse button with Alt pressed.
		if (Event.current.type == EventType.MouseDrag &&
			(Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
			Event.current.button == 2) {
			Vector2 delta = Event.current.delta;
			delta /= ZoomFactor * 512f;
			ZoomCenter -= new Vector2(delta.x, -delta.y);

			Event.current.Use();
		}
	}
	private void HandleEventsBatchMove(Rect canvas, Event e = null) {
		DragDelta = Vector2.zero;
		if (e == null) e = Event.current;
		if (e != null) {
			if (e.type == EventType.Repaint) {
				switch (CurrentUVEditAction) {
					case UVEditAction.Translate:
						EditorGUIUtility.AddCursorRect(canvas, MouseCursor.MoveArrow);
						break;
					case UVEditAction.Rotate:
						EditorGUIUtility.AddCursorRect(canvas, MouseCursor.RotateArrow);
						break;
					case UVEditAction.Scale:
						EditorGUIUtility.AddCursorRect(canvas, MouseCursor.ScaleArrow);
						break;
					case UVEditAction.None:
						EditorGUIUtility.AddCursorRect(canvas, MouseCursor.Arrow);
						break;
				}
			}
			int id = GUIUtility.GetControlID(FocusType.Passive);
			switch (e.GetTypeForControl(id)) {
				case EventType.MouseDown:
					if (canvas.Contains(e.mousePosition)) {
						if (e.button == 0 && (e.modifiers & EventModifiers.Shift) != EventModifiers.None) {
							// Translate
							e.Use();
							CurrentUVEditAction = UVEditAction.Translate;
							DragStartPosition = e.mousePosition - canvas.position;
							DragCurrentPosition = DragStartPosition;
							DragOnlyUV = (e.modifiers & EventModifiers.Alt) == EventModifiers.None;
							Repaint();
							//Debug.Log($"Start {CurrentUVEditAction}");
						} else if (e.button == 1 && (e.modifiers & EventModifiers.Shift) != EventModifiers.None) {
							// Rotate
							e.Use();
							CurrentUVEditAction = UVEditAction.Rotate;
							DragStartPosition = e.mousePosition - canvas.position;
							DragCurrentPosition = DragStartPosition;
							DragOnlyUV = (e.modifiers & EventModifiers.Alt) == EventModifiers.None;
							Repaint();
							//Debug.Log($"Start {CurrentUVEditAction}");
						} else if (e.button == 0 && (e.modifiers & EventModifiers.Control) != EventModifiers.None) {
							// Scale
							e.Use();
							CurrentUVEditAction = UVEditAction.Scale;
							DragStartPosition = e.mousePosition - canvas.position;
							DragCurrentPosition = DragStartPosition;
							DragOnlyUV = (e.modifiers & EventModifiers.Alt) == EventModifiers.None;
							Repaint();
							//Debug.Log($"Start {CurrentUVEditAction}");
						}
					}
					break;

				case EventType.MouseDrag:
					if (CurrentUVEditAction != UVEditAction.None) {
						
						DragCurrentPosition = e.mousePosition - canvas.position;
						DragDelta = e.delta;
						e.Use();
						DragOnlyUV = (e.modifiers & EventModifiers.Alt) == EventModifiers.None;
						//Debug.Log($"Drag {CurrentUVEditAction}");
					}
					break;

				case EventType.MouseUp:
					CurrentUVEditAction = UVEditAction.None;
					DragOnlyUV = false;
					Repaint();
					break;
			}
		}
	}

	#region Handles
	Vector2 DrawPoint(int index, Vector2 uvPos, Color pointColor, bool editable = false, Event e = null) {
		var pointSize = PointSize;
		Rect handleRect;
		CalculateRect();
		void CalculateRect() {
			handleRect = new Rect(uvPos.x - pointSize / 2, uvPos.y - pointSize / 2, pointSize, pointSize);
		}
		void Draw(bool selected = false) {
			if(!Display_Points) return;
			if (!DrawGUI) return;
			Handles.DrawSolidRectangleWithOutline(handleRect, pointColor, selected ? Color.yellow : Color.white);
		}

		if (editable) {
			if (CurrentUVEditAction == UVEditAction.None) {
				if (e == null) e = Event.current;
				if (e != null) {
					/*if (e.type == EventType.MouseDown && handleRect.Contains(e.mousePosition)) {
						SelectedPointIndex = index;
						e.Use();
					}

					if (e.type == EventType.MouseDrag && SelectedPointIndex == index && e.button == 0) {
						uvPos += e.delta;
						e.Use();
					}

					if (e.type == EventType.MouseUp && SelectedPointIndex == index) {
						SelectedPointIndex = -1;
						e.Use();
					}*/

					int id = GUIUtility.GetControlID(FocusType.Passive);
					switch (e.GetTypeForControl(id)) {
						case EventType.MouseDown:
							if (handleRect.Contains(e.mousePosition) && e.button == 0) {
								GUIUtility.hotControl = id;
								//SelectedPointIndex = index;
								e.Use();
							}
							break;

						case EventType.MouseDrag:
							if (GUIUtility.hotControl == id) {
								uvPos += e.delta;
								e.Use();
							}
							break;

						case EventType.MouseUp:
							if (GUIUtility.hotControl == id) {
								GUIUtility.hotControl = 0;
								e.Use();
							}
							break;

						case EventType.Repaint:
							Draw(selected: GUIUtility.hotControl == id);
							break;
					}
				}
			} else if (CurrentUVEditAction == UVEditAction.Translate) {
				uvPos += DragDelta;

				CalculateRect();
				Draw();
			} else if (CurrentUVEditAction == UVEditAction.Rotate) {
				float angleDelta = (DragDelta.x + DragDelta.y) * 1f * Mathf.Deg2Rad;

				Vec2d diff = (uvPos - DragUVCenter).GetUbiArtVector().Rotate(angleDelta);
				uvPos = DragUVCenter + diff.GetUnityVector();

				CalculateRect();
				Draw();
			} else if (CurrentUVEditAction == UVEditAction.Scale) {
				float scaleFactor = 1f + (DragDelta.x - DragDelta.y) * 0.01f;
				scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 2f);

				var diff = (uvPos - DragUVCenter) * scaleFactor;
				uvPos = DragUVCenter + diff;


				CalculateRect();
				Draw();
			}
		} else {
			Draw();
		}
		return uvPos;
	}
	Vector2 DrawNormal(Vector2 startPos, Vector2 endPos, bool editable = false, Event e = null) {
		var normalLineSize = LineSize;
		void Draw(float factor = 1f) {
			if(!Display_Normals) return;
			if(!DrawGUI) return;
			Handles.DrawLine(startPos, endPos, normalLineSize * factor);
		}

		if (editable) {
			if (CurrentUVEditAction == UVEditAction.None) {
				if (e == null) e = Event.current;
				if (e != null) {
					var normalSelectionSize = (endPos - startPos).magnitude * 1.75f; /*NormalSelectionSize * ZoomFactor*/;
					var midPos = startPos + (endPos - startPos) / 2;
					var handleRect = new Rect(
						midPos.x - normalSelectionSize / 2, midPos.y - normalSelectionSize / 2,
						normalSelectionSize, normalSelectionSize);
					int id = GUIUtility.GetControlID(FocusType.Passive);
					switch (e.GetTypeForControl(id)) {
						case EventType.MouseDown:
							if (handleRect.Contains(e.mousePosition) && e.button == 1) {
								GUIUtility.hotControl = id;
								//SelectedPointIndex = index;
								e.Use();
							}
							break;

						case EventType.MouseDrag:
							if (GUIUtility.hotControl == id) {
								endPos += e.delta;
								e.Use();
							}
							break;

						case EventType.MouseUp:
							if (GUIUtility.hotControl == id) {
								GUIUtility.hotControl = 0;
								e.Use();
							}
							break;

						case EventType.Repaint:
							if (Display_Normals) {
								if (GUIUtility.hotControl == id) {
									var color = Handles.color;
									Handles.color = Color.yellow;
									Draw(factor: 2);
									Handles.color = color;
								} else {
									Draw();
								}
							}
							break;
					}
				}
			} else if (CurrentUVEditAction == UVEditAction.Translate) {
				Draw();
			} else if (CurrentUVEditAction == UVEditAction.Rotate) {
				float angleDelta = (DragDelta.x + DragDelta.y) * 1f * Mathf.Deg2Rad;

				Vec2d diff = (endPos - startPos).GetUbiArtVector().Rotate(angleDelta);
				endPos = startPos + diff.GetUnityVector();
				Draw();
			} else if (CurrentUVEditAction == UVEditAction.Scale) {
				Draw();
			}
		} else {
			Draw();
		}
		return endPos;
	}
	void DrawCubicBezier(Rect rect, Vec2d[] controlPoints, Func<Vector2, Vector2> GetTexturePositionOnRect) {
		if(!Display_Lines) return;
		if (!DrawGUI) return;
		var numPoints = 16;
		var lineSize = LineSize;
		Vec2d[] points = Enumerable.Range(0, numPoints).Select(i => BezierHelpers.CalculateCubicBezierPoint(i / (float)(numPoints - 1), controlPoints)).ToArray();
		Vector2[] pointsConv = points.Select(p => GetTexturePositionOnRect(p.GetUnityVector())).ToArray();
		for (int i = 0; i < points.Length - 1; i++) {
			Handles.DrawDottedLine(pointsConv[i], pointsConv[i + 1], lineSize);
		}
	}
	#endregion

	#region Helpers
	private Vector2 ToRectPosition(Rect rect, Vector2 pos, bool atlas = false) {
		var rectHalf = new Vector2(rect.width / 2f, rect.height / 2f);
		var size = atlas ? rect.size : new Vector2(rect.width, rect.width);
		var zoomCenter = new Vector2(ZoomCenter.x, (1f - ZoomCenter.y) * (atlas ? 1f : (rect.height / rect.width)));

		var relativePos = (pos - zoomCenter);
		return rectHalf + relativePos * size * ZoomFactor;
	}
	private Vector2 ToTexturePosition(Rect rect, Vector2 pos, bool atlas = false) {
		var rectHalf = new Vector2(rect.width / 2f, rect.height / 2f);
		var size = atlas ? rect.size : new Vector2(rect.width, rect.width);
		var zoomCenter = new Vector2(ZoomCenter.x, (1f - ZoomCenter.y) * (atlas ? 1f : (rect.height / rect.width)));

		var relativePos = (pos - rectHalf) / ZoomFactor / size;
		return relativePos + zoomCenter;
	}
	Rect AdjustAspectRatio(Rect canvas, float width, float height, bool centerVertically = true) {
		Rect subcanvas;
		var aspectRatio = width / height;
		var canvasRatio = canvas.width / (float)canvas.height;
		float ratioScale = canvasRatio / aspectRatio;
		if (ratioScale < 1f) {
			var newH = canvas.height * ratioScale;
			if (centerVertically) {
				subcanvas = new Rect(canvas.x, canvas.y + (canvas.height - newH) / 2f, canvas.width, newH);
			} else {
				subcanvas = new Rect(canvas.x, canvas.y, canvas.width, newH);
			}
		} else if (ratioScale > 1f) {
			var newW = canvas.width / ratioScale;
			subcanvas = new Rect(canvas.x + (canvas.width - newW) / 2f, canvas.y, newW, canvas.height);
		} else
			subcanvas = canvas;
		return subcanvas;

	}

	public Vec2d Vec2dField(Rect rect, Vec2d value, string name = null, float? labelWidth = null, float labelPaddingLeft = 4f) {
		if (value == null) value = new Vec2d();
		var controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
		if (name != null) {
			if (labelWidth.HasValue) {
				Rect labelRect = new Rect(rect.x + labelPaddingLeft, rect.y, labelWidth.Value, rect.height);
				rect = new Rect(rect.x + labelWidth.Value + labelPaddingLeft, rect.y, rect.width - labelWidth.Value - labelPaddingLeft, rect.height);
				EditorGUI.LabelField(labelRect, name, EditorStyles.miniLabel);
			} else {
				rect = EditorGUI.PrefixLabel(rect, controlID, new GUIContent(name));
			}
		}
		var indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var rects = UnityWindow.DivideRectHorizontally(rect, 2, spacing: 8f);
		value.x = CustomFloatField("X", value.x, rectToUse: rects[0], labelWidth: 16);
		value.y = CustomFloatField("Y", value.y, rectToUse: rects[1], labelWidth: 16);

		EditorGUI.indentLevel = indentLevel;

		return value;
	}

	public float CustomFloatField(string name, float value, Rect? rectToUse = null, bool prefixLabel = true, float? labelWidth = null,
		float? limitLower = null, float? limitUpper = null) {
		Rect rect = rectToUse ?? GetNextRect(ref YPos);
		if (prefixLabel) {
			if (labelWidth.HasValue) {
				Rect labelRect = new Rect(rect.x, rect.y, labelWidth.Value, rect.height);
				rect = new Rect(rect.x + labelWidth.Value, rect.y, rect.width - labelWidth.Value, rect.height);
				EditorGUI.LabelField(labelRect, name, EditorStyles.miniLabel);
			} else {
				rect = EditorGUI.PrefixLabel(rect, new GUIContent(name));
			}
		}
		var indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var backgroundColor = GUI.backgroundColor;
		value = EditorGUI.FloatField(rect, value);
		if (limitUpper.HasValue && value  > limitUpper.Value) value = limitUpper.Value;
		if (limitLower.HasValue && value < limitLower.Value) value = limitLower.Value;
		EditorGUI.indentLevel = indentLevel;
		return value;
	}
	#endregion

	private TextureCooked SelectedTexture {
		get {
			if (!string.IsNullOrWhiteSpace(SelectedTextureFile)) {
				Path texPath = new Path(SelectedTextureFile);

				var cache = Controller.MainContext?.Cache?.Structs;
				if (cache != null && cache.ContainsKey(typeof(TextureCooked)) && cache[typeof(TextureCooked)].ContainsKey(texPath.stringID)) {
					var tex = cache[typeof(TextureCooked)][texPath.stringID] as TextureCooked;
					if (tex != null) {
						return tex;
					}
				}
			}
			return null;
		}
	}

	/// <summary>
	/// The file selection dropdown
	/// </summary>
	private FileSelectionDropdown Dropdown { get; set; }

	/// <summary>
	/// The selected texture file
	/// </summary>
	private string SelectedTextureFile { get; set; }

	private class PatchPointLine {
		public AnimPatchPoint point0;
		public AnimPatchPoint point1;

		public override bool Equals(object obj) {
			var line = obj as PatchPointLine;
			if (line == default) return false;

			return (point0 == line.point0 && point1 == line.point1) || (point0 == line.point1 || point1 == line.point0);
		}

		public override int GetHashCode() {
			var pt0 = point0.key.stringID > point1.key.stringID ? point1 : point0;
			var pt1 = point0.key.stringID > point1.key.stringID ? point0 : point1;
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 23 + pt0.GetHashCode();
				hash = hash * 23 + pt1.GetHashCode();
				return hash;
			}
		}
	}


	public enum Tab {
		AtlasContainer,
		PatchBank
	}
	public Tab CurrentTab { get; set; }

	public enum UVUnit {
		UV,
		Pixels
	}
	public UVUnit CustomUVOperationUnit { get; set; } = UVUnit.UV;
	public float CustomUVOperationMultiplyUniform { get; set; } = 1f;
	public Vec2d CustomUVOperationMultiply { get; set; } = Vec2d.One;
	public Vec2d CustomUVOperationAdd { get; set; } = Vec2d.Zero;

	public enum UVEditAction {
		None,
		Translate,
		Rotate,
		Scale
	}
	public UVEditAction CurrentUVEditAction { get; set; } = UVEditAction.None;
	public Vector2 DragStartPosition { get; set; }
	public Vector2 DragCurrentPosition { get; set; }
	public Vector2 DragDelta { get; set; }
	public Vector2 DragUVCenter { get; set; }
	public bool DragOnlyUV { get; set; } = false;
	/*public Vec2d UVTranslatePosition { get; set; } = Vec2d.Zero;
	public Vec2d UVRotatePosition { get; set; } = Vec2d.Zero;
	public Vec2d UVScalePosition { get; set; } = Vec2d.Zero;*/

	/// <summary>
	/// The file selection dropdown
	/// </summary>
	private FileSelectionDropdown PBKDropdown { get; set; }

	private PatchBankTemplateDropdown TemplateDropdown { get; set; }

	/// <summary>
	/// The selected texture file
	/// </summary>
	private string SelectedPBKFile { get; set; }

	public int CurrentPBKTemplate { get; set; }

	private Dictionary<ulong, bool> ShowTemplate { get; set; } = new Dictionary<ulong, bool>();
	private Dictionary<int, bool> ShowAtlas { get; set; } = new Dictionary<int, bool>();

	private bool recheckFiles = false;

	private bool ResetAtlas { get; set; }
	private bool ResetPBK { get; set; }
	public Color BackgroundColor { get; set; } = new Color(0,0,0,0.2f);

	public bool AlphaBlending { get; set; } = true;

	public float ZoomFactor { get; set; } = 1f;
	public float ZoomMin { get; set; } = 1f;
	public float ZoomMax { get; set; } = 16f;
	public Vector2 ZoomCenter { get; set; } = new Vector2(0.5f, 0.5f);

	public float PointSize = 6f;
	public float LineSize = 0.1f;
	public float NormalLength = 0.01f;
	public float NormalSelectionSize = 12f;
	public int SelectedPointIndex { get; set; } = -1;

	// Patch bank export options
	public bool PatchBankExport_IncludeImage { get; set; } = true;
	public float PatchBankExport_Scale { get; set; } = 1f;
	public int PatchBankExport_Padding { get; set; } = 0;
	public float PatchBankExport_PointSize { get; set; } = 6f;
	public float PatchBankExport_LineSize { get; set; } = 1f;
	public float PatchBankExport_PivotSize { get; set; } = 10f;
	public bool PatchBankExport_IncludeNormals { get; set; } = false;

	public bool Display_Points { get; set; } = true;
	public bool Display_Normals { get; set; } = true;
	public bool Display_Lines { get; set; } = true;
	public bool Display_Pivots { get; set; } = true;
	public bool Display_UseFullWidth { get; set; } = false;

	public float CanvasHeight { get; set; } = 512;

	public bool IsEditHelpOpen = false;
	public bool IsShowTemplateOpen = false;
	public bool DrawGUI = false;
}