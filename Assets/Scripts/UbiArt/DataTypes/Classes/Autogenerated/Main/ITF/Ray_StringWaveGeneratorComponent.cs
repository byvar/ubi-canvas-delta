namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_StringWaveGeneratorComponent : ActorComponent {
		public int startsActivated;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				startsActivated = s.Serialize<int>(startsActivated, name: "startsActivated");
			}
		}
		public override uint? ClassCRC => 0xA42F9D46;
	}
}

