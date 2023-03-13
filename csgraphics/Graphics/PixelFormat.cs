using System;

namespace CooS.Graphics {

	/*
	public struct ColorComponentFormat {

		public int BitPosition;
		public int BitCount;

		public int Mask {
			get {
				return ((1<<this.BitCount)-1)<<this.BitPosition;
			}
		}

		public static bool operator ==(ColorComponentFormat op1, ColorComponentFormat op2) {
			if(op1.BitCount!=op2.BitCount) return false;
			if(op1.BitCount==0) return true;
			if(op1.BitPosition!=op2.BitPosition) return false;
			return true;
		}

		public static bool operator !=(ColorComponentFormat op1, ColorComponentFormat op2) {
			return !(op1==op2);
		}

	}

	public struct PixelFormat {

		public int PixelSize;
		public ColorComponentFormat Red;
		public ColorComponentFormat Green;
		public ColorComponentFormat Blue;
		public ColorComponentFormat Alpha;

		public Color GetPixel(byte* p) {
		}

		public void SetPixel(byte* p, Color c) {
		}

		public static bool operator ==(PixelFormat op1, PixelFormat op2) {
			if(op1.PixelSize!=op2.PixelSize) return false;
			if(op1.Red!=op2.Red) return false;
			if(op1.Blue!=op2.Blue) return false;
			if(op1.Green!=op2.Green) return false;
			return true;
		}

		public static bool operator !=(PixelFormat op1, PixelFormat op2) {
			return !(op1==op2);
		}

	}
	*/

}
