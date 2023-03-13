using System;

namespace CooS.Formats.CLI.IL {

	[Flags]
	public enum InstructionPrefixes {
		None		= 0,
		Volatile	= 1,
		Tailcall	= 2,
		Unaligned	= 4,
		Constrained	= 8,
	}

}
