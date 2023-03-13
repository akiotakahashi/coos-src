using System;
using System.IO;

namespace CooS.CodeModels.Java {
	using Metadata;
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public sealed class JavaUtility {

		private JavaUtility() {
		}

		public static attribute_info[] ReadAttributes(BinaryReader reader) {
			u2 attributes_count = reader.ReadUInt16();
			attribute_info[] attributes = new attribute_info[attributes_count];
			for(int i=0; i<attributes_count; ++i) {
				attributes[i].Parse(reader);
			}
			return attributes;
		}

	}

}
