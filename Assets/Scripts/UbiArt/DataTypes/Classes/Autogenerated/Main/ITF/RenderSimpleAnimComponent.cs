namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class RenderSimpleAnimComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1AB45C5D;
	}
}

