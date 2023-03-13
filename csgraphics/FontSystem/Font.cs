using System;
using System.Drawing;
using System.Drawing.Imaging;
using FreeType;
using CooS.Graphics;

namespace CooS.FontSystem {

	public class Font {
		
		readonly FontData fontdata;
		readonly int height;
		readonly GlyphData[] glyphs = new GlyphData[128];

		public Font(byte[] fontdata, int height_pixel) {
			this.fontdata = new FontData(fontdata);
			this.height = height_pixel;
			this.fontdata.SetSizeByPixel(0, height_pixel);
		}

		public int Height {
			get {
				return this.height;
			}
		}

		private GlyphData LoadGlyph(char ch) {
			if(ch<128) {
				if(glyphs[ch]!=null) {
					return glyphs[ch];
				} else {
					return glyphs[ch] = new GlyphData(ch, this.fontdata);
				}
			} else {
				return new GlyphData(ch, this.fontdata);
			}
		}

		public Size GetBearingSize(char ch) {
			return this.LoadGlyph(ch).BearingSize;
		}

		public int Draw(char ch, Canvas canvas, int x, int y) {
			GlyphData glyph = this.LoadGlyph(ch);
			if(glyph.Image!=null) {
				canvas.Blt(x, y, glyph.Image, 0, 0, glyph.Image.Size.Width, glyph.Image.Size.Height);
			}
			return glyph.Metrics.Advance.Width>>6;	// since scaled by 1/64
		}

	}

}
