/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;

namespace CooS.CodeModels.CLI.Metadata {

	/// <summary>
	/// #GUID heap
	/// </summary>
	/// <remarks>
	/// 23.1.5
	/// </remarks>
	public class GuidHeap : Heap {

		internal GuidHeap(MetadataRoot mdroot, MDStream stream) : base(mdroot,stream) {
		}

		public Guid this[int index] {
			get {
				if(index+16 > this.Stream.Size) throw new IndexOutOfRangeException();
				BinaryReader reader = new BinaryReader(this.OpenStream(index));
				return new Guid(reader.ReadBytes(16));
			}
		}

	}

}