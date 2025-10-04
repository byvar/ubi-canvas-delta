namespace UbiArt.ITF {
	[Games(GameFlags.RL)]
	public partial class RO2_HomeDoorComponent_Template : RO2_HomeComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x81BB9C51;
	}
}

