﻿using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UbiArt;
using UbiCanvas.Helpers;
using UbiCanvas.Tools;
using UnityEditor;
using UnityEngine;

public class UnityWindowTools : UnityWindow 
{
	[MenuItem("Ubi-Canvas/Game Tools")]
	public static void ShowWindow() => GetWindow<UnityWindowTools>("Game Tools");

	private void OnEnable() 
	{
		titleContent = EditorGUIUtility.IconContent("CustomTool");
		titleContent.text = "Game Tools";
	}

	protected override void UpdateEditorFields() {
		base.UpdateEditorFields();

		foreach (GameTool tool in GameTools.Tools)
		{
			var toolKey = tool.GetType().GetFormattedName();
			if (!_toolFoldouts.ContainsKey(toolKey))
				_toolFoldouts.Add(toolKey, false);

			bool foldout = _toolFoldouts[toolKey] = EditorGUI.BeginFoldoutHeaderGroup(GetNextRect(ref YPos, vPadding: 2), _toolFoldouts[toolKey], tool.Name);

			if (foldout)
			{
				string descr = tool.Description;

				if (descr != null)
					EditorGUI.LabelField(GetNextRect(ref YPos, vPaddingBottom: 4), descr, EditorStyles.miniLabel);

				var notMetRequirements = tool.Requirements.Select(x => x.IsAvailable()).Where(x => !x.IsAvailable).ToList();

				if (notMetRequirements.Any())
				{
					EditorHelpBox("The tool can not run due to the following requirements not being met:" +
					                        $"{Environment.NewLine}" +
					                        $"{String.Join(Environment.NewLine, notMetRequirements.Select(x => $"- {x.RequirementText}"))}", MessageType.Warning,
											rect: GetNextRect(ref YPos, linesCount: (notMetRequirements.Count + 1)));

					EditorGUI.EndFoldoutHeaderGroup();
					continue;
				}

				ShowGameToolGUI(tool);
			}

			EditorGUI.EndFoldoutHeaderGroup();
		}
	}

