﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
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
	}

	protected override void UpdateEditorFields() {
		base.UpdateEditorFields();

		if (EditorApplication.isPlaying) {
			if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
				var c = Controller.MainContext;

				DrawHeader("Select texture");
				DrawTextureSelection();

				if (!string.IsNullOrWhiteSpace(SelectedTextureFile)) {
					Path texPath = new Path(SelectedTextureFile);
					var tex = SelectedTexture;
					
					if (tex != null) {
						var t = tex.GetUnityTexture(Controller.MainContext);
						var heightFactor = t.HeightFactor;
						var unityTex = t.Texture;
						if (unityTex != null) {
							ZoomFactor = EditorGUI.Slider(GetNextRect(height: 25f), ZoomFactor, ZoomMin, ZoomMax);
							var canvas = GetNextRect(height: 512, vPadding: 4);
							Event e = Event.current;
							Rect subcanvas = AdjustAspectRatio(canvas, unityTex.width, unityTex.height, centerVertically: false);
							YPos -= canvas.height - subcanvas.height;

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


							var rect = GetNextRect();
							rect = EditorGUI.PrefixLabel(rect, new GUIContent("UV Source"));
							CurrentTab = (Tab)GUI.Toolbar(rect, (int)CurrentTab, new string[] { "Atlas Container", "Patch Bank" });
							switch (CurrentTab) {
								case Tab.AtlasContainer:
									DrawAtlasUI(tex, subcanvas);
									break;
								case Tab.PatchBank:
									DrawPatchBankUI(tex, subcanvas);
									break;
							}

							HandleEvents();
						}
					} else {
						if (EditorButton("Load")) {
							async Task LoadTex() {
								Controller.MainContext.Loader.LoadTexture(texPath, t => {});
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

	void DrawAtlasUI(TextureCooked tex, Rect rect) {
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
		}
	}

	void DrawAtlas(TextureCooked tex, Rect rect) {
		GUI.BeginGroup(rect);
		Vector2 GetTexturePositionOnRect(Vector2 pos) {
			var zoomCenter = new Vector2(ZoomCenter.x, 1f - ZoomCenter.y);
			var relativePos = (pos - zoomCenter);
			var rectHalf = new Vector2(rect.width / 2f, rect.height / 2f);
			return rectHalf + new Vector2(relativePos.x, relativePos.y) * rect.size * ZoomFactor;
		}
		if (tex.atlas != null) {
			var pointSize = 6;
			var pivotSize = 10;
			var pivotThickness = 0f;
			var lineSize = 0.1f;
			Handles.BeginGUI();
			var atlas = tex.atlas;
			UnityEngine.Random.InitState(12675);

			// Draw grid
			if (atlas.gridX > 1f) {
				var col = Color.grey;
				float step = 1f / atlas.gridX;
				for (float i = step; i < 1f; i += step) {
					using (new Handles.DrawingScope(col)) {
						var uvPos1 = GetTexturePositionOnRect(new Vector2(i, 0));
						var uvPos2 = GetTexturePositionOnRect(new Vector2(i, 1));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
					}
				}
			}
			if (atlas.gridY > 1f) {
				var col = Color.grey;
				float step = 1f / atlas.gridY;
				for (float i = step; i < 1f; i += step) {
					using (new Handles.DrawingScope(col)) {
						var uvPos1 = GetTexturePositionOnRect(new Vector2(0, i));
						var uvPos2 = GetTexturePositionOnRect(new Vector2(1, i));
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
				if(uvdata.uvs == null) continue;
				var pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
				// Draw points
				foreach (var uv in uvdata.uvs) {
					var uvPos = GetTexturePositionOnRect(new Vector2(uv.x, uv.y));
					Handles.DrawSolidRectangleWithOutline(
						new Rect(
							uvPos.x - pointSize / 2, uvPos.y - pointSize / 2, pointSize, pointSize), pointColor, Color.white);
				}

				// Connect points
				int count = uvdata.uvs.Count;
				if (count == 2) {
					using (new Handles.DrawingScope(pointColor)) {
						var uv1 = uvdata.uvs[0];
						var uv2 = uvdata.uvs[1];
						var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
						var uvPos2 = GetTexturePositionOnRect(new Vector2(uv1.x, uv2.y));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
						uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
						uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv1.y));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
						uvPos1 = GetTexturePositionOnRect(new Vector2(uv2.x, uv1.y));
						uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
						uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv2.y));
						uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
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
									var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
									var uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
									var uvPos3 = GetTexturePositionOnRect(new Vector2(uv3.x, uv3.y));
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
								var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
								var uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
								Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
							}
						}
					}
				}

				// Draw pivot
				if (atlas.pivots != null && atlas.pivots.ContainsKey(uvPair.Key)) {
					var pivot = atlas.pivots[uvPair.Key];
					var uvPos = GetTexturePositionOnRect(new Vector2(pivot.x, pivot.y));

					using (new Handles.DrawingScope(pointColor)) {
						Handles.DrawLine(uvPos - new Vector2(pivotSize / 2f, 0f), uvPos + new Vector2(pivotSize / 2f, 0f), pivotThickness);
						Handles.DrawLine(uvPos - new Vector2(0f, pivotSize / 2f), uvPos + new Vector2(0f, pivotSize / 2f), pivotThickness);
					}
					/*Handles.Draw
					Handles.DrawSolidRectangleWithOutline(
						new Rect(
							uvPos.x - pivotSize / 2, uvPos.y - pivotSize / 2, pivotSize, pivotSize), pointColor, Color.black);*/
				}
			}
			Handles.EndGUI();
		}
		GUI.EndGroup();
	}

	void DrawPatchBankUI(TextureCooked tex, Rect rect) {
		DrawPatchBankSelection();
		if (!string.IsNullOrWhiteSpace(SelectedPBKFile)) {
			Path pbkPath = new Path(SelectedPBKFile);

			var cache = Controller.MainContext?.Cache?.Structs;
			if (cache != null && cache.ContainsKey(typeof(AnimPatchBank)) && cache[typeof(AnimPatchBank)].ContainsKey(pbkPath.stringID)) {
				var pbk = cache[typeof(AnimPatchBank)][pbkPath.stringID] as AnimPatchBank;
				if (pbk != null) {
					DrawPatchBankTemplateSelection(pbk);

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
						int index = 0;
						foreach (var entry in showTemplateCopy) {
							if (MiniButton($"{index} - {entry.Key:X8}", entry.Value)) {
								ShowTemplate[entry.Key] = !entry.Value;
							}
							index++;
						}
					}
					DrawHeader("Patch Bank PNG Export");
					if (EditorButton("Export as PNG"))
					{
						var defaultName = $"{new Path(SelectedTextureFile).GetFilenameWithoutExtension()}.png";
						string filePath = EditorUtility.SaveFilePanel("Output PNG file", "", defaultName, "png");

						if (!string.IsNullOrWhiteSpace(filePath))
							SavePatchBankAsPNG(tex, filePath, pbk);
					}

					PatchBankExport_IncludeImage = EditorField("Include image", PatchBankExport_IncludeImage);
					PatchBankExport_Scale = EditorField("Scale", PatchBankExport_Scale);
					PatchBankExport_Padding = EditorField("Padding", PatchBankExport_Padding);
					PatchBankExport_PointSize = EditorField("Point size", PatchBankExport_PointSize);
					PatchBankExport_LineSize = EditorField("Line size", PatchBankExport_LineSize);
					PatchBankExport_IncludeNormals = EditorField("Include normals", PatchBankExport_IncludeNormals);
				}
			} else {
				if (EditorButton("Load")) {
					async Task LoadPBK() {
						Controller.MainContext.Loader.LoadFile<AnimPatchBank>(pbkPath, t => {});
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
	}

	void SavePatchBankAsPNG(TextureCooked tex, string outputPath, AnimPatchBank pbk)
	{
		Texture2D tex2d = tex.GetUnityTexture(Controller.MainContext).Texture;
		using MagickImage img = PatchBankExport_IncludeImage 
			? new MagickImage(tex2d.Decompress().EncodeToPNG(), MagickFormat.Png) 
			: new MagickImage(MagickColors.Transparent, tex2d.width, tex2d.height);
		
		// Make sure it's not vertically flipped!
		img.Flip();

		// Optionally resize
		if (PatchBankExport_Scale != 1)
		{
			//img.Interpolate = PixelInterpolateMethod.Integer;
			//img.FilterType = FilterType.Point;
			img.Resize(new Percentage(PatchBankExport_Scale * 100));
		}

		// Optionally add padding
		if (PatchBankExport_Padding != 0)
		{
			img.Extent(img.Width + PatchBankExport_Padding * 2, img.Height + PatchBankExport_Padding * 2, Gravity.Center, MagickColors.Transparent);
		}

		// Add UVs
		DrawPatchBankToImage(img, new Rect(PatchBankExport_Padding, PatchBankExport_Padding, img.Width - PatchBankExport_Padding * 2, img.Height - PatchBankExport_Padding * 2), pbk);

		// Save
		img.Write(outputPath);
	}

	// Re-implemented from DrawPatchBank
	void DrawPatchBankToImage(MagickImage img, Rect rect, AnimPatchBank pbk)
	{
		Vector2 getTexturePositionOnRect(Vector2 pos) => rect.position + pos * rect.width;

		DrawableStrokeWidth lineSize = new(PatchBankExport_LineSize);

		UnityEngine.Random.InitState(12675);
		for (int tpl_i = 0; tpl_i < pbk.templates.Count; tpl_i++)
		{
			AnimTemplate tpl = pbk.templates[tpl_i];
			ulong tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
			if (CurrentPBKTemplate == -1)
			{
				ShowTemplate.TryAdd(tplKey, true);
				if (!ShowTemplate[tplKey]) 
					continue;
			}
			else if (tpl_i != CurrentPBKTemplate)
			{
				continue;
			}

			// Create a random color
			Color unityColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
			MagickColor color = new MagickColor(
				(byte)(unityColor.r * 255),
				(byte)(unityColor.g * 255),
				(byte)(unityColor.b * 255));
			DrawableFillColor fillColor = new(color);
			DrawableStrokeColor strokeColor = new(color);

			// Draw points
			foreach (AnimPatchPoint point in tpl.patchPoints)
			{
				// Draw point
				Vector2 uvPos = getTexturePositionOnRect(point.uv.GetUnityVector());
				img.Draw(fillColor, new DrawableRectangle(
					upperLeftX: uvPos.x - PatchBankExport_PointSize / 2, 
					upperLeftY: uvPos.y - PatchBankExport_PointSize / 2, 
					lowerRightX: uvPos.x + PatchBankExport_PointSize / 2, 
					lowerRightY: uvPos.y + PatchBankExport_PointSize / 2)); 

				// Draw normal
				if (PatchBankExport_IncludeNormals)
				{
					Vec2d normalTest = point.uv + point.normal * 0.01f;
					Vector2 normalTestPos = getTexturePositionOnRect(normalTest.GetUnityVector());
					img.Draw(new DrawableStrokeColor(MagickColors.White), lineSize, new DrawableLine(uvPos.x, uvPos.y, normalTestPos.x, normalTestPos.y));
				}
			}

			if (tpl.patchPoints.Count >= 2)
			{
				var pt1 = tpl.patchPoints[0];
				var pt2 = tpl.patchPoints[1];
				var uv1 = pt1.uv;
				var uv2 = pt2.uv;
				var uvPos1 = getTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
				var uvPos2 = getTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
				img.Draw(strokeColor, lineSize, new DrawableLine(uvPos1.x, uvPos1.y, uvPos2.x, uvPos2.y));
			}
			if (tpl.patchPoints.Count >= 3)
			{
				var cnt = tpl.patchPoints.Count;
				var pt1 = tpl.patchPoints[cnt - 2];
				var pt2 = tpl.patchPoints[cnt - 1];
				var uv1 = pt1.uv;
				var uv2 = pt2.uv;
				var uvPos1 = getTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
				var uvPos2 = getTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
				img.Draw(strokeColor, lineSize, new DrawableLine(uvPos1.x, uvPos1.y, uvPos2.x, uvPos2.y));
			}

			Dictionary<Link, AnimPatchPoint> points = tpl.patchPoints.ToDictionary(p => p.key, p => p);
			foreach (var patch in tpl.patches)
			{
				int count = patch.points.Length;

				if (count != 4) 
					throw new Exception("Unexpected patch points count!");

				Vec2d[] controlPoints = AnimTemplate.GetPatchControlPoints(
					new Vec2d[] {
						points[patch.points[0]].uv,
						points[patch.points[2]].uv,
						points[patch.points[3]].uv,
						points[patch.points[1]].uv,
					},
					new Vec2d[] {
						points[patch.points[0]].normal,
						points[patch.points[2]].normal,
						points[patch.points[3]].normal,
						points[patch.points[1]].normal,
					});

				drawCubicBezier(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
				drawCubicBezier(controlPoints[4], controlPoints[5], controlPoints[6], controlPoints[7]);

				void drawCubicBezier(Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3)
				{
					const int numPoints = 16;
					
					Vec2d[] bezierPoints = Enumerable.Range(0, numPoints).
						Select(i => CalculateCubicBezierPoint(i / (float)(numPoints - 1), p0, p1, p2, p3)).
						ToArray();
					Vector2[] pointsConv = bezierPoints.
						Select(p => getTexturePositionOnRect(p.GetUnityVector())).
						ToArray();
					
					for (int i = 0; i < bezierPoints.Length - 1; i++)
					{
						img.Draw(strokeColor, lineSize, new DrawableLine(pointsConv[i].x, pointsConv[i].y, pointsConv[i + 1].x, pointsConv[i + 1].y));
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

	void DrawPatchBank(TextureCooked tex, Rect rect, AnimPatchBank pbk) {
		var t = tex?.GetUnityTexture(Controller.MainContext);
		var heightFactor = t?.HeightFactor;
		GUI.BeginGroup(rect);
		Vector2 GetTexturePositionOnRect(Vector2 pos) {
			var size = rect.width; //Mathf.Max(rect.width, rect.height);
			var zoomCenter = new Vector2(ZoomCenter.x, (1f - ZoomCenter.y) * rect.height / rect.width);
			var relativePos = (pos - zoomCenter);
			var rectHalf = new Vector2(rect.width / 2f, rect.height / 2f);
			return rectHalf + new Vector2(relativePos.x, relativePos.y) * size * ZoomFactor;
		}
		if (pbk != null) {
			var pointSize = 6;
			var lineSize = 0.1f;
			Handles.BeginGUI();
			UnityEngine.Random.InitState(12675);
			for(int tpl_i = 0; tpl_i < pbk.templates.Count; tpl_i++) {
				var tpl = pbk.templates[tpl_i];
				var tplKey = pbk.templateKeys.GetKeyFromValue(tpl_i);
				if (CurrentPBKTemplate == -1) {
					if (!ShowTemplate.ContainsKey(tplKey)) {
						ShowTemplate[tplKey] = true;
					}
					if (!ShowTemplate[tplKey]) continue;
				} else if(tpl_i != CurrentPBKTemplate) continue;

				Dictionary<Link, AnimPatchPoint> points = tpl.patchPoints.ToDictionary(p => p.key, p => p);
				var pointColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1f);
				foreach (var point in tpl.patchPoints) {
					var uvPos = GetTexturePositionOnRect(point.uv.GetUnityVector());
					Handles.DrawSolidRectangleWithOutline(
						new Rect(
							uvPos.x - pointSize / 2, uvPos.y - pointSize / 2, pointSize, pointSize), pointColor, Color.white);


					var normalTest = point.uv + (point.normal * 0.01f);
					var normalTestPos = GetTexturePositionOnRect(normalTest.GetUnityVector());
					Handles.DrawLine(uvPos, normalTestPos, lineSize);

				}
				int patchesCount = tpl.patches.Count;
				int curPatch = 0;

				using (new Handles.DrawingScope(pointColor)) {
					if (tpl.patchPoints.Count >= 2) {
						var pt1 = tpl.patchPoints[0];
						var pt2 = tpl.patchPoints[1];
						var uv1 = pt1.uv;
						var uv2 = pt2.uv;
						var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
						var uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
					}
					if (tpl.patchPoints.Count >= 3) {
						var cnt = tpl.patchPoints.Count;
						var pt1 = tpl.patchPoints[cnt - 2];
						var pt2 = tpl.patchPoints[cnt - 1];
						var uv1 = pt1.uv;
						var uv2 = pt2.uv;
						var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
						var uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
						Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
					}
				}

				foreach (var patch in tpl.patches) {
					using (new Handles.DrawingScope(pointColor)) {
						var count = patch.points.Length;
						if(count != 4) throw new System.Exception("Unexpected patch points count!");
						var patchPoints = patch.points.Select(p => points[p]).ToArray();
						Vec2d[] controlPoints = AnimTemplate.GetPatchControlPoints(
							new Vec2d[] {
								patchPoints[0].uv,
								patchPoints[2].uv,
								patchPoints[3].uv,
								patchPoints[1].uv,
							},
							new Vec2d[] {
								patchPoints[0].normal,
								patchPoints[2].normal,
								patchPoints[3].normal,
								patchPoints[1].normal,
							});
						DrawCubicBezier(rect, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], GetTexturePositionOnRect);
						DrawCubicBezier(rect, controlPoints[4], controlPoints[5], controlPoints[6], controlPoints[7], GetTexturePositionOnRect);

						/*var pt1 = points[patch.points[(0 + 0) % count]];
						var pt2 = points[patch.points[(0 + 1) % count]];
						for (int i = 0; i < count - 2; i++) {
							var pt3 = points[patch.points[(i + 2) % count]];
							var uv1 = pt1.uv;
							var uv2 = pt2.uv;
							var uv3 = pt3.uv;
							var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
							var uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
							var uvPos3 = GetTexturePositionOnRect(new Vector2(uv3.x, uv3.y));
							if (i == 0) {
								Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
							}
							Handles.DrawDottedLine(uvPos2, uvPos3, lineSize);
							Handles.DrawDottedLine(uvPos3, uvPos1, lineSize);
							pt1 = pt2;
							pt2 = pt3;
						}*/
						/*for (int i = 0; i < count - 1; i++) {
							var pt1 = points[patch.points[(i) % count]];
							var pt2 = points[patch.points[(i + 1) % count]];
							var uv1 = pt1.uv;
							var uv2 = pt2.uv;
							var uvPos1 = GetTexturePositionOnRect(new Vector2(uv1.x, uv1.y));
							var uvPos2 = GetTexturePositionOnRect(new Vector2(uv2.x, uv2.y));
							Handles.DrawDottedLine(uvPos1, uvPos2, lineSize);
						}*/
						curPatch++;
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

	Vec2d CalculateCubicBezierPoint(float t, Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3) {
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vec2d p = p0 * uuu;
		p += p1 * 3f * uu * t;
		p += p2 * 3f * u * tt;
		p += p3 * ttt;

		return p;
	}
	void DrawCubicBezier(Rect rect, Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3, Func<Vector2, Vector2> GetTexturePositionOnRect) {
		var numPoints = 16;
		var lineSize = 0.1f;
		Vec2d[] points = Enumerable.Range(0, numPoints).Select(i => CalculateCubicBezierPoint(i / (float)(numPoints - 1), p0,p1,p2,p3)).ToArray();
		Vector2[] pointsConv = points.Select(p => GetTexturePositionOnRect(p.GetUnityVector())).ToArray();
		for (int i = 0; i < points.Length - 1; i++) {
			Handles.DrawDottedLine(pointsConv[i], pointsConv[i+1], lineSize);
		}
	}

	void DrawTextureSelection() {
		var c = Controller.MainContext;
		string[] extensions = new string[] {
			$"*.tga{((c.Settings.Cooked && !c.Settings.PastaStructure) ? ".ckd" : "")}",
			$"*.png{((c.Settings.Cooked && !c.Settings.PastaStructure) ? ".ckd" : "")}"
		};
		Rect rect = GetNextRect(vPaddingBottom: 4f);
		string buttonString = "No texture selected";
		if (SelectedTextureFile != null) {
			buttonString = System.IO.Path.GetFileName(SelectedTextureFile);
		}
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Texture"));
		if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
			string directory = (Controller.MainContext.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
			if (!directory.EndsWith("/")) directory += "/";
			while (directory.Contains("//")) directory = directory.Replace("//", "/");

			if (recheckFiles || Dropdown == null || Dropdown.directory != directory || Dropdown.extensions == null || !Enumerable.SequenceEqual(Dropdown.extensions, extensions) || Dropdown.mode != c.Settings.Mode) {
				Dropdown = new FileSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory, extensions) {
					name = "Texture files",
					mode = c.Settings.Mode
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
		EditorGUI.TextArea(GetNextRect(), SelectedTextureFile);
	}

	void DrawPatchBankSelection() {
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
		}
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Patch Bank"));
		if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
			string directory = (Controller.MainContext.BasePath + c.Settings.ITFDirectory).Replace(System.IO.Path.DirectorySeparatorChar, '/');
			if (!directory.EndsWith("/")) directory += "/";
			while (directory.Contains("//")) directory = directory.Replace("//", "/");

			if (recheckFiles || PBKDropdown == null || PBKDropdown.directory != directory || PBKDropdown.extensions == null || !Enumerable.SequenceEqual(PBKDropdown.extensions, PBKextensions) || PBKDropdown.mode != c.Settings.Mode) {
				PBKDropdown = new FileSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory, PBKextensions) {
					name = "Patch bank files",
					mode = c.Settings.Mode
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
		EditorGUI.TextArea(GetNextRect(), SelectedPBKFile);
	}


	void DrawPatchBankTemplateSelection(AnimPatchBank pbk) {
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

	private void HandleEvents() {
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


	public enum Tab {
		AtlasContainer,
		PatchBank
	}
	public Tab CurrentTab { get; set; }

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
	public float ZoomMax { get; set; } = 8f;
	public Vector2 ZoomCenter { get; set; } = new Vector2(0.5f, 0.5f);

	public float patchHLevel { get; set; } = 2f;
	public float patchVLevel { get; set; } = 2f;

	// Patch bank export options
	public bool PatchBankExport_IncludeImage { get; set; } = true;
	public float PatchBankExport_Scale { get; set; } = 1f;
	public int PatchBankExport_Padding { get; set; } = 0;
	public float PatchBankExport_PointSize { get; set; } = 6f;
	public float PatchBankExport_LineSize { get; set; } = 1f;
	public bool PatchBankExport_IncludeNormals { get; set; } = false;
}