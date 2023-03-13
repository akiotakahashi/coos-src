using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Formats.CLI {
	using Metadata;
	using Metadata.Rows;

	class TypeRefInfo : TypeDeclInfo, IResolutionScope, IMemberRefParent {

		private TypeRefRow row;
		private IResolutionScope res;

		internal TypeRefInfo(AssemblyDefInfo assembly, TypeRefRow row) : base(assembly) {
			this.row = row;
			this.res = assembly.GetResolutionScope(row.ResolutionScope);
		}

		protected internal TypeRefInfo(AssemblyDefInfo assembly) : base(assembly) {
		}

		public override int RowIndex {
			get {
				return this.row.Index;
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
				return row.ResolutionScope.TableId==TableId.TypeRef;
			}
		}

		public virtual IResolutionScope ResolutionScope {
			get {
				return this.res;
			}
		}

	}

}
