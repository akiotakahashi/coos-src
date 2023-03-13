using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public abstract class memberref_info : cp_info {
		
		public u2 class_index;
		public u2 name_and_type_index;

		public memberref_info(BinaryReader reader) {
			this.class_index = reader.ReadUInt16();
			this.name_and_type_index = reader.ReadUInt16();
		}

		public override void Dump(ClassFile file) {
			Console.WriteLine("{0}: clsidx={1}, name&type_idx={2}"
				, this.GetType().Name
				, this.class_index
				, this.name_and_type_index);
		}


	}

}
