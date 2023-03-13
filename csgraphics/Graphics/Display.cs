using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using CooS.Drivers.DisplayAdapters.VBE;

namespace CooS.Graphics {

	public class Display : Canvas {

		ModeInfoBlock mib;
		int scanline_bytes;
		int pixel_bytes;
		int r_mask;
		int g_mask;
		int b_mask;
		int r_shift;	// Shift quantity from 8 bit color component to DI-bits
		int g_shift;
		int b_shift;
		PixelFormat pf;

		public Display() {
			if(Engine.Privileged) {
				this.mib = ModeInfoBlock.Current;
#if true
				Console.WriteLine("X-Resolution   = {0}", mib.XResolution);
				Console.WriteLine("Y-Resolution   = {0}", mib.YResolution);
				Console.WriteLine("Bits/Pixel     = {0}", mib.BitsPerPixel);
				Console.WriteLine("Bytes/ScanLine = {0}", mib.BytesPerScanLine);
				Console.WriteLine("R:pos          = {0}", mib.RedFieldPosition);
				Console.WriteLine("R:size         = {0}", mib.RedMaskSize);
				Console.WriteLine("G:pos          = {0}", mib.GreenFieldPosition);
				Console.WriteLine("G:size         = {0}", mib.GreenMaskSize);
				Console.WriteLine("B:pos          = {0}", mib.BlueFieldPosition);
				Console.WriteLine("B:mask         = {0}", mib.BlueMaskSize);
#endif
			} else {
				Console.WriteLine("*** NO VESA VRAM INFORMATION ***");
				this.mib.PhysBasePtr = IntPtr.Zero;
				this.mib.RedFieldPosition = 16;
				this.mib.BlueFieldPosition = 0;
				this.mib.GreenFieldPosition = 8;
				this.mib.RedMaskSize = 8;
				this.mib.BlueMaskSize = 8;
				this.mib.GreenMaskSize = 8;
				this.mib.BitsPerPixel = 32;
				this.mib.XResolution = 800;
				this.mib.YResolution = 600;
				this.mib.BytesPerScanLine = 4*800;
			}
			if(this.mib.PhysBasePtr==IntPtr.Zero) {
				Console.WriteLine("*** NO GRAPGHIC BUFFER FRAME ***");
				byte[] buf = new byte[this.mib.BytesPerScanLine*this.mib.YResolution];
				this.mib.PhysBasePtr = GCHandle.Alloc(buf,GCHandleType.Pinned).AddrOfPinnedObject();
			}
			this.scanline_bytes = mib.BytesPerScanLine;
			this.pixel_bytes = (mib.BitsPerPixel+7)/8;
			this.r_mask = (1<<mib.RedMaskSize)-1;
			this.b_mask = (1<<mib.BlueMaskSize)-1;
			this.g_mask = (1<<mib.GreenMaskSize)-1;
			this.r_mask <<= mib.RedFieldPosition;
			this.b_mask <<= mib.BlueFieldPosition;
			this.g_mask <<= mib.GreenFieldPosition;
			this.r_shift = mib.RedFieldPosition+8-mib.RedMaskSize;
			this.b_shift = mib.BlueFieldPosition+8-mib.BlueMaskSize;
			this.g_shift = mib.GreenFieldPosition+8-mib.GreenMaskSize;
			switch(this.pixel_bytes) {
			case 4:
				if(this.r_mask==0xFF0000 && this.g_mask==0xFF00 && this.b_mask==0xFF) {
					this.pf = PixelFormat.Format32bppRgb;
				} else {
					goto default;
				}
				break;
			case 3:
				if(this.r_mask==0xFF0000 && this.g_mask==0xFF00 && this.b_mask==0xFF) {
					this.pf = PixelFormat.Format24bppRgb;
				} else {
					goto default;
				}
				break;
			case 2:
				if(this.r_mask==0x7C00 && this.g_mask==0x3E0 && this.b_mask==0x1F) {
					this.pf = PixelFormat.Format16bppRgb555;
				} else if(this.r_mask==0xF80000 && this.g_mask==0x7E00 && this.b_mask==0x1F) {
					this.pf = PixelFormat.Format16bppRgb565;
				} else {
					goto default;
				}
				break;
			default:
				this.pf = PixelFormat.Undefined;
				break;
			}
		}

		public override PixelFormat PixelFormat {
			get {
				return this.pf;
			}
		}

		public override int PixelSize {
			get {
				return this.pixel_bytes;
			}
		}

		public override int ScanLineSize {
			get {
				return mib.BytesPerScanLine;
			}
		}

		public override Size Size {
			get {
				return new Size(this.mib.XResolution, this.mib.YResolution);
			}
		}

		public override unsafe FixedFrame GetFixedFrame() {
			return new FixedMemoryFrame(this,mib.PhysBasePtr);
		}

		public override unsafe Color this[int x, int y] {
			get {
				byte* p = (byte*)mib.PhysBasePtr.ToPointer();
				p += this.scanline_bytes*y;
				p += this.pixel_bytes*x;
				int v = *(int*)p;
				int r = (this.r_mask & v) >> mib.RedFieldPosition;
				int b = (this.b_mask & v) >> mib.BlueFieldPosition;
				int g = (this.g_mask & v) >> mib.GreenFieldPosition;
				return Color.FromArgb(r, g, b);
			}
			set {
				int v = 0;
				v |= this.r_mask & (value.R << this.r_shift);
				v |= this.g_mask & (value.G << this.g_shift);
				v |= this.b_mask & (value.B << this.b_shift);
				byte* p = (byte*)mib.PhysBasePtr.ToPointer();
				p += this.scanline_bytes*y;
				p += this.pixel_bytes*x;
				if(this.pixel_bytes==4) {
					*(int*)p = v;
				} else {
					*(uint*)p &= 0xFF000000;
					*(int*)p |= v;
				}
			}
		}

	}

}
