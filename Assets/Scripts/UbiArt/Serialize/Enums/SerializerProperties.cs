using System;

namespace UbiArt {
	[Flags]
	public enum SerializerProperties {
		None = 0,
		Binary = 1,
		Tool = 1 << 1,
		MemoryDump = 1 << 2,
		LIP = 1 << 3, // LoadInPlace
		BinSkip = 1 << 4,
		NoDeprecate = 1 << 5,

		// Not in RL
		Flags6 = 1 << 6,
		NoRaw = 1 << 7,
		Flags8 = 1 << 8,
		Flags9 = 1 << 9,
		Flags10 = 1 << 10,
		Flags11 = 1 << 11,
		Flags12 = 1 << 12,
		Flags13 = 1 << 13,
		Flags14 = 1 << 14,
		Flags15 = 1 << 15,
		Flags16 = 1 << 16,
		Flags17 = 1 << 17,
		Flags18 = 1 << 18,
		Flags19 = 1 << 19,
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
	}
}
