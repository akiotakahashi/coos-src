using System;
using System.Runtime.InteropServices;

namespace CooS.Graphics {

	public class FixedBufferFrame : FixedFrame {

		private readonly Canvas canvas;
		private readonly byte[] buffer;
		private readonly GCHandle handle;
		private IntPtr address;

		public FixedBufferFrame(Canvas canvas, byte[] buf) {
			this.canvas = canvas;
			this.buffer = buf;
			this.handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
			this.address = this.handle.AddrOfPinnedObject();
		}

		public override void Dispose() {
			if(this.buffer!=null) {
				this.handle.Free();
				this.address = IntPtr.Zero;
			}
		}

		protected override Canvas Canvas {
			get {
				return this.canvas;
			}
		}

		public override IntPtr Address {
			get {
				return this.address;
			}
		}

	}

}
