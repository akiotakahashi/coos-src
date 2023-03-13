using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Manipulation {

	class ILStream {

		static readonly OpCode[] OpValues = new OpCode[256];
		static readonly Hashtable TailValues = new Hashtable();
		readonly AssemblyDef assembly;
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

		public ILStream(AssemblyDef assembly, Stream stream) {
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
				// �v���t�B�b�N�X�̏ꍇ�͐�ǂ�
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
			// �I�y�����h���m��
			object operand;
			switch(opcode.OperandType) {
			case OperandType.InlineBrTarget:
				//InlineBrTarget �I�y�����h�� 32 �r�b�g�����̕���̃^�[�Q�b�g�ł��B 
				operand = this.reader.ReadInt32();
				break;
			case OperandType.InlineField:
			case OperandType.InlineMethod:
			case OperandType.InlineType:
				//�I�y�����h�� 32 �r�b�g ���^�f�[�^ �g�[�N���ł��B 
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineI:
				//�I�y�����h�� 32 �r�b�g�����ł��B 
				operand = this.reader.ReadInt32();
				break;
			case OperandType.InlineI8:
				//�I�y�����h�� 64 �r�b�g�����ł��B
				operand = this.reader.ReadInt64();
				break;
			case OperandType.InlineNone:
				//�I�y�����h�Ȃ��B 
				operand = null;
				break;
			case OperandType.InlinePhi:
				//���̃����o�́A.NET Framework �C���t���X�g���N�`���̃T�|�[�g��ړI�Ƃ��Ă��܂��B
				//�Ǝ��ɍ쐬�����R�[�h���Œ��ڎg�p���邱�Ƃ͂ł��܂���B 
				throw new NotSupportedException("OperandType.InlinePhi");
			case OperandType.InlineR:
				//�I�y�����h�� 64 �r�b�g IEEE ���������_���ł��B 
				operand = this.reader.ReadDouble();
				break;
			case OperandType.InlineSig:
				//�I�y�����h�� 32 �r�b�g ���^�f�[�^�̃V�O�l�`�� �g�[�N���ł��B
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineString:
				//�I�y�����h�� 32 �r�b�g ���^�f�[�^�̕�����g�[�N���ł��B 
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineSwitch:
				//�I�y�����h�� switch ���߂� 32 �r�b�g���������ł��B 
				uint n = this.reader.ReadUInt32();
				int[] targets = new int[n];
				for(int i=0; i<n; ++i) {
					targets[i] = this.reader.ReadInt32();
				}
				operand = targets;
				break;
			case OperandType.InlineTok:
				//�I�y�����h�� FieldRef �A MethodRef �A�܂��� TypeRef �̃g�[�N���ł��B 
				operand = (MDToken)this.reader.ReadUInt32();
				break;
			case OperandType.InlineVar:
				//�I�y�����h�́A���[�J���ϐ��܂��͈����̏������܂� 16 �r�b�g�����ł��B 
				operand = this.reader.ReadUInt16();
				break;
			case OperandType.ShortInlineBrTarget:
				//�I�y�����h�� 8 �r�b�g�����̕���̃^�[�Q�b�g�ł��B
				operand = this.reader.ReadSByte();
				break;
			case OperandType.ShortInlineI:
				//�I�y�����h�� 8/16 �r�b�g�����ł��B 
				//�i�������ł͂Ȃ�idc.i4.s���ߎd�l�����m�F�̂��ƁB�j
				if(opcode.Value==OpCodes.Ldc_I4_S.Value) {
					operand = this.reader.ReadByte();
				} else {
					operand = this.reader.ReadInt16();
				}
				break;
			case OperandType.ShortInlineR:
				//�I�y�����h�� 32 �r�b�g IEEE ���������_���ł��B 
				operand = this.reader.ReadSingle();
				break;
			case OperandType.ShortInlineVar:
				//�I�y�����h�́A���[�J���ϐ��܂��͈����̏������܂� 8 �r�b�g�����ł��B 
				operand = this.reader.ReadByte();
				break;
			default:
				throw new NotSupportedException(opcode.OperandType.ToString());
			}
			return new Instruction(this.assembly, pos, opcode, operand, prefixes);
		}

	}

}
