namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_HomeFriendComponent : RO2_HomeComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xEA90263B;
	}
}

