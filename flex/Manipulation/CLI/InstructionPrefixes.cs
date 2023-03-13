using System;

namespace CooS.Manipulation.CLI {

	[Flags]
	public enum InstructionPrefixes {
		None		= 0,
		Volatile	= 1,
		Tailcall	= 2,
		Unaligned	= 4,
	}

}
