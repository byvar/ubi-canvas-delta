namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class TrajectoryNodeComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x75D611DC;
	}
}

