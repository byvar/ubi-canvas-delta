namespace UbiArt.ITF {
	[Games(GameFlags.VH)]
	public partial class W1W_Breakable : ActorComponent {
		public bool bool__0;
		public CArrayO<W1W_BreakableSequence> CArray_W1W_BreakableSequence__1;
		public Enum_VH_0 Enum_VH_0__2;
		public int int__3;
		public Generic<Event> Generic_Event__4;
		public Generic<Event> Generic_Event__5;
		public Generic<Event> Generic_Event__6;
		public Generic<Event> Generic_Event__7;
		public Generic<Event> Generic_Event__8;
		public CArrayO<Generic<Event>> CArray_Generic_Event__9;
		public bool bool__10;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			CArray_W1W_BreakableSequence__1 = s.SerializeObject<CArrayO<W1W_BreakableSequence>>(CArray_W1W_BreakableSequence__1, name: "CArray<W1W_BreakableSequence>__1");
			if (s.HasFlags(SerializeFlags.Group_Data)) {
				Enum_VH_0__2 = s.Serialize<Enum_VH_0>(Enum_VH_0__2, name: "Enum_VH_0__2");
				int__3 = s.Serialize<int>(int__3, name: "int__3");
				Generic_Event__4 = s.SerializeObject<Generic<Event>>(Generic_Event__4, name: "Generic<Event>__4");
				Generic_Event__5 = s.SerializeObject<Generic<Event>>(Generic_Event__5, name: "Generic<Event>__5");
				Generic_Event__6 = s.SerializeObject<Generic<Event>>(Generic_Event__6, name: "Generic<Event>__6");
				Generic_Event__7 = s.SerializeObject<Generic<Event>>(Generic_Event__7, name: "Generic<Event>__7");
				Generic_Event__8 = s.SerializeObject<Generic<Event>>(Generic_Event__8, name: "Generic<Event>__8");
				CArray_Generic_Event__9 = s.SerializeObject<CArrayO<Generic<Event>>>(CArray_Generic_Event__9, name: "CArray<Generic<Event>>__9");
			}
			bool__10 = s.Serialize<bool>(bool__10, name: "bool__10");
			if (s.HasFlags(SerializeFlags.Group_Checkpoint)) {
				bool__0 = s.Serialize<bool>(bool__0, name: "bool__0");
			}
		}
		public enum Enum_VH_0 {
			[Serialize("Value_0"  )] Value_0 = 0,
			[Serialize("Value_1"  )] Value_1 = 1,
			[Serialize("Value_2"  )] Value_2 = 2,
			[Serialize("Value_4"  )] Value_4 = 4,
			[Serialize("Value_8"  )] Value_8 = 8,
			[Serialize("Value_12" )] Value_12 = 12,
			[Serialize("Value_14" )] Value_14 = 14,
			[Serialize("Value_16" )] Value_16 = 16,
			[Serialize("Value_32" )] Value_32 = 32,
			[Serialize("Value_33" )] Value_33 = 33,
			[Serialize("Value_64" )] Value_64 = 64,
			[Serialize("Value_128")] Value_128 = 128,
			[Serialize("Value__1" )] Value__1 = -1,
		}
		public override uint? ClassCRC => 0x6BEA7750;
	}
}

