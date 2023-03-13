using System;
using System.IO;
using System.Text;

namespace CooS.Formats.CLI.Metadata.Heaps {

	public sealed class UserStringsHeap : Heap {

		internal UserStringsHeap(MetadataRoot mdroot, HeapStream stream) : base(mdroot,stream) {
		}

		public string this [UserStringHeapIndex index] {
			get {
				if(index.RawIndex<0) throw new ArgumentOutOfRangeException(index.ToString());
				BinaryReader reader = new BinaryReader(this.OpenData(index.RawIndex));
				byte[] buf = reader.ReadBytes((int)reader.BaseStream.Length);
				return Encoding.Unicode.GetString(buf);
			}
		}

		public override int IndexSize {
			get {
				throw new NotImplementedException();
			}
		}

	}

}
