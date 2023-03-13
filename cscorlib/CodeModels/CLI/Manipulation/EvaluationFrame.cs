using System;
using CooS.Reflection;

namespace CooS.CodeModels.CLI.Manipulation {

	class EvaluationFrame : ICloneable {

		public readonly AssemblyDef Assembly;
		public readonly TypeImpl Type;
		public readonly MethodDefInfo Method;
		public EvaluationStack Stack;

		public EvaluationFrame(MethodDefInfo mi) {
			this.Method = mi;
			this.Type = (TypeImpl)mi.DeclaringType;
			this.Assembly = (AssemblyDef)this.Type.AssemblyInfo;
			this.Stack = new EvaluationStack();
		}

		private EvaluationFrame(EvaluationFrame org) {
			this.Method = org.Method;
			this.Type = org.Type;
			this.Assembly = org.Assembly;
			this.Stack = (EvaluationStack)org.Stack.Clone();
		}

		#region ICloneable ÉÅÉìÉo

		public object Clone() {
			return new EvaluationFrame(this);
		}

		#endregion

	}

}
