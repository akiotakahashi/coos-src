using System;

namespace IA32Toolkit.Assembler {
	/// <summary>
	/// Utility ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
	/// </summary>
	public class Utility {
		
		public static readonly string OpCodeExpression = @"^(?<fmt>(?<opcode>(([0-9A-F]{2})(\s*\+\s*(rb|rw|rd|i))?\s+)+)(?<reg>(/[0-7]|/r)\s+)?(?<c>(cb|cw|cd|cp|cw/cd)\s+)?(?<i>(ib|iw|id)\s+)*)";
	
		public static readonly string OperandExpression =
			@"(EAX|ECX|EDX|EBX|ESP|EBP|ESI|EDI"+
			@"|CS|DS|SS|ES|FS|GS"+
			@"|AX|CX|DX|BX|SP|BP|SI|DI"+
			@"|AL|AH|CL|CH|DL|DH|BL|BH"+
			@"|1|CR[0-4]|DR0-DR7"+
			@"|rel(8|16|32)|rel16/32|ptr16:(16|32)|r(8|16|32)|r32/m16"+
			@"|imm(8|16|32)|r/m(8|16|32)|m|m(8|16|32)|m64|m128"+
			@"|m16:(16|32)|m16&(16|32)|m32&32|moffs(8|16|32)|Sreg"+
			@"|m(32|64|80)fp|m(16|32|64)int|ST|ST\((i|[0-7])\)"+
			@"|mm(1|2)?|mm(2)?/m(32|64|128)|xmm(1|2)?|xmm(2)?/m(32|64|128))\**";
	
		public static readonly string MnemonicExpression =
			@"(?<asm>"+
				@"(?<name>(REP |REPE |REPNE )?[A-Z][A-Z0-9]+)"+
				@"(?<operands>\s+"+
					@"("+OperandExpression+")"+
					@"(,\s*("+OperandExpression+"))*"+
				@")?"+
				@"(?<desc> .+)?$"+
			@")";

	}

}
