using System;
using CooS.Reflection;

namespace CooS.CodeModels.Java {
	using Metadata;

#if JAVA

	public class MethodDefInfo : MethodInfoImpl {
	
		readonly method_info info;

		public MethodDefInfo(method_info method) {
			this.info = method;
		}

		#region NotImplemented

		public override AssemblyBase Assembly {
			get {
				throw new NotImplementedException();
			}
		}

		public override int DeclaringTypeIndex {
			get {
				throw new NotImplementedException();
			}
		}

		public override MethodInfoImpl[] GetCallings() {
			throw new NotImplementedException();
		}

		public override CodeInfo GetEntryPoint(CodeLevel level) {
			throw new NotImplementedException();
		}

		public override CodeInfo GetExecutable(CodeLevel level) {
			throw new NotImplementedException();
		}

		public override TypeImpl GetParameterType(int index) {
			throw new NotImplementedException();
		}

		public override Type GetVariableType(int index) {
			throw new NotImplementedException();
		}

		public override int ParameterCount {
			get {
				throw new NotImplementedException();
			}
		}

		public override int RowIndex {
			get {
				throw new NotImplementedException();
			}
		}

		public override int SlotIndex {
			get {
				throw new NotImplementedException();
			}
		}

		public override int VariableCount {
			get {
				throw new NotImplementedException();
			}
		}

		public override Type DeclaringType {
			get {
				throw new NotImplementedException();
			}
		}

		public override string Name {
			get {
				throw new NotImplementedException();
			}
		}

		public override Type ReflectedType {
			get {
				throw new NotImplementedException();
			}
		}

		public override System.Reflection.MethodAttributes Attributes {
			get {
				throw new NotImplementedException();
			}
		}

		public override System.Reflection.MethodImplAttributes GetMethodImplementationFlags() {
			throw new NotImplementedException();
		}

		public override System.Reflection.ParameterInfo[] GetParameters() {
			throw new NotImplementedException();
		}

		public override System.Reflection.MethodInfo GetBaseDefinition() {
			throw new NotImplementedException();
		}

		public override Type ReturnType {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsBlank {
			get {
				throw new NotImplementedException();
			}
		}

		#endregion

	}

#endif

}
