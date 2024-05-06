namespace UbiArt.ITF {
	public partial class AIAction : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}

		public override uint? ClassCRC => 0x71353EC7;
	}
}

