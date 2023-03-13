using System;
using System.IO;
using System.Collections;

namespace CooS.CodeModels.Java {
	using Metadata;
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class ClassFile {

		// By the specification
		u4 magic;
		u2 minor_version;
		u2 major_version;
		//u2 constant_pool_count;
		cp_info[] constant_pool;
		u2 access_flags;
		u2 this_class;
		u2 super_class;
		//u2 interfaces_count;
		u2[] interfaces;
		//u2 fields_count;
		field_info[] fields;
		//u2 methods_count;
		method_info[] methods;
		//u2 attributes_count;
		attribute_info[] attributes;

		// for reflection
		class_info[] classlist;
		
		public ClassFile(Stream stream) {
			BinaryReader reader;
			if(BitConverter.IsLittleEndian) {
				reader = new ReversiveReader(stream);
			} else {
				reader = new BinaryReader(stream);
			}
			this.magic = reader.ReadUInt32();
			if(this.magic!=0xCAFEBABE) throw new FormatException();
			this.minor_version = reader.ReadUInt16();
			this.major_version = reader.ReadUInt16();
			u2 constant_pool_count = reader.ReadUInt16();
			this.constant_pool = new cp_info[constant_pool_count];
			ArrayList list = new ArrayList();
			for(int i=1 /* this one is intended */; i<constant_pool_count; ++i) {
				//Console.WriteLine("#{0} [{2:X2}] is beginning at 0x{1:X}", i, reader.BaseStream.Position, (byte)reader.PeekChar());
				this.constant_pool[i] = cp_info.Parse(reader);
				switch(this.constant_pool[i].Tag) {
				case ConstantTag.Class:
					list.Add(this.constant_pool[i]);
					break;
				}
			}
			this.classlist = (class_info[])list.ToArray(typeof(class_info));
			this.access_flags = reader.ReadUInt16();
			this.this_class = reader.ReadUInt16();
			this.super_class = reader.ReadUInt16();
			u2 interfaces_count = reader.ReadUInt16();
			this.interfaces = new u2[interfaces_count];
			for(int i=0; i<interfaces_count; ++i) {
				this.interfaces[i] = reader.ReadUInt16();
			}
			u2 fields_count = reader.ReadUInt16();
			this.fields = new field_info[fields_count];
			for(int i=0; i<fields_count; ++i) {
				this.fields[i] = new field_info(this, reader);
			}
			u2 methods_count = reader.ReadUInt16();
			this.methods = new method_info[methods_count];
			for(int i=0; i<methods_count; ++i) {
				this.methods[i] = new method_info(this, reader);
			}
			this.attributes = JavaUtility.ReadAttributes(reader);
			Console.WriteLine("Analyzing completed having {0} bytes rest", reader.BaseStream.Length-reader.BaseStream.Position);
		}

		public string this[Utf8Index index] {
			get {
				return ((utf8_info)this.constant_pool[index]).Text;
			}
		}

		public void Dump() {
			foreach(cp_info cp in this.constant_pool) {
				if(cp!=null) {
					cp.Dump(this);
				}
			}
			foreach(method_info method in this.methods) {
				method.Dump();
			}
			Console.WriteLine("{0} entries in constant pool", this.constant_pool.Length-1);
		}

		public IList FieldCollection {
			get {
				return this.fields;
			}
		}

		public IList MethodCollection {
			get {
				return this.methods;
			}
		}

		public IList ConstantPool {
			get {
				return this.constant_pool;
			}
		}

	}

}
