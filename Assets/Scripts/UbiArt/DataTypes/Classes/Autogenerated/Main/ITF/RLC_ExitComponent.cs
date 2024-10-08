namespace UbiArt.ITF {
	[Games(GameFlags.RA | GameFlags.RM)]
	public partial class RLC_ExitComponent : ActorComponent {
		public Vec3d captainExitRitualOffset = new Vec3d(3,0,0);
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			captainExitRitualOffset = s.SerializeObject<Vec3d>(captainExitRitualOffset, name: "captainExitRitualOffset");
		}
		public override uint? ClassCRC => 0xD337C6AA;
	}
}

