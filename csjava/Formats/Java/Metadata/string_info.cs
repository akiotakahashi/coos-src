using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class string_info : cp_info {
		
		public Utf8Index string_index;

		public string_info(BinaryReader reader) {
			this.string_index = reader.ReadUInt16();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.String;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("String: {0}", file[this.string_index]);
		}

	}

}
