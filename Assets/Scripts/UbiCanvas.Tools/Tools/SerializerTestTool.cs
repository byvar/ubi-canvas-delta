using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Bundle;
using UbiArt.ITF;
using UbiCanvas.Helpers;
using Path = UbiArt.Path;

namespace UbiCanvas.Tools
{
	public class SerializerTestTool : GameTool
	{
		public SerializerTestTool()
		{
			Requirements.Add(new ModeGameToolRequirement(Mode.RaymanLegendsPC, Mode.RaymanOriginsPC));
		}

		public override string Name => "Serializer Test Tool";
		public override string Description => "Serializes all generic files in the game to test the serializer and fix memory size issues.";

		public async Task SerializeAsync(string basePath, string outputPath = null)
		{
			bool fixFiles = outputPath != null;
			var Bundle = new BundleFile();

			using Context context = CreateContext(basePath: basePath, enableSerializerLog: false);
			bool originalLogValue = UnitySettings.Log;
			UnitySettings.Log = false;
			await context.Loader.LoadInitial();
			UnitySettings.Log = originalLogValue;

			string itfDir = context.Settings.ITFDirectory;

			async Task ProcessAllGeneric(string extension) {
				context.SystemLogger?.LogInfo($"Serializing all generic files with extension: {extension}");
				foreach (string configFile in Directory.GetFiles(
							 System.IO.Path.Combine(context.BasePath, itfDir),
							 $"*.{extension}.ckd", SearchOption.AllDirectories)) {
					string relativePath = configFile.Substring(configFile.IndexOf(itfDir) + itfDir.Length);
					context.Loader.LoadGenericFile(new Path(relativePath), g => {
						if (fixFiles) {
							Bundle.AddFile(new Path(relativePath).UncookedPath(context).CookedPath(context), g);
						}
					});
					await context.Loader.LoadLoop(single: true);
				}
			}

			await ProcessAllGeneric("isg");
			await ProcessAllGeneric("tpl");
			await ProcessAllGeneric("fcg");
			await ProcessAllGeneric("gmt");
			await ProcessAllGeneric("frt");
			await ProcessAllGeneric("msh");
			await ProcessAllGeneric("tfn");
			await ProcessAllGeneric("cache");

			if (fixFiles) {
				if (!Bundle.IsEmpty) await context.Loader.WriteFilesRaw(outputPath, Bundle);
			}
			context.SystemLogger?.LogInfo("Completed.");
		}

		public async Task SerializeNonGenericAsync(string basePath) {
			using Context context = CreateContext(basePath: basePath, enableSerializerLog: false);
			bool originalLogValue = UnitySettings.Log;
			UnitySettings.Log = false;
			await context.Loader.LoadInitial();
			UnitySettings.Log = originalLogValue;

			string itfDir = context.Settings.ITFDirectory;

			async Task ProcessAllNonGeneric(string extension) {
				context.SystemLogger?.LogInfo($"Serializing all non-generic files with extension: {extension}");
				foreach (string configFile in Directory.GetFiles(
							 System.IO.Path.Combine(context.BasePath, itfDir),
							 $"*.{extension}.ckd", SearchOption.AllDirectories)) {
					string relativePath = configFile.Substring(configFile.IndexOf(itfDir) + itfDir.Length);
					var path = new Path(relativePath).UncookedPath(context);
					//context.SystemLogger?.LogInfo($"Serializing file: {path.FullPath}");
					path.LoadObject(context);
					await context.Loader.LoadLoop(single: true);
				}
			}
			async Task ProcessFile<T>(string relativePath) where T : CSerializable, new() {
				var path = new Path(relativePath);
				context.Loader.LoadFile<T>(path, _ => { });
				await context.Loader.LoadLoop(single: true);
			}

			if (context.Settings.EngineVersion == EngineVersion.RO) {
				// Config files
				await ProcessFile<ZInputManager_Template>("inputs/input_win32.isg");
				await ProcessFile<ZInputManager_Template>("inputs/menu/input_menu_x360.isg");
				await ProcessFile<ZInputManager_Template>("inputs/cheat/input_cheat_win32.isg");
				await ProcessFile<Ray_GameManagerConfig_Template>("gameconfig/gameconfig.isg");
				await ProcessFile<FactionManager_Template>("gameconfig/factionconfig.isg");
				await ProcessFile<CameraShakeConfig_Template>("gameconfig/camerashakeconfig.isg");
				await ProcessFile<ContextIconsManager_Template>("gameconfig/contexticons.isg");
				await ProcessFile<PadRumbleManager_Template>("gameconfig/padrumbleconfig.isg");
				await ProcessFile<Ray_RewardContainer_Template>("gameconfig/rewardlist.isg");
				await ProcessFile<SoundConfig_Template>("gameconfig/soundconfig.isg");
				await ProcessFile<UITextManager_Template>("gameconfig/uitextconfig.isg");

				await ProcessAllNonGeneric("isc");
				await ProcessAllNonGeneric("act");
				await ProcessAllNonGeneric("tsc");
				await ProcessAllNonGeneric("frz");
				await ProcessAllNonGeneric("isg");
				await ProcessAllNonGeneric("fcg");
				await ProcessAllNonGeneric("gmt");
				await ProcessAllNonGeneric("frt");
				await ProcessAllNonGeneric("msh");
				await ProcessAllNonGeneric("tfn");
				await ProcessAllNonGeneric("cache");
				await ProcessAllNonGeneric("imt");

				// Animation
				await ProcessAllNonGeneric("anm");
				await ProcessAllNonGeneric("pbk");
				await ProcessAllNonGeneric("skl");

				// to add: .dep (uncooked), .alias (uncooked)
			} else {
				await ProcessAllNonGeneric("isc");
				await ProcessAllNonGeneric("act");
				await ProcessAllNonGeneric("tsc");
				await ProcessAllNonGeneric("frz");

				// Animation
				await ProcessAllNonGeneric("anm");
				await ProcessAllNonGeneric("pbk");
				await ProcessAllNonGeneric("skl");
			}

			context.SystemLogger?.LogInfo("Completed.");
		}
	}
}