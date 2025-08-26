namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class AdditionalBehaviorsComponent : ActorComponent {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x0E48D199;
	}
}

