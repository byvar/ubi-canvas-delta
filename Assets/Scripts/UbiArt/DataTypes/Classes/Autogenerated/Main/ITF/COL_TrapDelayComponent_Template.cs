namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TrapDelayComponent_Template : ActorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEC3FA8D7;
	}
}

