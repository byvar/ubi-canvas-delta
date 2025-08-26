namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class TeleporterComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4DD45610;
	}
}

