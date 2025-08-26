namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class PlayerControllerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x4912A3E0;
	}
}

