using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using BindingFlags = System.Reflection.BindingFlags;

namespace CooS.Formats.CLI.IL {

	public enum ImmidiateValues : byte {
		p0,
		p1,
		p2,
		p3,
		p4,
		p5,
		p6,
		p7,
		m1,
		any,
		INVALID,
	}

	[Flags]
	public enum InstructionFlags : byte {
		ovf = 1,	// overflow
		un = 2,		// unsigned
		s = 4,		// short-form
		INVALID = 0x80,
	}

	public enum TypeSuffixes : byte {
		i,
		u,
		i1,
		i2,
		i4,
		i8,
		r4,
		r8,
		r,		// for conv.r.un
		u1,
		u2,
		u4,
		u8,
		obj,	// alternative 'ref'
		INVALID,
	}

	[Flags]
	public enum TokenTypes : byte {
		td = 0x01,	// TypeDef
		fd = 0x02,	// FieldDef
		md = 0x04,	// MethodDef
		tr = 0x08,	// TypeRef
		ts = 0x10,	// TypeSpec
		ms = 0x20,	// MethodSpec
		mr = 0x40,	// MemberRef
		INVALID = 0x80,
	}

	public struct Instruction {

		private static readonly OpCode[] SingleOpecodes = new OpCode[256];
		private static readonly Dictionary<short, OpCode> DoubleOpecodes = new Dictionary<short, OpCode>();

		static Instruction() {
			foreach(System.Reflection.FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(fi.FieldType!=typeof(OpCode)) { continue; }
				OpCode opcode = (OpCode)fi.GetValue(null);
				if(opcode.Size==1) {
					if(opcode.Value!=0xfe) {
						SingleOpecodes[opcode.Value] = opcode;
					}
				} else if(opcode.Size==2) {
					DoubleOpecodes[opcode.Value] = opcode;
				} else {
					throw new InvalidProgramException(string.Format("{0}'s size is {1}",opcode.Name,opcode.Size));
				}
			}
		}
		
		private static OpCode GetOpecode(byte[] code, int firstIndex) {
			OpCode opcode = SingleOpecodes[code[firstIndex]];
			if(opcode.Size!=0) {
				return opcode;
			} else {
				try {
					return DoubleOpecodes[(short)(((int)code[firstIndex]<<8)|code[firstIndex+1])];
				} catch(KeyNotFoundException) {
					throw new BadCodeException("Invalid opcode: 0x"+(((int)code[firstIndex]<<8)|code[firstIndex+1]).ToString("X4"));
				}
			}
		}

		public readonly InstructionPrefixes Prefixes;
		public readonly int OpcodeAddress;
		public readonly OpCode OpCode;

		public readonly InstructionFlags Flags;
		public readonly ImmidiateValues Imm;
		public readonly TypeSuffixes Suffix;
		public readonly TokenTypes OpeType;

		public static Instruction[] Read(byte[] code) {
			List<Instruction> list = new List<Instruction>();
			int p = 0;
			while(p<code.Length) {
				list.Add(Read(code, ref p));
			}
			return list.ToArray();
		}

		public static Instruction Read(byte[] code, ref int address) {
			InstructionPrefixes prefixes = InstructionPrefixes.None;
			OpCode opcode;
			for(; ; ) {
				opcode = GetOpecode(code, address);
				// プリフィックスの場合は先読み
				if(opcode.Value==OpCodes.Volatile.Value) {
					prefixes |= InstructionPrefixes.Volatile;
				} else if(opcode.Value==OpCodes.Tailcall.Value) {
					prefixes |= InstructionPrefixes.Tailcall;
				} else if(opcode.Value==OpCodes.Unaligned.Value) {
					prefixes |= InstructionPrefixes.Unaligned;
#if false
				} else if(opcode.Value==OpCodes.Constrained.Value) {
					prefixes |= InstructionPrefixes.Constrained;
					constraned = (MDToken)reader.ReadUInt32();
#endif
				} else {
					break;
				}
				address += opcode.Size;
			}
			return new Instruction(code, ref address, opcode, prefixes);
		}

		private Instruction(byte[] code, ref int address, OpCode opcode, InstructionPrefixes prefixes) {
			if(code==null) { throw new ArgumentNullException(); }
			if(opcode.Size==0) { throw new ArgumentNullException(); }
			this.OpcodeAddress = address;
			this.OpCode = opcode;
			this.Prefixes = prefixes;
			this.Imm = ImmidiateValues.INVALID;
			this.Flags = InstructionFlags.INVALID;
			this.Suffix = TypeSuffixes.INVALID;
			this.OpeType = TokenTypes.INVALID;
			string[] parts = opcode.Name.Split('.');
			for(int i=1; i<parts.Length; ++i) {
				string part = parts[i];
				int num;
				if(int.TryParse(part, out num)) {
					this.Imm = (ImmidiateValues)num;
				} else if(Enum.IsDefined(typeof(ImmidiateValues), part)) {
					this.Imm = (ImmidiateValues)Enum.Parse(typeof(ImmidiateValues), part);
				} else if(Enum.IsDefined(typeof(InstructionFlags), part)) {
					this.Flags = (InstructionFlags)Enum.Parse(typeof(InstructionFlags), part);
				} else if(Enum.IsDefined(typeof(TypeSuffixes), part)) {
					this.Suffix = (TypeSuffixes)Enum.Parse(typeof(TypeSuffixes), part);
				} else if(Enum.IsDefined(typeof(TokenTypes), part)) {
					this.OpeType = (TokenTypes)Enum.Parse(typeof(TokenTypes), part);
				} else {
					switch(part) {
					case "ref":
						this.Suffix = TypeSuffixes.obj;
						break;
					case "":
						// maybe prefix's last dot
						break;
					default:
						throw new BadCodeException("Unknown modifier: "+part+" /"+opcode.Name);
					}
				}
			}
			address += this.OpCode.Size;
			address += GetOperandSize(code);
		}

		public int OperandAddress {
			get {
				return this.OpcodeAddress+this.OpCode.Size;
			}
		}

		public int GetOperandSize(byte[] code) {
			switch(this.OpCode.OperandType) {
			case OperandType.InlineNone:
				//オペランドなし。
				return 0;
			case OperandType.InlineBrTarget:
			case OperandType.InlineField:
			case OperandType.InlineMethod:
			case OperandType.InlineI:
			case OperandType.InlineSig:
			case OperandType.InlineString:
			case OperandType.InlineTok:
			case OperandType.InlineType:
			case OperandType.ShortInlineR:
				//オペランドは 32 ビットです。 
				return 4;
			case OperandType.InlineI8:
			case OperandType.InlineR:
				//オペランドは 64 ビットです。
				return 8;
			case OperandType.InlineSwitch:
				//オペランドは switch 命令の 32 ビット整数引数です。 
				int length = BitConverter.ToInt32(code, this.OperandAddress);
				return 4+4*length;
			case OperandType.InlineVar:
				//オペランドは、16 ビットです。 
				return 2;
			case OperandType.ShortInlineBrTarget:
			case OperandType.ShortInlineVar:
			case OperandType.ShortInlineI:
				//オペランドは 8 ビットです。
				return 1;
			default:
				throw new NotSupportedException(this.OpCode.OperandType.ToString());
			}
		}

		public override string ToString() {
			return string.Format("[{0:X2}] {1,-10}", this.OpcodeAddress, this.OpCode.Name);
		}

		public void Dump(System.IO.TextWriter writer) {
			writer.Write("[{0:X2}] {1,-10}", this.OpcodeAddress, this.OpCode.Name);
		}

	}

}
