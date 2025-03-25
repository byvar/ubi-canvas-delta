﻿using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using UbiCanvas.Helpers;

public class UnityWindow : EditorWindow {
	protected Rect GetNextRect(ref float yPos, float padding = 4f, float height = 0f, float vPadding = 0f, float vPaddingBottom = 0f, bool ignoreIndent = false, float linesCount = 1f) {
		if (height == 0f)
			height = EditorGUIUtility.singleLineHeight * linesCount;
		float indent = (ignoreIndent ? 0f : (IndentLevel * 16f));

		Rect rect = new Rect(padding + indent, yPos + vPadding, Mathf.Max(0f, EditorGUIUtility.currentViewWidth - padding * 2f - indent - (scrollbarShown ? scrollbarWidth : 0f)), height);

		yPos += height + vPadding * 2f + vPaddingBottom;

		return rect;
	}
	protected Rect GetNextRect(float padding = 4f, float height = 0f, float vPadding = 0f, float vPaddingBottom = 0f, bool ignoreIndent = false, float linesCount = 1f)
		=> GetNextRect(ref YPos, padding: padding, height: height, vPadding: vPadding, vPaddingBottom: vPaddingBottom, ignoreIndent: ignoreIndent, linesCount: linesCount);

	protected void DrawHeader(ref float yPos, string header) {
		if (yPos > 0) {
			Rect rect = GetNextRect(ref yPos, padding: 0f, height: EditorStyles.toolbarButton.fixedHeight, vPadding: 4f);
			EditorGUI.LabelField(rect, new GUIContent(header), EditorStyles.toolbarButton);
		} else {
			Rect rect = GetNextRect(ref yPos, padding: 0f, height: EditorStyles.toolbarButton.fixedHeight, vPaddingBottom: 4f);
			EditorGUI.LabelField(rect, new GUIContent(header), EditorStyles.toolbarButton);
		}
	}

	protected void DrawHeader(string header) => DrawHeader(ref YPos, header);

