namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_GameObjectComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x211608D3;
	}
}

