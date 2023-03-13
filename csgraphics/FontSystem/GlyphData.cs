using System;
using System.Drawing;
using System.Drawing.Imaging;
using FreeType;
using CooS.Graphics;

namespace CooS.FontSystem {

	class GlyphData {

		public readonly Canvas Image;
		public readonly Size BearingSize;
		public readonly GlyphMetrics Metrics;

		public GlyphData(char ch, FontData fontdata) {
			fontdata.LoadGlyph(ch);
			this.Metrics.Advance = fontdata.AdvanceSize;
			this.BearingSize = fontdata.BearingSize;
			if(fontdata.BearingSize.IsEmpty) {
				this.Image = null;
			} else {
				this.Image = new Graphics.Bitmap(PixelFormat.Format16bppGrayScale, fontdata.BitmapSize);
				fontdata.SetPainter(new FontDrawHandler(RenderPixel));
				fontdata.DrawGlyph();
			}
		}

		void RenderPixel(int x, int y, int level) {
			this.Image[x,y] = Color.FromArgb(level,level,level);
		}

	}

}
