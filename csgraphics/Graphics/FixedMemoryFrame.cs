using System;

namespace CooS.Graphics {

	public class FixedMemoryFrame : FixedFrame {

		readonly Canvas canvas;
		readonly IntPtr address;
		
		public FixedMemoryFrame(Canvas canvas, IntPtr address) {
			this.canvas = canvas;
			this.address = address;
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

		public override void Dispose() {
			// NOP
		}

	}

}
