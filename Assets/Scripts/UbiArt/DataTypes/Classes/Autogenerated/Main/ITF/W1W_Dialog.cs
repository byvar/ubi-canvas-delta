namespace UbiArt.ITF {
	[Games(GameFlags.VH)]
	public partial class W1W_Dialog : W1W_InteractiveGenComponent {
		public bool bool__0;
		public bool bool__1;
		public StringID StringID__2;
		public StringID StringID__3_;
		public StringID StringID__4_;
		public StringID StringID__5_;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				bool__0 = s.Serialize<bool>(bool__0, name: "bool__0");
				bool__1 = s.Serialize<bool>(bool__1, name: "bool__1");
			}
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				StringID__2 = s.SerializeObject<StringID>(StringID__2, name: "StringID__2");
				StringID__3_ = s.SerializeObject<StringID>(StringID__3_, name: "StringID__3");
				StringID__4_ = s.SerializeObject<StringID>(StringID__4_, name: "StringID__4");
				StringID__5_ = s.SerializeObject<StringID>(StringID__5_, name: "StringID__5");
			}
		}
		public override uint? ClassCRC => 0x562EEC6F;
	}
}

