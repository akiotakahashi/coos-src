using System;
using System.IO;

namespace CooS.Formats.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public struct exception_table {

		public u2 start_pc;
		public u2 end_pc;
		public u2 handler_pc;
		public u2 catch_type;

		public void Parse(BinaryReader reader) {
			this.start_pc = reader.ReadUInt16();
			this.end_pc = reader.ReadUInt16();
			this.handler_pc = reader.ReadUInt16();
			this.catch_type = reader.ReadUInt16();
		}

	}

	public class base_attribute {

		public readonly attribute_info Attribute;

		public base_attribute(attribute_info attr) {
			this.Attribute = attr;
		}

		public virtual void Dump(ClassFile file) {
			Console.WriteLine("{0}: {1} bytes", this.GetType().Name, this.Attribute.info.Length);
		}

		protected BinaryReader CreateReader() {
			if(BitConverter.IsLittleEndian) {
				return new ReversiveReader(new MemoryStream(this.Attribute.info));
			} else {
				return new BinaryReader(new MemoryStream(this.Attribute.info));
			}
		}

	}

	/// <summary>
	/// code_attribute ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
	/// </summary>
	public class code_attribute : base_attribute {

		u2 max_stack;
		u2 max_locals;
		//u4 code_length;
		u1[] code;
		//u2 exception_table_length;
		exception_table[] exception_table;
		//u2 attributes_count;
		attribute_info[] attributes;

		public code_attribute(attribute_info attr) : base(attr) {
			BinaryReader reader = this.CreateReader();
			this.max_stack = reader.ReadUInt16();
			this.max_locals = reader.ReadUInt16();
			u4 code_length = reader.ReadUInt32();
			this.code = reader.ReadBytes((int)code_length);
			u2 exception_table_length = reader.ReadUInt16();
			this.exception_table = new exception_table[exception_table_length];
			for(int i=0; i<exception_table_length; ++i) {
				this.exception_table[i].Parse(reader);
			}
			this.attributes = JavaUtility.ReadAttributes(reader);
		}

		public override void Dump(ClassFile file) {
			base.Dump(file);
			Console.WriteLine("Code: maxstack={0}, maxlocals={1}", this.max_stack, this.max_locals);
			Utility.Dump(Console.Out, this.code);
		}

	}

}
