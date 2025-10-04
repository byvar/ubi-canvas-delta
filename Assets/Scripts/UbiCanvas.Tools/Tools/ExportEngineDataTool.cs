using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UbiArt;
using UbiArt.ITF;
using UbiArt.UV;
using UbiCanvas.Helpers;
using Path = UbiArt.Path;

namespace UbiCanvas.Tools
{
	public class ExportEngineDataTool : ExportActionGameTool
	{
		public ExportEngineDataTool()
		{
			//Requirements.Add(new ModeGameToolRequirement(Settings.Mode.RaymanLegendsPC, Settings.Mode.RaymanAdventuresAndroid));
		}

		public override string Name => "Export Engine Data as JSON";
		public override string Description => "This tool exports all generic files in the EngineData folder to serialized JSON files";

		public override async Task InvokeAsync(string outputDir)
		{
			using Context context = CreateContext(loadStrings: true);
			await context.Loader.LoadInitial();

			string itfDir = context.Settings.ITFDirectory;

			if (context.Settings.EngineVersion >= EngineVersion.RL) {
				foreach (string configFile in Directory.GetFiles(
								System.IO.Path.Combine(context.BasePath, itfDir, "enginedata"),
								"*.isg.ckd", SearchOption.AllDirectories)) {
					string relativePath = configFile.Substring(configFile.IndexOf("enginedata"));
					context.Loader.LoadGenericFile(new Path(relativePath), x => {
						string exportFile = System.IO.Path.Combine(outputDir, $"{relativePath}.json");
						Directory.CreateDirectory(System.IO.Path.GetDirectoryName(exportFile));
						File.WriteAllText(exportFile, JsonConvert.SerializeObject(x, Formatting.Indented));
					});
				}
			} else {
				void ProcessFile<T>(string relativePath) where T : CSerializable, new() {
					var path = new Path(relativePath);
					context.Loader.LoadFile<T>(path, x => {
						string exportFile = System.IO.Path.Combine(outputDir, $"{relativePath}.json");
						Directory.CreateDirectory(System.IO.Path.GetDirectoryName(exportFile));
						File.WriteAllText(exportFile, JsonConvert.SerializeObject(x, Formatting.Indented));
					});
				}
				ProcessFile<ZInputManager_Template>("inputs/input_win32.isg");
				ProcessFile<ZInputManager_Template>("inputs/menu/input_menu_x360.isg");
				ProcessFile<ZInputManager_Template>("inputs/cheat/input_cheat_win32.isg");
				ProcessFile<Ray_GameManagerConfig_Template>("gameconfig/gameconfig.isg");
				ProcessFile<FactionManager_Template>("gameconfig/factionconfig.isg");
				ProcessFile<CameraShakeConfig_Template>("gameconfig/camerashakeconfig.isg");
				ProcessFile<ContextIconsManager_Template>("gameconfig/contexticons.isg");
				ProcessFile<PadRumbleManager_Template>("gameconfig/padrumbleconfig.isg");
				ProcessFile<Ray_RewardContainer_Template>("gameconfig/rewardlist.isg");
				ProcessFile<SoundConfig_Template>("gameconfig/soundconfig.isg");
				ProcessFile<UITextManager_Template>("gameconfig/uitextconfig.isg");
			}

			// Load UV Atlas manager for textures
			Path pAtlas = new Path("", "atlascontainer.ckd");
			context.Loader.LoadFile<UVAtlasManager>(pAtlas, x => {
				string exportFile = System.IO.Path.Combine(outputDir, $"atlascontainer.json");
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(exportFile));
				File.WriteAllText(exportFile, JsonConvert.SerializeObject(x, Formatting.Indented));
			});

			await context.Loader.LoadLoop();
		}
	}
}