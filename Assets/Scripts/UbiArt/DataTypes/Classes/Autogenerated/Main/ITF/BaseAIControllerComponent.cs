namespace UbiArt.ITF {
	[Games(GameFlags.RO | GameFlags.RL | GameFlags.RAVersion)]
	public partial class BaseAIControllerComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0xD623AB32;
	}
}

