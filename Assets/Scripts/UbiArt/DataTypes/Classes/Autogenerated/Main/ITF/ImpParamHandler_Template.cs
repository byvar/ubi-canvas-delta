namespace UbiArt.ITF {
	[Games(GameFlags.VH | GameFlags.RA)]
	public partial class ImpParamHandler_Template : CSerializable {
		public CRefList<ImpParamHandler_Template.ImpParamData> list;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			list = s.SerializeObject<CRefList<ImpParamHandler_Template.ImpParamData>>(list, name: "list");
		}
		[Games(GameFlags.VH | GameFlags.RA)]
		public partial class ImpParamData : CSerializable {
			public StringID name;
			public string value;
			public uint type;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				name = s.SerializeObject<StringID>(name, name: "name");
				value = s.Serialize<string>(value, name: "value");
				type = s.Serialize<uint>(type, name: "type");
			}
		}
	}
}

