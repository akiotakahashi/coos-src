using System;
using System.IO;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	public class SignatureReader : BinaryReader {

		bool prediction;
		byte pred;
		long pos0;

		public SignatureReader(Stream stream) : base(stream) {
			prediction = false;
			pred = 255;
			pos0 = stream.Position;
		}

		public override string ToString() {
			return "SignatureReader/"+base.BaseStream.ToString();
		}

		public void Dump() {
			long p = this.BaseStream.Position;
			this.BaseStream.Position = pos0;
			while(this.BaseStream.Position<this.BaseStream.Length) {
				Console.Write("{0:X2} ",this.BaseStream.ReadByte());
				if(p==this.BaseStream.Position) Console.Write(">> ");
			}
			Console.WriteLine();
		}

		public ElementType GetMark() {
			if(!this.prediction) {
				this.pred = ReadByte();
				this.prediction = true;
			}
			return (ElementType)this.pred;
		}

		public ElementType ReadMark() {
			ElementType mark = GetMark();
			this.prediction = false;
			return mark;
		}

		public void ConfirmMark(ElementType mark) {
			if(ReadMark()!=mark) throw new BadSignatureException("Position is 0x"+this.BaseStream.Position.ToString("X"));
		}

		public override int ReadInt32() {
			return BlobHeap.ReadCompressedInt(this.BaseStream);
		}

		public override uint ReadUInt32() {
			return (uint)BlobHeap.ReadCompressedInt(this.BaseStream);
		}

		public override short ReadInt16() {
			throw new NotSupportedException();
		}

		public override ushort ReadUInt16() {
			throw new NotSupportedException();
		}

		public override long ReadInt64() {
			throw new NotSupportedException();
		}

		public override ulong ReadUInt64() {
			throw new NotSupportedException();
		}

	}

}
