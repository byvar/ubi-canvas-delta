namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_StargateComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD2215529;
	}
}

