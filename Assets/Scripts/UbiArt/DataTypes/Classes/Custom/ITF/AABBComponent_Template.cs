namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class AABBComponent_Template : ActorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}

		public override uint? ClassCRC => 0xF56AFAF8;
	}
}
