namespace UbiArt.ITF {
	[Games(GameFlags.RO)]
	public partial class Ray_AINetworkComponent_Template : ActorComponent_Template {
		public Generic<PhysShape> shape;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
		}
		public override uint? ClassCRC => 0x6D04E396;
	}
}

