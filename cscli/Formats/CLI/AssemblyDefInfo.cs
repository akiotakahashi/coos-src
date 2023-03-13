using System;
using System.IO;
using CooS.Collections;
using System.Collections.Generic;
using AssemblyName=System.Reflection.AssemblyName;
using TypeAttributes=System.Reflection.TypeAttributes;
using TrieTreeInt=CooS.Collections.TrieTree<int>;
using TrieTreeList=CooS.Collections.TrieTree<System.Collections.Generic.List<int>>;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;
	using Metadata.Indexes;

	sealed partial class AssemblyDefInfo : AssemblyDeclInfo, IDisposable {

		public override string Name {
			get {
				return this.AssemblyName.Name;
			}
		}

		public override System.Globalization.CultureInfo Culture {
			get {
				return this.AssemblyName.CultureInfo;
			}
		}

		public override Version Version {
			get {
				return this.AssemblyName.Version;
			}
		}


		#region IDisposable メンバ

		public void Dispose() {
			this.mdroot.Dispose();
		}

		#endregion

	}

	sealed partial class AssemblyDefInfo {

		public static AssemblyDefInfo LoadAssembly(Stream stream) {
			MetadataRoot mdroot = MetadataRoot.LoadMetadataFromStream(stream);
			return new AssemblyDefInfo(mdroot);
		}

	}

	sealed partial class AssemblyDefInfo {

		private MemberCollection<TypeDefInfo, AssemblyDefInfo> _typeDefCollection;
		private MemberCollection<TypeRefInfo, AssemblyDefInfo> _typeRefCollection;
		private MemberCollection<FieldDefInfo, AssemblyDefInfo> _fieldDefCollection;
		private MemberCollection<MethodDefInfo, AssemblyDefInfo> _methodDefCollection;
		private MemberCollection<MethodSpecInfo, AssemblyDefInfo> _methodSpecCollection;
		private MemberCollection<ParameterDefInfo, AssemblyDefInfo> _paramDefCollection;

		public MemberCollection<TypeDefInfo, AssemblyDefInfo> TypeDefCollection {
			get {
				if(_typeDefCollection==null) {
					_typeDefCollection = new MemberCollection<TypeDefInfo, AssemblyDefInfo>(this,
						0, this.typedefs.Length, delegate(AssemblyDefInfo assembly, int index)
					{
						return assembly.GetTypeDef(index);
					});
				}
				return _typeDefCollection;
			}
		}

		public MemberCollection<TypeRefInfo, AssemblyDefInfo> TypeRefCollection {
			get {
				if(_typeRefCollection==null) {
					_typeRefCollection = new MemberCollection<TypeRefInfo, AssemblyDefInfo>(this,
						0, this.typerefs.Length, delegate(AssemblyDefInfo assembly, int index)
					{
						return assembly.GetTypeRef(index);
					});
				}
				return _typeRefCollection;
			}
		}

		internal MemberCollection<FieldDefInfo, AssemblyDefInfo> CreateFieldDefCollection(int rowIndex, int count) {
			return new MemberCollection<FieldDefInfo, AssemblyDefInfo>(this, rowIndex-1, count
				, delegate(AssemblyDefInfo assembly, int index)
			{
				return assembly.GetFieldDef(index);
			});
		}

		internal MemberCollection<MethodDefInfo, AssemblyDefInfo> CreateMethodDefCollection(int rowIndex, int count) {
			return new MemberCollection<MethodDefInfo, AssemblyDefInfo>(this, rowIndex-1, count
				, delegate(AssemblyDefInfo assembly, int index)
			{
				return assembly.GetMethodDef(index);
			});
		}

		internal MemberCollection<MethodSpecInfo, AssemblyDefInfo> CreateMethodSpecCollection(int rowIndex, int count) {
			return new MemberCollection<MethodSpecInfo, AssemblyDefInfo>(this, rowIndex-1, count
				, delegate(AssemblyDefInfo assembly, int index)
			{
				return assembly.GetMethodSpec(index);
			});
		}

		internal MemberCollection<ParameterDefInfo, AssemblyDefInfo> CreateParamDefCollection(int rowIndex, int count) {
			return new MemberCollection<ParameterDefInfo, AssemblyDefInfo>(this, rowIndex-1, count
				, delegate(AssemblyDefInfo assembly, int index)
			{
				return assembly.GetParameterDef(index);
			});
		}

		internal IEnumerable<TypeDeclInfo> CreateInterfaceImplCollection(int rowIndex) {
			Table<InterfaceImplRow,InterfaceImplRowIndex> table = this.mdroot.Tables.InterfaceImpl;
			if(table!=null) {
				foreach(InterfaceImplRow row in table) {
					if(row.Class.Value==rowIndex) {
						yield return this.GetType(row.Interface);
					}
				}
			}
		}

		public MemberCollection<FieldDefInfo, AssemblyDefInfo> FieldDefCollection {
			get {
				if(_fieldDefCollection==null) {
					_fieldDefCollection = this.CreateFieldDefCollection(1, this.fields.Length);
				}
				return _fieldDefCollection;
			}
		}

		public MemberCollection<MethodDefInfo, AssemblyDefInfo> MethodDefCollection {
			get {
				if(_methodDefCollection==null) {
					_methodDefCollection = this.CreateMethodDefCollection(1, this.methods.Length);
				}
				return _methodDefCollection;
			}
		}

		public MemberCollection<MethodSpecInfo, AssemblyDefInfo> MethodSpecCollection {
			get {
				if(_methodSpecCollection==null) {
					_methodSpecCollection = this.CreateMethodSpecCollection(1, this.methodspecs.Length);
				}
				return _methodSpecCollection;
			}
		}

		public MemberCollection<ParameterDefInfo, AssemblyDefInfo> ParamDefCollection {
			get {
				if(_paramDefCollection==null) {
					_paramDefCollection = this.CreateParamDefCollection(1, this.paramdefs.Length);
				}
				return _paramDefCollection;
			}
		}
	
	}

	sealed partial class AssemblyDefInfo {

		//private readonly TrieTreeInt pathtree = new TrieTreeInt(-1);
		private readonly TrieTreeList nametree = new TrieTreeList();
		private readonly List<int> fieldtable;
		private readonly List<int> methodtable;
		private readonly List<int> paramtable;

		internal MethodIL OpenMethod(MethodDefRow row) {
			return this.mdroot.GetMethodCode(row);
		}

		internal LocalVarSig LoadLocalVarSig(RowToken localVarSigTok) {
			StandAloneSigRow sas = this.mdroot.Tables.StandAloneSig[localVarSigTok.RowIndex];
			return (LocalVarSig)this.mdroot.Blob.ReadSignature(sas.Signature, LocalVarSig.Factory);
		}

		internal IEnumerable<int> EnumTypeCandidatesHavingName(string name) {
			List<int> value;
			if(this.nametree.TryFind(name, out value)) {
				return value;
			} else {
				return new int[0];
			}
		}

		public TypeDefInfo SearchTypeDef(string name) {
			int i = name.LastIndexOf('.');
			string ns = name.Substring(0, i);
			name = name.Substring(i+1);
			i = name.LastIndexOf('+');
			if(i>=0) {
				string outer = name.Substring(0,i);
				name = name.Substring(i+1);
				return this.SearchTypeDef(name, outer, ns);
			} else {
				return this.SearchTypeDef(name, ns);
			}
		}

		public TypeDefInfo SearchTypeDef(string name, string ns) {
			foreach(int rowIndex in this.EnumTypeCandidatesHavingName(name)) {
				TypeDefInfo typedef = this.GetTypeDefByIndex(rowIndex);
				if(typedef.Namespace==ns) {
					return typedef;
				}
			}
			return null;
		}

		public TypeDefInfo SearchTypeDef(string name, string outer, string ns) {
			foreach(int rowIndex in this.EnumTypeCandidatesHavingName(name)) {
				TypeDefInfo typedef = this.GetTypeDefByIndex(rowIndex);
				if(!typedef.IsNested) { continue; }
				TypeDefInfo enc = typedef.EnclosingType;
				if(enc.Namespace==ns && enc.Name==outer) {
					return typedef;
				}
			}
			return null;
		}

		internal int GetFieldCount(int rowIndex) {
			switch(rowIndex.CompareTo(this.fieldtable.Count)) {
			case +1:
				throw new ArgumentOutOfRangeException();
			case 0:
				return this.fields.Length-this.fieldtable[this.fieldtable.Count-1]+1;
			case -1:
				return this.fieldtable[rowIndex]-this.fieldtable[rowIndex-1];
			default:
				throw new InvalidProgramException();
			}
		}

		internal int GetMethodCount(int rowIndex) {
			switch(rowIndex.CompareTo(this.methodtable.Count)) {
			case +1:
				throw new ArgumentOutOfRangeException();
			case 0:
				return this.methods.Length-this.methodtable[this.methodtable.Count-1]+1;
			case -1:
				return this.methodtable[rowIndex]-this.methodtable[rowIndex-1];
			default:
				throw new InvalidProgramException();
			}
		}

		internal int GetParameterCount(int rowIndex) {
			switch(rowIndex.CompareTo(this.paramtable.Count)) {
			case +1:
				throw new ArgumentOutOfRangeException();
			case 0:
				return this.paramdefs.Length-this.paramtable[this.paramtable.Count-1]+1;
			case -1:
				return this.paramtable[rowIndex]-this.paramtable[rowIndex-1];
			default:
				throw new InvalidProgramException();
			}
		}

		internal TypeDefInfo SearchTypeOfField(int rowIndex) {
			int index = fieldtable.BinarySearch(rowIndex);
			if(index<0) {
				index = (~index)-1;
			} else {
				while(index<fieldtable.Count-1 && rowIndex>=fieldtable[index+1]) {
					++index;
				}
			}
			System.Diagnostics.Debug.Assert(fieldtable[index]<=rowIndex);
			System.Diagnostics.Debug.Assert(rowIndex<fieldtable[index+1]);
			return this.TypeDefCollection[index];
		}

		internal TypeDefInfo SearchTypeOfMethod(int rowIndex) {
			int index = methodtable.BinarySearch(rowIndex);
			if(index>=0) {
				while(index<methodtable.Count-1 && rowIndex>=methodtable[index+1]) {
					++index;
				}
				return this.TypeDefCollection[index];
			} else {
				return this.TypeDefCollection[(~index)-1];
			}
		}

		internal MethodDefInfo SearchMethodOfParam(int rowIndex) {
			int index = paramtable.BinarySearch(rowIndex);
			if(index>=0) {
				while(index<paramtable.Count-1 && rowIndex>=paramtable[index+1]) {
					++index;
				}
				return this.MethodDefCollection[index];
			} else {
				return this.MethodDefCollection[(~index)-1];
			}
		}

	}

	sealed partial class AssemblyDefInfo {

		private readonly MetadataRoot mdroot;
		private readonly CooS.Formats.CLI.Metadata.Heaps.TablesHeap tables;
		private AssemblyName name = null;
		private readonly AssemblyRefInfo[] asmrefs = null;
		private readonly TypeDefInfo[] typedefs = null;
		private readonly TypeRefInfo[] typerefs = null;
		private readonly TypeSpecInfo[] typespecs = null;
		private readonly FieldDefInfo[] fields = null;
		private readonly MethodDefInfo[] methods = null;
		private readonly MemberRefInfo[] memberrefs = null;
		private readonly ParameterDefInfo[] paramdefs = null;
		private readonly MethodSpecInfo[] methodspecs = null;

		internal AssemblyDefInfo(MetadataRoot mdroot) {
			this.mdroot = mdroot;
			this.tables = mdroot.Tables;
			this.asmrefs = new AssemblyRefInfo[this.tables.GetRowCount(TableId.AssemblyRef)];
			this.typedefs = new TypeDefInfo[this.tables.GetRowCount(TableId.TypeDef)];
			this.typerefs = new TypeRefInfo[this.tables.GetRowCount(TableId.TypeRef)];
			this.typespecs = new TypeSpecInfo[this.tables.GetRowCount(TableId.TypeSpec)];
			this.fields = new FieldDefInfo[this.tables.GetRowCount(TableId.Field)];
			this.methods = new MethodDefInfo[this.tables.GetRowCount(TableId.MethodDef)];
			this.paramdefs = new ParameterDefInfo[this.tables.GetRowCount(TableId.Param)];
			this.memberrefs = new MemberRefInfo[this.tables.GetRowCount(TableId.MemberRef)];
			this.methodspecs = new MethodSpecInfo[this.tables.GetRowCount(TableId.MethodSpec)];
			//
			foreach(TypeDefRow row in mdroot.Tables[TableId.TypeDef]) {
				if(TypeUtility.IsPublicFlag(row.Flags)) {
					string name = this.LoadBlobString(row.TypeName);
					List<int> list;
					if(nametree.ContainsKey(name)) {
						list = nametree[name];
					} else {
						nametree.Add(name, list = new List<int>());
					}
					list.Add(row.Index);
				}
			}
			//
			this.fieldtable = new List<int>(this.typedefs.Length);
			this.methodtable = new List<int>(this.typedefs.Length);
			foreach(TypeDefRow row in mdroot.Tables[TableId.TypeDef]) {
				fieldtable.Add(row.FieldList.Value);
				methodtable.Add(row.MethodList.Value);
			}
			this.paramtable = new List<int>(this.methods.Length);
			foreach(MethodDefRow row in mdroot.Tables[TableId.MethodDef]) {
				paramtable.Add(row.ParamList.Value);
			}
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

		private void LoadAllTypes() {
			for(int i=0; i<typedefs.Length; ++i) {
				if(typedefs[i]==null) {
					typedefs[i] = LoadTypeDef(i);
				}
			}
		}

		#endregion

		#region 要素へのアクセス

		#region AssemblyRef

		AssemblyRefInfo LoadAssemblyRef(RowIndex rowIndex) {
			AssemblyRefRow row = tables.AssemblyRef[rowIndex];
			return new AssemblyRefInfo(this, row);
		}

		public AssemblyRefInfo GetAssemlbyRef(RowIndex rowIndex) {
			if(this.asmrefs[rowIndex.Value-1]!=null) {
				return this.asmrefs[rowIndex.Value-1];
			} else {
				return this.asmrefs[rowIndex.Value-1] = LoadAssemblyRef(rowIndex);
			}
		}

		#endregion

		#region Type

		private TypeDefInfo LoadTypeDef(RowIndex rowIndex) {
			TypeDefRow row = this.tables.TypeDef[rowIndex];
			return new TypeDefInfo(this, row);
		}

		internal TypeDefInfo GetTypeDef(RowIndex rowIndex) {
			if(rowIndex.IsInvalid) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex.IsNull) {
				return null;
			} else {
				int arrIndex = rowIndex.Value-1;
				if(typedefs[arrIndex]!=null) {
					return typedefs[arrIndex];
				} else {
					return typedefs[arrIndex] = LoadTypeDef(rowIndex);
				}
			}
		}

		public TypeDefInfo GetTypeDefByIndex(int index) {
			return this.GetTypeDef(index+1);
		}

		private TypeRefInfo LoadTypeRef(RowIndex rowIndex) {
			TypeRefRow row = tables.TypeRef[rowIndex];
			switch(row.ResolutionScope.TableId) {
			case TableId.Module:
			case TableId.ModuleRef:
				throw new NotImplementedException();
			case TableId.TypeRef:
			case TableId.AssemblyRef:
				return new TypeRefInfo(this, row);
			default:
				throw new BadMetadataException();
			}
		}

		public TypeRefInfo GetTypeRef(RowIndex rowIndex) {
			if(rowIndex.IsInvalid) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex.IsNull) {
				return null;
			} else {
				int arrIndex = rowIndex.Value-1;
				if(this.typerefs[arrIndex]!=null) {
					return typerefs[arrIndex];
				} else {
					return this.typerefs[arrIndex] = LoadTypeRef(rowIndex);
				}
			}
		}

		private TypeSpecInfo LoadTypeSpec(RowIndex rowIndex) {
			TypeSpecRow row = this.tables.TypeSpec[rowIndex];
			return new TypeSpecInfo(this, row);
		}

		public TypeSpecInfo GetTypeSpec(RowIndex rowIndex) {
			if(rowIndex.IsInvalid) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex.IsNull) {
				return null;
			} else {
				int arrIndex = rowIndex.Value-1;
				if(this.typespecs[arrIndex]!=null) {
					return typespecs[arrIndex];
				} else {
					return this.typespecs[arrIndex] = LoadTypeSpec(rowIndex);
				}
			}
		}

		internal IResolutionScope GetResolutionScope(ResolutionScopeCodedIndex token) {
			switch(token.TableId) {
			case TableId.Module:
			case TableId.ModuleRef:
				throw new NotImplementedException();
			case TableId.TypeRef:
				return GetTypeRef(token.RowIndex);
			case TableId.AssemblyRef:
				return GetAssemlbyRef(token.RowIndex);
			default:
				throw new BadMetadataException();
			}
		}

		#endregion

		#region FieldDef

		private FieldDefInfo LoadFieldDef(RowIndex rowIndex) {
			FieldRow row = this.tables.FieldDef[rowIndex];
			return new FieldDefInfo(this, row);
		}

		public FieldDefInfo GetFieldDef(RowIndex rowIndex) {
			if(rowIndex.IsInvalid) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex.IsNull) {
				return null;
			} else {
				int arrIndex = rowIndex-1;
				if(this.fields[arrIndex]==null) {
					this.fields[arrIndex] = LoadFieldDef(rowIndex);
				}
				return this.fields[arrIndex];
			}
		}

		#endregion

		#region MethodDef

		private MethodDefInfo LoadMethodDef(RowIndex rowIndex) {
			MethodDefRow row = this.tables.MethodDef[rowIndex];
			return new MethodDefInfo(this, row);
		}

		public MethodDefInfo GetMethodDef(RowIndex rowIndex) {
			if(rowIndex.IsInvalid) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex.IsNull) {
				return null;
			} else {
				int arrIndex = rowIndex.Value-1;
				if(this.methods[arrIndex]==null) {
					this.methods[arrIndex] = LoadMethodDef(rowIndex);
				}
				return this.methods[arrIndex];
			}
		}

		#endregion

		#region MethodSpec

		public MethodSpecInfo LoadMethodSpec(RowIndex rowIndex) {
			MethodSpecRow row = this.tables.MethodSpec[rowIndex];
			return new MethodSpecInfo(this, row);
		}

		public MethodSpecInfo GetMethodSpec(RowIndex rowIndex) {
			if(rowIndex.IsInvalid) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex.IsNull) {
				return null;
			} else {
				int arrIndex = rowIndex-1;
				if(this.methodspecs[arrIndex]!=null) {
					return this.methodspecs[arrIndex];
				} else {
					return this.methodspecs[arrIndex] = LoadMethodSpec(rowIndex);
				}
			}
		}

		#endregion

		#region MemberRef (FieldRef/MethodRef)

		private MemberRefInfo LoadMemberRef(int rowIndex) {
			MemberRefRow row = this.tables.MemberRef[rowIndex];
			return new MemberRefInfo(this, row);
		}

		public MemberRefInfo GetMemberRef(int rowIndex) {
			if(rowIndex<0) { throw new ArgumentOutOfRangeException(); }
			if(rowIndex==0) {
				return null;
			} else {
				int arrIndex = rowIndex-1;
				if(this.memberrefs[arrIndex]!=null) {
					return this.memberrefs[arrIndex];
				} else {
					return this.memberrefs[arrIndex] = LoadMemberRef(rowIndex);
				}
			}
		}

		#endregion

		#region ParameterDef

		public ParameterDefInfo LoadParameterDef(RowIndex rowIndex) {
			ParamRow row = this.tables.Param[rowIndex];
			return new ParameterDefInfo(this, row);
		}

		public ParameterDefInfo GetParameterDef(int rowIndex) {
			if(rowIndex<0) throw new ArgumentOutOfRangeException();
			if(rowIndex==0) return null;
			int arrIndex = rowIndex-1;
			if(this.paramdefs[arrIndex]!=null) {
				return this.paramdefs[arrIndex];
			} else {
				return this.paramdefs[arrIndex] = LoadParameterDef(rowIndex);
			}
		}

		#endregion

		#endregion

		#region シグネチャ

		private readonly Dictionary<int,object> signatures = new Dictionary<int,object>();

		internal object LoadSignature(BlobHeapIndex rowIndex, SignatureFactory factory) {
			object value;
			if(signatures.TryGetValue(rowIndex.RawIndex, out value)) {
				return value;
			} else {
				return this.signatures[rowIndex.RawIndex] = this.mdroot.Blob.ReadSignature(rowIndex, factory);
			}
		}

		#endregion

		#region トークン解決

		internal TypeDeclInfo GetType(TypeDefOrRefCodedIndex token) {
			switch(token.TableId) {
			case TableId.TypeDef:
				return this.GetTypeDef(token.RowIndex.Value);
			case TableId.TypeRef:
				return this.GetTypeRef(token.RowIndex.Value);
			case TableId.TypeSpec:
				TypeSpecRow row = this.mdroot.Tables.TypeSpec[token.RowIndex];
				return this.LookupType((TypeSig)this.mdroot.Blob.ReadSignature(row.Signature, TypeSig.Factory));
			default:
				throw new ArgumentException("ResolveType can't do about "+(int)token.TableId);
			}
		}

		internal TypeDeclInfo GetType(MemberRefParentCodedIndex codedIndex) {
			throw new NotImplementedException();
		}

		internal MemberDeclInfo GetMember(RowToken token) {
			switch(token.TableId) {
			case TableId.Field:
				return this.GetFieldDef(token.RowIndex.Value);
			case TableId.MethodDef:
				return this.GetMethodDef(token.RowIndex.Value);
			case TableId.MemberRef:
				return this.GetMemberRef(token.RowIndex.Value);
			case TableId.StandAloneSig:
				return new MethodSigInfo(this, this.mdroot.Tables.StandAloneSig[token.RowIndex]);
			default:
				throw new ArgumentException("Illegal TableId: "+((int)token.TableId).ToString("X2"));
			}
		}

		internal FieldDeclInfo GetField(RowToken token) {
			return (FieldDeclInfo)this.GetMember(token);
		}

		internal MethodDeclInfo GetMethod(RowToken token) {
			return (MethodDeclInfo)this.GetMember(token);
		}

		internal MethodDeclInfo GetMethod(MethodDefOrRefCodedIndex codedIndex) {
			return this.GetMethod(new RowToken(codedIndex.TableId, codedIndex.RowIndex));
		}

		#endregion

		#region プリミティブ型

		private TypeDeclInfo[] primitivetypes = new TypeDeclInfo[32 /*decent*/];

		private bool IsMscorlib {
			get {
				return this.AssemblyName.Name=="mscorlib";
			}
		}

		internal TypeDeclInfo LookupPrimitiveType(ElementType type) {
			if(primitivetypes[(int)type]==null) {
				string name;
				switch(type) {
				default:
					throw new ArgumentException();
				case ElementType.Boolean:
					name = "Boolean";
					break;
				case ElementType.Char:
					name = "Char";
					break;
				case ElementType.I1:
					name = "SByte";
					break;
				case ElementType.U1:
					name = "Byte";
					break;
				case ElementType.I2:
					name = "Int16";
					break;
				case ElementType.U2:
					name = "UInt16";
					break;
				case ElementType.I4:
					name = "Int32";
					break;
				case ElementType.U4:
					name = "UInt32";
					break;
				case ElementType.I8:
					name = "Int64";
					break;
				case ElementType.U8:
					name = "UInt64";
					break;
				case ElementType.R4:
					name = "Single";
					break;
				case ElementType.R8:
					name = "Double";
					break;
				case ElementType.I:
					name = "IntPtr";
					break;
				case ElementType.U:
					name = "UIntPtr";
					break;
				case ElementType.String:
					name = "String";
					break;
				case ElementType.Object:
					name = "Object";
					break;
				case ElementType.Void:
					name = "Void";
					break;
				case ElementType.ByRef:
					name = "ByRefPointer";
					break;
				case ElementType.ByVal:
					name = "ByValPointer";
					break;
				case ElementType.SzArray:
					name = "SzArray";
					break;
				case ElementType.MnArray:
					name = "MnArray";
					break;
				case ElementType.TypedByRef:
					name = "TypedReference";
					break;
				}
				if(this.IsMscorlib) {
					return primitivetypes[(int)type] = this.SearchTypeDef(name, "System");
				} else {
					AssemblyRefInfo mscorlib = null;
					foreach(AssemblyRefRow row in this.mdroot.Tables[TableId.AssemblyRef]) {
						string asmname = this.LoadBlobString(row.Name);
						if(asmname.ToLower()=="mscorlib") {
							mscorlib = new AssemblyRefInfo(this, row);
							break;
						}
					}
					if(mscorlib==null) throw new AssemblyNotFoundException("mscorlib");
					TypeRefInfo typeref = new PrimitiveTypeRefInfo(this, mscorlib, name, "System");
					return primitivetypes[(int)type] = typeref;
				}
			}
			return primitivetypes[(int)type];
		}

		List<GenericParamInfo> genericTVarList = new List<GenericParamInfo>();
		List<GenericParamInfo> genericMVarList = new List<GenericParamInfo>();

		public TypeDeclInfo LookupType(TypeSig signature) {
			if(signature==null) throw new ArgumentNullException("signature");
			switch(signature.ElementType) {
			default:
				throw new BadSignatureException();
			case ElementType.Boolean:
			case ElementType.Char:
			case ElementType.I1:
			case ElementType.U1:
			case ElementType.I2:
			case ElementType.U2:
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R4:
			case ElementType.R8:
			case ElementType.I:
			case ElementType.U:
			case ElementType.String:
			case ElementType.Object:
			case ElementType.Void:
			case ElementType.TypedByRef:
				return this.LookupPrimitiveType(signature.ElementType);
			case ElementType.ByRef:
				return this.LookupType(signature.Type).GetByRefType();
			case ElementType.ByVal:
				return this.LookupType(signature.Type).GetByValType();
			case ElementType.SzArray:
				return this.LookupType(signature.Type).GetSzArrayType();
			case ElementType.MnArray:
				return this.LookupType(signature.Type).GetMnArrayType(signature.ArrayShape);
			case ElementType.ValueType:
			case ElementType.Class:
				return this.GetType(signature.TypeDefOrRef.Token);
			case ElementType.FnPtr:
				return new FnPtrInfo(this, signature.Method);
			case ElementType.Pinned:
				return this.LookupType(signature.Type);
			case ElementType.GenericInst:
				return this.GetType(signature.TypeDefOrRef.Token).Specialize(signature.BindingTypes);
			case ElementType.Var:
				while(this.genericTVarList.Count<=signature.Number) {
					genericTVarList.Add(null);
				}
				if(genericTVarList[signature.Number]!=null) {
					return genericTVarList[signature.Number];
				} else {
					return genericTVarList[signature.Number] = new GenericParamInfo(this, signature);
				}
			case ElementType.MVar:
				while(this.genericMVarList.Count<=signature.Number) {
					genericMVarList.Add(null);
				}
				if(genericMVarList[signature.Number]!=null) {
					return genericMVarList[signature.Number];
				} else {
					return genericMVarList[signature.Number] = new GenericParamInfo(this, signature);
				}
			case ElementType.CModReqd:
			case ElementType.CModOpt:
			case ElementType.Internal:
			case ElementType.Modifier:
			case ElementType.Sentinel:
				throw new BadSignatureException("TypeSig::Parse detects unsupported beginning: "+(int)signature.ElementType);
			}
		}
		
		public TypeDeclInfo LookupType(RetType rettype) {
			if(rettype.Void || rettype.TypedByRef) {
				return this.LookupPrimitiveType(ElementType.Void);
			} else {
				return this.LookupType(rettype.Type);
			}
		}

		public TypeDeclInfo LookupType(TypeDefOrRefEncoded token) {
			return this.GetType(token.Token);
		}

		#endregion

		#region 名前解決

		private Dictionary<int, int> enclosingTypeMap;
		private TrieTreeList nestedTypeMap;

		public TypeDefInfo GetEnclosingClass(int nestedClassRowIndex) {
			if(enclosingTypeMap==null) {
				enclosingTypeMap = new Dictionary<int, int>();
				foreach(NestedClassRow row in this.mdroot.Tables.NestedClass) {
					enclosingTypeMap[row.NestedClass.Value] = row.EnclosingClass.Value;
				}
			}
			int value;
			if(enclosingTypeMap.TryGetValue(nestedClassRowIndex, out value)) {
				return this.GetTypeDef(value);
			} else {
				return null;
			}
		}

		public IEnumerable<TypeDefInfo> EnumNestedTypeCandidatesIn(string name) {
			if(nestedTypeMap==null) {
				nestedTypeMap = new TrieTreeList();
				foreach(NestedClassRow row in this.mdroot.Tables[TableId.NestedClass]) {
					TypeDefRow td = this.mdroot.Tables.TypeDef[row.EnclosingClass];
					string n = this.LoadBlobString(td.TypeName);
					if(!nestedTypeMap.ContainsKey(n)) {
						nestedTypeMap[n] = new List<int>();
					}
					nestedTypeMap[n].Add(row.NestedClass.Value);
				}
			}
			List<int> list;
			if(nestedTypeMap.TryFind(name, out list)) {
				foreach(int rowIndex in list) {
					yield return this.GetTypeDef(rowIndex);
				}
			}
		}

