namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_BossMapEndAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3DE62818;
	}
}

