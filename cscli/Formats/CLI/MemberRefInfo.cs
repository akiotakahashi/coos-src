using System;
using FieldAttributes=System.Reflection.FieldAttributes;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	sealed class MemberRefInfo : MemberDeclInfo {

		private MemberRefRow row;
		private MemberSig sig;

		internal MemberRefInfo(AssemblyDefInfo assembly, MemberRefRow row) : base(assembly) {
			this.row = row;
		}

		public override string ToString() {
			return "Member: "+this.Type.FullName+":"+this.Name;
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		public override System.Reflection.MemberTypes Kind {
			get {
				MemberSig sig = this.Signature;
				if(sig is FieldSig) {
					return System.Reflection.MemberTypes.Field;
				} else if(sig is MethodSig) {
					return System.Reflection.MemberTypes.Method;
				} else {
					throw new UnexpectedException();
				}
			}
		}

		public override string Name {
			get {
				return this.Assembly.LoadBlobString(this.row.Name);
			}
		}

		public override TypeDeclInfo Type {
			get {
				return this.Assembly.GetType(row.Class);
			}
		}

		public IMemberRefParent Class {
			get {
				switch(this.row.Class.TableId) {
				case TableId.MethodDef:
					return this.Assembly.GetMethodDef(this.row.Class.RowIndex);
				case TableId.TypeDef:
					return this.Assembly.GetTypeDef(this.row.Class.RowIndex);
				case TableId.TypeRef:
					return this.Assembly.GetTypeRef(this.row.Class.RowIndex);
				case TableId.TypeSpec:
					return this.Assembly.GetTypeSpec(this.row.Class.RowIndex);
				default:
					throw new NotSupportedException();
				}
			}
		}

		public MemberSig Signature {
			get {
				if(sig==null) {
					sig = (MemberSig)this.Assembly.LoadSignature(this.row.Signature, MemberSig.Factory);
				}
				return sig;
			}
		}

		private bool IsGenericType(TypeSig sig) {
			if(sig.BindingTypes!=null) {
				return false;
			} else if(sig.TypeDefOrRef.Token.RowIndex.IsNotNull) {
				TypeDeclInfo typedecl = this.Assembly.GetType(sig.TypeDefOrRef.Token);
				return typedecl.IsGenericType;
			} else if(sig.Type!=null) {
				return IsGenericType(sig.Type);
			} else {
				return false;
			}
		}

		private bool IsGenericType(RetType sig) {
			if(sig.Void) {
				return false;
			} else if(sig.TypedByRef) {
				return false;
			} else {
				return IsGenericType(sig.Type);
			}
		}

		public override bool ContainsGenericParameters {
			get {
				MemberSig memsig = this.Signature;
				if(memsig is MethodSig) {
					MethodSig sig = (MethodSig)memsig;
					if(sig.Generic) { return true; }
					return IsGenericType(sig.RetType);
				} else if(memsig is FieldSig) {
					FieldSig sig = (FieldSig)memsig;
					return false;
				} else {
					throw new UnexpectedException();
				}
			}
		}

	}

}
