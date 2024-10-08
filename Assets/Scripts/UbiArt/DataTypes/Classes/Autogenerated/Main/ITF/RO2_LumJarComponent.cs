namespace UbiArt.ITF {
	[Games(GameFlags.RL | GameFlags.RA | GameFlags.RM)]
	public partial class RO2_LumJarComponent : ActorComponent {
		public EditableShape shape;
		public bool hookUp;
		public bool hookDn;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.Settings.Game == Game.RL) {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					shape = s.SerializeObject<EditableShape>(shape, name: "shape");
				}
			} else {
				if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
					shape = s.SerializeObject<EditableShape>(shape, name: "shape");
					hookUp = s.Serialize<bool>(hookUp, name: "hookUp");
					hookDn = s.Serialize<bool>(hookDn, name: "hookDn");
				}
			}
		}
		public override uint? ClassCRC => 0x3F4B0FFD;
	}
}

