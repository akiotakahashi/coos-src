using System;
using System.IO;
using System.Collections;

namespace CooS.CodeModels.Java.Metadata {
	using u1 = Byte;
	using u2 = UInt16;
	using u4 = UInt32;

	public abstract class member_info {
		
		public readonly ClassFile File;

		// by the specification
		public u2 access_flags;
		public Utf8Index name_index;
		public u2 descriptor_index;
		//public u2 attributes_count;
		public attribute_info[] attributes;

		// for reflection
		public Hashtable attrtable = null;

		public member_info(ClassFile file, BinaryReader reader) {
			this.File = file;
			this.access_flags = reader.ReadUInt16();
			this.name_index = reader.ReadUInt16();
			this.descriptor_index = reader.ReadUInt16();
			this.attributes = JavaUtility.ReadAttributes(reader);
		}

		public virtual void Dump() {
			Console.WriteLine("{0}: name = {1}", this.GetType().Name, this.File[this.name_index]);
			Console.WriteLine("   access_flags = {0}", this.access_flags);
			Console.WriteLine("   descriptor_index = {0}", this.descriptor_index);
		}

		public attribute_info this[string name] {
			get {
				if(this.attrtable==null) {
					this.attrtable = new Hashtable();
					foreach(attribute_info attr in this.attributes) {
						this.attrtable[this.File[attr.attribute_name_index]] = attr;
					}
				}
				return (attribute_info)this.attrtable[name];
			}
		}

	}

}
