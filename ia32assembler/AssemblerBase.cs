using System;
using System.IO;

namespace IA32Toolkit.Assembler {

	public enum AddressMode {
		Direct				= 3,
		Indirect			= 0,
		IndirectWithDisp8	= 1,
		IndirectWithDisp32	= 2,
	}

	public class IA32AssemblerBase {

		public readonly Stream BaseStream;
		AddressMode lastmod;
		int lastrm;

		public IA32AssemblerBase() {
			this.BaseStream = new MemoryStream();
		}

		public IA32AssemblerBase(Stream stream) {
			this.BaseStream = stream;
		}

		private void WriteByte(int value) {
			this.BaseStream.WriteByte((byte)value);
		}

		private void Write(byte[] data, int start, int length) {
			this.BaseStream.Write(data, 0, length);
		}

		public bool WithSIB {
			get {
				if(lastmod==AddressMode.Direct) return false;
				if(lastrm!=4) return false;
				return true;
			}
		}

		public bool WithDisp {
			get {
				switch(this.lastmod) {
				case AddressMode.Direct:
					return false;
				case AddressMode.Indirect:
					if(lastrm==4) return true;
					return false;
				case AddressMode.IndirectWithDisp8:
				case AddressMode.IndirectWithDisp32:
					return true;
				default:
					throw new ApplicationException("Last AddressMode is invalid");
				}
			}
		}
		
		protected void WriteOpCode(int value, params int[] extends) {
			this.WriteByte(value);
			foreach(int ex in extends) {
				this.WriteByte(ex);
			}
		}

		private void WriteModRM(int rm, int reg, AddressMode mod) {
			if(rm<0 || 7<rm) throw new ArgumentOutOfRangeException("rm",rm,"[0,7]");
			if(reg<0 || 7<reg)  throw new ArgumentOutOfRangeException("reg",reg,"[0,7]");
			this.lastmod = mod;
			this.lastrm = rm;
			this.WriteByte(rm | (reg<<3) | ((int)mod<<6));
		}

		private void WriteSIB(int basereg, int index, int scale) {
			if(basereg<0 || 7<basereg) throw new ArgumentOutOfRangeException("basereg");
			if(index<0 || 7<index) throw new ArgumentOutOfRangeException("index");
			if(!WithSIB) throw new InvalidOperationException();
			int ss;
			switch(scale) {
			case 0:
				// No indexed
				ss = 0;
				break;
			case 1:
				ss = 0;
				break;
			case 2:
				ss = 1;
				break;
			case 4:
				ss = 2;
				break;
			case 8:
				ss = 3;
				break;
			default:
				throw new ArgumentOutOfRangeException("scale",scale,"not 2^N");
			}
			this.WriteByte((byte)(basereg | ((int)index<<3) | (ss<<6)));
		}

		private void WriteDisplacement(int displacement) {
			int bits;
			switch(this.lastmod) {
			case AddressMode.Direct:
				throw new InvalidOperationException("mod must not be 3");
			case AddressMode.Indirect:
				if(lastrm!=5) throw new InvalidOperationException("R/M must be 5");
				bits = 32;
				break;
			case AddressMode.IndirectWithDisp8:
				bits = 8;
				break;
			case AddressMode.IndirectWithDisp32:
				bits = 32;
				break;
			default:
				throw new ApplicationException("Last AddressMode si invalid");
			}
			byte[] data = BitConverter.GetBytes(displacement);
			this.Write(data, 0, bits/8);
		}

		protected void WriteModifiers(RegMem rm, int regop) {
			if(rm.Scale==-1) {
				// indicates directly reg but [reg]
				this.WriteModRM((int)rm.Reg, regop, AddressMode.Direct);
			} else {
				// [reg]
				// disp32
				// disp8[reg]
				// disp32[reg]
				// [reg][index]
				// disp32[index]
				// disp8[reg][index]
				// disp32[reg][index]
				int reg;
				AddressMode mod;
				switch(rm.Scale) {
				case 0:
					if(rm.Reg==4) {
						// below indicates none
						rm.Index = Register32.ESP;
						rm.Scale = 1;
					}
					reg = rm.Reg;
					break;
				case 1:
				case 2:
				case 4:
				case 8:
					reg = 4;
					break;
				default:
					throw new ApplicationException();
				}
				if(rm.Disp==0) {
					if(rm.Reg==5) {
						mod = AddressMode.IndirectWithDisp8;
					} else {
						mod = AddressMode.Indirect;
					}
				} else if(-128<=rm.Disp && rm.Disp<=+127) {
					mod = AddressMode.IndirectWithDisp8;
				} else {
					mod = AddressMode.IndirectWithDisp32;
				}
				this.WriteModRM(reg, regop, mod);
				if(reg==4) {
					this.WriteSIB(rm.Reg, (int)rm.Index, rm.Scale);
				}
				switch(mod) {
				case AddressMode.IndirectWithDisp8:
				case AddressMode.IndirectWithDisp32:
					this.WriteDisplacement(rm.Disp);
					break;
				}
			}
		}

		protected void WriteImmediate(sbyte imm) {
			this.WriteByte((byte)imm);
		}

		protected void WriteImmediate(byte imm) {
			this.WriteByte(imm);
		}

		protected void WriteImmediate(ushort imm) {
			byte[] data = BitConverter.GetBytes(imm);
			this.Write(data, 0, data.Length);
		}

		protected void WriteImmediate(uint imm) {
			byte[] data = BitConverter.GetBytes(imm);
			this.Write(data, 0, data.Length);
		}

		protected void WriteImmediate(short imm) {
			byte[] data = BitConverter.GetBytes(imm);
			this.Write(data, 0, data.Length);
		}

		protected void WriteImmediate(int imm) {
			byte[] data = BitConverter.GetBytes(imm);
			this.Write(data, 0, data.Length);
		}

	}

}
