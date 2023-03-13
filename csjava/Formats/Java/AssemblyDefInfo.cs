using System;
using System.Collections.Generic;
using AssemblyName=System.Reflection.AssemblyName;

namespace CooS.Formats.Java {
	using Metadata;

	public class AssemblyDefInfo : IDisposable {

		AssemblyName name = new AssemblyName();

		private readonly List<TypeDefInfo> types = new List<TypeDefInfo>();
		private readonly SortedList<int, ClassFile> cp_list = new SortedList<int, ClassFile>();
		private readonly SortedList<int, ClassFile> fieldlist = new SortedList<int, ClassFile>();
		private readonly SortedList<int, ClassFile> methodlist = new SortedList<int, ClassFile>();

		private readonly FieldDefInfo[] fields;
		private readonly MethodDefInfo[] methods;
		private readonly Dictionary<string,TypeDeclInfo> reftype = new Dictionary<string,TypeDeclInfo>();

		public AssemblyDefInfo(string name, ClassFile[] files) {
			this.name = new AssemblyName();
			this.name.Name = name;
			int offset_c = 0;
			int offset_f = 0;
			int offset_m = 0;
			foreach(ClassFile file in files) {
				types.Add(new TypeDefInfo(this, file, types.Count));
				this.cp_list.Add(offset_c, file);
				this.fieldlist.Add(offset_f, file);
				this.methodlist.Add(offset_m, file);
				offset_c += file.ConstantPool.Count;
				offset_f += file.FieldCollection.Count;
				offset_m += file.MethodCollection.Count;
			}
			this.fields = new FieldDefInfo[offset_f];
			this.methods = new MethodDefInfo[offset_m];
		}

		#region IDisposable ÉÅÉìÉo

		public void Dispose() {
			foreach(TypeDefInfo type in types) {
				type.Dispose();
			}
		}

		#endregion

		public string Name {
			get {
				return this.name.Name;
			}
		}

		public int TypeCount {
			get {
				return this.types.Count;
			}
		}

		public int FieldCount {
			get {
				return this.fields.Length;
			}
		}

		public int MethodCount {
			get {
				return this.methods.Length;
			}
		}

		public IEnumerable<TypeDefInfo> EnumTypes(bool exportedOnly) {
			foreach(TypeDefInfo type in this.types) {
				if(!exportedOnly || type.IsPublic) {
					yield return type;
				}
			}
		}

		public TypeDefInfo GetTypeInfo(int rowIndex) {
			return this.types[rowIndex];
		}

		internal FieldDefInfo GetFieldInfo(int rowIndex) {
			if(rowIndex<0) throw new ArgumentException();
			if(this.fields[rowIndex]!=null) {
				return this.fields[rowIndex];
			} else {
				int i = rowIndex;
				foreach(TypeDefInfo type in this.types) {
					if(i>=type.FieldCount) {
						i -= type.FieldCount;
					} else {
						return this.fields[rowIndex] = type.FieldCollection[i];
					}
				}
				throw new ArgumentException();
			}
		}

		public IEnumerable<FieldDefInfo> EnumFields() {
			for(int i=0; i<this.FieldCount; ++i) {
				yield return this.GetFieldInfo(i);
			}
		}

		internal MethodDefInfo GetMethodInfo(int rowIndex) {
			if(rowIndex<0)
				throw new ArgumentException();
			if(this.methods[rowIndex]!=null) {
				return this.methods[rowIndex];
			} else {
				int i = rowIndex;
				foreach(TypeDefInfo type in this.types) {
					if(i>=type.MethodCount) {
						i -= type.MethodCount;
					} else {
						return this.methods[rowIndex] = type.MethodCollection[i];
					}
				}
				throw new ArgumentException();
			}
		}

		public IEnumerable<MethodDefInfo> EnumMethods() {
			for(int i=0; i<this.MethodCount; ++i) {
				yield return this.GetMethodInfo(i);
			}
		}

		public TypeDefInfo FindType(string name, string ns) {
			throw new Exception("The method or operation is not implemented.");
		}

		internal TypeDeclInfo LoadType(string description) {
			if(this.reftype.ContainsKey(description)) {
				return this.reftype[description];
			} else {
				return this.reftype[description] = new TypeSpecInfo(this, description);
			}
		}

		internal TypeDeclInfo LoadType(CooS.Formats.Java.Description.FieldSig signature) {
			return this.LoadType(signature.ToDescription());
		}

	}

}
