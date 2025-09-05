namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_PushPullObjectInteraction : COL_ObjectInteraction {
		public Enum_movableType movableType;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			movableType = s.Serialize<Enum_movableType>(movableType, name: "movableType");
		}
		public enum Enum_movableType {
			[Serialize("Value_0")] Value_0 = 0,
			[Serialize("Value_1")] Value_1 = 1,
		}
		public override uint? ClassCRC => 0x23860972;
	}
}

