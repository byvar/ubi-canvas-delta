namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_ChildLaunchComponent : ActorComponent {
		public uint nextLaunchIndex;
		public int hintFxEnabled;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				nextLaunchIndex = s.Serialize<uint>(nextLaunchIndex, name: "nextLaunchIndex");
			}
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				hintFxEnabled = s.Serialize<int>(hintFxEnabled, name: "hintFxEnabled");
			}
		}
		public override uint? ClassCRC => 0xD18C8678;
	}
}

