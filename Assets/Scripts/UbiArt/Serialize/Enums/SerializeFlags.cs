using System;

namespace UbiArt {
	[Flags]
	public enum SerializeFlags {
		None = 0,
		PropertyEdit_Load = 1 << 0,
		PropertyEdit_Save = 1 << 1,
		Checkpoint_Load = 1 << 2,
		Checkpoint_Save = 1 << 3,
		Editor_Load = 1 << 4,
		Editor_Save = 1 << 5,
		Data_Load = 1 << 6,
		Data_Save = 1 << 7,
		Deprecate = 1 << 8,
		DataRaw = 1 << 9,
		DataBin = 1 << 10,
		InstanceLoad = 1 << 11,
		InstanceSave = 1 << 12,
		ForcedValues_Load = 1 << 13,
		ForcedValues_Save = 1 << 14,
		CSharp = 1 << 15,

		// Not in RL
		Flags16 = 1 << 16, // Load
		Flags17 = 1 << 17, // Save
		Flags18 = 1 << 18, // Load
		Flags19 = 1 << 19, // Save

		Flags20 = 1 << 20,
		Flags21 = 1 << 21,
		Flags22 = 1 << 22,
		Flags23 = 1 << 23,
		Flags24 = 1 << 24,
		Flags25 = 1 << 25,
		Flags26 = 1 << 26,
		Flags27 = 1 << 27,
		Flags28 = 1 << 28,
		Flags29 = 1 << 29,
		Flags30 = 1 << 30,
		Flags31 = 1 << 31,

		// Groups
		PropertyEdit = PropertyEdit_Load | PropertyEdit_Save,
		Checkpoint = Checkpoint_Load | Checkpoint_Save,
		Editor = Editor_Load | Editor_Save,
		Data = Data_Load | Data_Save,
		Instance = InstanceLoad | InstanceSave,
		ForcedValues = ForcedValues_Load | ForcedValues_Save,
		Group_Checkpoint = Checkpoint,
		Group_Data = Data,
		Group_PropertyEdit = PropertyEdit,
		Group_DataEditable = Group_PropertyEdit | Group_Data,
	}
}
