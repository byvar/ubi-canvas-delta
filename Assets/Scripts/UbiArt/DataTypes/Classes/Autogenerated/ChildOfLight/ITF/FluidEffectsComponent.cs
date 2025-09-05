namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class FluidEffectsComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xF612BB7C;
	}
}

