using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class field_info : member_info {

		public field_info(ClassFile file, BinaryReader reader) : base(file, reader) {
		}

	}

}
