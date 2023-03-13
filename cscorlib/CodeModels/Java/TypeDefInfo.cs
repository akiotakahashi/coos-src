using System;
using CooS.Reflection;
using System.Reflection;
using System.Collections;

namespace CooS.CodeModels.Java {

#if JAVA

	public class TypeDefInfo : TypeImpl {

		readonly AssemblyDef assembly;
		readonly ClassFile file;

		public TypeDefInfo(AssemblyDef assembly, ClassFile file) {
			this.assembly = assembly;
			this.file = file;
		}

		public override AssemblyBase AssemblyInfo {
			get {
				return this.assembly;
			}
		}

		protected override MethodInfoImpl[] ConstructSlots() {
			return null;
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

		public FieldDefInfo GetFieldInfo(int rowIndex) {
			throw new NotImplementedException();
		}

		public MethodDefInfo GetMethodInfo(int rowIndex) {
			throw new NotImplementedException();
		}

		#region メンバコレクション

		class FieldCollection : ICollection {

			readonly TypeDefInfo owner;

			public FieldCollection(TypeDefInfo owner) {
				this.owner = owner;
			}

			class FieldEnumerator : IEnumerator {

				readonly FieldCollection col;
				int currentRow;

				public FieldEnumerator(FieldCollection c) {
					this.col = c;
					this.currentRow = -1;
				}

				#region IEnumerator メンバ

				public void Reset() {
					this.currentRow = -1;
				}

				public object Current {
					get {
						return this.col.owner.GetFieldInfo(this.currentRow);
					}
				}

				public bool MoveNext() {
					if(this.currentRow+1>=this.col.Count) {
						return false;
					} else {
						++this.currentRow;
						return true;
					}
				}

				#endregion

			}

			#region IEnumerable メンバ

			public IEnumerator GetEnumerator() {
				return new FieldEnumerator(this);
			}

			#endregion

			#region ICollection メンバ

			public bool IsSynchronized {
				get {
					return false;
				}
			}

			public int Count {
				get {
					return this.owner.FieldCount;
				}
			}

			public void CopyTo(Array array, int index) {
				throw new NotSupportedException();
			}

			public object SyncRoot {
				get {
					throw new NotSupportedException();
				}
			}

			#endregion

		}

		class MethodCollection : ICollection {

			readonly TypeDefInfo owner;

			public MethodCollection(TypeDefInfo owner) {
				this.owner = owner;
			}

			class MethodEnumerator : IEnumerator {

				readonly MethodCollection col;
				int currentRow;

				public MethodEnumerator(MethodCollection c) {
					this.col = c;
					this.currentRow = -1;
				}

				#region IEnumerator メンバ

				public void Reset() {
					this.currentRow = -1;
				}

				public object Current {
					get {
						return this.col.owner.GetMethodInfo(this.currentRow);
					}
				}

				public bool MoveNext() {
					if(this.currentRow+1>=this.col.Count) {
						return false;
					} else {
						++this.currentRow;
						return true;
					}
				}

				#endregion

			}

			#region IEnumerable メンバ

			public IEnumerator GetEnumerator() {
				return new MethodEnumerator(this);
			}

			#endregion

			#region ICollection メンバ

			public bool IsSynchronized {
				get {
					return false;
				}
			}

			public int Count {
				get {
					return this.owner.MethodCount;
				}
			}

			public void CopyTo(Array array, int index) {
				throw new NotSupportedException();
			}

			public object SyncRoot {
				get {
					throw new NotSupportedException();
				}
			}

			#endregion

		}

		#endregion

		public override IEnumerable DeclaredFields {
			get {
				return new FieldCollection(this);
			}
		}

		public override IEnumerable DeclaredMethods {
			get {
				return new MethodFilteringCollection(new MethodCollection(this), false);
			}
		}

		public override IEnumerable DeclaredConstructors {
			get {
				return new MethodFilteringCollection(new MethodCollection(this), true);
			}
		}

		public override IEnumerable DeclaredEvents {
			get {
				return new EventInfo[0];
			}
		}

		public override IEnumerable DeclaredPeoperties {
			get {
				return new PropertyInfo[0];
			}
		}

		public override IEnumerable DeclaredNestedTypes {
			get {
				return new TypeImpl[0];
			}
		}

		#region Not Implemented

		public override TypeImpl GetByRefPointerType() {
			throw new NotImplementedException();
		}

		public override TypeImpl GetByValPointerType() {
			throw new NotImplementedException();
		}

		public override TypeImpl GetSzArrayType() {
			throw new NotImplementedException();
		}

		public override TypeImpl GetMnArrayType(int dimension) {
			throw new NotImplementedException();
		}

		public override TypeAttributes AttributeFlags {
			get {
				throw new NotImplementedException();
			}
		}

		public override int InstanceSize {
			get {
				throw new NotImplementedException();
			}
		}

		protected override bool IsByRefImpl() {
			throw new NotImplementedException();
		}

		public override bool IsByRefPointer {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsByValPointer {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsEnumImpl {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsNested {
			get {
				throw new NotImplementedException();
			}
		}

		public override int OffsetToContents {
			get {
				throw new NotImplementedException();
			}
		}

		public override int RowIndex {
			get {
				throw new NotImplementedException();
			}
		}

		public override int StaticSize {
			get {
				throw new NotImplementedException();
			}
		}

		public override int VariableSize {
			get {
				throw new NotImplementedException();
			}
		}

		public override string Name {
			get {
				throw new NotImplementedException();
			}
		}

		public override Type BaseType {
			get {
				throw new NotImplementedException();
			}
		}

		public override Type GetElementType() {
			throw new NotImplementedException();
		}

		protected override bool IsArrayImpl() {
			throw new NotImplementedException();
		}

		public override string Namespace {
			get {
				throw new NotImplementedException();
			}
		}

		#endregion

	}

#endif

}
