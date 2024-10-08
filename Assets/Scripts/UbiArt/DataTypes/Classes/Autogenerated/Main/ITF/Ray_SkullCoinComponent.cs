namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_SkullCoinComponent : GraphicComponent {
		public int isTaken;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				isTaken = s.Serialize<int>(isTaken, name: "isTaken");
			}
		}
		public override uint? ClassCRC => 0x56F2D5DC;
	}
}

