namespace UbiCanvas.Tools
{
	public static class GameTools
	{
		public static GameTool[] Tools { get; } =
		{
			new SerializableFileTool(),
			new ExportEngineDataTool(),
			new SerializerTestTool(),
			new ExportLocalisationTool(),
			new ExportTimelineTool(),
			new AdventuresSaveTool(),
			new BuildModIPKTool(),
			new CRCCalculatorTool(),
		};
	}
}