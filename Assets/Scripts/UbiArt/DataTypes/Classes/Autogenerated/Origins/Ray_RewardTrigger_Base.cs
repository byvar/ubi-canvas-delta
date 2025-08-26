namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_RewardTrigger_Base : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xBCF8BFEA;
	}
}

