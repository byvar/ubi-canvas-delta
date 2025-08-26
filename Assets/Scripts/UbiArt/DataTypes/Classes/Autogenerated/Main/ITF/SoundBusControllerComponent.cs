namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class SoundBusControllerComponent : ActorComponent {
		public Placeholder busData;
		public Placeholder inputs;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			busData = s.SerializeObject<Placeholder>(busData, name: "busData");
			inputs = s.SerializeObject<Placeholder>(inputs, name: "inputs");
		}
		public override uint? ClassCRC => 0xAFFF34B3;
	}
}

