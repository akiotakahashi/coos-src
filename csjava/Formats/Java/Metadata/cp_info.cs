using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public abstract class cp_info {

		public abstract ConstantTag Tag {get;}

		public abstract void Dump(ClassFile file);

		public static cp_info Parse(BinaryReader reader) {
			ConstantTag tag = (ConstantTag)reader.ReadByte();
			switch(tag) {
			case ConstantTag.Class:
				return new class_info(reader);
			case ConstantTag.Fieldref:
				return new fieldref_info(reader);
			case ConstantTag.Methodref:
				return new methodref_info(reader);
			case ConstantTag.InterfaceMethodref:
				return new interfacemethodref_info(reader);
			case ConstantTag.String:
				return new string_info(reader);
			case ConstantTag.Integer:
				return new integer_info(reader);
			case ConstantTag.Float:
				return new float_info(reader);
			case ConstantTag.Long:
				return new long_info(reader);
			case ConstantTag.Double:
				return new double_info(reader);
			case ConstantTag.NameAndType:
				return new nameandtype_info(reader);
			case ConstantTag.Utf8:
				return new utf8_info(reader);
			default:
				throw new NotImplementedException("position="+reader.BaseStream.Position+", tag="+tag.ToString());
			}
		}

	}

}
