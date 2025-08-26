namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class ParticlePhysComponent : PhysComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA5396A8F;
	}
}

