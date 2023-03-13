using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class class_info : cp_info {
		
		public Utf8Index name_index;
		
		public class_info(BinaryReader reader) {
			this.name_index = reader.ReadUInt16();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.Class;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("Class: {0}", file[this.name_index]);
		}

	}

}
