using System;
using System.Runtime.InteropServices;

namespace CooS.Drivers.DisplayAdapters.VBE {
	using db = Byte;
	using dw = UInt16;
	using dd = UInt32;

	[StructLayoutAttribute(LayoutKind.Sequential, Pack=1)]
	public struct PMInfoBlock {
		public dd Signature;			// db 'PMID' ; PM Info Block Signature
		public dw EntryPoint;			// dw ? ; Offset of PM entry point within BIOS
		public dw PMInitialize;			// dw ? ; Offset of PM initialization entry point
		public dw BIOSDataSel;			// dw 0 ; Selector to BIOS data area emulation block
		public dw A0000Sel;				// dw A000h ; Selector to access A0000h physical mem
		public dw B0000Sel;				// dw B000h ; Selector to access B0000h physical mem
		public dw B8000Sel;				// dw B800h ; Selector to access B8000h physical mem
		public dw CodeSegSel;			// dw C000h ; Selector to access code segment as data
		public db InProtectMode;		// db 0 ; Set to 1 when in protected mode
		public db Checksum;				// db ? ; Checksum byte for structure
	}

}
