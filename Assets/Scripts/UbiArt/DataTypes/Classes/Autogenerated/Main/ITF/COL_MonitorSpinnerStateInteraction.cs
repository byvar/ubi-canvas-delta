namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MonitorSpinnerStateInteraction : COL_ObjectInteraction {
		public uint state;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			state = s.Serialize<uint>(state, name: "state");
		}
		public override uint? ClassCRC => 0xCD12E058;
	}
}

