namespace UbiArt.ITF {
	[Games(GameFlags.VH)]
	public partial class W1W_HookComponent : ActorComponent {
		public CArrayO<W1W_HookComponent.CanBeHookedItem> CArray_W1W_HookComponent_CanBeHookedItem__0;
		public Vec2d Vector2__1;
		public StringID StringID__2;
		public StringID StringID__3;
		protected override void SerializeImpl(CSerializerObject s) {
			base.SerializeImpl(s);
			if (s.HasFlags(SerializeFlags.Group_DataEditable)) {
				CArray_W1W_HookComponent_CanBeHookedItem__0 = s.SerializeObject<CArrayO<W1W_HookComponent.CanBeHookedItem>>(CArray_W1W_HookComponent_CanBeHookedItem__0, name: "CArray<W1W_HookComponent.CanBeHookedItem>__0");
				Vector2__1 = s.SerializeObject<Vec2d>(Vector2__1, name: "Vector2__1");
				StringID__2 = s.SerializeObject<StringID>(StringID__2, name: "StringID__2");
				StringID__3 = s.SerializeObject<StringID>(StringID__3, name: "StringID__3");
			}
		}
		[Games(GameFlags.VH)]
		public partial class CanBeHookedItem : CSerializable {
			public Enum_VH_0 Enum_VH_0__0;
			protected override void SerializeImpl(CSerializerObject s) {
				base.SerializeImpl(s);
				Enum_VH_0__0 = s.Serialize<Enum_VH_0>(Enum_VH_0__0, name: "Enum_VH_0__0");
			}
			public enum Enum_VH_0 {
				[Serialize("Value_0" )] Value_0 = 0,
				[Serialize("Value_1" )] Value_1 = 1,
				[Serialize("Value_2" )] Value_2 = 2,
				[Serialize("Value_3" )] Value_3 = 3,
				[Serialize("Value_4" )] Value_4 = 4,
				[Serialize("Value_5" )] Value_5 = 5,
				[Serialize("Value_6" )] Value_6 = 6,
				[Serialize("Value_7" )] Value_7 = 7,
				[Serialize("Value_8" )] Value_8 = 8,
				[Serialize("Value_9" )] Value_9 = 9,
				[Serialize("Value_10")] Value_10 = 10,
				[Serialize("Value_11")] Value_11 = 11,
				[Serialize("Value_12")] Value_12 = 12,
				[Serialize("Value_13")] Value_13 = 13,
				[Serialize("Value_14")] Value_14 = 14,
				[Serialize("Value_15")] Value_15 = 15,
				[Serialize("Value_16")] Value_16 = 16,
				[Serialize("Value_17")] Value_17 = 17,
				[Serialize("Value_18")] Value_18 = 18,
				[Serialize("Value_19")] Value_19 = 19,
				[Serialize("Value_20")] Value_20 = 20,
				[Serialize("Value_21")] Value_21 = 21,
				[Serialize("Value_22")] Value_22 = 22,
				[Serialize("Value_23")] Value_23 = 23,
				[Serialize("Value_24")] Value_24 = 24,
				[Serialize("Value_25")] Value_25 = 25,
				[Serialize("Value_26")] Value_26 = 26,
				[Serialize("Value_27")] Value_27 = 27,
				[Serialize("Value_28")] Value_28 = 28,
				[Serialize("Value_29")] Value_29 = 29,
				[Serialize("Value_30")] Value_30 = 30,
				[Serialize("Value_31")] Value_31 = 31,
				[Serialize("Value_32")] Value_32 = 32,
				[Serialize("Value_33")] Value_33 = 33,
				[Serialize("Value_34")] Value_34 = 34,
				[Serialize("Value_35")] Value_35 = 35,
				[Serialize("Value_36")] Value_36 = 36,
				[Serialize("Value_37")] Value_37 = 37,
				[Serialize("Value_38")] Value_38 = 38,
				[Serialize("Value_39")] Value_39 = 39,
				[Serialize("Value_40")] Value_40 = 40,
				[Serialize("Value_41")] Value_41 = 41,
				[Serialize("Value_42")] Value_42 = 42,
				[Serialize("Value_43")] Value_43 = 43,
				[Serialize("Value_44")] Value_44 = 44,
				[Serialize("Value_45")] Value_45 = 45,
				[Serialize("Value_46")] Value_46 = 46,
				[Serialize("Value_47")] Value_47 = 47,
				[Serialize("Value_48")] Value_48 = 48,
				[Serialize("Value_49")] Value_49 = 49,
				[Serialize("Value_50")] Value_50 = 50,
				[Serialize("Value_51")] Value_51 = 51,
				[Serialize("Value_52")] Value_52 = 52,
				[Serialize("Value_53")] Value_53 = 53,
				[Serialize("Value_55")] Value_55 = 55,
				[Serialize("Value_56")] Value_56 = 56,
				[Serialize("Value_54")] Value_54 = 54,
				[Serialize("Value_58")] Value_58 = 58,
				[Serialize("Value_59")] Value_59 = 59,
				[Serialize("Value_60")] Value_60 = 60,
				[Serialize("Value__1")] Value__1 = -1,
			}
		}
		public override uint? ClassCRC => 0x12794C11;
	}
}

