namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class SoundBankComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE7493C74;
	}
}

