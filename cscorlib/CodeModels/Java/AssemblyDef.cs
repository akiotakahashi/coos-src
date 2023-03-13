using System;
using CooS.Reflection;
using System.Reflection;
using System.Collections;

namespace CooS.CodeModels.Java {
	using Metadata;

#if JAVA

	public class AssemblyDef : AssemblyBase {

		AssemblyName name = new AssemblyName();

		ArrayList files = new ArrayList();
		SortedList cp_list = new SortedList();
		SortedList fieldlist = new SortedList();
		SortedList methodlist = new SortedList();

		TypeDefInfo[] types = null;
		FieldDefInfo[] fields = null;
		MethodDefInfo[] methods = null;

		public AssemblyDef(string name, ClassFile[] files) {
			this.name = new AssemblyName();
			this.name.Name = name;
			this.files.AddRange(files);
			int offset_c = 0;
			int offset_f = 0;
			int offset_m = 0;
			foreach(ClassFile file in files) {
				this.cp_list.Add(offset_c, file);
				this.fieldlist.Add(offset_f, file);
				this.methodlist.Add(offset_m, file);
				offset_c += file.ConstantPool.Count;
				offset_f += file.FieldCollection.Count;
				offset_m += file.MethodCollection.Count;
			}
			this.types = new TypeDefInfo[files.Length];
			this.fields = new FieldDefInfo[offset_f];
			this.methods = new MethodDefInfo[offset_m];
		}

		public override System.Reflection.AssemblyName GetName(bool copiedName) {
			return this.name;
		}

		public override System.Reflection.MethodInfo EntryPoint {
			get {
				return null;
			}
		}

		public override string GetTypeName(int rowIndex) {
			if(this.types[rowIndex]!=null) {
				return this.types[rowIndex].Name;
			} else {
				throw new NotImplementedException();
				//return ((ClassFile)this.files[rowIndex]).Name;
			}
		}

		public override TypeImpl GetTypeInfo(int rowIndex) {
			if(this.types[rowIndex]!=null) {
				return this.types[rowIndex];
			} else {
				ClassFile file = (ClassFile)this.files[rowIndex];
				return this.types[rowIndex] = new TypeDefInfo(this, file);
			}
		}

		public override MethodInfoImpl GetMethodInfo(int rowIndex) {
			if(rowIndex<0) throw new ArgumentException();
			if(this.methods[rowIndex]!=null) {
				return this.methods[rowIndex];
			} else {
				foreach(ClassFile file in this.files) {
					if(rowIndex<file.MethodCollection.Count) {
						return this.methods[rowIndex] = new MethodDefInfo((method_info)file.MethodCollection[rowIndex]);
					}
					rowIndex -= file.MethodCollection.Count;
				}
				throw new ArgumentException();
			}
		}

		bool loaded_all_types = false;

		public override Type[] GetTypes(bool exportedOnly) {
			if(!this.loaded_all_types) {
				for(int i=0; i<this.types.Length; ++i) {
					this.GetTypeInfo(i);
				}
			}
			return this.types;
		}

		public override TypeImpl FindType(string fullname, bool throwOnMiss) {
			return null;
		}

	}

#endif

}
