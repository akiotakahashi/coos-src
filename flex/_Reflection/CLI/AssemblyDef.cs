using System;
using System.IO;
using System.Reflection;
using System.Collections;
using CooS.Formats.CLI;
using CooS.Formats.CLI.Signature;

namespace CooS.Reflection.CLI {
	using Metatype;

	sealed class AssemblyDef : AssemblyBase {

		private readonly AssemblyDefInfo mdroot;
		private readonly ConcreteType[] typedefs = null;
		private readonly ConcreteType[] typerefs = null;
		private readonly FieldDef[] fields = null;
		private readonly MethodDef[] methods = null;
		private readonly MemberInfo[] memberrefs = null;
		private AssemblyName name = null;

		public AssemblyDef(AssemblyDefInfo mdroot) {
			this.mdroot = mdroot;
			this.tables = mdroot.Tables;
			this.typedefs = new ConcreteType[this.tables.GetRowCount(TableId.TypeDef)];
			this.typerefs = new ConcreteType[this.tables.GetRowCount(TableId.TypeRef)];
			this.fields = new FieldDefInfo[this.tables.GetRowCount(TableId.Field)];
			this.methods = new MethodDefInfo[this.tables.GetRowCount(TableId.Method)];
			this.memberrefs = new MemberInfo[this.tables.GetRowCount(TableId.MemberRef)];
		}

		public AssemblyDefInfo Metadata {
			get {
				return mdroot;
			}
		}

		public IDomain Domain {
			get {
				throw new NotImplementedException();
			}
		}

		public override FieldInfoImpl GetFieldInfo(int rowIndex) {
			return this.GetFieldDefInfo(rowIndex);
		}

		public override MethodInfoImpl GetMethodInfo(int rowIndex) {
			return this.GetMethodDefInfo(rowIndex);
		}

		#region プロパティ

		public int TypeDefCount {
			get {
				return this.typedefs.Length;
			}
		}

		public int TypeRefCount {
			get {
				return this.typerefs.Length;
			}
		}

		#endregion

		#region 型のロード
		
		internal bool IsTypeDefLoaded(int rowIndex) {
			return typedefs[rowIndex-1]!=null;
		}

		ConcreteType LoadTypeDef(int rowIndex) {
			TypeDefRow row = (TypeDefRow)this.tables[TableId.TypeDef][rowIndex];
			if(0!=(row.Flags&TypeAttributes.Interface)) {
				return new InterfaceType(this,row);
			} else if(row.Extends.IsNilToken) {
				// Only for <Module> or System.Object
				ConcreteType type = new ClassType(this,row);
				//Console.WriteLine("Loaded root type: [{0}] {1}", this.GetName().Name, type.FullName);
				return type;
			} else {
				Type extends = this.ResolveType(row.Extends);
				if(extends==null) {
					// 親がない場合はSystem.Objectである
					return new ClassType(this,row);
				} else if(extends==this.Manager.Object) {
					// 直接の親がSystem.Objectの場合はそれはClassTypeである
					return new ClassType(this,row);
				} else if(extends==this.Manager.ValueType) {
					// ValueTypeの子クラスはStructTypeである
					return new StructType(this,row);
				} else if(extends==this.Manager.Enum) {
					// System.Enumの子クラスはEnumTypeである
					return new EnumType(this,row);
				} else if(extends==this.Manager.Delegate || extends.IsSubclassOf(this.Manager.Delegate)) {
					// Delegateの子クラスはDelegateTypeである
					return new DelegateType(this,row);
				} else if(extends.IsSubclassOf(this.Manager.Object)) {
					// すべての型はSystem.Objectから派生していなければならない
					return new ClassType(this,row);
				} else {
					Console.WriteLine("Class  : {0}.{1}", Metadata.Strings[row.Namespace],Metadata.Strings[row.Name]);
					Console.WriteLine("Extends: {0}", extends.FullName);
					throw new BadMetadataException();
				}
			}
		}

		private ConcreteType LoadTypeRef(int rowIndex) {
			TypeRefRow typeref = (TypeRefRow)tables[TableId.TypeRef][rowIndex];
			switch(typeref.ResolutionScope.TableId) {
			case TableId.Module:
			case TableId.ModuleRef:
				throw new NotImplementedException();
			case TableId.TypeRef:
				throw new NotImplementedException();
			case TableId.AssemblyRef:
				AssemblyRefRow asmref = (AssemblyRefRow)tables[TableId.AssemblyRef][typeref.ResolutionScope.RID];
				AssemblyBase assembly = ResolveAssembly(asmref);
				string ns = this.Metadata.Strings[typeref.Namespace];
				string name = this.Metadata.Strings[typeref.Name];
				if(ns==null || ns.Length==0) throw new NotSupportedException();
				return (ConcreteType)assembly.FindType(ns+"."+name,false);
			default:
				throw new BadMetadataException();
			}
		}

