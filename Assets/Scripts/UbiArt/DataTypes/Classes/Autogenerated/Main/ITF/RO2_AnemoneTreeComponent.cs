namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.VH | GameFlags.RA)]
	public partial class RO2_AnemoneTreeComponent : ActorComponent {
		public bool startOpen;
		public float branchSpeed;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				startOpen = s.Serialize<bool>(startOpen, name: "startOpen");
				branchSpeed = s.Serialize<float>(branchSpeed, name: "branchSpeed");
			}
		}
		public override uint? ClassCRC => 0x084CC871;
	}
}

