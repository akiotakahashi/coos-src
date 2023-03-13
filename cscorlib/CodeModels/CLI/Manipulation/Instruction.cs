using System;
using CooS.Reflection;
using System.Reflection.Emit;

namespace CooS.CodeModels.CLI.Manipulation {

	class Instruction : IBranchTarget {

		public readonly AssemblyDef assembly;

		public readonly int Address;
		public readonly OpCode OpCode;
		public readonly object Operand;
		public readonly InstructionPrefixes Prefixes;
		public readonly int BrTarget;
		public readonly int[] BrTargets;
		public EvaluationStack StackState;
		public int NativeAddress;

		// for new instruction model
		public readonly string CoreName;
		public readonly TypeImpl OpType;
		public readonly bool Unsigned = false;
		public readonly bool Overflow = false;
		public readonly int Number = 0x77777777;

		public Instruction(AssemblyDef assembly, int addr, OpCode opcode, object operand, InstructionPrefixes prefixes) {
			this.assembly = assembly;
			AssemblyManager manager = assembly.Manager;
			this.Address = addr;
			this.OpCode = opcode;
			this.Operand = operand;
			this.Prefixes = prefixes;
			this.BrTarget = -1;
			this.BrTargets = null;
			this.StackState = null;
			this.NativeAddress = -1;
			switch(opcode.OperandType) {
			case OperandType.InlineBrTarget:
				this.BrTarget = this.BrBaseAddress+(int)this.Operand;
				break;
			case OperandType.ShortInlineBrTarget:
				this.BrTarget = this.BrBaseAddress+(sbyte)this.Operand;
				break;
			case OperandType.InlineSwitch:
				this.BrTargets = (int[])this.Operand;
				for(int i=0; i<this.BrTargets.Length; ++i) {
					this.BrTargets[i] += this.BrBaseAddress;
				}
				break;
			case OperandType.InlineVar:
				this.Number = (ushort)this.Operand;
				break;
			case OperandType.ShortInlineVar:
				this.Number = (byte)this.Operand;
				break;
			case OperandType.InlineI:
				this.Number = (int)this.Operand;
				break;
			case OperandType.ShortInlineI:
				if(this.OpCode.Value==OpCodes.Ldc_I4_S.Value) {
					// idc.i4.s のオペランド情報が変。
					this.Number = (sbyte)this.Operand;
				} else {
					this.Number = (short)this.Operand;
				}
				break;
				/*
			case OperandType.InlineI8:
				this.Number = (long)this.Operand;
				break;
			case OperandType.InlineR:
				this.Number = (double)this.Operand;
				break;
			case OperandType.ShortInlineR:
				this.Number = (float)this.Operand;
				break;
				*/
			}
			//
			string[] parts = this.OpCode.Name.ToLower().Split('.');
			this.CoreName = parts[0];
			for(int i=1; i<parts.Length; ++i) {
				string part = parts[i];
				switch(part) {
				case "ovf":
					this.Overflow = true;
					break;
				case "un":
					this.Unsigned = true;
					break;
				case "s":
					break;
				case "i":
					this.OpType = manager.IntPtr;
					break;
				case "u":
					this.OpType = manager.UIntPtr;
					break;
				case "i1":
					this.OpType = manager.SByte;
					break;
				case "i2":
					this.OpType = manager.Int16;
					break;
				case "i4":
					this.OpType = manager.Int32;
					break;
				case "i8":
					this.OpType = manager.Int64;
					break;
				case "r4":
				case "r":	// conv.r.un
					this.OpType = manager.Single;
					break;
				case "r8":
					this.OpType = manager.Double;
					break;
				case "u1":
					this.OpType = manager.Byte;
					break;
				case "u2":
					this.OpType = manager.UInt16;
					break;
				case "u4":
					this.OpType = manager.UInt32;
					break;
				case "u8":
					this.OpType = manager.UInt64;
					break;
				case "ref":
					this.OpType = manager.Object;
					break;
				case "0":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
					this.Number = int.Parse(part);
					break;
				case "m1":
					this.Number = -1;
					break;
				default:
					throw new UnexpectedException(part);
				}
			}
		}

		public override string ToString() {
			return string.Format("[{0:X2}] {1,-10} ({2})", this.Address, this.OpCode.Name, this.Operand);
		}

		public void Dump(System.IO.TextWriter writer) {
			writer.Write("[{0:X2}] {1,-10}<", this.Address, this.OpCode.Name);
			if(this.StackState==null) {
				writer.WriteLine(" (n/a)");
			} else {
				for(int i=0; i<this.StackState.Length; ++i) {
					if(this.StackState[i]==null) {
						writer.Write(" (null)");
					} else {
						writer.Write(" {0}", this.StackState[i].Name);
					}
				}
				writer.WriteLine();
			}
		}

		public int OperandSize {
			get {
				switch(this.OpCode.OperandType) {
				case OperandType.InlinePhi:
					//このメンバは、.NET Framework インフラストラクチャのサポートを目的としています。
					//独自に作成したコード内で直接使用することはできません。 
					throw new NotImplementedException();
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
					return 4+4*((int[])this.Operand).Length;
				case OperandType.InlineVar:
				case OperandType.ShortInlineI:
					//オペランドは、16 ビットです。 
					return 2;
				case OperandType.ShortInlineBrTarget:
				case OperandType.ShortInlineVar:
					//オペランドは 8 ビットです。
					return 1;
				default:
					throw new NotSupportedException(this.OpCode.OperandType.ToString());
				}
			}
		}

		public int BrBaseAddress {
			get {
				return this.Address+this.OpCode.Size+this.OperandSize;
			}
		}

		public Metadata.MDToken Token {
			get {
				return (Metadata.MDToken)this.Operand;
			}
		}

		public TypeImpl TypeInfo {
			get {
				return assembly.ResolveType(this.Token);
			}
		}

		public FieldInfoImpl FieldInfo {
			get {
				return assembly.ResolveField(this.Token);
			}
		}

		public MethodInfoImpl MethodInfo {
			get {
				return assembly.ResolveMethod(this.Token);
			}
		}

		#region IBranchTarget メンバ

		IntPtr IBranchTarget.Address {
			get {
				return new IntPtr(this.NativeAddress);
			}
		}

		#endregion

	}

}