#if false
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

#endif

		#endregion

		#region アセンブリ

		public MethodDefInfo EntryPoint {
			get {
				return (MethodDefInfo)this.GetMethod(this.mdroot.Header.EntryPointToken);
			}
		}

		public override AssemblyName AssemblyName {
			get {
				if(name==null) {
					if(this.tables.GetRowCount(TableId.Assembly)!=1) {
						throw new BadMetadataException();
					}
					AssemblyRow row = this.tables.Assembly[0];
					name = new AssemblyName();
					name.Name = this.LoadBlobString(row.Name);
					string culture = this.LoadBlobString(row.Culture);
					if(culture==null) {
						name.CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
					} else {
						name.CultureInfo = System.Globalization.CultureInfo.GetCultureInfo(culture);
					}
					name.Version = new Version(row.MajorVersion, row.MinorVersion, row.BuildNumber, row.RevisionNumber);
					//TODO: Lack detail information for AssemblyName
				}
				return name;
			}
		}

		#endregion

		public string LoadBlobString(StringHeapIndex rowIndex) {
			if(rowIndex.RawIndex==0) {
				return null;
			} else {
				return this.mdroot.Strings[rowIndex];
			}
		}

		public string LoadUserString(UserStringHeapIndex rowIndex) {
			return this.mdroot.UserStrings[rowIndex];
		}

#if false
		public int GetFieldOwnerIndex(int rowIndex) {
			MetadataRoot metadata = this.Metadata;
			for(int i=this.TypeDefCount; i>0; --i) {
				if(this.IsTypeDefLoaded(i)) {
					if(this.GetTypeDef(i).OwnsField(rowIndex)) {
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

		public TypeDefInfo GetFieldOwner(int rowIndex) {
			int ownerIndex = this.GetFieldOwnerIndex(rowIndex);
			if(ownerIndex<0)
				throw new ArgumentException();
			return this.GetTypeDef(ownerIndex);
		}
#endif

#if false
		public int GetMethodOwnerIndex(int rowIndex) {
			MetadataRoot metadata = this.Metadata;
			for(int i=this.TypeDefCount; i>0; --i) {
				if(this.IsTypeDefLoaded(i)) {
					if(this.GetTypeDef(i).OwnsMethod(rowIndex)) {
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

		public TypeDefInfo GetMethodOwner(int rowIndex) {
			int ownerIndex = this.GetMethodOwnerIndex(rowIndex);
			if(ownerIndex<0)
				throw new ArgumentException();
			return this.GetTypeDef(ownerIndex);
		}
#endif

	}

}
