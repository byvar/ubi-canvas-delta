namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class KeepAliveComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF7572C23;
	}
}

