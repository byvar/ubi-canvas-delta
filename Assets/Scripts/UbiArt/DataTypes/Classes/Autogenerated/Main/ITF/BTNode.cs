namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class BTNode : CSerializable {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x63F3B293;
	}
}

