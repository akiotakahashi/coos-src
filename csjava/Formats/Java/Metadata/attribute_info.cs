using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public struct attribute_info {

		public Utf8Index attribute_name_index;
		//public u4 attribute_length;
		public u1[] info;
		
		public void Parse(BinaryReader reader) {
			this.attribute_name_index = reader.ReadUInt16();
			u4 attribute_length = reader.ReadUInt32();
			this.info = reader.ReadBytes((int)attribute_length);
		}

	}

}
