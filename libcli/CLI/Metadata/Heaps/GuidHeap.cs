using System;
using System.IO;

namespace CooS.Formats.CLI.Metadata.Heaps {

	public sealed class GuidHeap : Heap {

		internal GuidHeap(MetadataRoot mdroot, HeapStream stream) : base(mdroot, stream) {
		}

		public override int IndexSize {
			get {
				return this.Metadata.GuidIndexSize;
			}
		}

		public Guid this[GuidHeapIndex index] {
			get {
				if(index.RawIndex+16 > this.Stream.Size) { throw new IndexOutOfRangeException(); }
				BinaryReader reader = new BinaryReader(this.OpenStream(index.RawIndex));
				return new Guid(reader.ReadBytes(16));
			}
		}

	}

}
