using System;
using CooS.Formats;
using CooS.Formats.CLI;
using CooS.Formats.CLI.IL;
using CooS.Formats.CLI.Metadata;
using System.Collections.Generic;
using CooS.Reflection.Generics;

namespace CooS.Reflection.CLI {
	
	public abstract class MethodImpl : MethodBase {

		protected readonly TypeImpl type;
		protected readonly ParamImpl[] parameters;

		internal MethodImpl(TypeImpl type, int paramcount) {
			this.type = type;
			this.parameters = new ParamImpl[paramcount];
		}

		public AssemblyImpl _Assembly {
			get {
				return type._Assembly;
			}
		}

		public TypeImpl _Type {
			get {
				return this.type;
			}
		}

		public override AssemblyBase Assembly {
			get {
				return type.Assembly;
			}
		}

		public override TypeBase Type {
			get {
				return this.type;
			}
		}

		internal TypeBase Realize(GenericParamInfo type) {
			switch(type.SourceType) {
			case GenericSources.Type:
				return this._Type.Realize(type);
			case GenericSources.Method:
				return this.GetGenericArgumentType(type.Number);
			default:
				throw new UnexpectedException();
			}
		}

		internal TypeBase Realize(TypeDeclInfo type) {
			if(type==null) {
				return null;
			} else if(type is GenericParamInfo) {
				return this.Realize((GenericParamInfo)type);
			} else {
				return this._Assembly.Realize(type);
			}
		}

		internal ParamBase Realize(ParameterDefInfo param) {
			if(parameters[param.Sequence-1]==null) {
				parameters[param.Sequence-1] = new ParamImpl(this, param);
			}
			return parameters[param.Sequence-1];
		}

		private SpecializedList<SpecializedMethod> specialized;

		public override MethodBase Specialize(TypeBase[] args) {
			if(!this.ContainsGenericParameters) {
				return this;
			} else {
				SpecializedMethod value;
				if(specialized.TryGetValue(args, out value)) {
					return value;
				} else {
					return specialized[args] = new SpecializedMethod(this, args);
				}
			}
		}

		public override MethodBase Instantiate(IGenericParameterize resolver) {
			return this;
		}

		public override MethodBase Specialize(IGenericParameterize resolver) {
			TypeBase[] args = new TypeBase[resolver.GenericParameterCount];
			for(int i=0; i<args.Length; ++i) {
				args[i] = resolver.GetGenericArgumentType(i);
			}
			return this.Specialize(args);
		}

	}

}
