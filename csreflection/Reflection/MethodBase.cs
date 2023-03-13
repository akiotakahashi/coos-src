using System;
using CooS.Reflection.Generics;
using System.Collections.Generic;
using MemberTypes=System.Reflection.MemberTypes;

namespace CooS.Reflection {

	public abstract class MethodBase : MemberBase, IGenericParameterize {

		public abstract AssemblyBase Assembly {
			get;
		}

		public abstract int Id {
			get;
		}

		public override MemberTypes Kind {
			get { return MemberTypes.Method; }
		}

		public abstract bool IsStatic {
			get;
		}

		/// <summary>
		/// 総称メソッドであるか示します。
		/// C&lt;T&gt;:f() は false を返します。
		/// </summary>
		[Obsolete]
		public abstract bool IsGenericMethod {
			get;
		}

		public abstract bool IsConstructor {
			get;
		}

		public abstract bool IsBlank {
			get;
		}

		public abstract bool IsVirtual {
			get;
		}

		public abstract bool HasNewSlot {
			get;
		}

		public abstract System.Reflection.MethodImplAttributes ImplFlags {
			get;
		}

		public abstract TypeBase ReturnType {
			get;
		}

		public bool HasReturnValue {
			get {
				return this.ReturnType!=this.Assembly.World.Resolve(PrimitiveTypes.Void);
			}
		}

		public abstract int ParameterCount {
			get;
		}

		public abstract TypeBase GetParameterType(int index);
		public abstract IEnumerable<ParamBase> EnumParameterInfo();

		public IEnumerable<TypeBase> EnumParameterType() {
			for(int i=0; i<this.ParameterCount; ++i) {
				yield return this.GetParameterType(i);
			}
		}

		public int ArgumentCount {
			get {
				if(IsStatic) {
					return this.ParameterCount;
				}
				return this.ParameterCount+1;
			}
		}

		public IEnumerable<TypeBase> EnumArguments() {
			if(!this.IsStatic) {
				yield return this.Type;
			}
			foreach(TypeBase type in this.EnumParameterType()) {
				yield return type;
			}
		}

		public TypeBase GetArgumentType(int index) {
			if(this.IsStatic) {
				return this.GetParameterType(index);
			} else {
				if(index==0) {
					return this.Type;
				} else {
					return this.GetParameterType(index-1);
				}
			}
		}

		public abstract int VariableCount {
			get;
		}

		public abstract TypeBase GetVariableType(int index);

		public abstract IEnumerable<object> EnumInstructions();

		public MethodBase FindWrappingMethod() {
			TypeBase wraptype = this.Assembly.World.ResolveType("_"+this.Type.Name, "CooS.Wrap._"+this.Type.Namespace);
			if(wraptype==null) { return null; }
			MemberRefDesc desc = new MemberRefDesc(this);
			if(this.IsConstructor) { desc.name = "InternalAllocateInstance"; }
			desc.returntype = null;
			return wraptype.FindMethod(desc);
		}

		#region IGenericParameterize メンバ

		public virtual bool ContainsGenericParameters {
			get {
				return this.GenericParameterCount>0;
			}
		}

		public virtual bool HasGenericParameters {
			get {
				return this.GenericParameterCount>0;
			}
		}

		[Obsolete]
		public virtual bool IsClosedGeneric {
			get {
				return this.Type.IsClosedGeneric && !this.ContainsGenericParameters;
			}
		}

		public abstract int GenericParameterCount { get; }

		TypeBase[] genparamtypes = null;

		public virtual TypeBase GetGenericArgumentType(int position) {
			if(!this.ContainsGenericParameters) { throw new InvalidOperationException(this.FullName); }
			if(this.genparamtypes==null) {
				this.genparamtypes = new TypeBase[this.GenericParameterCount];
			}
			if(this.genparamtypes[position]!=null) {
				return this.genparamtypes[position];
			} else {
				return this.genparamtypes[position] = new GenericParameterType(this.Assembly, CooS.Formats.GenericSources.Method, position);
			}
		}

		public TypeBase ResolveGenericParameterType(TypeBase type) {
			if(!type.IsGenericParam) {
				return type;
			} else {
				switch(type.GenericSource) {
				case CooS.Formats.GenericSources.Type:
					return this.Type.ResolveGenericParameterType(type);
				case CooS.Formats.GenericSources.Method:
					return this.GetGenericArgumentType(type.GenericParamPosition);
				default:
					throw new ArgumentException();
				}
			}
		}

		#endregion

		public MethodBase Bind(InstantiatedType instantiatedType) {
			throw new Exception("The method or operation is not implemented.");
		}

		public abstract MethodBase Specialize(TypeBase[] args);

		/// <summary>
		/// 宣言に基づいて総称型パラメタを束縛します。
		/// C&lt;T&gt;を束縛すると、C&lt;int&gt;もしくはC&lt;!0&gt;などになります。
		/// </summary>
		/// <param name="resolver"></param>
		/// <returns></returns>
		public abstract MethodBase Instantiate(IGenericParameterize resolver);

		/// <summary>
		/// スコープに基づいて総称型パラメタを束縛します。
		/// f&lt;int&gt;()におけるC&lt;!!0&gt;をC&lt;int&gt;にします。
		/// </summary>
		/// <param name="resolver"></param>
		/// <returns></returns>
		public abstract MethodBase Specialize(IGenericParameterize resolver);

	}

}
