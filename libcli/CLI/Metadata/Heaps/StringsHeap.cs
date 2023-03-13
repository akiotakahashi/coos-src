using System;
using System.IO;
using System.Text;

namespace CooS.Formats.CLI.Metadata.Heaps {

	public sealed class StringsHeap : Heap {

		internal StringsHeap(MetadataRoot mdroot, HeapStream stream) : base(mdroot,stream) {
		}

		public string this [StringHeapIndex index] {
			get {
				if(index.RawIndex<0) { throw new ArgumentOutOfRangeException(index.ToString()); }
				BinaryReader reader = new BinaryReader(this.OpenStream(index.RawIndex));
				int len = 0;
				while(reader.ReadByte()!=0) {
					++len;
				}
				reader.BaseStream.Seek(-len-1, SeekOrigin.Current);
				byte[] buf = reader.ReadBytes(len);
				return Encoding.UTF8.GetString(buf);
			}
		}

		public override int IndexSize {
			get {
				return this.Metadata.StringsIndexSize;
			}
		}

	}

}