	public Rect AffixToggle(Rect rect, ref bool value, float paddingLeft = 4f) {
		value = EditorGUI.Toggle(new Rect(rect.x + rect.width - rect.height, rect.y, rect.height, rect.height), value);
		return new Rect(rect.x, rect.y, rect.width - rect.height - paddingLeft, rect.height);
	}
	public Rect PrefixToggle(Rect rect, ref bool value) {
		value = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.height, rect.height), value);
		return new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height);
	}
	public Rect RowEntryRect(ref Rect rect, float width, Func<Rect> getNextRect) {
		Rect miniRect = new Rect(rect.x, rect.y, width, rect.height);
		var newW = rect.width - width;
		Rect nextRect = new Rect(miniRect.xMax, rect.y, newW, rect.height);
		if (newW < width) {
			rect = getNextRect();
		} else
			rect = nextRect;

		return miniRect;
	}
	public static Rect[] DivideRectHorizontally(Rect rect, int count, float spacing = 0f) {
		Rect[] rects = new Rect[count];
		for (int i = 0; i < rects.Length; i++) {
			rects[i] = new Rect(rect.x + (rect.width / count) * i, rect.y, (rect.width / count), rect.height);
		}
		if (spacing != 0f) {
			for (int i = 0; i < rects.Length - 1; i++) {
				rects[i].width -= spacing / 2f;
			}
			for (int i = 1; i < rects.Length; i++) {
				rects[i].width -= spacing / 2f;
				rects[i].x += spacing / 2f;
			}
		}
		return rects;
	}

	protected void BrowseButton(Rect rect, string name, GUIContent content, Action action, int width) {
		GUIStyle butStyle = EditorStyles.miniButtonRight;
		Rect buttonRect = new Rect(rect.x + rect.width - width, rect.y, width, rect.height);
		GUI.SetNextControlName("Button " + name);

		if (GUI.Button(buttonRect, content, butStyle))
			action();
	}

	public string DirectoryField(Rect rect, string title, string value, bool includeLabel = true) {
		BrowseButton(rect, title, EditorGUIUtility.IconContent("Folder Icon"), () => {
			string selectedFolder = EditorUtility.OpenFolderPanel(title, value, "");
			if (!string.IsNullOrEmpty(selectedFolder)) {
				GUI.FocusControl("Button " + title);
				value = selectedFolder;
				Dirty = true;
			}
		}, ButtonWidth);

		rect = new Rect(rect.x, rect.y, rect.width - ButtonWidth, rect.height);

		BrowseButton(rect, title, EditorGUIUtility.IconContent("UpArrow"), () => {
			if (Directory.Exists(value))
				Process.Start(value);
		}, ButtonWidth);

		rect = new Rect(rect.x, rect.y, rect.width - ButtonWidth, rect.height);

		value = !includeLabel ? EditorGUI.TextField(rect, value) : EditorGUI.TextField(rect, new GUIContent(title, title), value);

		return value;
	}

	public string FileField(Rect rect, string title, string value, bool save, string extension, bool includeLabel = true) {
		BrowseButton(rect, title, EditorGUIUtility.IconContent("Folder Icon"), () => {
			string directory = "";
			string defaultName = "";

			if (!string.IsNullOrEmpty(value)) {
				directory = Path.GetFileName(Path.GetFullPath(value));
				defaultName = Path.GetFileName(value);
			}

			var file = save ? EditorUtility.SaveFilePanel(title, directory, defaultName, extension) : EditorUtility.OpenFilePanel(title, directory, extension);

			if (!string.IsNullOrEmpty(file)) {
				GUI.FocusControl("Button " + title);
				value = file;
				Dirty = true;
			}
		}, ButtonWidth);

		rect = new Rect(rect.x, rect.y, rect.width - ButtonWidth, rect.height);

		value = !includeLabel ? EditorGUI.TextField(rect, value) : EditorGUI.TextField(rect, new GUIContent(title), value);
		return value;
	}

	/// <summary>
	/// Any changes made?
	/// </summary>
	protected bool Dirty { get; set; }

	protected bool scrollbarShown { get; set; }

	protected float scrollbarWidth { get; set; } = 16f;

	protected const int ButtonWidth = 24;

	public int IndentLevel { get; set; } = 0;

	protected void OnInspectorUpdate() {
		if (EditorApplication.isPlaying && !EditorApplication.isPaused) {
			Repaint();
		}
	}

	#region Editor GUI

	public float YPos;
	public float TotalYPos;
	public Vector2 ScrollPosition;
	protected virtual bool AllowScroll => true;

	protected Dictionary<string, string[]> EnumOptions = new Dictionary<string, string[]>();

	public void OnGUI() {
		YPos = 0;

		if (AllowScroll) {
			if (TotalYPos == 0f)
				TotalYPos = position.height;

			scrollbarShown = TotalYPos > position.height;
			ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), ScrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), TotalYPos));
		}

		UpdateEditorFields();

		if (AllowScroll) {
			TotalYPos = YPos;
			GUI.EndScrollView();
		}
	}

	protected virtual void UpdateEditorFields() { }

	protected void ExecuteTask(UniTask task) {
		async UniTask ExecuteTask_(UniTask task) {
			try {
				TimeController.StartStopwatch();
				// Run the action
				await task;
			} catch (Exception ex) {
				UnityEngine.Debug.LogError(ex.ToString());
			} finally {
				TimeController.StopStopwatch();
			}
		}
		_ = ExecuteTask_(task);
	}
	protected void ExecuteTask(Task task) => ExecuteTask(task.AsUniTask());

	public bool EditorField(string label, bool value, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.Toggle(rect ?? GetNextRect(ref YPos), label, value);
	}

	public T EditorField<T>(string label, T value, bool isVisible = true, Func<string[]> getEnumOptions = null, Rect? rect = null)
		where T : Enum {
		if (!isVisible)
			return value;

		if (!EnumOptions.ContainsKey(label))
			EnumOptions[label] = getEnumOptions == null ? EnumHelpers.GetValues<T>().Select(x => x.GetDescription()).ToArray() : getEnumOptions();

		switch (Type.GetTypeCode(typeof(T))) {
			case TypeCode.Byte: return (T)(object)(byte)EditorGUI.Popup(rect ?? GetNextRect(ref YPos), label, (byte)(object)value, EnumOptions[label]);
			default: return (T)(object)EditorGUI.Popup(rect ?? GetNextRect(ref YPos), label, (int)(object)value, EnumOptions[label]);
		}
	}

	public int EditorField(string label, int value, string[] options, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.Popup(rect ?? GetNextRect(ref YPos), label, value, options);
	}

	public string EditorField(string label, string value, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.TextField(rect ?? GetNextRect(ref YPos), label, value);
	}

	public int EditorField(string label, int value, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.IntField(rect ?? GetNextRect(ref YPos), label, value);
	}

	public long EditorField(string label, long value, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.LongField(rect ?? GetNextRect(ref YPos), label, value);
	}

	public float EditorField(string label, float value, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.FloatField(rect ?? GetNextRect(ref YPos), label, value);
	}

	public double EditorField(string label, double value, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return value;

		return EditorGUI.DoubleField(rect ?? GetNextRect(ref YPos), label, value);
	}

	public bool EditorButton(string label, bool isVisible = true, Rect? rect = null) {
		if (!isVisible)
			return false;

		return GUI.Button(rect ?? GetNextRect(ref YPos), label);
	}

	public void EditorHelpBox(string message, MessageType type, bool isVisible = true, float height = 40f, float padding = 4f, Rect? rect = null) {
		if (!isVisible) return;

		EditorGUI.HelpBox(rect ?? GetNextRect(ref YPos, height: height, padding: padding, vPadding: padding), message, type);
	}
	#endregion
}