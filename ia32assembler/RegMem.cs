using System;

namespace IA32Toolkit.Assembler {

	public struct RegMem {
		public int Reg;
		public Register32 Index;
		public int Scale;
		public int Disp;
		public static RegMem Empty = new RegMem(0,(Register32)0,0,0);
		public bool WithIndex {
			get {
				return Scale>0;
			}
		}
		public bool WithDisp {
			get {
				return Disp!=0;
			}
		}
		public RegMem Value {
			get {
				return this;
			}
		}
		private RegMem(int reg, Register32 idx, int scale, int disp) {
			this.Reg = reg;
			this.Disp = disp;
			this.Index = idx;
			this.Scale = scale;
		}
		private static RegMem Direct(int r) {
			return new RegMem(r, Register32.Invalid,-1, 0);
		}
		public static RegMem Direct(Register8 r) {
			return Direct((int)r);
		}
		public static RegMem Direct(Register16 r) {
			return Direct((int)r);
		}
		public static RegMem Direct(Register32 r) {
			return Direct((int)r);
		}
		public static RegMem Indirect(Register32 reg) {
			return new RegMem((int)reg, Register32.Invalid,0, 0);
		}
		public static RegMem Indirect(int disp) {
			return new RegMem((int)Register32.Invalid, Register32.Invalid,0, disp);
		}
		public static RegMem Indirect(Register32 reg, int disp) {
			return new RegMem((int)reg, Register32.Invalid,0, disp);
		}
		public static RegMem Indirect(Register32 reg, Register32 idx, int scale) {
			return new RegMem((int)reg, idx,scale, 0);
		}
		public static RegMem Indirect(Register32 idx, int scale, int disp) {
			return new RegMem((int)Register32.Invalid, idx,scale, disp);
		}
		public static RegMem Indirect(Register32 reg, Register32 idx, int scale, int disp) {
			return new RegMem((int)reg, idx,scale, disp);
		}
	}

	public struct RegMem8 {
		public RegMem Value;
		public static implicit operator RegMem8(RegMem rm) {
			RegMem8 sizedrm;
			sizedrm.Value = rm;
			return sizedrm;
		}
		public static implicit operator RegMem8(Register8 r) {
			return RegMem.Direct((Register32)r);
		}
	}

	public struct RegMem16 {
		public RegMem Value;
		public static implicit operator RegMem16(RegMem rm) {
			RegMem16 sizedrm;
			sizedrm.Value = rm;
			return sizedrm;
		}
		public static implicit operator RegMem16(Register16 r) {
			return RegMem.Direct((Register32)r);
		}
	}

	public struct RegMem32 {
		public RegMem Value;
		public static implicit operator RegMem32(RegMem rm) {
			RegMem32 sizedrm;
			sizedrm.Value = rm;
			return sizedrm;
		}
		public static implicit operator RegMem32(Register32 r) {
			return RegMem.Direct(r);
		}
	}

	public struct RegMem64 {
		public RegMem Value;
		public static implicit operator RegMem64(RegMem rm) {
			RegMem64 sizedrm;
			sizedrm.Value = rm;
			return sizedrm;
		}
		public static implicit operator RegMem64(Register32 r) {
			return RegMem.Direct(r);
		}
	}

	public struct Mem32F {
		public RegMem Value;
		public static implicit operator Mem32F(RegMem rm) {
			Mem32F sizedrm;
			sizedrm.Value = rm;
			return sizedrm;
		}
	}

	public struct Mem64F {
		public RegMem Value;
		public static implicit operator Mem64F(RegMem rm) {
			Mem64F sizedrm;
			sizedrm.Value = rm;
			return sizedrm;
		}
	}

}
