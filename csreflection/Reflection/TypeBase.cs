using System;
using System.Collections.Generic;
using CooS.Collections;
using CooS.Formats;
using AmbiguousMatchException = System.Reflection.AmbiguousMatchException;

namespace CooS.Reflection {

	public abstract class TypeBase : IGenericParameterize {

		private static int idseed = 1;
		private readonly int _id;

		protected TypeBase() {
			this._id = idseed++;
		}

		public abstract AssemblyBase Assembly {get;}
		public abstract int Id {get;}
		public abstract string Name {get;}
		public abstract string Namespace {get;}

		/// <summary>
		/// ���̌^�����̌^�p�����^�ł��邩�ǂ����������܂��B
		/// T �� true �ɂȂ�܂����AT[], List&lt;T&gt; �� false �ł��B
		/// </summary>
		public abstract bool IsGenericParam {get;}

		/// <summary>
		/// ���̌^����ꉻ����ۂɎQ�Ƃ���鑍�̌^�����̃\�[�X�������܂��B
		/// </summary>
		public virtual GenericSources GenericSource {
			get {
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// ���̑��̌^�ł̑��̌^�p�����^���X�g�ɂ����邱�̑��̌^�p�����^�̈ʒu���擾���܂��B
		/// </summary>
		public virtual int GenericParamPosition {
			get {
				throw new InvalidOperationException();
			}
		}

		#region IGenericParameterize �����o

		/// <summary>
		/// ���̌^�̒�`�ɑ��̌^�p�����^���܂܂�Ă��邩�ǂ��������܂��B
		/// T, T[], List&lt;T&gt; �ł͑S��true�ɂȂ�܂��B
		/// C&lt;T&gt;.D �ł� true �ɂȂ�܂��B
		/// </summary>
		public virtual bool ContainsGenericParameters {
			get {
				return this.GenericParameterCount>0;
			}
		}

		/// <summary>
		/// ���̌^�̒�`�ɑ��̌^�p�����^���܂܂�Ă��邩�ǂ��������܂��B
		/// T, T[], List&lt;T&gt; �ł͑S��true�ɂȂ�܂��B
		/// C&lt;T&gt;.D �ł� false �ɂȂ�܂��B
		/// </summary>
		public abstract bool HasGenericParameters { get; }

		/// <summary>
		/// ���̌^�����S�ɌŗL�����邽�߂ɕK�v�ȑ��̌^�p�����^�̌���Ԃ��܂��B
		/// C&lt;T&gt;.D&lt;X&gt; �ł� 2 �ɂȂ�܂��B
		/// </summary>
		public abstract int GenericParameterCount { get; }

		[Obsolete]
		public virtual bool IsClosedGeneric {
			get {
				return !this.ContainsGenericParameters;
			}
		}

		TypeBase[] genparamtypes = null;

		/// <summary>
		/// ���̌^�p�����^�̌^���擾���܂��B��������Ă��Ȃ��ꍇ�ɂ͑��̃p�����^�^��Ԃ��܂��B
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual TypeBase GetGenericArgumentType(int position) {
			if(!this.ContainsGenericParameters) { throw new InvalidOperationException(this.FullName); }
			if(this.genparamtypes==null) {
				this.genparamtypes = new TypeBase[this.GenericParameterCount];
			}
			if(this.genparamtypes[position]!=null) {
				return this.genparamtypes[position];
			} else {
				return this.genparamtypes[position] = new GenericParameterType(this.Assembly, CooS.Formats.GenericSources.Type, position);
			}
		}

		public TypeBase ResolveGenericParameterType(TypeBase type) {
			if(!type.ContainsGenericParameters) {
				// ���̌^�p�����^���܂�ł��Ȃ��B
				return type;
			} else if(!type.IsCompound) {
				// ���̌^�p�����^���܂ޒP���^
				if(!type.IsGenericParam) {
					return type;
				} else {
					switch(type.GenericSource) {
					case CooS.Formats.GenericSources.Type:
						return this.GetGenericArgumentType(type.GenericParamPosition);
					case CooS.Formats.GenericSources.Method:
						return type;
					default:
						throw new ArgumentException();
					}
				}
			} else {
				// ���̌^�p�����^���܂ލ����^
				TypeBase et = this.ResolveGenericParameterType(type.ElementType);
				Derived.DerivedType dt = (Derived.DerivedType)type;
				switch(dt.Kind) {
				case CompondKind.SzArray:
					return et.GetSzArrayType();
				case CompondKind.MnArray:
					throw new NotImplementedException();
				case CompondKind.ByRef:
					return et.GetByRefPointerType();
				case CompondKind.ByVal:
					return et.GetByValPointerType();
				case CompondKind.FnPtr:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
				}
			}
		}

		#endregion

		public abstract bool IsInterface { get; }
		public abstract bool IsAbstract { get; }
		public abstract bool IsSealed { get; }
		public abstract bool IsNested { get; }
		public abstract bool IsValueType { get; }
		public abstract bool IsEnum { get; }
		public abstract bool IsByRefPointer { get; }
		public abstract bool IsByValPointer { get; }

		public virtual bool IsPrimitive {
			get {
				return false;
			}
		}

		public virtual bool IsSigned {
			get {
				throw new NotSupportedException("Can't determine signed or unsigned: "+this.FullName);
			}
		}

		public virtual bool IsCompound {
			get {
				return IsArray || IsPointer;
			}
		}

		public virtual bool IsArray {
			get {
				return false;
			}
		}

		public virtual bool IsPointer {
			get {
				return this.IsByRefPointer || this.IsByValPointer;
			}
		}

		public virtual IntrinsicTypes IntrinsicType {
			get {
				return IntrinsicTypes.Any;
			}
		}

		public virtual int IntrinsicSize {
			get {
				return 0;
			}
		}

		public abstract TypeBase BaseType {get;}
		public abstract TypeBase ElementType {get;}
		public abstract TypeBase EnclosingType {get;}

		public string FullName {
			get {
				if(IsGenericParam) {
					return this.Name;
				} else if(IsNested) {
					return this.EnclosingType.FullName+"+"+this.Name;
				} else {
					string ns = this.Namespace;
					if(string.IsNullOrEmpty(ns)) {
						return this.Name;
					} else {
						return ns+"."+this.Name;
					}
				}
			}
		}

		public override string ToString() {
			if(!IsGenericParam && IsNested) {
				return this.EnclosingType.ToString()+"+"+this.Name;
			} else {
				return this.FullName;
			}
		}

		public abstract int GetArrayRank();

		public bool IsSubclassOf(TypeBase c) {
			if(c==null) {
				return false;
			} else if(c.IsInterface) {
				return false;
			} else {
				TypeBase x = this;
				do {
					x = x.BaseType;
					if(x==c) {
						return true;
					}
				} while(x!=null);
				return false;
			}
		}

		public bool IsAssignableFrom(TypeBase c) {
			if(c==null) { return false; }
			if(c==this) { return true; }
			if(this.IsInterface) {
				foreach(TypeBase t in c.EnumInterfaces()) {
					if(t==this) {
						return true;
					}
				}
				return false;
			} else {
				if(!this.IsArray || !c.IsArray) {
					return c.IsSubclassOf(this);
				} else {
					if(this.GetArrayRank()!=c.GetArrayRank()) {
						return false;
					} else {
						return this.ElementType.IsAssignableFrom(c.ElementType);
					}
				}
			}
		}

		TypeBase sztype = null;
		TypeBase mntype = null;
		TypeBase byref = null;
		TypeBase byval = null;

		public TypeBase GetSzArrayType() {
			if(this.sztype!=null) {
				return this.sztype;
			} else {
				return this.sztype = new Derived.SzArrayType(this.Assembly, this);
			}
		}

		public TypeBase GetMnArrayType(int rank) {
			throw new NotImplementedException();
		}

		public TypeBase GetByRefPointerType() {
			if(this.byref!=null) {
				return this.byref;
			} else {
				return this.byref = new Derived.ByRefPointerType(this.Assembly, this);
			}
		}

		public TypeBase GetByValPointerType() {
			if(this.byval!=null) {
				return this.byval;
			} else {
				return this.byval = new Derived.ByValPointerType(this.Assembly, this);
			}
		}

		public abstract int FieldCount { get; }
		public abstract int MethodCount { get; }
		public abstract IEnumerable<FieldBase> EnumFields();
		public abstract IEnumerable<MethodBase> EnumMethods();
		public abstract IEnumerable<TypeBase> EnumInterfaces();
		public abstract IEnumerable<TypeBase> EnumNestedTypes();

		class FindOneValue<T, NFE, AME>
			where T : class
			where NFE : Exception, new()
			where AME : Exception, new() {
			private T value;
			public T Value {
				get {
					/*
					if(this.value==default(T)) {
						throw new NFE();
					}
					*/
					return this.value;
				}
				set {
					if(this.value!=default(T)) {
						throw new AME();
					}
					this.value = value;
				}
			}
		}

		public TypeBase FindType(TypeRefDesc desc) {
			FindOneValue<TypeBase, TypeNotFoundException, AmbiguousMatchException> cand = new FindOneValue<TypeBase, TypeNotFoundException, AmbiguousMatchException>();
			foreach(TypeBase type in this.EnumNestedTypes()) {
				if(desc.IsMatch(type)) {
					cand.Value = type;
				}
			}
			return cand.Value;
		}

		public FieldBase FindField(MemberRefDesc desc) {
			FindOneValue<FieldBase, FieldNotFoundException, AmbiguousMatchException> cand = new FindOneValue<FieldBase,FieldNotFoundException,AmbiguousMatchException>();
			foreach(FieldBase field in this.EnumFields()) {
				if(desc.IsMatch(field)) {
					cand.Value = field;
				}
			}
			return cand.Value;
		}

		public MethodBase FindMethod(MemberRefDesc desc) {
			FindOneValue<MethodBase, MethodNotFoundException, AmbiguousMatchException> cand = new FindOneValue<MethodBase,MethodNotFoundException,AmbiguousMatchException>();
			foreach(MethodBase method in this.EnumMethods()) {
				if(desc.IsMatch(method)) {
					cand.Value = method;
				}
			}
			return cand.Value;
		}

		public MemberBase FindMember(MemberRefDesc desc) {
			MemberBase m = this.FindMethod(desc);
			MemberBase f = this.FindField(desc);
			if(m!=null && f!=null) { throw new AmbiguousMatchException(desc.ToString()); }
			if(m!=null) return m;
			if(f!=null) return f;
			return null;
		}

		public MethodBase[] FindMethods(MemberRefDesc desc) {
			CandidateList<MethodBase> cands = new CandidateList<MethodBase>();
			foreach(MethodBase method in this.EnumMethods()) {
				if(desc.IsMatch(method)) {
					cands.Add(method);
				}
			}
			return cands.ToArray();
		}

		public TypeBase FindType(string name) {
			TypeRefDesc desc = new TypeRefDesc();
			desc.name = name;
			return FindType(desc);
		}

		public FieldBase FindField(string name) {
			MemberRefDesc desc = new MemberRefDesc();
			desc.name = name;
			return FindField(desc);
		}

		public MethodBase FindMethod(string name) {
			MemberRefDesc desc = new MemberRefDesc();
			desc.name = name;
			return FindMethod(desc);
		}

		public MethodBase[] FindMethods(string name) {
			MemberRefDesc desc = new MemberRefDesc();
			desc.name = name;
			return FindMethods(desc);
		}

		public TypeBase FindNestedType(string name) {
			foreach(TypeBase child in this.EnumNestedTypes()) {
				if(child.Name==name) return child;
			}
			return null;
		}

		public TypeBase Bind(CooS.Reflection.Generics.InstantiatedType instantiatedType) {
			throw new Exception("The method or operation is not implemented.");
		}

		public abstract TypeBase Specialize(TypeBase[] args);
		public abstract TypeBase Instantiate(IGenericParameterize resolver);

	}

}
