using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {

	abstract class TypeDeclInfo : ElementInfo {

		private readonly AssemblyDefInfo assembly;

		public TypeDeclInfo(AssemblyDefInfo assembly) {
			this.assembly = assembly;
		}

		public override string ToString() {
			return this.FullName;
		}

		public override AssemblyDefInfo Assembly {
			get {
				return this.assembly;
			}
		}

		public abstract string Name {
			get;
		}

		public abstract string Namespace {
			get;
		}

		public virtual bool IsNested {
			get {
				return false;
			}
		}

		public virtual bool IsGenericParam {
			get {
				return false;
			}
		}

		public virtual bool IsGenericType {
			get {
				return TypeUtility.IsGenericName(this.Name);
			}
		}

		public virtual int GenericParameterCount {
			get {
				return TypeUtility.GetGenericParameterCount(this.Name);
			}
		}

		public virtual Signature.TypeSig[] GenericArguments {
			get {
				return null;
			}
		}

		public string FullName {
			get {
				if(IsNested) {
					TypeDeclInfo enclosing = this.Assembly.GetEnclosingClass(this.RowIndex);
					if(enclosing==null)
						throw new ArgumentException();
					return enclosing.FullName+"/"+this.Name;
				} else if(IsGenericParam) {
					return this.Namespace+this.Name;
				} else {
					string ns = this.Namespace;
					if(string.IsNullOrEmpty(ns)) {
						return this.Name;
					} else {
						return ns+"."+this.Name;
					}
				}
			}
		}

		private DerivedTypeInfo byref;
		private DerivedTypeInfo byval;
		private DerivedTypeInfo szarr;

		internal TypeDeclInfo GetByRefType() {
			if(byref==null) {
				byref = new DerivedTypeInfo(this.assembly, this, ElementType.ByRef);
			}
			return byref;
		}

		internal TypeDeclInfo GetByValType() {
			if(byval==null) {
				byval = new DerivedTypeInfo(this.assembly, this, ElementType.ByVal);
			}
			return byval;
		}

		internal TypeDeclInfo GetSzArrayType() {
			if(szarr==null) {
				szarr = new DerivedTypeInfo(this.assembly, this, ElementType.SzArray);
			}
			return szarr;
		}

		internal TypeDeclInfo GetMnArrayType(CooS.Formats.CLI.Signature.ArrayShape arrayShape) {
			throw new Exception("The method or operation is not implemented.");
		}

		internal TypeDeclInfo Specialize(CooS.Formats.CLI.Signature.TypeSig[] args) {
			return new SpecializedTypeInfo(this, args);
		}

	}

}
