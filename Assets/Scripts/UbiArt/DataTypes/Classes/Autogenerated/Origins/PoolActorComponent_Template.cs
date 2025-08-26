namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class PoolActorComponent_Template : ActorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xE9D43C95;
	}
}

