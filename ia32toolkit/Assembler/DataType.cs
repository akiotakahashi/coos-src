using System;

namespace IA32Toolkit.Assembler {
	
	public class DataType {
	
		public readonly string Text;

		public DataType(string typetext) {
			switch(typetext) {
			case "imm8":
			case "imm16":
			case "imm32":
			case "r8":
			case "r16":
			case "r32":
			case "m8":
			case "m16":
			case "m32":
			case "r/m8":
			case "r/m16":
			case "r/m32":
			case "rel8":
			case "rel16":
			case "rel32":
				break;
			default:
				throw new ArgumentException(typetext);
			}
			this.Text = typetext;
		}

	}

}
