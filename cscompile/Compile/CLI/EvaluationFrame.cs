using System;
using CooS.Formats.CLI;
using CooS.Reflection.CLI;
using CooS.Execution;

namespace CooS.Compile.CLI {

	class EvaluationFrame : ICloneable {

		public readonly Compiler Compiler;
		public readonly MethodInfo Method;
		public EvaluationStack Stack;

		public EvaluationFrame(MethodInfo method, Compiler compiler) {
			this.Compiler = compiler;
			this.Method = method;
			this.Stack = new EvaluationStack();
		}

		public AssemblyImpl Assembly {
			get {
				return (AssemblyImpl)Method.Assembly;
			}
		}

		public TypeInfo Type {
			get {
				return this.Compiler.Engine.Realize(Method.Type);
			}
		}

		private EvaluationFrame(EvaluationFrame org) {
			this.Method = org.Method;
			this.Stack = (EvaluationStack)org.Stack.Clone();
		}

		#region ICloneable ÉÅÉìÉo

		public object Clone() {
			return new EvaluationFrame(this);
		}

		#endregion

	}

}