		private void LoadAllTypes() {
			for(int i=0; i<typedefs.Length; ++i) {
				if(typedefs[i]==null) {
					typedefs[i] = LoadTypeDef(i+1);
				}
			}
		}

		#endregion

		#region 型へのアクセス

		public override TypeImpl GetTypeInfo(int rowIndex) {
			return this.GetTypeDef(rowIndex);
		}

		public SuperType GetTypeDef(int rowIndex) {
			if(rowIndex<0) throw new ArgumentOutOfRangeException();
			if(rowIndex==0) return null;
			int arrIndex = rowIndex-1;
			if(typedefs[arrIndex]==null) {
				typedefs[arrIndex] = LoadTypeDef(rowIndex);
			}
			return typedefs[arrIndex];
		}

		public SuperType GetTypeRef(int rowIndex) {
			if(rowIndex<0) throw new ArgumentOutOfRangeException();
			if(rowIndex==0) return null;
			int arrIndex = rowIndex-1;
			if(this.typerefs[arrIndex]==null) {
				this.typerefs[arrIndex] = LoadTypeRef(rowIndex);
			}
			return typerefs[arrIndex];
		}

		public SuperType GetTypeRef(int rowIndex, bool throwOnMiss) {
			SuperType type = this.GetTypeRef(rowIndex);
			if(type==null && throwOnMiss)
				throw new TypeNotFoundException();
			return type;
		}

		public override Type[] GetTypes(bool exportedOnly) {
			// THIS METHOD IS CALLED VIA SUBSTITUTE BROKER.
			LoadAllTypes();
			if(!exportedOnly) {
				return this.typedefs;
			} else {
				System.Collections.ArrayList list = new System.Collections.ArrayList(typedefs.Length/2);
				foreach(ConcreteType type in this.typedefs) {
					if(type.IsPublic) {
						list.Add(type);
					}
				}
				return (Type[])list.ToArray(typeof(Type));
			}
		}

		#endregion

		#region ブート専用

		internal TypeImpl RestructureAsPrimitive(TypeImpl _type) {
			StructType type = (StructType)_type;
			this.typedefs[type.Row.Index-1] = null;
			return new PrimitiveType(this, type.Row);
		}

		internal void AssignTypeDef(int rowIndex, ConcreteType type) {
			if(IsTypeDefLoaded(rowIndex)) {
				throw new InvalidOperationException(type.FullName);
			}
			typedefs[rowIndex-1] = type;
		}

		#endregion

		#region トークン解決

#if false
		public SuperType ResolveType(MDToken token) {
			switch(token.TableId) {
			case TableId.TypeDef:
				return this.GetTypeDef(token.RID);
			case TableId.TypeRef:
				return this.GetTypeRef(token.RID, true);
			case TableId.TypeSpec:
				TypeSpecRow row = (TypeSpecRow)this.Metadata.Tables[token];
				SignatureReader reader = this.Metadata.Blob.OpenReader(row.Signature);
				TypeSig typesig = new Signature.TypeSig(reader);
				return (SuperType)typesig.ResolveTypeAt(this);
			default:
				throw new ArgumentException("ResolveType can't do about "+(int)token.TableId);
			}
		}

		public MemberInfo ResolveMember(MDToken token) {
			switch(token.TableId) {
			case TableId.Field:
				return this.GetFieldDefInfo(token.RID);
			case TableId.Method:
				return this.GetMethodDefInfo(token.RID);
			case TableId.MemberRef:
				return this.GetMemberRef(token.RID, true);
			case TableId.StandAloneSig:
				StandAloneSigRow row = (StandAloneSigRow)this.Metadata.Tables[token];
				return new MethodSigInfo(this, row.Signature);
			default:
				throw new ArgumentException("Illegal TableId: "+((int)token.TableId).ToString("X2"));
			}
		}
#endif

		public AssemblyBase ResolveAssembly(AssemblyRefInfo asmref) {
			if(this.Manager==null) throw new InvalidOperationException("AssemblyManager is not set up");
			return this.Manager.ResolveAssembly(TypeUtility.ConvertToAssemblyName(this,asmref),true);
		}

