using System;
using System.Collections.Generic;
using CooS.Collections;
using CooS.Formats;

namespace CooS.Reflection {
	
	public class GenericParameterType : TypeBase {

		private readonly AssemblyBase assembly;
		private readonly GenericSources source;
		private readonly int position;

		public GenericParameterType(AssemblyBase assembly, GenericSources source, int position) {
			this.assembly = assembly;
			this.source = source;
			this.position = position;
		}

		public override AssemblyBase Assembly {
			get {
				return this.assembly;
			}
		}

		public override int Id {
			get {
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override string Name {
			get {
				switch(this.source) {
				case GenericSources.Type:
					return "!"+this.position;
				case GenericSources.Method:
					return "!!"+this.position;
				default:
					throw new UnexpectedException();
				}
			}
		}

		public override bool HasGenericParameters {
			get {
				return false;
			}
		}

		public override bool IsClosedGeneric {
			get {
				return false;
			}
		}

		public override int GenericParameterCount {
			get {
				return 0;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return true;
			}
		}

		public override bool IsGenericParam {
			get {
				return true;
			}
		}

		public override GenericSources GenericSource {
			get {
				return this.source;
			}
		}

		public override int GenericParamPosition {
			get {
				return this.position;
			}
		}

		public override string Namespace {
			get {
				return null;
			}
		}

#if f
		public override TypeBase Specialize(IResolveGenericParameterType resolver) {
			switch(this.source) {
			case GenericSources.Type:
				return resolver.GetTypeArgument(this.position);
			case GenericSources.Method:
				return resolver.GetMethodArgument(this.position);
			default:
				throw new UnexpectedException();
			}
		}
#endif

		public override bool IsInterface {
			get {
				throw new NotSupportedException();
			}
		}

		public override bool IsAbstract {
			get {
				throw new NotSupportedException();
			}
		}

		public override bool IsSealed {
			get {
				throw new NotSupportedException();
			}
		}

		public override bool IsNested {
			get {
				throw new NotSupportedException();
			}
		}

		public override bool IsValueType {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsEnum {
			get {
				throw new NotImplementedException();
			}
		}

		public override bool IsByRefPointer {
			get {
				return false;
			}
		}

		public override bool IsByValPointer {
			get {
				return false;
			}
		}

		public override TypeBase BaseType {
			get {
				throw new NotImplementedException();
			}
		}

		public override TypeBase ElementType {
			get {
				return null;
			}
		}

		public override TypeBase EnclosingType {
			get {
				return null;
			}
		}

		public override int GetArrayRank() {
			throw new InvalidOperationException();
		}

		public override int FieldCount {
			get {
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override int MethodCount {
			get {
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public override IEnumerable<FieldBase> EnumFields() {
			throw new NotImplementedException();
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			throw new NotImplementedException();
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			throw new NotImplementedException();
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			throw new NotSupportedException();
		}

		public override TypeBase Specialize(TypeBase[] args) {
			throw new NotSupportedException();
		}

		public override TypeBase Instantiate(IGenericParameterize resolver) {
			return resolver.ResolveGenericParameterType(this);
		}

	}

}
