namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class ActorSpawnComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		[Games(GameFlags.RJR | GameFlags.RFR)]
		public partial class SpawnData : CSerializable {
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
			}
		}
		public override uint? ClassCRC => 0xAE141184;
	}
}

