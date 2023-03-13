#pragma once

#include "stdafx.h"
#include "assembly.h"


namespace Signature {

	class Signature : Reflection::AssemblyItem {
		enum FirstByteFlags {
			HASTHIS			= 0x20,		// used to encode the keyword instance in the calling convention, see Section 14.3
			EXPLICITTHIS	= 0x40,		// used to encode the keyword explicit in the calling convention, see Section 14.3
			DEFAULT			= 0x00,		// used to encode the keyword default in the calling convention, see Section 14.3
			VARARG			= 0x05,		// used to encode the keyword vararg in the calling convention, see Section 14.3
		};
	private:
		Signature(Reflection::Assembly& parent, uint tableIndex);
	};

}
