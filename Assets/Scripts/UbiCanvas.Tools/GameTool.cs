using System;
using System.Collections.Generic;
using System.IO;
using UbiArt;

namespace UbiCanvas.Tools
{
	public abstract class GameTool
	{
		protected Context CreateContext(string basePath = null, Mode? mode = null,
			bool enableSerializerLog = true, bool? loadAnimations = null, bool? loadAllPaths = null, bool? loadStrings = null)
		{
			if (!mode.HasValue) mode = UnitySettings.GameMode;
			if (basePath == null) basePath = UnitySettings.GameDirs[mode.Value];

			Context context = new(basePath, Settings.FromMode(mode.Value),
				serializerLogger: enableSerializerLog ? new MapViewerSerializerLogger() : null,
				fileManager: new MapViewerFileManager(),
				systemLogger: new UnitySystemLogger(),
				asyncController: new UniTaskAsyncController());

			context.Loader.LoadAnimations = loadAnimations ?? UnitySettings.LoadAnimations;
			context.Loader.LoadAllPaths = loadAllPaths ?? UnitySettings.LoadAllPaths;

			if (loadStrings == true) {
				if (context.Loader.FileManager.FileExists("strings.txt")) {
					using (var str = context.Loader.FileManager.GetFileReadStream("strings.txt")) {
						using (StreamReader r = new StreamReader(str)) {
							while (!r.EndOfStream) {
								var line = r.ReadLine();
								context.AddToStringCache(line);
							}
						}
					}
				}
			}

			return context;

		}

		public abstract string Name { get; }
		public virtual string Description => null;

		public List<GameToolRequirement> Requirements { get; } = new();

		/// <summary>
		/// Arguments: title, folder, defaultName | Output: folder
		/// </summary>
		public Func<string, string, string, string> OpenFolderPanel { get; set; }
	}
}