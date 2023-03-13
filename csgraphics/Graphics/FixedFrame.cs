using System;

namespace CooS.Graphics {

	public abstract class FixedFrame : IDisposable {

		#region IDisposable ƒƒ“ƒo

		public abstract void Dispose();

		#endregion

		protected abstract Canvas Canvas {get;}
		public abstract IntPtr Address {get;}

		public unsafe byte* this[int x, int y] {
			get {
				return (byte*)this.Address.ToPointer()+y*this.Canvas.ScanLineSize+x*this.Canvas.PixelSize;
			}
		}

	}

}
