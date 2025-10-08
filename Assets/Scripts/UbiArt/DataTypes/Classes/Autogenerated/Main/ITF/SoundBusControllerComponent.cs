namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class SoundBusControllerComponent : ActorComponent {
		public CListO<SoundBusControllerComponent.Bus> busData;
		public CListO<Input> inputs;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			busData = s.SerializeObject<CListO<SoundBusControllerComponent.Bus>>(busData, name: "busData");
			inputs = s.SerializeObject<CListO<Input>>(inputs, name: "inputs");
		}
		public override uint? ClassCRC => 0xAFFF34B3;


		[Games(GameFlags.RO)]
		public class Bus : CSerializable {
			public StringID busName;
			public ProceduralInputData busFilterFrequency;
			public ProceduralInputData busFilterOneOverQ;
			public ProceduralInputData busVolume;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				busName = s.SerializeObject<StringID>(busName, name: "busName");
				busFilterFrequency = s.SerializeObject<ProceduralInputData>(busFilterFrequency, name: "busFilterFrequency");
				busFilterOneOverQ = s.SerializeObject<ProceduralInputData>(busFilterOneOverQ, name: "busFilterOneOverQ");
				busVolume = s.SerializeObject<ProceduralInputData>(busVolume, name: "busVolume");
			}
		}
	}
}

