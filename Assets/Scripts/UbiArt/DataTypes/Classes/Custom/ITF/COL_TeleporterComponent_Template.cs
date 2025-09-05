namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_TeleporterComponent_Template : ActorComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x3F9240A8;
	}
}

