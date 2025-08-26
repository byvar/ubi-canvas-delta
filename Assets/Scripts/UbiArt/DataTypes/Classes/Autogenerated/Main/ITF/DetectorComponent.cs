namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class DetectorComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x6E03EEC7;
	}
}

