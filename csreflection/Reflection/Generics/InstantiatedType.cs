using System;
using System.Collections.Generic;
using MethodAttributes=System.Reflection.MethodAttributes;

namespace CooS.Reflection.Generics {
	
	/// <summary>
	/// 特殊化された包含クラスと束縛されたクラスを表します。
	/// </summary>
	public sealed class InstantiatedType : GenericTypeBase {

		private IGenericParameterize resolver;

		internal InstantiatedType(GenericTypeBase generic, IGenericParameterize resolver) : base(generic) {
			if(!resolver.ContainsGenericParameters) { throw new ArgumentException("総称型解決子が総称パラメタを持っていません。"); }
			this.resolver = resolver;
		}

		public override TypeBase EnclosingType {
			get {
				TypeBase enc = this.master.EnclosingType;
				if(enc==null) { return null; }
				return enc.Instantiate(this.resolver);
			}
		}

		public override TypeBase GetGenericArgumentType(int index) {
			return this.resolver.ResolveGenericParameterType(base.GetGenericArgumentType(index));
		}

		public override TypeBase BaseType {
			get {
				return this.resolver.ResolveGenericParameterType(this.master.BaseType);
			}
		}

		public override bool IsClosedGeneric {
			get {
				return true;
			}
		}

		public override bool IsGenericParam {
			get {
				return false;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return false;
			}
		}

		public override IEnumerable<FieldBase> EnumFields() {
			foreach(FieldBase field in this.master.EnumFields()) {
				yield return field.Bind(this);
			}
		}

		public override IEnumerable<MethodBase> EnumMethods() {
			foreach(MethodBase method in this.master.EnumMethods()) {
				yield return method.Bind(this);
			}
		}

		public override IEnumerable<TypeBase> EnumInterfaces() {
			foreach(TypeBase type in this.master.EnumInterfaces()) {
				yield return type.Bind(this);
			}
		}

		public override IEnumerable<TypeBase> EnumNestedTypes() {
			foreach(TypeBase type in this.master.EnumNestedTypes()) {
				yield return type.Bind(this);
			}
		}

	}

}
