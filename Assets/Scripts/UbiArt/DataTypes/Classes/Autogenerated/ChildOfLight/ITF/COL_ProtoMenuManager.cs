namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_ProtoMenuManager : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x1A337B4A;
	}
}

