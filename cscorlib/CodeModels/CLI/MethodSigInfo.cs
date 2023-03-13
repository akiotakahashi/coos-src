using System;
using CooS.Reflection;
using System.Reflection;

namespace CooS.CodeModels.CLI {
	using Metadata;
	using Signature;

	class MethodSigInfo : MethodInfoImpl {

		protected readonly AssemblyDef assembly;
		protected readonly MethodSig methodSig;

		public MethodSigInfo(AssemblyDef assembly, int rowIndex) {
			this.assembly = assembly;
			this.methodSig = new MethodSigImpl(assembly.Metadata.Blob.OpenReader(rowIndex));
		}

		public override AssemblyBase Assembly {
			get {
				return this.assembly;
			}
		}

		public AssemblyDef MyAssembly {
			get {
				return this.assembly;
			}
		}

		public override int RowIndex {
			get {
				throw new NotSupportedException();
			}
		}

		public override Type ReturnType {
			get {
				return this.methodSig.GetReturnType(this.assembly);
			}
		}

		#region ÉpÉâÉÅÉ^

		public override int ParameterCount {
			get {
				return this.methodSig.ParameterCount;
			}
		}

		public override TypeImpl GetParameterType(int index) {
			return this.methodSig.GetParameter(index,this.assembly).Type.ResolveTypeAt(this.assembly);
		}

		public override ParameterInfo[] GetParameters() {
			throw new NotSupportedException();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture) {
			if(binder==null) {
				return Engine.Invoke(this, obj, parameters);
			} else {
				object state;
				binder.BindToMethod(invokeAttr, new MethodBase[]{this}, ref parameters, null, culture, null, out state);
				object retval = Engine.Invoke(this, obj, parameters);
				binder.ReorderArgumentArray(ref parameters, state);
				return retval;
			}
		}

		#endregion

		public override string Name {
			get {
				throw new NotSupportedException();
			}
		}

		public override bool IsBlank {
			get {
				return true;
			}
		}

		public override void MakeBlank() {
			// NOP
		}

		public override MethodAttributes Attributes {
			get {
				return this.methodSig.HasThis ? (MethodAttributes)0 : MethodAttributes.Static;
			}
		}

		public override Type DeclaringType {
			get {
				throw new NotSupportedException();
			}
		}

		public override Type ReflectedType {
			get {
				throw new NotSupportedException();
			}
		}

		public override MethodInfo GetBaseDefinition() {
			throw new NotSupportedException();
		}

		public override MethodImplAttributes GetMethodImplementationFlags() {
			throw new NotSupportedException();
		}

		#region ã«èäïœêî

		public override int VariableCount {
			get {
				throw new NotSupportedException();
			}
		}

		public override Type GetVariableType(int index) {
			throw new NotSupportedException();
		}

		#endregion

		public override int DeclaringTypeIndex {
			get {
				throw new NotSupportedException();
			}
		}

		public override int SlotIndex {
			get {
				throw new NotSupportedException();
			}
		}

		public override MethodInfoImpl[] GetCallings() {
			throw new NotSupportedException();
		}

		public override CodeInfo GenerateCallableCode(CodeLevel level) {
			throw new NotSupportedException();
		}

		public override CodeInfo GenerateExecutableCode(CodeLevel level) {
			throw new NotSupportedException();
		}

	}

}
