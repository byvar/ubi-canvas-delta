namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_LightPuzzlePieceAccumulatorComponent_Template : COL_LightPuzzlePieceComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xA8A58E0D;
	}
}

