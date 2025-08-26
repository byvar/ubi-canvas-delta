namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_ElevatorAIComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD668B9AB;
	}
}

