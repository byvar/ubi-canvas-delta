namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_OnlineBallComponent : ActorComponent {
		public bool disableWindForce;
		public bool startWithHalo;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				disableWindForce = s.Serialize<bool>(disableWindForce, name: "disableWindForce", options: CSerializerObject.Options.BoolAsByte);
				startWithHalo = s.Serialize<bool>(startWithHalo, name: "startWithHalo", options: CSerializerObject.Options.BoolAsByte);
			}
		}
		public override uint? ClassCRC => 0x389D417B;
	}
}

