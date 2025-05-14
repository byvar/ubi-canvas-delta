using System;
using System.Reflection;
using System.Threading.Tasks;
using UbiArt;
using UbiArt.ITF;

namespace UbiCanvas.Tools
{
	public class SerializableFileTool : GameTool
	{
		public override string Name => "Deserialize, log & edit file";
		public override string Description => "This tool deserializes the specified file so that it can be logged or edited. The type specifies the C# data type to deserialize the file as. If the type is empty then the file is assumed to be generic.";

		public string FilePath { get; set; }
		public string Type { get; set; }
		public string Namespace { get; set; } = "UbiArt.ITF";
		public bool UseContainer { get; set; }
		public bool LogInitialFiles { get; set; }
		public bool LoadConfig { get; set; }
		public bool AutomaticallyDetermineType { get; set; } = true;
		public bool LoadDependencies { get; set; } = true;

		public async Task<(Path, Context)> DeserializeAsync()
		{
			using Context context = CreateContext();

			bool originalLogValue = UnitySettings.Log;
			if (!LogInitialFiles)
				UnitySettings.Log = false;

			await context.Loader.LoadInitial();

			if (LoadConfig) {
				switch (context.Settings.EngineVersion) {
					case EngineVersion.RO:
						context.Loader.LoadFile<Ray_GameManagerConfig_Template>(new Path("gameconfig/gameconfig.isg"), config => {
							context.Loader.gameConfigRO = config;
						});
						break;
					case EngineVersion.RL:
						context.Loader.LoadGenericFile(new Path("enginedata/gameconfig/gameconfig.isg"), isg =>
						{
							context.Loader.gameConfig = isg.obj as RO2_GameManagerConfig_Template;
						});
						break;
				}
			}

			if (!LogInitialFiles)
				UnitySettings.Log = originalLogValue;

			Path path = new(FilePath);

			if (AutomaticallyDetermineType)
			{
				path.LoadObject(context, removeCooked: true);
			}
			else
			{
				if (String.IsNullOrWhiteSpace(Type))
				{
					context.Loader.LoadGenericFile(path, x => path.Object = x);
				}
				else
				{
					Type type;
					if (UseContainer)
						type = typeof(ContainerFile<>).MakeGenericType(System.Type.GetType($"{Namespace}.{Type}"));
					else
						type = System.Type.GetType($"{Namespace}.{Type}");

					context.Loader.LoadFile(path, type, x => path.Object = x);
				}
			}

			await context.Loader.LoadLoop(single: !LoadDependencies);
			return (path, context);
		}
	}
}