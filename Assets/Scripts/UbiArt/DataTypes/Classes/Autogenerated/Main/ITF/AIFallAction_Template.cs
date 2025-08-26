namespace UbiArt.ITF {
	[Games(GameFlags.All)]
	public partial class AIFallAction_Template : AIAction_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9C2A29B5;
	}
}

