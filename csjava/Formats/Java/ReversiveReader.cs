using System;
using System.IO;

namespace CooS.Formats.Java {

	public class ReversiveReader : BinaryReader {

		public ReversiveReader(Stream stream) : base(stream) {
		}

		public override ushort ReadUInt16() {
			byte v1 = base.ReadByte();
			byte v2 = base.ReadByte();
			return (ushort)((v1<<8) | v2);
		}

		public override uint ReadUInt32() {
			ushort v1 = this.ReadUInt16();
			ushort v2 = this.ReadUInt16();
			return (uint)((v1<<16) | v2);
		}

		public override ulong ReadUInt64() {
			uint v1 = this.ReadUInt32();
			uint v2 = this.ReadUInt32();
			return ((ulong)v1<<32) | v2;
		}

		public override char ReadChar() {
			return (char)this.ReadUInt16();
		}

		public override short ReadInt16() {
			return (short)this.ReadUInt16();
		}

		public override int ReadInt32() {
			return (int)this.ReadUInt32();
		}

		public override long ReadInt64() {
			return (long)this.ReadUInt64();
		}

	}

}
