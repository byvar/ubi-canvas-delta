namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class ParticleGeneratorComponent : GraphicComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x60A3B68B;
	}
}

