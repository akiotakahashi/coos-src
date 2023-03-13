using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CooS.Graphics {

	public abstract class Canvas {
		
		public Canvas() {
		}

		public abstract PixelFormat PixelFormat {get;}
		public abstract int PixelSize {get;}
		public abstract int ScanLineSize {get;}
		public abstract Size Size {get;}
		public abstract FixedFrame GetFixedFrame();
		public abstract Color this[int x, int y] { get; set; }

		public virtual double DPI { get { return 96; } }
		public virtual string Profile { get { return "sRGB"; } }

		public virtual Painter CreatePainter() {
			return new Painter(this);
		}

		public virtual int GetPixelFromColor(Color c) {
			switch(this.PixelFormat) {
			case PixelFormat.Undefined:
				throw new NotSupportedException();
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
				return c.ToArgb();
			case PixelFormat.Format16bppGrayScale:
				return ((int)c.R+c.G+c.B)/3;
			default:
				throw new NotImplementedException();
			}
		}

		public virtual unsafe void Blt(int dx, int dy, Canvas src, int sx, int sy, int width, int height) {
			Canvas dst = this;
			if(dst.PixelFormat==src.PixelFormat) {
				int size = dst.PixelSize;
				using(FixedFrame fd = dst.GetFixedFrame()) {
					using(FixedFrame fs = dst.GetFixedFrame()) {
						byte* pd = (byte*)fd.Address.ToPointer();
						byte* ps = (byte*)fd.Address.ToPointer();
						pd += dst.ScanLineSize*dy+dx;
						ps += src.ScanLineSize*sy+sx;
						for(int y=0; y<height; ++y) {
							CooS.Tuning.Memory.Copy(pd, ps, size*width);
							pd += dst.ScanLineSize;
							ps += src.ScanLineSize;
						}
					}
				}
			} else {
				for(int y=0; y<height; ++y) {
					for(int x=0; x<width; ++x) {
						this[dx+x,dy+y] = src[sx+x,sy+y];
					}
				}
			}
		}

	}

}
