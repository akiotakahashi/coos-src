using System;
using CooS.CodeModels.CLI.Metadata;
using CooS.Reflection;
using CooS.CodeModels.CLI.Signature;

namespace CooS.CodeModels.CLI {

	class ParameterDefInfo : ParameterInfoImpl {

		public ParameterDefInfo(MethodDefInfo method, ParamSig signature, ParamRow row) {
			this.MemberImpl = method;
			this.ClassImpl = signature.Type.ResolveTypeAt(method.MyAssembly);
			if(row!=null) {
				this.AttrsImpl = row.Flags;
				//TODO: this.DefaultValueImpl
				this.NameImpl = row.Table.Heap.Root.Strings[row.Name];
				this.PositionImpl = row.Sequence;
			}
		}

	}

}
