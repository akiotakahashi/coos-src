using System;
using System.Runtime.InteropServices;

namespace CooS.Drivers.DisplayAdapters.VBE {
	using db = Byte;
	using dw = UInt16;
	using dd = UInt32;

	[StructLayoutAttribute(LayoutKind.Sequential, Pack=1)]
	public unsafe struct VbeInfoBlock {

		public dd	VbeSignature;			// db 'VESA' ; VBE Signature
		public dw	VbeVersion;				// dw 0200h or 0300h ; VBE Version
		public db*	OemStringPtr;			// dd ? ; VbeFarPtr to OEM String
		public dd	Capabilities;			// db 4 dup (?) ; Capabilities of graphics controller
		public dw*	VideoModePtr;			// dd ? ; VbeFarPtr to VideoModeList
		public dw	TotalMemory;			// dw ? ; Number of 64kb memory blocks
		// Added for VBE 2.0+
		public dw	OemSoftwareRev;			// dw ? ; VBE implementation Software revision
		public db*	OemVendorNamePtr;		// dd ? ; VbeFarPtr to Vendor Name String
		public db*	OemProductNamePtr;		// dd ? ; VbeFarPtr to Product Name String
		public db*	OemProductRevPtr;		// dd ? ; VbeFarPtr to Product Revision String
		//public db	Reserved[222];			// db 222 dup (?) ; Reserved for VBE implementation scratch area
		//public db	OemData[256];			// db 256 dup (?) ; Data Area for OEM Strings

		public static VbeInfoBlock Current {
			get {
				return *(VbeInfoBlock*)0x800;
			}
		}

	}

}
