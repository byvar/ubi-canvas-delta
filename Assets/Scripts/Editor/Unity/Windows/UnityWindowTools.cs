using Cysharp.Threading.Tasks;
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
			if (UnitySettings.GameMode == Mode.RaymanOriginsPC) {
				if (EditorButton("Scan non-generic files")) {
					string inputDir = EditorUtility.OpenFolderPanel("Select input directory", UnitySettings.GameDirs[UnitySettings.GameMode], "");
					if (!String.IsNullOrWhiteSpace(inputDir)) {
						ExecuteTask(serializeGenericTool.SerializeNonGenericAsync(inputDir));
					}
				}
			} else {
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
		else if (tool is ConvertMiniPbkToLegendsTool convertMiniPbkTool)
		{
			convertMiniPbkTool.MiniGame = EditorField("Mini file game", convertMiniPbkTool.MiniGame);
			convertMiniPbkTool.MiniPBKPath = EditorField("Mini PBK path (start at world/...)", convertMiniPbkTool.MiniPBKPath);

			convertMiniPbkTool.LegendsGame = EditorField("Legends base game", convertMiniPbkTool.LegendsGame);
			convertMiniPbkTool.LegendsPBKPath = EditorField("Legends PBK path (start at world/...)", convertMiniPbkTool.LegendsPBKPath);

			convertMiniPbkTool.OutputGame = EditorField("Output game folder", convertMiniPbkTool.OutputGame);
			convertMiniPbkTool.OutputPBKPath = EditorField("Output PBK path (start at world/...)", convertMiniPbkTool.OutputPBKPath);
			convertMiniPbkTool.ExcludedTemplateKeyCount = Math.Max(0, EditorField("Excluded key count", convertMiniPbkTool.ExcludedTemplateKeyCount));
			while (convertMiniPbkTool.ExcludedTemplateKeyInputs.Count < convertMiniPbkTool.ExcludedTemplateKeyCount)
				convertMiniPbkTool.ExcludedTemplateKeyInputs.Add(string.Empty);
			while (convertMiniPbkTool.ExcludedTemplateKeyInputs.Count > convertMiniPbkTool.ExcludedTemplateKeyCount)
				convertMiniPbkTool.ExcludedTemplateKeyInputs.RemoveAt(convertMiniPbkTool.ExcludedTemplateKeyInputs.Count - 1);

			for (int i = 0; i < convertMiniPbkTool.ExcludedTemplateKeyInputs.Count; i++) {
				convertMiniPbkTool.ExcludedTemplateKeyInputs[i] = EditorField($"Excluded key {i + 1} (dec/hex)", convertMiniPbkTool.ExcludedTemplateKeyInputs[i]);
			}
			convertMiniPbkTool.ExcludeUntranslatableKeys = EditorField("Exclude keys with reverse string = Not found", convertMiniPbkTool.ExcludeUntranslatableKeys);
			convertMiniPbkTool.CopyOnlyPresetRaymanKeyNames = EditorField("Copy only preset key names (liste fournie)", convertMiniPbkTool.CopyOnlyPresetRaymanKeyNames);
			convertMiniPbkTool.ForceAddMissingKeysFromSource = EditorField("Force add missing source keys", convertMiniPbkTool.ForceAddMissingKeysFromSource);
			convertMiniPbkTool.KeepOriginalTags = EditorField("Keep original pointLinks/patches (+ patchPoints limited to existing, tags copied)", convertMiniPbkTool.KeepOriginalTags);
			convertMiniPbkTool.ReplaceAllLegendsKeysWithSource = EditorField("Replace all Legends keys/templates with source (preserve Legends UInt32 when possible)", convertMiniPbkTool.ReplaceAllLegendsKeysWithSource);

			if (EditorButton("Convert"))
				ExecuteTask(convertMiniPbkTool.ConvertAsync());
		}

		else if (tool is PbkKeyCompareTool keyCompareTool)
		{
			keyCompareTool.FirstGame = EditorField("First game", keyCompareTool.FirstGame);
			keyCompareTool.FirstPBKPath = EditorField("First PBK path (start at world/...)", keyCompareTool.FirstPBKPath);

			keyCompareTool.SecondGame = EditorField("Second game", keyCompareTool.SecondGame);
			keyCompareTool.SecondPBKPath = EditorField("Second PBK path (start at world/...)", keyCompareTool.SecondPBKPath);

			if (EditorButton("Compare"))
				ExecuteTask(keyCompareTool.CompareAsync());

		}
		else if (tool is PbkDeepDiffTool pbkDeepDiffTool)
		{
			pbkDeepDiffTool.FirstGame = EditorField("First game", pbkDeepDiffTool.FirstGame);
			pbkDeepDiffTool.FirstPBKPath = EditorField("First PBK path (start at world/...)", pbkDeepDiffTool.FirstPBKPath);

			pbkDeepDiffTool.SecondGame = EditorField("Second game", pbkDeepDiffTool.SecondGame);
			pbkDeepDiffTool.SecondPBKPath = EditorField("Second PBK path (start at world/...)", pbkDeepDiffTool.SecondPBKPath);
			pbkDeepDiffTool.MaxDifferencesToLog = Math.Max(1, EditorField("Max differences to log", pbkDeepDiffTool.MaxDifferencesToLog));

			if (EditorButton("Deep compare"))
				ExecuteTask(pbkDeepDiffTool.CompareAsync());

		}
		else if (tool is PbkAtlasTemplateTransferTool transferTool)
		{
			transferTool.SourceGame = EditorField("Source game", transferTool.SourceGame);
			transferTool.SourceTexturePath = EditorField("Source texture path (start at world/...)", transferTool.SourceTexturePath);
			transferTool.SourcePBKPath = EditorField("Source PBK path (start at world/...)", transferTool.SourcePBKPath);

			transferTool.TargetGame = EditorField("Target game", transferTool.TargetGame);
			transferTool.TargetTexturePath = EditorField("Target texture path (start at world/...)", transferTool.TargetTexturePath);
			transferTool.TargetPBKPath = EditorField("Target PBK path (start at world/...)", transferTool.TargetPBKPath);

			transferTool.OutputGame = EditorField("Output game folder", transferTool.OutputGame);
			transferTool.OutputPBKPath = EditorField("Output PBK path (start at world/...)", transferTool.OutputPBKPath);
			transferTool.PreserveTargetUInt32Identifiers = EditorField("Preserve target UInt32 identifiers (recommended)", transferTool.PreserveTargetUInt32Identifiers);
			transferTool.PreserveTargetPatchTopology = EditorField("Preserve target patch topology/UV links (recommended)", transferTool.PreserveTargetPatchTopology);
			transferTool.PreserveTargetBoneDynamicsAndScale = EditorField("Preserve target bone dynamics/scale (recommended)", transferTool.PreserveTargetBoneDynamicsAndScale);

			if (EditorButton("Copy all templates and save PBK"))
				ExecuteTask(transferTool.TransferAsync());

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

					UnityWindowSerializableEditor window = CreateWindow<UnityWindowSerializableEditor>();
					window.SetPath(path);
					window.SetContext(context);
					window.Show();
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