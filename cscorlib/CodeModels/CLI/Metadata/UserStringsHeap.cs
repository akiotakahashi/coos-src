using System;
using System.IO;
using System.Text;
using System.Collections;

namespace CooS.CodeModels.CLI.Metadata {

	public class UserStringsHeap : Heap {

		public UserStringsHeap(MetadataRoot mdroot, MDStream stream) : base(mdroot,stream) {
		}

		public string this [int index] {
			get {
				if(index<0) throw new ArgumentOutOfRangeException(index.ToString());
				BinaryReader reader = new BinaryReader(this.OpenData(index));
				byte[] buf = reader.ReadBytes((int)reader.BaseStream.Length);
				return Encoding.Unicode.GetString(buf);
			}
		}

	}

}
