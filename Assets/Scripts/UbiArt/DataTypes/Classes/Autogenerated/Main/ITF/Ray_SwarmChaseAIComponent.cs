namespace UbiArt.ITF {
	[Games(GameFlags.RFR)]
	public partial class Ray_SwarmChaseAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA6B09162;
	}
}

