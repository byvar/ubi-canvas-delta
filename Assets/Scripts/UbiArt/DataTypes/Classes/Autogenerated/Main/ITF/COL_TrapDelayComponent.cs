namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TrapDelayComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x22A1FF1B;
	}
}

