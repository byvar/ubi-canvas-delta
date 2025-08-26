namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class SimpleAIComponent : GenericAIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3016E8D6;
	}
}

