namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class SoundBusControllerComponent_Template : ActorComponent_Template {
		public CListO<SoundBusControllerComponent.Bus> busData;
		public CListO<Input> inputs;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			busData = s.SerializeObject<CListO<SoundBusControllerComponent.Bus>>(busData, name: "busData");
			inputs = s.SerializeObject<CListO<Input>>(inputs, name: "inputs");
		}
		public override uint? ClassCRC => 0x84212367;
	}
}

