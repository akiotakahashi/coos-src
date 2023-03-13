using System;
using CooS.Reflection;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI {
	using Metatype;

	/// <summary>
	/// このクラスはCLIの型情報を表します。
	/// </summary>
	abstract class SuperType : TypeImpl {

		AssemblyDef assembly;
		SzArrayType szarraytype;
		//MnArrayType[] mnarraytypes;
		ByRefPointerType byrefptrtype;
		ByValPointerType byvalptrtype;

		protected SuperType(AssemblyDef assembly) {
			if(assembly==null) throw new ArgumentNullException();
			this.assembly = assembly;
		}
		
		public virtual void CompleteSetup() {
			// NOP
		}

		public override AssemblyBase AssemblyInfo {
			get {
				return this.assembly;
			}
		}

		public AssemblyDef MyAssembly {
			get {
				return this.assembly;
			}
		}

		public override TypeImpl GetSzArrayType() {
			if(this.szarraytype==null) {
				this.szarraytype = new SzArrayType(assembly, this);
			}
			return this.szarraytype;
		}

		public override TypeImpl GetMnArrayType(int dimension) {
			throw new NotImplementedException();
		}

		public override TypeImpl GetByRefPointerType() {
			if(this.byrefptrtype==null) {
				this.byrefptrtype = new ByRefPointerType(assembly, this);
			}
			return this.byrefptrtype;
		}

		public override TypeImpl GetByValPointerType() {
			if(this.byvalptrtype==null) {
				this.byvalptrtype = new ByValPointerType(assembly, this);
			}
			return this.byvalptrtype;
		}

	}

}
