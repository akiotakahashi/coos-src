using System;
using System.IO;
using CooS.IO;

namespace CooS.Formats.CLI.Metadata.Heaps {

	public abstract class Heap {

		public readonly MetadataRoot Metadata;
		internal readonly HeapStream Stream;

		internal Heap(MetadataRoot mdroot, HeapStream stream) {
			this.Metadata = mdroot;
			this.Stream = stream;
		}

		public abstract int IndexSize {
			get;
		}

		public int HeapSize {
			get {
				return (int)this.Stream.Size;
			}
		}

		protected Stream OpenStream(long offset) {
			return this.Metadata.OpenStream(this.Stream.Offset+offset);
		}

		protected Stream OpenData(int rowIndex) {
			Stream stream = this.OpenStream(rowIndex);
			int length = ReadCompressedInt(stream);
			long start = stream.Position;
			return new BoundStream(stream, start, length);
		}

		public static int ReadCompressedInt(byte[] buf, ref int startIndex) {
			int fst = buf[startIndex++];
			if(fst<0) { throw new IOException("First byte is -1"); }
			if((fst&0x80)==0) {
				return fst;
			} else {
				int snd = buf[startIndex++];
				if(snd<0) { throw new IOException("Second byte is -1"); }
				if((fst&0x40)==0) {
					return snd | ((fst&0x3F)<<8);
				} else if((fst&0x20)==0) {
					int trd = buf[startIndex++];
					int fth = buf[startIndex++];
					if(trd<0) { throw new IOException("Third byte is -1"); }
					if(fth<0) { throw new IOException("Fourth byte is -1"); }
					return fth | (trd<<8) | (snd<<16) | ((fst&0x1F)<<24);
				} else {
					throw new FormatException("Compressed Int ‚ª³‚µ‚­‚ ‚è‚Ü‚¹‚ñB");
				}
			}
		}

		public static int ReadCompressedInt(Stream stream) {
			int fst = stream.ReadByte();
			if(fst<0) throw new IOException("First byte is -1/"+stream.Position);
			if((fst&0x80)==0) {
				return fst;
			} else {
				int snd = stream.ReadByte();
				if(snd<0) throw new IOException("Second byte is -1");
				if((fst&0x40)==0) {
					return snd | ((fst&0x3F)<<8);
				} else if((fst&0x20)==0) {
					int trd = stream.ReadByte();
					int fth = stream.ReadByte();
					if(trd<0) throw new IOException("Third byte is -1");
					if(fth<0) throw new IOException("Fourth byte is -1");
					return fth | (trd<<8) | (snd<<16) | ((fst&0x1F)<<24);
				} else {
					throw new FormatException("Compressed Int ‚ª³‚µ‚­‚ ‚è‚Ü‚¹‚ñB");
				}
			}
		}

	}
}
