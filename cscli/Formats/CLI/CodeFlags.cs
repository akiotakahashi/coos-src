using System;

namespace CooS.Formats.CLI {

	[Flags]
	public enum CodeFlags {
		TypeMask	= 0x03,
		Tiny		= 0x02,
		Fat			= 0x03,
		MoreSects	= 0x08,		// More sections follow after this header (see Section 24.4.5).
		InitLocals	= 0x10,		// Call default constructor on all local variables
	}

}
