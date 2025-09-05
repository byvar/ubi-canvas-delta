namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_ExplorationHUDManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xB98C0C2B;
	}
}

