namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class ActorPlugStateImplement : WithAnimStateImplement {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x0ED65DBE;
	}
}

