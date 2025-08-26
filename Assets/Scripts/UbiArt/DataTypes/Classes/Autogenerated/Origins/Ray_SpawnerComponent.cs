namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_SpawnerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x5A0579F6;
	}
}

