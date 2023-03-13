using System;
using System.IO;
using CooS.Formats.CLI.Signature;

namespace CooS.Formats.CLI.Metadata.Heaps {

	public sealed class BlobHeap : Heap {

		private byte[] data;
		private MemoryStream ms;

		internal BlobHeap(MetadataRoot mdroot, HeapStream stream) : base(mdroot, stream) {
			this.data = new byte[stream.Size];
			this.OpenStream(0).Read(this.data, 0, (int)stream.Size);
			this.ms = new MemoryStream(this.data);
		}

		public override int IndexSize {
			get {
				return this.Metadata.BlobIndexSize;
			}
		}

		public Stream this[BlobHeapIndex index] {
			get {
				int pos = index.RawIndex;
				int length = ReadCompressedInt(this.data, ref pos);
				return new MemoryStream(this.data, pos, length);
			}
		}

		public object ReadSignature(BlobHeapIndex index, SignatureFactory factory) {
			using(SignatureReader reader = new SignatureReader(this[index])) {
				return factory.Parse(reader);
			}
		}

	}

}