		#endregion

		#region 名前解決

		public int GetEnclosingClass(int nestedClassRowIndex) {
			MetadataRoot metadata = this.Metadata;
			for(int row=1; row<=metadata.Tables.GetRowCount(TableId.NestedClass); ++row) {
				NestedClassRow nestedcls = (NestedClassRow)metadata.Tables[TableId.NestedClass][row];
				if(nestedcls.NestedClass==nestedClassRowIndex) {
					return nestedcls.EnclosingClass;
				}
			}
			return -1;
		}

		public override string GetTypeName(int rowIndex) {
			MetadataRoot metadata = this.Metadata;
			TypeDefRow typedef = (TypeDefRow)metadata.Tables[TableId.TypeDef][rowIndex];
			if(!TypeUtility.IsNestedFlag(typedef.Flags)) {
				return metadata.Strings[typedef.Namespace]+"."+metadata.Strings[typedef.Name];
			} else {
				int enclosingIndex = this.GetEnclosingClass(rowIndex);
				if(enclosingIndex<0) throw new ArgumentException();
				string enclosingName = this.GetTypeName(enclosingIndex);
				return enclosingName+"+"+metadata.Strings[typedef.Name];
			}
		}

		private void ConstructNameTree() {
			Console.WriteLine("<constructing name tree about {0}>", this.GetName(false).Name);
			this.nametree = new CooS.Collections.TrieTree();
			for(int i=1; i<=this.typedefs.Length; ++i) {
				string fullname;
				if(this.IsTypeDefLoaded(i)) {
					fullname = this.GetTypeDef(i).FullName;
				} else {
					fullname = this.GetTypeName(i);
				}
				if(Engine.Infrastructured) {
					Console.Write("{0}/{1}\r", i, this.typedefs.Length);
				}
				this.nametree.Add(fullname, i);
			}
		}

		private int SearchNameTree(string fullname) {
			if(this.nametree==null) {
				this.ConstructNameTree();
			}
			object rowIndex = this.nametree[fullname];
			if(rowIndex==null) return -1;
			return (int)rowIndex;
		}


		public override TypeImpl FindType(string fullname, bool throwOnMiss) {
			for(int i=0; i<typedefs.Length; ++i) {
				ConcreteType type = typedefs[i];
				if(type!=null && type.FullName==fullname) {
					return type;
				}
			}
			int rowIndex = this.SearchNameTree(fullname);
			if(rowIndex>=0) return this.GetTypeDef(rowIndex);
			if(throwOnMiss) throw new TypeNotFoundException(fullname);
			return null;
		}

		/*
		public override Type InternalGetType(Module module, string name, bool throwOnError, bool ignoreCase) {
			// THIS METHOD IS CALLED VIA SUBSTITUTE BROKER.
			if(module!=null) throw new NotSupportedException("module must be null");
			LoadAllTypes();
			if(ignoreCase) name=name.ToUpper();
			foreach(Type type in typedefs) {
				if(ignoreCase) {
					if(type.Name.ToUpper()==name) {
						return type;
					}
				} else {
					if(type.Name==name) {
						return type;
					}
				}
			}
			if(throwOnError) {
				throw new ReflectionTypeLoadException(new Type[]{null}, new Exception[]{new TypeLoadException()});
			}
			return null;
		}
		*/

		#endregion

		#region アセンブリ
		
		public override MethodInfo EntryPoint {
			get {
				return this.ResolveMethod((MDToken)this.Metadata.Header.EntryPointToken);
			}
		}

		public override AssemblyName GetName(bool copiedName) {
			if(copiedName) throw new NotSupportedException();
			if(name==null) {
				if(Metadata.Tables.GetRowCount(TableId.Assembly)!=1) {
					throw new BadMetadataException();
				}
				AssemblyRow row = (AssemblyRow)Metadata.Tables[TableId.Assembly][1];
				name = new AssemblyName();
				name.Name = Metadata.Strings[row.Name];
				name.Version = new Version(row.MajorVersion,row.MinorVersion,row.BuildNumber,row.RevisionNumber);
				//TODO: Lack detail information for AssemblyName
			}
			return name;
		}

		#endregion

		public override string LoadString(int rowIndex) {
			return this.Metadata.UserStrings[rowIndex];
		}

		#region フィールドとメソッド

