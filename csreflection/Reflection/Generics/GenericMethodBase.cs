using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {

	public abstract class GenericMethodBase : MethodBase {

		protected readonly MethodBase master;

		protected GenericMethodBase(MethodBase method) {
			this.master = method;
		}

		#region IGenericParameterize ÉÅÉìÉo

		public override bool ContainsGenericParameters {
			get {
				return true;
			}
		}

		public override int GenericParameterCount {
			get {
				return this.master.GenericParameterCount;
			}
		}

		public override TypeBase GetGenericArgumentType(int index) {
			return this.GetGenericArgumentType(index);
		}

		#endregion

		[Obsolete]
		public override bool IsGenericMethod {
			get {
				return this.ContainsGenericParameters;
			}
		}
		
		public override TypeBase ReturnType {
			get {
				return this.BaseMethod.ReturnType;
			}
		}

		public override TypeBase GetParameterType(int index) {
			return this.BaseMethod.GetParameterType(index);
		}

		public override TypeBase GetVariableType(int index) {
			return this.BaseMethod.GetVariableType(index);
		}

		public override IEnumerable<object> EnumInstructions() {
			return this.master.EnumInstructions();
		}

		public MethodBase BaseMethod {
			get {
				return this.master;
			}
		}

		public override AssemblyBase Assembly {
			get { return this.master.Assembly; }
		}

		public override TypeBase Type {
			get {
				return this.master.Type;
			}
		}

		public override string Name {
			get {
				return this.BaseMethod.Name;
			}
		}

		public override int Id {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsStatic {
			get {
				return this.BaseMethod.IsStatic;
			}
		}

		public override bool IsConstructor {
			get {
				return this.BaseMethod.IsConstructor;
			}
		}

		public override bool IsBlank {
			get {
				return this.BaseMethod.IsBlank;
			}
		}

		public override bool IsVirtual {
			get {
				return this.BaseMethod.IsVirtual;
			}
		}

		public override bool HasNewSlot {
			get {
				return this.BaseMethod.HasNewSlot;
			}
		}

		public override System.Reflection.MethodImplAttributes ImplFlags {
			get {
				return this.BaseMethod.ImplFlags;
			}
		}

		public override int ParameterCount {
			get {
				return this.BaseMethod.ParameterCount;
			}
		}

		public override IEnumerable<ParamBase> EnumParameterInfo() {
			return this.BaseMethod.EnumParameterInfo();
		}

		public override int VariableCount {
			get {
				return this.BaseMethod.VariableCount;
			}
		}

		public sealed override MethodBase Specialize(TypeBase[] args) {
			return this.master.Specialize(args);
		}

		private SpecializedList<InstantiatedMethod> instantiated;

		public override MethodBase Instantiate(IGenericParameterize resolver) {
			if(!this.ContainsGenericParameters) {
				return this;
			} else {
				InstantiatedMethod value;
				if(instantiated.TryGetValue(resolver, out value)) {
					return value;
				} else {
					return instantiated[resolver] = new InstantiatedMethod(this, resolver);
				}
			}
		}

		public override MethodBase Specialize(IGenericParameterize resolver) {
			return this.master.Specialize(resolver);
		}

	}

}
