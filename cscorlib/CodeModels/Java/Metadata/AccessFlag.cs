using System;

namespace CooS.CodeModels.Java.Metadata {

	public enum AccessFlag {
		Public			= 0x0001,	// Declared public; may be accessed from outside its package.
		Private			= 0x0002,	// Declared private; usable only within the defining class.
		Protected		= 0x0004,	// Declared protected; may be accessed within subclasses.
		Static			= 0x0008,	// Declared static.
		Final			= 0x0010,	// Declared final; no subclasses allowed. Or no further assignment after initialization.
		Super			= 0x0020,	// Treat superclass methods specially when invoked by the invokespecial instruction.
		Synchronized	= 0x0020,	// Declared synchronized; invocation is wrapped in a monitor lock.
		Volatile		= 0x0040,	// Declared volatile; cannot be cached.
		Transient		= 0x0080,	// Declared transient; not written or read by a persistent object manager.
		Native			= 0x0100,	// Declared native; implemented in a language other than Java.
		Interface		= 0x0200,	// Is an interface, not a class.
		Abstract		= 0x0400,	// Declared abstract; may not be instantiated.
		Strict			= 0x0800,	// Declared strictfp; floating-point mode is FP-strict.
	}

}
