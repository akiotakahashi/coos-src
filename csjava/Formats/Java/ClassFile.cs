using System;
using System.IO;
using System.Collections.Generic;

namespace CooS.Formats.Java {
	using Metadata;
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public class ClassFile : IDisposable {

		// By the specification
		private readonly u4 magic;
		private readonly u2 minor_version;
		private readonly u2 major_version;
		//u2 constant_pool_count;
		private readonly cp_info[] constant_pool;
		private readonly u2 access_flags;
		private readonly u2 this_class;
		private readonly u2 super_class;
		//u2 interfaces_count;
		private readonly u2[] interfaces;
		//u2 fields_count;
		private readonly field_info[] fields;
		//u2 methods_count;
		private readonly method_info[] methods;
		//u2 attributes_count;
		private readonly attribute_info[] attributes;

		// for reflection
		private readonly class_info[] classlist;
		private readonly string name;
		private readonly string ns;
		
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
			List<class_info> list = new List<class_info>();
			// Beginnig from 1 is intended because the first one cp is invalid (tag=0).
			for(int i=1; i<constant_pool_count; ++i) {
				this.constant_pool[i] = cp_info.Parse(reader);
				switch(this.constant_pool[i].Tag) {
				case ConstantTag.Class:
					list.Add((class_info)this.constant_pool[i]);
					break;
				}
			}
			this.classlist = list.ToArray();
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
			// validation
			if(reader.BaseStream.Length-reader.BaseStream.Position>0) {
				Console.Error.WriteLine("Analyzing completed but {0} bytes rest.", reader.BaseStream.Length-reader.BaseStream.Position);
			}
			if(this.classlist.Length==0) {
				Console.Error.WriteLine("ConstantPool has no class_info to identify this class.");
			} else {
				string fullname = this[this.classlist[0].name_index];
				int i = fullname.LastIndexOf('.');
				if(i<0) {
					this.name = fullname;
					this.ns = null;
				} else {
					this.name = fullname.Substring(i+1);
					this.ns = fullname.Substring(0,i);
				}
			}
		}

		#region IDisposable ƒƒ“ƒo

		public void Dispose() {
			// NOP
		}

		#endregion

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

		public string Name {
			get {
				return this.name;
			}
		}

		public string Namespace {
			get {
				return this.ns;
			}
		}

		public bool IsAbstract {
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsSealed {
			get {
				return false;
			}
		}

		public bool IsNested {
			get {
				return false;
			}
		}

		public bool IsGenericType {
			get {
				return false;
			}
		}

		public string this[Utf8Index index] {
			get {
				return ((utf8_info)this.constant_pool[index]).Text;
			}
		}

		public IList<cp_info> ConstantPool {
			get {
				return this.constant_pool;
			}
		}

		public IList<field_info> FieldCollection {
			get {
				return this.fields;
			}
		}

		public IList<method_info> MethodCollection {
			get {
				return this.methods;
			}
		}

		public IEnumerable<class_info> EnumClassInfo() {
			foreach(class_info cls in this.classlist) {
				yield return cls;
			}
		}

	}

}
