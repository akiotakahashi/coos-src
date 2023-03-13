using System;
using System.IO;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Signature {

	internal class SignatureReader : BinaryReader {

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

		public void Dump(TextWriter writer) {
			long p = this.BaseStream.Position;
			this.BaseStream.Position = pos0;
			while(this.BaseStream.Position<this.BaseStream.Length) {
				writer.Write("{0:X2} ", this.BaseStream.ReadByte());
				if(p==this.BaseStream.Position) {
					writer.Write(">> ");
				}
			}
			writer.WriteLine();
		}

		public override byte ReadByte() {
			if(this.prediction) {
				this.prediction = false;
				return this.pred;
			} else {
				return base.ReadByte();
			}
		}

		public ElementType PeekMark() {
			if(!this.prediction) {
				this.pred = base.ReadByte();
				this.prediction = true;
			}
			return (ElementType)this.pred;
		}

		public ElementType ReadMark() {
			return (ElementType)this.ReadByte();
		}

		public void ConfirmMark(ElementType mark) {
			if(ReadMark()!=mark) throw new BadSignatureException("Position is 0x"+this.BaseStream.Position.ToString("X"));
		}

		public override int ReadInt32() {
			if(this.prediction) { throw new InvalidOperationException(); }
			return BlobHeap.ReadCompressedInt(this.BaseStream);
		}

		public override uint ReadUInt32() {
			if(this.prediction) { throw new InvalidOperationException(); }
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
