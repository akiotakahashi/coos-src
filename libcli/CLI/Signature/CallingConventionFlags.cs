using System;

namespace CooS.Formats.CLI.Signature {

	public enum CallingConventionFlags : byte {
		HASTHIS			= 0x20,		// used to encode the keyword instance in the calling convention, see Section 14.3
		EXPLICITTHIS	= 0x40,		// used to encode the keyword explicit in the calling convention, see Section 14.3
		DEFAULT			= 0x00,		// used to encode the keyword default in the calling convention, see Section 14.3
		VARARG			= 0x05,		// used to encode the keyword vararg in the calling convention, see Section 14.3
		GENERIC			= 0x10,
		SENTINEL		= 0x41,
		C_LANG			= 0x01,
		STDCALL			= 0x02,
		THISCALL		= 0x03,
		FASTCALL		= 0x04,
	}

}
