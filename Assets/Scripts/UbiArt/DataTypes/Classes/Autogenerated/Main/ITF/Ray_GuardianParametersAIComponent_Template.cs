namespace UbiArt.ITF {
	[Games(GameFlags.RFR)]
	public partial class Ray_GuardianParametersAIComponent_Template : ActorComponent_Template {
		public int int__0;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			int__0 = s.Serialize<int>(int__0, name: "int__0");
		}
		public override uint? ClassCRC => 0xB8760440;
	}
}

