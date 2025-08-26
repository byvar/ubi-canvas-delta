namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_PushedAIComponent : Ray_AIComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x2C914E41;
	}
}