	private void ShowGameToolGUI(GameTool tool) {
		if (tool is ActionGameTool actionGameTool)
		{
			if(EditorButton("Run action"))
				ExecuteTask(actionGameTool.InvokeAsync());
		}
		else if (tool is ExportActionGameTool exportActionGameTool)
		{
			if (EditorButton("Export"))
			{
				string outputDir = EditorUtility.OpenFolderPanel("Select output directory", null, "");
				
				if (!String.IsNullOrWhiteSpace(outputDir))
					ExecuteTask(exportActionGameTool.InvokeAsync(outputDir));
			}
		}
		else if (tool is SerializerTestTool serializeGenericTool)
		{
			if (EditorButton("Scan all generic files")) {
				string inputDir = EditorUtility.OpenFolderPanel("Select input directory", UnitySettings.GameDirs[UnitySettings.GameMode], "");
				if (!String.IsNullOrWhiteSpace(inputDir)) {
					ExecuteTask(serializeGenericTool.SerializeAsync(inputDir));
				}
			}
			if (EditorButton("Scan non-generic files")) {
				string inputDir = EditorUtility.OpenFolderPanel("Select input directory", UnitySettings.GameDirs[UnitySettings.GameMode], "");
				if (!String.IsNullOrWhiteSpace(inputDir)) {
					ExecuteTask(serializeGenericTool.SerializeNonGenericAsync(inputDir));
				}
			}
			if (EditorButton("Fix all memory sizes")) {
				string inputDir = EditorUtility.OpenFolderPanel("Select input directory", UnitySettings.GameDirs[UnitySettings.GameMode], "");
				if (!String.IsNullOrWhiteSpace(inputDir)) {
					string outputDir = EditorUtility.OpenFolderPanel("Select output directory", inputDir, "");

					if (!String.IsNullOrWhiteSpace(outputDir))
						ExecuteTask(serializeGenericTool.SerializeAsync(inputDir, outputDir));
				}
			}
		}
		else if (tool is ExportTimelineTool exportTimelineTool)
		{
			if (EditorButton("Export"))
			{

				string inputDir = EditorUtility.OpenFolderPanel("Select input directory containing IPKs", null, "");
				if (!String.IsNullOrWhiteSpace(inputDir)) {
					string outputDir = EditorUtility.OpenFolderPanel("Select output directory", null, "");

					if (!String.IsNullOrWhiteSpace(outputDir))
						ExecuteTask(exportTimelineTool.ExportSingleTimelineAsync(inputDir, outputDir));
				}
			}
			if (EditorButton("Iterate over versions")) {

				string inputDir = EditorUtility.OpenFolderPanel("Select input directory containing subdirectories containing IPKs", null, "");
				if (!String.IsNullOrWhiteSpace(inputDir)) {
					ExecuteTask(exportTimelineTool.ExportMultipleTimelineAsync(inputDir));
				}
			}
		}
		else if (tool is BuildModIPKTool buildModIPKTool)
		{
			EditorGUI.BeginChangeCheck();

			UnitySettings.Tools_BuildModIPK_GameMode = EditorField<Mode>("Game", UnitySettings.Tools_BuildModIPK_GameMode);

			UnitySettings.Tools_BuildModIPK_InputPath = DirectoryField(GetNextRect(ref YPos), "Input path (raw files)", UnitySettings.Tools_BuildModIPK_InputPath, true);
			UnitySettings.Tools_BuildModIPK_OutputPath = DirectoryField(GetNextRect(ref YPos), "Output path", UnitySettings.Tools_BuildModIPK_OutputPath, true);
			UnitySettings.Tools_BuildModIPK_OriginalBundlesPath = DirectoryField(GetNextRect(ref YPos), "Original bundles path", UnitySettings.Tools_BuildModIPK_OriginalBundlesPath, true);

			UnitySettings.Tools_BuildModIPK_CreateSecureFatAfterIPK = EditorField("Create secure_fat", UnitySettings.Tools_BuildModIPK_CreateSecureFatAfterIPK);
			UnitySettings.Tools_BuildModIPK_BundleBaseName = EditorField("Bundle base name", UnitySettings.Tools_BuildModIPK_BundleBaseName);
			UnitySettings.Tools_BuildModIPK_BundleOrder = EditorField("Bundle order", UnitySettings.Tools_BuildModIPK_BundleOrder);
			
			if (EditorGUI.EndChangeCheck()) {
				UnitySettings.Save();
			}

			foreach (MultiActionGameTool.InvokableAction invokableAction in buildModIPKTool.Actions) {
				if (EditorButton(invokableAction.Name))
					ExecuteTask(invokableAction.Action());
			}
		}
		else if (tool is MultiActionGameTool multiActionGameTool)
		{
			foreach (MultiActionGameTool.InvokableAction invokableAction in multiActionGameTool.Actions)
			{
				if (EditorButton(invokableAction.Name))
					ExecuteTask(invokableAction.Action());
			}
		}
		else if (tool is CRCCalculatorTool crcTool)
		{
			crcTool.CurrentString = EditorField("String", crcTool.CurrentString);
			EditorGUI.TextField(GetNextRect(), "String CRC", crcTool.StringCRC);
			if (UnityEngine.Application.isPlaying) {
				if (GlobalLoadState.LoadState == GlobalLoadState.State.Finished) {
					crcTool.CurrentReverseCRCString = EditorField("Reverse CRC", crcTool.CurrentReverseCRCString);
					string result = "Invalid CRC";
					if (uint.TryParse(crcTool.CurrentReverseCRCString,
						System.Globalization.NumberStyles.HexNumber,
						System.Globalization.CultureInfo.InvariantCulture,
						out uint crc)) {
						var sid = new StringID(crc);
						if (Controller.MainContext?.StringCache != null && Controller.MainContext.StringCache.ContainsKey(sid)) {
							result = Controller.MainContext.StringCache[sid];
						} else {
							result = "Not found";
						}
					}
					EditorGUI.TextField(GetNextRect(), "Reverse CRC: String", result);
				}
			}
			crcTool.ShowRegularCRCDropdown = EditorGUI.Foldout(GetNextRect(vPadding: 2), crcTool.ShowRegularCRCDropdown, "Other CRC");
			if (crcTool.ShowRegularCRCDropdown) {
				EditorGUI.TextField(GetNextRect(), "CRC", crcTool.CRC(null));
				foreach (var uafTag in EnumHelpers.GetValues<CSerializerObject.UAFTag>()) {
					EditorGUI.TextField(GetNextRect(), $"CRC - {uafTag}", crcTool.CRC((uint)uafTag));
				}
				var rect = GetNextRect();
				rect = EditorGUI.PrefixLabel(rect, new GUIContent("CRC - Custom number"));
				crcTool.CustomType = (uint)EditorGUI.IntField(new Rect(rect.x, rect.y, rect.width / 4 - 4, rect.height), (int)crcTool.CustomType);
				EditorGUI.TextField(new Rect(rect.x + rect.width / 4, rect.y, rect.width / 4 * 3, rect.height), crcTool.CRC(crcTool.CustomType));
			}
		}
		else if (tool is SerializableFileTool logFileTool)
		{
			logFileTool.FilePath = EditorField("File path", logFileTool.FilePath);

			logFileTool.AutomaticallyDetermineType = EditorField("Use extension for type", logFileTool.AutomaticallyDetermineType);

			if (!logFileTool.AutomaticallyDetermineType) {
				logFileTool.Type = EditorField("File type", logFileTool.Type);
				logFileTool.Namespace = EditorField("Namespace", logFileTool.Namespace);
				logFileTool.UseContainer = EditorField("Use file container", logFileTool.UseContainer);
			}

			logFileTool.LogInitialFiles = EditorField("Log initial files", logFileTool.LogInitialFiles);
			logFileTool.LoadConfig = EditorField("Load config", logFileTool.LoadConfig);
			logFileTool.LoadDependencies = EditorField("Load dependencies", logFileTool.LoadDependencies);

			if (EditorButton("Deserialize"))
				ExecuteTask(logFileTool.DeserializeAsync());
			if (EditorButton("Edit"))
			{
				async UniTask edit()
				{
					(Path path, Context context) = await logFileTool.DeserializeAsync();

					if (path?.Object == null)
					{
						Debug.LogError("Object is null");
						return;
					}

					UnityWindowSerializableEditor window = GetWindow<UnityWindowSerializableEditor>(false, "Serializer", true);
					window.Path = path;
					window.SetContext(context);
				}

				ExecuteTask(edit());
			}
		}
		else
		{
			EditorHelpBox($"The tool type {tool.GetType().GetFormattedName()} does not have a supported UI", MessageType.Error);
		}
	}

	private readonly Dictionary<string, bool> _toolFoldouts = new();
}