		internal void AssignFieldOwner(int rowIndex, int count, ConcreteType owner) {
			for(int i=rowIndex; i<rowIndex+count; ++i) {
				if(this.fields[i-1]!=null) {
					this.fields[i-1].AssignOwner(owner);
				}
			}
		}

		internal void AssignMethodOwner(int rowIndex, int count, ConcreteType owner) {
			for(int i=rowIndex; i<rowIndex+count; ++i) {
				if(this.methods[i-1]!=null) {
					this.methods[i-1].AssignOwner(owner);
				}
			}
		}

		public FieldDef GetFieldDefInfo(int rowIndex) {
			if(rowIndex<0) throw new ArgumentOutOfRangeException();
			if(rowIndex==0) return null;
			int arrIndex = rowIndex-1;
			if(this.fields[arrIndex]==null) {
				this.fields[arrIndex] = LoadFieldDefInfo(rowIndex);
			}
			return this.fields[arrIndex];
		}

		public MethodDef GetMethodDefInfo(int rowIndex) {
			if(rowIndex<0) throw new ArgumentOutOfRangeException();
			if(rowIndex==0) return null;
			int arrIndex = rowIndex-1;
			if(this.methods[arrIndex]==null) {
				this.methods[arrIndex] = LoadMethodDefInfo(rowIndex);
			}
			return this.methods[arrIndex];
		}

		private MemberInfo GetMemberRef(int rowIndex, bool throwOnMiss) {
			if(rowIndex<0) throw new ArgumentOutOfRangeException();
			if(rowIndex==0) return null;
			int arrIndex = rowIndex-1;
			if(this.memberrefs[arrIndex]==null) {
				this.memberrefs[arrIndex] = LoadMemberRef(rowIndex);
				if(this.memberrefs[arrIndex]==null) {
					if(throwOnMiss) throw new MemberNotFoundException();
				}
			}
			return this.memberrefs[arrIndex];
		}

		public FieldInfoImpl GetFieldRefInfo(int rowIndex, bool throwOnMiss) {
			return (FieldInfoImpl)this.GetMemberRef(rowIndex,throwOnMiss);
		}

		public MethodInfoImpl GetMethodRefInfo(int rowIndex, bool throwOnMiss) {
			return (MethodInfoImpl)this.GetMemberRef(rowIndex,throwOnMiss);
		}

		private FieldDef LoadFieldDefInfo(int rowIndex) {
			FieldRow row = (FieldRow)this.tables[TableId.Field][rowIndex];
			return new FieldDefInfo(this, row);
		}

		private MethodDef LoadMethodDefInfo(int rowIndex) {
			MethodRow row = (MethodRow)this.tables[TableId.Method][rowIndex];
			return new MethodDefInfo(this, row);
		}

		private MemberInfo LoadMemberRef(int rowIndex) {
			MemberRefRow row = (MemberRefRow)this.tables[TableId.MemberRef][rowIndex];
			string name = this.mdroot.Strings[row.Name];
			SuperType owner = this.ResolveType(row.Class);
			foreach(MemberInfo mi in owner.GetMember(name,BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static)) {
				switch(mi.MemberType) {
				case MemberTypes.Constructor: {
					ConstructorInfoImpl method = (ConstructorInfoImpl)mi;
					MethodSig msig = new MethodSigImpl(this.OpenSig(row.Signature));
					if(TypeUtility.MatchesSignature(method,msig,this)) {
						return mi;
					}
				}
					break;
				case MemberTypes.Method: {
					MethodInfoImpl method = (MethodInfoImpl)mi;
					MethodSig msig = new MethodSigImpl(this.OpenSig(row.Signature));
					if(TypeUtility.MatchesSignature(method,msig,this)) {
						return method;
					}
				}
					break;
				case MemberTypes.Field: {
					FieldInfoImpl field = (FieldInfoImpl)mi;
					FieldSig fsig = new FieldSig(this.OpenSig(row.Signature));
					if(field.FieldType==fsig.Type.ResolveTypeAt(this)) {
						return field;
					}
				}
					break;
				case MemberTypes.Custom:
				case MemberTypes.Event:
				case MemberTypes.NestedType:
				case MemberTypes.Property:
				case MemberTypes.TypeInfo:
				default:
					throw new NotSupportedException();
				}
			}
			throw new MemberAccessException(string.Format("[{0:X}] {1} in {2}", rowIndex, name, owner.FullName));
		}

		#endregion

		#region メンバオーナー

