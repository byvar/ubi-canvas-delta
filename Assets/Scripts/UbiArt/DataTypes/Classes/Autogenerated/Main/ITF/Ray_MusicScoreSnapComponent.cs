namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_MusicScoreSnapComponent : CSerializable {
		public float distOnCurve;
		public float intervalNote;
		public uint note;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				distOnCurve = s.Serialize<float>(distOnCurve, name: "distOnCurve");
				intervalNote = s.Serialize<float>(intervalNote, name: "intervalNote");
				note = s.Serialize<uint>(note, name: "note");
			}
		}
		public override uint? ClassCRC => 0x76A9C107;
	}
}

