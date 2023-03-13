/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
using System.Text;
using System.Collections;

namespace CooS.CodeModels.CLI.Metadata {

	// 23.1.3
	public class StringsHeap : Heap {

		public StringsHeap(MetadataRoot mdroot, MDStream stream) : base(mdroot, stream) {
		}

		public string this [int index] {
			get {
				if(index<0) throw new ArgumentOutOfRangeException(index.ToString());
				BinaryReader reader = new BinaryReader(OpenStream(index));
				int len = 0;
				while(reader.ReadByte()!=0) {
					++len;
				}
				reader.BaseStream.Seek(-len-1, SeekOrigin.Current);
				byte[] buf = reader.ReadBytes(len);
				return Encoding.UTF8.GetString(buf);
			}
		}

	}

}