		public int GetFieldOwnerIndex(int rowIndex) {
			MetadataRoot metadata = this.Metadata;
			for(int i=this.TypeDefCount; i>0; --i) {
				if(this.IsTypeDefLoaded(i)) {
					if(((ConcreteType)this.GetTypeDef(i)).OwnsField(rowIndex)) {
						return i;
					}
				} else {
					TypeDefRow row = (TypeDefRow)metadata.Tables[TableId.TypeDef][i];
					if(row.FieldList<=rowIndex) {
						return i;
					}
				}
			}
			return -1;
		}

		public ConcreteType GetFieldOwner(int rowIndex) {
			int ownerIndex = this.GetFieldOwnerIndex(rowIndex);
			if(ownerIndex<0) throw new ArgumentException();
			return (ConcreteType)this.GetTypeDef(ownerIndex);
		}

		public int GetMethodOwnerIndex(int rowIndex) {
			MetadataRoot metadata = this.Metadata;
			for(int i=this.TypeDefCount; i>0; --i) {
				if(this.IsTypeDefLoaded(i)) {
					if(((ConcreteType)this.GetTypeDef(i)).OwnsMethod(rowIndex)) {
						return i;
					}
				} else {
					TypeDefRow row = (TypeDefRow)metadata.Tables[TableId.TypeDef][i];
					if(row.MethodList<=rowIndex) {
						return i;
					}
				}
			}
			return -1;
		}

		public ConcreteType GetMethodOwner(int rowIndex) {
			int ownerIndex = this.GetMethodOwnerIndex(rowIndex);
			if(ownerIndex<0) throw new ArgumentException();
			return (ConcreteType)this.GetTypeDef(ownerIndex);
		}

		#endregion

		#region メンバコレクション

		class FieldCollection : ICollection {

			readonly AssemblyDef assembly;
			readonly ConcreteType owner;
			readonly int rowIndex;
			readonly int count;

			public FieldCollection(AssemblyDef assembly, ConcreteType owner, int rowIndex, int count) {
				this.assembly = assembly;
				this.owner = owner;
				this.rowIndex = rowIndex;
				this.count = count;
			}

			class FieldEnumerator : IEnumerator {

				readonly FieldCollection col;
				int currentRow;

				public FieldEnumerator(FieldCollection c) {
					this.col = c;
					this.currentRow = c.rowIndex-1;
				}

				#region IEnumerator メンバ

				public void Reset() {
					this.currentRow = this.col.rowIndex;
				}

				public object Current {
					get {
						return this.col.assembly.GetFieldDefInfo(this.currentRow);
					}
				}

				public bool MoveNext() {
					if(this.currentRow+1>=this.col.rowIndex+this.col.count) {
						return false;
					} else {
						++this.currentRow;
						((FieldDefInfo)this.Current).AssignOwner(this.col.owner);
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
					return this.count;
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

			readonly AssemblyDef assembly;
			readonly ConcreteType owner;
			readonly int rowIndex;
			readonly int count;

			public MethodCollection(AssemblyDef assembly, ConcreteType owner, int rowIndex, int count) {
				this.assembly = assembly;
				this.owner = owner;
				this.rowIndex = rowIndex;
				this.count = count;
			}

			class MethodEnumerator : IEnumerator {

				readonly MethodCollection col;
				int currentRow;

				public MethodEnumerator(MethodCollection c) {
					this.col = c;
					this.currentRow = c.rowIndex-1;
				}

				#region IEnumerator メンバ

				public void Reset() {
					this.currentRow = this.col.rowIndex;
				}

				public object Current {
					get {
						return this.col.assembly.GetMethodDefInfo(this.currentRow);
					}
				}

				public bool MoveNext() {
					if(this.currentRow+1>=this.col.rowIndex+this.col.count) {
						return false;
					} else {
						++this.currentRow;
						((MethodDefInfo)this.Current).AssignOwner(this.col.owner);
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
					return this.count;
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

		public IEnumerable CreateFieldCollection(ConcreteType owner, int rowIndex, int count) {
			return new FieldCollection(this, owner, rowIndex, count);
		}

		public IEnumerable CreateMethodCollection(ConcreteType owner, int rowIndex, int count) {
			return new MethodFilteringCollection(new MethodCollection(this, owner, rowIndex, count), false);
		}

		public IEnumerable CreateConstructorCollection(ConcreteType owner, int rowIndex, int count) {
			return new MethodFilteringCollection(new MethodCollection(this, owner, rowIndex, count), true);
		}

		#endregion

	}

}
