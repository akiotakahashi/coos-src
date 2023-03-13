using System;
using System.Drawing;

namespace CooS.Graphics {

	public class Painter {

		private readonly Canvas canvas;

		public Painter(Canvas canvas) {
			this.canvas = canvas;
		}

		public unsafe void DrawLine(int x0, int y0, int x1, int y1, Color color) {
			int dx, dy;
			if(x0<x1) dx=1; else dx=-1;
			if(y0<y1) dy=1; else dy=-1;
			int lx = Math.Abs(x1-x0);
			int ly = Math.Abs(y1-y0);
			int lmax = (lx>ly) ? lx : ly;
			if(lmax<1) {
				this.canvas[x0,y0] = color;
				this.canvas[x1,y1] = color;
			} else {
				int c = this.canvas.GetPixelFromColor(color);
				int slsz = this.canvas.ScanLineSize;
				int px = 0;
				int py = 0;
				// If I use 'switch', then this method can't be compiled
				// because the compiler don't support switch instruction.
				if(this.canvas.PixelSize==4) {
					using(FixedFrame frame = this.canvas.GetFixedFrame()) {
						byte* p = (byte*)frame.Address.ToPointer();
						for(int i=0; i<=lmax/2; ++i) {
							*(int*)(p+4*(x0+(px>>8))+slsz*(y0+(py>>8))) = c;
							*(int*)(p+4*(x1-(px>>8))+slsz*(y1-(py>>8))) = c;
							px += dx*(lx<<8)/lmax;
							py += dy*(ly<<8)/lmax;
						}
					}
				} else if(this.canvas.PixelSize==2) {
					using(FixedFrame frame = this.canvas.GetFixedFrame()) {
						byte* p = (byte*)frame.Address.ToPointer();
						for(int i=0; i<=lmax/2; ++i) {
							*(ushort*)(p+2*(x0+(px>>8))+slsz*(y0+(py>>8))) = (ushort)c;
							*(ushort*)(p+2*(x1-(px>>8))+slsz*(y1-(py>>8))) = (ushort)c;
							px += dx*(lx<<8)/lmax;
							py += dy*(ly<<8)/lmax;
						}
					}
				} else if(this.canvas.PixelSize==3) {
					c &= 0xFFFFFF;
					using(FixedFrame frame = this.canvas.GetFixedFrame()) {
						byte* p = (byte*)frame.Address.ToPointer();
						for(int i=0; i<=lmax/2; ++i) {
							int p1 = 3*(x0+(px>>8))+slsz*(y0+(py>>8));
							int p2 = 3*(x1-(px>>8))+slsz*(y1-(py>>8));
							*(uint*)(p+p1) &= 0xFF000000;
							*(uint*)(p+p2) &= 0xFF000000;
							*(int*)(p+p1) |= c;
							*(int*)(p+p2) |= c;
							px += dx*(lx<<8)/lmax;
							py += dy*(ly<<8)/lmax;
						}
					}
				} else {
					//Console.WriteLine("Unrecoginized pixel size: {0}", this.canvas.PixelSize.ToString());
					for(int i=0; i<=lmax/2; ++i) {
						this.canvas[x0+(px>>8),y0+(py>>8)] = color;
						this.canvas[x1-(px>>8),y1-(py>>8)] = color;
						px += dx*(lx<<8)/lmax;
						py += dy*(ly<<8)/lmax;
					}
				}
			}
		}

		public void DrawRect() {
		}

		public unsafe void FillRect(int x, int y, int w, int h, Color c) {
			if(w==0 || h==0) return;
			if(w<0) { x=x-w+1; w=-w; }
			if(h<0) { y=y-h+1; h=-h; }
			this.DrawLine(x, y, x+w-1, y, c);
			int scanline = this.canvas.ScanLineSize;
			int linesize = this.canvas.PixelSize*w;
			using(FixedFrame frame = this.canvas.GetFixedFrame()) {
				byte* p = frame[x,y];
				for(int dy=1; dy<h; ++dy) {
					byte* dst = p+dy*scanline;
					CooS.Tuning.Memory.Copy(dst, p, linesize);
				}
			}
		}

	}

}
