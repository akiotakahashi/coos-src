using System;
using CooS.Formats;

namespace CooS.Manipulation {

	public class RelativeLabel : CodeLabel {

		readonly int position;
		readonly IBranchTarget target;

		public RelativeLabel(int position, IBranchTarget brtgt) {
			this.position = position;
			this.target = brtgt;
		}

		public override unsafe void Rewrite(byte[] code) {
			int address = target.Address.ToInt32();
			int old;
			fixed(byte* p = &code[this.position]) {
				int operand = address-(int)(p+4);
				old = *(int*)p;
				*(int*)p = operand;
			}
			//Console.WriteLine("Rewrite [+0x{0:X8}] = 0x{2:X08} > 0x{1:X08} /REL", this.position, address, old);
		}

	}

}
