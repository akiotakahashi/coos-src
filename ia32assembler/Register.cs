using System;

namespace IA32Toolkit.Assembler {

	public enum Register8 {
		AL,
		CL,
		DL,
		BL,
		AH,
		CH,
		DH,
		BH,
	}

	public enum Register16 {
		AX,
		CX,
		DX,
		BX,
		SP,
		BP,
		SI,
		DI,
	}
	
	public enum Register32 {
		EAX,
		ECX,
		EDX,
		EBX,
		ESP,
		EBP,
		ESI,
		EDI,
		Invalid = -1,
	}

}
