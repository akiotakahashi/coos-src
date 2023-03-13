using System;
using FieldAttributes=System.Reflection.FieldAttributes;

namespace CooS.Formats.CLI {

	abstract class FieldDeclInfo : MemberDeclInfo {

		protected FieldDeclInfo(AssemblyDefInfo assembly) : base(assembly) {
		}

		public override System.Reflection.MemberTypes Kind {
			get {
				return System.Reflection.MemberTypes.Field;
			}
		}

		public string FullName {
			get {
				return this.Type.FullName+":"+this.Name;
			}
		}

	}

}
