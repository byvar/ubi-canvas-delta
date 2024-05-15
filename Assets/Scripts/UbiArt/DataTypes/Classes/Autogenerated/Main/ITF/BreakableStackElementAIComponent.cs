namespace UbiArt.ITF {
	[Games(GameFlags.RAVersion)]
	public partial class BreakableStackElementAIComponent : AIComponent {
		public ObjectPath managerPath;
		public uint countSpawnMax;
		public bool blockIsDestroy;
		public uint checkPointRow;
		public uint checkPointCol;
		public uint blockState;
		public bool hasTuto;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				managerPath = s.SerializeObject<ObjectPath>(managerPath, name: "managerPath");
			}
			countSpawnMax = s.Serialize<uint>(countSpawnMax, name: "countSpawnMax");
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				blockIsDestroy = s.Serialize<bool>(blockIsDestroy, name: "blockIsDestroy");
				checkPointRow = s.Serialize<uint>(checkPointRow, name: "checkPointRow");
				checkPointCol = s.Serialize<uint>(checkPointCol, name: "checkPointCol");
				blockState = s.Serialize<uint>(blockState, name: "blockState");
			}
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				hasTuto = s.Serialize<bool>(hasTuto, name: "hasTuto");
			}
		}
		public override uint? ClassCRC => 0x2ECD38C3;
	}
}

