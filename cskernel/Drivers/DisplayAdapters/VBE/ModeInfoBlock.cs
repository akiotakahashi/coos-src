using System;
using System.Runtime.InteropServices;

namespace CooS.Drivers.DisplayAdapters.VBE {
	using db = Byte;
	using dw = UInt16;
	using dd = UInt32;

	[StructLayoutAttribute(LayoutKind.Sequential, Pack=1)]
	public struct ModeInfoBlock {
		// Mandatory information for all VBE revisions
		public dw	ModeAttributes;				// dw ? ; mode attributes
		public db	WinAAttributes;				// db ? ; window A attributes
		public db	WinBAttributes;				// db ? ; window B attributes
		public dw	WinGranularity;				// dw ? ; window granularity
		public dw	WinSize;					// dw ? ; window size
		public dw	WinASegment;				// dw ? ; window A start segment
		public dw	WinBSegment;				// dw ? ; window B start segment
		public IntPtr WinFuncPtr;				// dd ? ; real mode pointer to window function
		public dw	BytesPerScanLine;			// dw ? ; bytes per scan line
		// Mandatory information for VBE 1.2 and above
		public dw	XResolution;				// dw ? ; horizontal resolution in pixels or characters3
		public dw	YResolution;				// dw ? ; vertical resolution in pixels or characters
		public db	XCharSize;					// db ? ; character cell width in pixels
		public db	YCharSize;					// db ? ; character cell height in pixels
		public db	NumberOfPlanes;				// db ? ; number of memory planes
		public db	BitsPerPixel;				// db ? ; bits per pixel
		public db	NumberOfBanks;				// db ? ; number of banks
		public db	MemoryModel;				// db ? ; memory model type
		public db	BankSize;					// db ? ; bank size in KB
		public db	NumberOfImagePages;			// db ? ; number of images
		public db	Reserved0;					// db 1 ; reserved for page function
		// Direct Color fields (required for direct/6 and YUV/7 memory models)
		public db	RedMaskSize;				// db ? ; size of direct color red mask in bits
		public db	RedFieldPosition;			// db ? ; bit position of lsb of red mask
		public db	GreenMaskSize;				// db ? ; size of direct color green mask in bits
		public db	GreenFieldPosition;			// db ? ; bit position of lsb of green mask
		public db	BlueMaskSize;				// db ? ; size of direct color blue mask in bits
		public db	BlueFieldPosition;			// db ? ; bit position of lsb of blue mask
		public db	RsvdMaskSize;				// db ? ; size of direct color reserved mask in bits
		public db	RsvdFieldPosition;			// db ? ; bit position of lsb of reserved mask
		public db	DirectColorModeInfo;		// db ? ; direct color mode attributes
		// Mandatory information for VBE 2.0 and above
		public IntPtr PhysBasePtr;				// dd ? ; physical address for flat memory frame buffer
		public dd	Reserved1;					// dd 0 ; Reserved - always set to 0
		public dw	Reserved2;					// dw 0 ; Reserved - always set to 0
		// Mandatory information for VBE 3.0 and above
		/*
		public dw	LinBytesPerScanLine;		// dw ? ; bytes per scan line for linear modes
		public db	BnkNumberOfImagePages;		// db ? ; number of images for banked modes
		public db	LinNumberOfImagePages;		// db ? ; number of images for linear modes
		public db	LinRedMaskSize;				// db ? ; size of direct color red mask (linear modes)
		public db	LinRedFieldPosition;		// db ? ; bit position of lsb of red mask (linear modes)
		public db	LinGreenMaskSize;			// db ? ; size of direct color green mask (linear modes)
		public db	LinGreenFieldPositiondb;	// db ? ; bit position of lsb of green mask (linear modes)
		public db	LinBlueMaskSize;			// db ? ; size of direct color blue mask (linear modes)
		public db	LinBlueFieldPosition;		// db ? ; bit position of lsb of blue mask (linear modes)
		public db	LinRsvdMaskSize;			// db ? ; size of direct color reserved mask (linear modes)
		public db	LinRsvdFieldPosition;		// db ? ; bit position of lsb of reserved mask (linear modes)
		public dd	MaxPixelClock;				// dd ? ; maximum pixel clock (in Hz) for graphics mode
		public db	Reserved3[189];				// db 189 dup (?) ; remainder of ModeInfoBlock
		*/

		public static unsafe ModeInfoBlock Current {
			get {
				return *(ModeInfoBlock*)0x800;
			}
		}

	}

}
