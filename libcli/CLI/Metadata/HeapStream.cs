using System;
using System.IO;

namespace CooS.Formats.CLI.Metadata {

	struct HeapStream {

		public readonly uint Offset;
		public readonly uint Size;
		public readonly string Name;

		public HeapStream(MetadataRoot root, BinaryReader reader) {
			Offset = reader.ReadUInt32();
			Size = reader.ReadUInt32();
			Name = CooS.Formats.DLL.ImageUtility.ReadString(reader, 16);
			root.AlignPosition(reader);
			if(Name==null || Name.Length==0) { throw new BadImageException(); }
		}

	}

}
