using System.Threading;

namespace UbiArt.ITF {
	[Games(GameFlags.COL)]
	public partial class COL_MonitorObjectInteraction : COL_ObjectInteraction {
		public CListO<COL_MonitorObjectInteraction.Monitor> monitors;
		public Enum_operatorType operatorType;
		public uint count;
		public bool saveChildState;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			monitors = s.SerializeObject<CListO<COL_MonitorObjectInteraction.Monitor>>(monitors, name: "monitors");
			operatorType = s.Serialize<Enum_operatorType>(operatorType, name: "operatorType");
			count = s.Serialize<uint>(count, name: "count");
			saveChildState = s.Serialize<bool>(saveChildState, name: "saveChildState", options: CSerializerObject.Options.BoolAsByte);
		}
		public enum Enum_operatorType {
			[Serialize("Value_0")] Value_0 = 0,
			[Serialize("Value_1")] Value_1 = 1,
			[Serialize("Value_2")] Value_2 = 2,
		}
		public override uint? ClassCRC => 0xD538B259;

		[Games(GameFlags.COL)]
		public class Monitor : CSerializable {
			public StringID state;
			public StringID tag;
			public Enum_operatorType operatorType;
			public uint count;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				state = s.SerializeObject<StringID>(state, name: "state");
				tag = s.SerializeObject<StringID>(tag, name: "tag");
				operatorType = s.Serialize<Enum_operatorType>(operatorType, name: "operatorType");
				count = s.Serialize<uint>(count, name: "count");
			}
		}
	}
}

