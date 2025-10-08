using System;
namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AIMedalBehavior : AIBehavior {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x402E8012;
	}
}

