using System;
using System.Collections.Generic;
using TypeAttributes=System.Reflection.TypeAttributes;

namespace CooS.Formats.CLI {
	using Metadata;
	using Metadata.Rows;

	sealed class TypeDefInfo : TypeDeclInfo, IMemberRefParent {

		private TypeDefRow row;

		internal TypeDefInfo(AssemblyDefInfo assembly, TypeDefRow row) : base(assembly) {
			this.row = row;
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		public int Index {
			get {
				return this.row.Index-1;
			}
		}

		public override string Name {
			get {
				return this.Assembly.LoadBlobString(this.row.TypeName);
			}
		}

		public override string Namespace {
			get {
				return this.Assembly.LoadBlobString(this.row.TypeNamespace);
			}
		}

		public override bool IsNested {
			get {
				return TypeUtility.IsNestedFlag(row.Flags);
			}
		}

		public bool IsInterface {
			get {
				return 0!=(row.Flags&TypeAttributes.Interface);
			}
		}

		public bool IsAbstract {
			get {
				return 0!=(row.Flags&TypeAttributes.Abstract);
			}
		}

		public bool IsSealed {
			get {
				return 0!=(row.Flags&TypeAttributes.Sealed);
			}
		}

		public TypeDeclInfo BaseType {
			get {
				return this.Assembly.GetType(this.row.Extends);
			}
		}

		public int FieldCount {
			get {
				return this.Assembly.GetFieldCount(this.RowIndex);
			}
		}

		public int MethodCount {
			get {
				return this.Assembly.GetMethodCount(this.RowIndex);
			}
		}

		public IEnumerable<FieldDefInfo> FieldCollection {
			get {
				return this.Assembly.CreateFieldDefCollection((RowIndex)this.row.FieldList, this.Assembly.GetFieldCount(this.row.Index));
			}
		}

		public IEnumerable<MethodDefInfo> MethodCollection {
			get {
				return this.Assembly.CreateMethodDefCollection((RowIndex)this.row.MethodList, this.Assembly.GetMethodCount(this.row.Index));
			}
		}

		public IEnumerable<TypeDeclInfo> InterfaceImplCollection {
			get {
				return this.Assembly.CreateInterfaceImplCollection(this.row.Index);
			}
		}

		public TypeDefInfo EnclosingType {
			get {
				return this.Assembly.GetEnclosingClass(this.RowIndex);
			}
		}

		public TypeDefInfo GetNestedType(string name) {
			foreach(TypeDefInfo typedef in this.Assembly.EnumNestedTypeCandidatesIn(this.Name)) {
				if(typedef.Name==name && typedef.EnclosingType==this) {
					return typedef;
				}
			}
			return null;
		}

		internal int GetFieldIndex(int rowIndex) {
			return rowIndex-(RowIndex)this.row.FieldList;
		}

		internal int GetMethodIndex(int rowIndex) {
			return rowIndex-(RowIndex)this.row.MethodList;
		}

	}

}
