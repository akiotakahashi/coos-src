using System;
using System.Reflection;
using CooS.Reflection;
using CooS.CodeModels;

namespace CooS.CodeModels.CLI {
	using Metatype;

#if false
	class MethodRefInfo : MethodInfoImpl {

		SuperType reflecttype;
		MethodDefInfo basemethod;

		public MethodRefInfo(SuperType owner, MethodDefInfo basemethod) {
			this.reflecttype = owner;
			this.basemethod = basemethod;
		}

		public override AssemblyBase Assembly {
			get {
				return this.basemethod.Assembly;
			}
		}
	
		protected override RuntimeMethodHandle GenerateNewHandle(MethodInfoImpl method) {
			return this.basemethod.Handle;
		}

		public override MethodInfoImpl[] GetCallings() {
			return this.basemethod.GetCallings();
		}

		public override int ParameterCount {
			get {
				return this.basemethod.ParameterCount;
			}
		}

		public override Type ReflectedType {
			get {
				return this.reflecttype;
			}
		}

		public override int DeclaringTypeIndex {
			get {
				return basemethod.DeclaringTypeIndex;
			}
		}

		public override Type DeclaringType {
			get {
				return basemethod.DeclaringType;
			}
		}
	
		public override Type ReturnType {
			get {
				return this.basemethod.ReturnType;
			}
		}

		public override string Name {
			get {
				return basemethod.Name;
			}
		}
	
		public override MethodAttributes Attributes {
			get {
				return basemethod.Attributes;
			}
		}

		public override MethodInfo GetBaseDefinition() {
			return this.basemethod;
		}

		public override MethodImplAttributes GetMethodImplementationFlags() {
			return basemethod.GetMethodImplementationFlags();
		}

		public override TypeImpl GetParameterType(int index) {
			return this.basemethod.GetParameterType(index);
		}

		public override ParameterInfo[] GetParameters() {
			return this.basemethod.GetParameters();
		}

		public override int VariableCount {
			get {
				return this.basemethod.VariableCount;
			}
		}

		public override Type GetVariableType(int index) {
			return this.basemethod.GetVariableType(index);
		}

		public override int RowIndex {
			get {
				return this.basemethod.RowIndex;
			}
		}

		public override CodeInfo GenerateCallableCode(CodeLevel level) {
			return this.basemethod.GenerateCallableCode(level);
		}

		public override CodeInfo GenerateExecutableCode(CodeLevel level) {
			return this.basemethod.GenerateExecutableCode(level);
		}

		public override bool IsBlank {
			get {
				return this.basemethod.IsBlank;
			}
		}

		public override void MakeBlank() {
			this.basemethod.MakeBlank();
		}

		public override int SlotIndex {
			get {
				return this.basemethod.SlotIndex;
			}
		}

	}

#endif

}
