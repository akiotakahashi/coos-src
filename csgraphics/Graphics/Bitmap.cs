using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CooS.Graphics {

	public class Bitmap : Canvas {
	
		readonly PixelFormat pixelformat;
		readonly int width;
		readonly int height;
		readonly int pixelsize;
		readonly int scanlinesize;
		readonly byte[] buffer;

		public Bitmap(PixelFormat pixelformat, int width, int height) {
			if(width<=0 || height<=0) throw new ArgumentOutOfRangeException();
			this.pixelformat = pixelformat;
			this.width = width;
			this.height = height;
			switch(pixelformat) {
			case PixelFormat.Format24bppRgb:
				this.pixelsize = 3;
				break;
			case PixelFormat.Format32bppRgb:
				this.pixelsize = 4;
				break;
			case PixelFormat.Format32bppArgb:
				this.pixelsize = 4;
				break;
			case PixelFormat.Format16bppGrayScale:
				this.pixelsize = 2;
				break;
			default:
				throw new NotSupportedException();
			}
			this.scanlinesize = (this.pixelsize*width+3)&~3;
			this.buffer = new byte[scanlinesize*height];
		}

		public Bitmap(PixelFormat pixelformat, Size size) : this(pixelformat,size.Width,size.Height) {
		}

		public override PixelFormat PixelFormat {
			get {
				return this.pixelformat;
			}
		}

		public override int PixelSize {
			get {
				return this.pixelsize;
			}
		}

		public override int ScanLineSize {
			get {
				return this.scanlinesize;
			}
		}

		public override Size Size {
			get {
				return new Size(this.width, this.height);
			}
		}

		public override FixedFrame GetFixedFrame() {
			return new FixedBufferFrame(this,this.buffer);
		}

		public override unsafe Color this[int x, int y] {
			get {
				fixed(byte* p = &this.buffer[this.pixelsize*x+this.scanlinesize*y]) {
					switch(this.pixelformat) {
					case PixelFormat.Format24bppRgb:
						return Color.FromArgb(p[2], p[1], p[0]);
					case PixelFormat.Format32bppRgb:
						return Color.FromArgb(p[2], p[1], p[0]);
					case PixelFormat.Format32bppArgb:
						return Color.FromArgb(p[3], p[2], p[1], p[0]);
					case PixelFormat.Format16bppGrayScale:
						int lv = *(ushort*)p >> 8;
						return Color.FromArgb(lv,lv,lv);
					default:
						throw new NotImplementedException();
					}
				}
			}
			set {
				if(x<0 || width<=x || y<0 || height<=y) throw new ArgumentException("(x, y) = ("+x+", "+y+") | ([0,"+width+"), [0,"+height+"))");
				if(value.A<255) throw new NotImplementedException("Alpha-blend operation not implemented");
				fixed(byte* p = &this.buffer[this.pixelsize*x+this.scanlinesize*y]) {
					switch(this.pixelformat) {
					case PixelFormat.Format24bppRgb:
					case PixelFormat.Format32bppRgb:
						p[0] = value.B;
						p[1] = value.G;
						p[2] = value.R;
						break;
					case PixelFormat.Format32bppArgb:
						p[0] = value.B;
						p[1] = value.G;
						p[2] = value.R;
						p[3] = value.A;
						break;
					case PixelFormat.Format16bppGrayScale:
						int lv = (((int)value.R+value.G+value.B)<<8)/3;
						*(ushort*)p = (ushort)lv;
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}
		}

	}

}
