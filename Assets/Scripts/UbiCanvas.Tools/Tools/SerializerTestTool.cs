using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.Bundle;
using Path = UbiArt.Path;

namespace UbiCanvas.Tools
{
	public class SerializerTestTool : GameTool
	{
		public SerializerTestTool()
		{
			Requirements.Add(new ModeGameToolRequirement(Mode.RaymanLegendsPC));
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
				context.SystemLogger?.LogInfo($"Serializing all generic files with extension: {extension}");
				foreach (string configFile in Directory.GetFiles(
							 System.IO.Path.Combine(context.BasePath, itfDir),
							 $"*.{extension}.ckd", SearchOption.AllDirectories)) {
					string relativePath = configFile.Substring(configFile.IndexOf(itfDir) + itfDir.Length);
					var path = new Path(relativePath).UncookedPath(context);
					path.LoadObject(context);
					await context.Loader.LoadLoop(single: true);
				}
			}

			await ProcessAllNonGeneric("isc");
			await ProcessAllNonGeneric("act");
			await ProcessAllNonGeneric("tsc");
			await ProcessAllNonGeneric("frz");

			context.SystemLogger?.LogInfo("Completed.");
		}

	}
}