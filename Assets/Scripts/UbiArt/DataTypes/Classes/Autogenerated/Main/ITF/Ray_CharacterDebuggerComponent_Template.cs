namespace UbiArt.ITF {
	[Games(GameFlags.ROVersion)]
	public partial class Ray_CharacterDebuggerComponent_Template : CharacterDebuggerComponent_Template {
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
		}
		public override uint? ClassCRC => 0x9DB64FB7;
	}
}

