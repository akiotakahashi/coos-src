using System;
using CooS.CodeModels.CLI.Metatype;

namespace CooS.CodeModels.CLI.Signature {

	abstract class MethodSig {
	
		protected MethodSig() {
		}

		public abstract bool HasThis {get;}
		public abstract int ParameterCount {get;}
		public abstract SuperType GetReturnType(AssemblyDef assembly);
		public abstract ParamSig[] GetParameters(AssemblyDef assembly);
		public abstract ParamSig GetParameter(int index, AssemblyDef assembly);

	}

}
