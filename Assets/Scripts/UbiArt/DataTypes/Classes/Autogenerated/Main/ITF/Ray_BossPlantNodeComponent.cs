namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_BossPlantNodeComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xFF2DEFA4;
	}
}

