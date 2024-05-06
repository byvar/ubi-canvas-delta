namespace UbiArt.ITF {
	[Games(GameFlags.RFR)]
	public partial class Ray_GuardianNodeAIComponent : ActorComponent {
		public Enum_RFR_0 activeMode;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			activeMode = s.Serialize<Enum_RFR_0>(activeMode, name: "activeMode");
		}
		public enum Enum_RFR_0 {
			[Serialize("Value_1")] Value_1 = 1,
		}
		public override uint? ClassCRC => 0xCA2CCDCE;
	}
}

