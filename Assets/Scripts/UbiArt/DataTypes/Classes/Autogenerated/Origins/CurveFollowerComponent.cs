namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class CurveFollowerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xC3AB1CEC;
	}
}

