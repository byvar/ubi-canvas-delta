namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA)]
	public partial class RO2_CamModeMoverComponent : ActorComponent {
		public float TimeToMove = 2;
		public float BlendCoeff = 0.1f;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				TimeToMove = s.Serialize<float>(TimeToMove, name: "TimeToMove");
				BlendCoeff = s.Serialize<float>(BlendCoeff, name: "BlendCoeff");
			}
		}
		public override uint? ClassCRC => 0xD2957F2A;
	}
}

