using System;
using CooS.Formats;
using CooS.Formats.CLI;
using CooS.Formats.CLI.IL;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;
using CooS.Reflection.Generics;

namespace CooS.Reflection.CLI {
	
	public sealed class FnPtrImpl : TypeBase {

		private readonly AssemblyImpl assembly;
		private readonly FnPtrInfo entity;
		//protected readonly ParamImpl[] parameters;

		internal FnPtrImpl(AssemblyImpl assembly, FnPtrInfo entity) {
			this.assembly = assembly;
			this.entity = entity;
			//this.parameters = new ParamImpl[entity.Signature.ParameterCount];
		}

		public AssemblyImpl _Assembly {
			get {
				return this.assembly;
			}
		}

		public override AssemblyBase Assembly {
			get {
				return this.assembly;
			}
		}

		public override int Id {
			get {
				return this.entity.RowIndex;
			}
		}

		public override string Name {
			get {
				return this.entity.Name;
			}
		}

		public override string Namespace {
			get { return null; }
		}

		public override int GenericParameterCount {
			get {
				return this.entity.GenericParameterCount;
			}
		}

		public override bool IsGenericParam {
			get { return false; }
		}

		public override bool HasGenericParameters {
			get { return false; }
		}

		public override bool IsInterface {
			get { return false; }
		}

		public override bool IsAbstract {
			get { return false; }
		}

		public override bool IsSealed {
			get { return true; }
		}

		public override bool IsNested {
			get { return false; }
		}

		public override bool IsValueType {
			get { return true; }
		}

		public override bool IsEnum {
			get { return false; }
		}

		public override bool IsByRefPointer {
			get { return false; }
		}

		public override bool IsByValPointer {
			get { return true; }
		}

		public override TypeBase BaseType {
			get { return null; }
		}

		public override TypeBase ElementType {
			get { return null; }
		}

		public override TypeBase EnclosingType {
			get { return null; }
		}

		public override int GetArrayRank() {
			throw new NotSupportedException();
		}

		public override int FieldCount {
			get { return 0; }
		}

		public override int MethodCount {
			get { return 0; }
		}

		public override IEnumerable<FieldBase> EnumFields() {
			yield break;
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			yield break;
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			yield break;
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			yield break;
		}

		public override TypeBase Specialize(TypeBase[] args) {
			return this;
		}

		public override TypeBase Instantiate(IGenericParameterize resolver) {
			return this;
		}
	}

}
