namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_CageAIComponent : Ray_FixedAIComponent {
		public int CageIndex;
		public int wasBrokenInSession;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				CageIndex = s.Serialize<int>(CageIndex, name: "CageIndex");
			}
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				wasBrokenInSession = s.Serialize<int>(wasBrokenInSession, name: "wasBrokenInSession");
			}
		}
		public override uint? ClassCRC => 0x1F1365D4;
	}
}

