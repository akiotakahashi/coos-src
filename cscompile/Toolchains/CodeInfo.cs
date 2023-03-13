using System;

namespace CooS.Toolchains {

	public class CodeInfo {

		public byte[] CodeBlock;
		public CodeLabel[] Labels;
		public int EntryPointOffset;

		public CodeInfo(byte[] code, int offset) {
			if(offset<0) throw new ArgumentException();
			this.CodeBlock = code;
			this.EntryPointOffset = offset;
		}

		public CodeInfo(IntPtr address) {
			this.CodeBlock = null;
			this.EntryPointOffset = address.ToInt32();
		}

		public unsafe IntPtr EntryPoint {
			get {
				if(this.CodeBlock!=null) {
					fixed(byte* p = &this.CodeBlock[this.EntryPointOffset]) {
						return new IntPtr(p);
					}
				} else {
					return new IntPtr(this.EntryPointOffset);
				}
			}
		}

		public void Link() {
			if(this.Labels==null) return;
			foreach(CodeLabel label in this.Labels) {
				label.Rewrite(this.CodeBlock);
			}
		}

		public override string ToString() {
			System.IO.StringWriter writer = new System.IO.StringWriter();
			Utility.Dump(writer, this.CodeBlock);
			return writer.ToString();
		}

	}

}
