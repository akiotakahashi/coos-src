using System;
using CooS.Collections;
using System.Collections.Generic;

namespace CooS.Formats.Java {
	using Metadata;

	public class TypeDefInfo : TypeDeclInfo, IDisposable {

		private readonly ClassFile file;
		private readonly int index;
		private readonly FieldDefInfo[] fields;
		private readonly MethodDefInfo[] methods;

		public TypeDefInfo(AssemblyDefInfo assembly, ClassFile file, int index) : base(assembly) {
			this.file = file;
			this.index = index;
			this.fields = new FieldDefInfo[file.FieldCollection.Count];
			this.methods = new MethodDefInfo[file.MethodCollection.Count];
		}

		#region IDisposable メンバ

		public void Dispose() {
			file.Dispose();
		}

		#endregion

		public int Index {
			get {
				return this.index;
			}
		}

		public override string Name {
			get {
				return this.file.Name;
			}
		}

		public override string Namespace {
			get {
				return this.file.Namespace;
			}
		}

		public TypeDefInfo BaseType {
			get {
				throw new NotImplementedException();
			}
		}

		public TypeDefInfo EnclosingType {
			get {
				return null;
			}
		}

		public bool IsInterface {
			get {
				return false;
			}
		}

		public bool IsAbstract {
			get {
				return this.file.IsAbstract;
			}
		}

		public bool IsPublic {
			get {
				return true;
			}
		}

		public bool IsSealed {
			get {
				return this.file.IsSealed;
			}
		}

		public bool IsNested {
			get {
				return this.file.IsNested;
			}
		}

		public bool IsGenericType {
			get {
				return this.file.IsGenericType;
			}
		}

		public bool IsGenericParam {
			get {
				return false;
			}
		}

		public IList<cp_info> ConstantPool {
			get {
				return this.file.ConstantPool;
			}
		}

		public int FieldCount {
			get {
				return this.file.FieldCollection.Count;
			}
		}

		public int MethodCount {
			get {
				return this.file.MethodCollection.Count;
			}
		}

		private FieldDefInfo GetFieldInfo(int rowIndex) {
			if(this.fields[rowIndex]!=null) {
				return this.fields[rowIndex];
			} else {
				return this.fields[rowIndex] = new FieldDefInfo(this, this.file.FieldCollection[rowIndex], rowIndex);
			}
		}

		private MethodDefInfo GetMethodInfo(int rowIndex) {
			if(this.methods[rowIndex]!=null) {
				return this.methods[rowIndex];
			} else {
				return this.methods[rowIndex] = new MethodDefInfo(this, this.file.MethodCollection[rowIndex], rowIndex);
			}
		}

		#region メンバコレクション

		public MemberCollection<FieldDefInfo, TypeDefInfo> FieldCollection {
			get {
				return new MemberCollection<FieldDefInfo, TypeDefInfo>(this,
					0, this.FieldCount, delegate(TypeDefInfo typedef, int rowIndex)
				{
					return typedef.GetFieldInfo(rowIndex);
				});
			}
		}

		public MemberCollection<MethodDefInfo, TypeDefInfo> MethodCollection {
			get {
				return new MemberCollection<MethodDefInfo, TypeDefInfo>(this,
					0, this.MethodCount, delegate(TypeDefInfo typedef, int rowIndex)
				{
					return typedef.GetMethodInfo(rowIndex);
				});
			}
		}

		#endregion

		internal string LoadString(Utf8Index index) {
			return this.file[index];
		}

		internal string LoadString(string_info info) {
			return this.file[info.string_index];
		}

	}

}
