using System;
using System.IO;

namespace CooS.CodeModels.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class nameandtype_info : cp_info {
		
		public Utf8Index name_index;
		public u2 descriptor_index;
		
		public nameandtype_info(BinaryReader reader) {
			this.name_index = reader.ReadUInt16();
			this.descriptor_index = reader.ReadUInt16();
		}

		public override ConstantTag Tag {
			get {
				return ConstantTag.NameAndType;
			}
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("NameAndType: name={0}, descidx={1}", file[this.name_index], this.descriptor_index);
		}

	}

}
