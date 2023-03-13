using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using CooS.Formats.CLI;

namespace CooS.Manipulation.CLI {

	class ILStream {

		static readonly OpCode[] OpValues = new OpCode[256];
		static readonly Hashtable TailValues = new Hashtable();
		readonly AssemblyDefInfo assembly;
		readonly BinaryReader reader;
		readonly long initpos;

		static ILStream() {
			Console.WriteLine("Constructing ILStream");
			foreach(FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static)) {
				if(fi.FieldType!=typeof(OpCode)) continue;
				OpCode opcode = (OpCode)fi.GetValue(null);
				if(opcode.Size==1) {
					OpValues[opcode.Value] = opcode;
				} else if(opcode.Size==2) {
					int fst = (byte)(opcode.Value >> 8);
					int snd = opcode.Value & 0xFF;
					if(!TailValues.ContainsKey(fst)) {
						TailValues[fst] = new OpCode[256];
					}
					((OpCode[])TailValues[fst])[snd] = opcode;
				} else {
					throw new InvalidProgramException(string.Format("{0}'s size is {1}",opcode.Name,opcode.Size));
				}
			}
		}

		public ILStream(AssemblyDefInfo assembly, Stream stream) {
			this.assembly = assembly;
			this.reader = new BinaryReader(stream);
			this.initpos = stream.Position;
		}

		public void Dump() {
			while(this.reader.BaseStream.Position<this.reader.BaseStream.Length) {
				Console.Write("{0:X2} ",this.reader.BaseStream.ReadByte());
			}
			Console.WriteLine();
		}

		public void Close() {
			this.reader.Close();
		}

		public bool AtEndOfStream {
			get {
				return this.reader.BaseStream.Position>=this.reader.BaseStream.Length;
			}
		}

		public Instruction Read() {
			int pos = (int)(this.reader.BaseStream.Position-this.initpos);
			InstructionPrefixes prefixes = InstructionPrefixes.None;
			OpCode opcode;
			for(;;) {
				int fst = this.reader.ReadByte();
				if(fst<0) throw new IOException();
				opcode = OpValues[fst];
				if(opcode.Value==0xFE) {
					OpCode[] tails = (OpCode[])TailValues[fst];
					if(tails==null) throw new BadILException(fst.ToString("X2")+" is invalid code");
					int snd = this.reader.ReadByte();
					if(snd<0) throw new IOException();
					opcode = tails[snd];
				}
				// プリフィックスの場合は先読み
				if(opcode.Value==OpCodes.Volatile.Value) {
					//TODO: Enable volatile prefix.
					prefixes |= InstructionPrefixes.Volatile;
				} else if(opcode.Value==OpCodes.Tailcall.Value) {
					//TODO: Enable tailcall prefix.
					prefixes |= InstructionPrefixes.Tailcall;
				} else if(opcode.Value==OpCodes.Unaligned.Value) {
					//TODO: Enable unaligned prefix.
					prefixes |= InstructionPrefixes.Unaligned;
				} else {
					break;
				}
			}
			// オペランドを確定
			object operand;
			switch(opcode.OperandType) {
			case OperandType.InlineBrTarget:
				//InlineBrTarget オペランドは 32 ビット整数の分岐のターゲットです。 
				operand = this.reader.ReadInt32();
				break;
			case OperandType.InlineField:
			case OperandType.InlineMethod:
			case OperandType.InlineType:
				//オペランドは 32 ビット メタデータ トークンです。 
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineI:
				//オペランドは 32 ビット整数です。 
				operand = this.reader.ReadInt32();
				break;
			case OperandType.InlineI8:
				//オペランドは 64 ビット整数です。
				operand = this.reader.ReadInt64();
				break;
			case OperandType.InlineNone:
				//オペランドなし。 
				operand = null;
				break;
			case OperandType.InlinePhi:
				//このメンバは、.NET Framework インフラストラクチャのサポートを目的としています。
				//独自に作成したコード内で直接使用することはできません。 
				throw new NotSupportedException("OperandType.InlinePhi");
			case OperandType.InlineR:
				//オペランドは 64 ビット IEEE 浮動小数点数です。 
				operand = this.reader.ReadDouble();
				break;
			case OperandType.InlineSig:
				//オペランドは 32 ビット メタデータのシグネチャ トークンです。
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineString:
				//オペランドは 32 ビット メタデータの文字列トークンです。 
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineSwitch:
				//オペランドは switch 命令の 32 ビット整数引数です。 
				uint n = this.reader.ReadUInt32();
				int[] targets = new int[n];
				for(int i=0; i<n; ++i) {
					targets[i] = this.reader.ReadInt32();
				}
				operand = targets;
				break;
			case OperandType.InlineTok:
				//オペランドは FieldRef 、 MethodRef 、または TypeRef のトークンです。 
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineVar:
				//オペランドは、ローカル変数または引数の序数を含んだ 16 ビット整数です。 
				operand = this.reader.ReadUInt16();
				break;
			case OperandType.ShortInlineBrTarget:
				//オペランドは 8 ビット整数の分岐のターゲットです。
				operand = this.reader.ReadSByte();
				break;
			case OperandType.ShortInlineI:
				//オペランドは 8/16 ビット整数です。 
				//（説明文ではなくidc.i4.s命令仕様書を確認のこと。）
				if(opcode.Value==OpCodes.Ldc_I4_S.Value) {
					operand = this.reader.ReadByte();
				} else {
					operand = this.reader.ReadInt16();
				}
				break;
			case OperandType.ShortInlineR:
				//オペランドは 32 ビット IEEE 浮動小数点数です。 
				operand = this.reader.ReadSingle();
				break;
			case OperandType.ShortInlineVar:
				//オペランドは、ローカル変数または引数の序数を含んだ 8 ビット整数です。 
				operand = this.reader.ReadByte();
				break;
			default:
				throw new NotSupportedException(opcode.OperandType.ToString());
			}
			return new Instruction(this.assembly, pos, opcode, operand, prefixes);
		}

	}

}
