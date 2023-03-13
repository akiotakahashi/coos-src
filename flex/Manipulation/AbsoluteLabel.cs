using System;
using System.Runtime.InteropServices;

namespace CooS.Manipulation {

	public class AbsoluteLabel : CodeLabel {

		readonly int position;
		readonly object target;
		readonly int offset;

		public AbsoluteLabel(int position, object target, int offset) {
			this.position = position;
			this.target = target;
			this.offset = offset;
		}

		public override unsafe void Rewrite(byte[] code) {
			GCHandle pin = GCHandle.Alloc(this.target, GCHandleType.Pinned);
			int address = Kernel.ObjectToValue(this.target).ToInt32()+offset;
			int old;
			fixed(byte* p = &code[this.position]) {
				old = *(int*)p;
				*(int*)p = address;
			}
			pin.Free();
			//Console.WriteLine("Rewrite [+0x{0:X8}] = 0x{2:X08} > 0x{1:X08} /ABS", this.position, address, old);
		}

	}

}
