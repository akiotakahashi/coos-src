using System;
using System.IO;

namespace CooS.Formats.CLI.Metadata {

	public class MethodIL {

		[Flags]
		private enum FormatFlags : ushort {
			Tiny=2,
			Fat=3,
			TinyMask=0x3,
			MoreSections=0x8,
			InitLocals=0x10,
			FatMask=0xFFF,
		}

		private readonly FormatFlags FormatFlag;
		public readonly int MaxStack;
		public readonly byte[] ByteCode;
		public readonly byte[] ExtraData;

		private static bool IsMethodTiny(FormatFlags flag) {
			return (flag&FormatFlags.TinyMask) == FormatFlags.Tiny;
		}

		internal MethodIL(BinaryReader reader) {
			this.FormatFlag = (FormatFlags)reader.ReadByte();
			if (IsMethodTiny(this.FormatFlag)) {
				int codeSize = (int)this.FormatFlag >> 2;
				this.MaxStack = 0;
				this.ByteCode = reader.ReadBytes(codeSize);
			} else {
				long p0 = reader.BaseStream.Position - 1;
				byte a = reader.ReadByte();
				this.FormatFlag |= FormatFlags.FatMask&(FormatFlags)((int)a << 8);
				this.MaxStack = reader.ReadInt16();
				int codeSize = reader.ReadInt32();
				int localTok = reader.ReadInt32();
				int datasize = ((a >> 4) & 0xF) << 2;
				this.ByteCode = reader.ReadBytes(codeSize);
				reader.BaseStream.Position += 3;
				reader.BaseStream.Position &= ~3;
				this.ExtraData = reader.ReadBytes(datasize);
#if false
				if(0!=(codeFlags&CodeFlags.MoreSects)) {
					this.extraSections = new List<MethodSection>();
					using(BinaryReader reader = new BinaryReader(assembly.OpenMethod(row))) {
						reader.BaseStream.Seek(this.codeOffset+this.codeSize, SeekOrigin.Current);
						long position = reader.BaseStream.Position;
						if(position%4 >0) {
							int diff = 4-(int)(position%4);
							reader.BaseStream.Seek(diff, SeekOrigin.Current);
						}
						MethodSectionFlags sectFlags;
						do {
							sectFlags = (MethodSectionFlags)reader.ReadByte();
							if(0!=(~0xC3&(int)sectFlags)) throw new BadILException(this.FullName);
							extraSections.Add(new MethodSection(sectFlags,position));
							int length;
							if(0!=(sectFlags&MethodSectionFlags.FatFormat)) {
								int fst = reader.ReadByte();
								int snd = reader.ReadByte();
								int trd = reader.ReadByte();
								length = (fst<<16) | (snd<<8) | trd;
								length -= 4;
							} else {
								length = reader.ReadByte();
								length -= 2;
							}
							reader.BaseStream.Seek(length, SeekOrigin.Current);
						} while(0!=(sectFlags&MethodSectionFlags.MoreSects));
					}
				}
#endif
			}
		}

		public int CodeSize {
			get {
				return (this.ByteCode!=null) ? this.ByteCode.Length : 0;
			}
		}

		public bool InitLocals {
			get {
				return (this.FormatFlag & FormatFlags.InitLocals) != 0;
			}
		}

		public bool HasMoreSections {
			get {
				return (this.FormatFlag & FormatFlags.MoreSections) != 0;
			}
		}

	}

}
