namespace UbiArt.ITF {
	[Games(GameFlags.RLVersion | GameFlags.VH | GameFlags.RA)]
	public partial class EditableShape : CSerializable {
		public Generic<PhysShape> shape;
		public StringID bindedBoneName;
		public Vec2d localOffset;
		public bool applyRotation = true;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				shape = s.SerializeObject<Generic<PhysShape>>(shape, name: "shape");
				if (s.Settings.Game == Game.RL || s.Settings.Game == Game.COL) {
					bindedBoneName = s.SerializeObject<StringID>(bindedBoneName, name: "bindedBoneName");
					localOffset = s.SerializeObject<Vec2d>(localOffset, name: "localOffset");
					applyRotation = s.Serialize<bool>(applyRotation, name: "applyRotation");
				}
			}
		}
	}
}

