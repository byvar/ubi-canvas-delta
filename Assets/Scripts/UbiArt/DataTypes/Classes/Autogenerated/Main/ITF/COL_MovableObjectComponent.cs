namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MovableObjectComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB6F2F926;
	}
}

