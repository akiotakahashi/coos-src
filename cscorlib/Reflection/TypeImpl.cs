using System;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using CooS.Collections;

namespace CooS.Reflection {

	public struct InterfaceBase {
		public RuntimeTypeHandle Handle;
		public int BaseIndex;
	}

	/// <summary>
	/// このクラスはVES非依存の型情報を表します。
	/// </summary>
	public abstract class TypeImpl : Type {
		
		// This field, _handle, is referred by name.
		// Keep it sync with mms.cpp in kernel.dll.
		private RuntimeTypeHandle _handle;
		private MethodInfoImpl[] slots;
		private InterfaceBase[] ifbases;

		protected TypeImpl() {
			_handle = new RuntimeTypeHandle();
		}

		public sealed override string ToString() {
			return "Type: "+this.FullName;
		}

		/***********************************************************************************************
		 *		Specialized Members
		 ***********************************************************************************************/

		public abstract int RowIndex {get;}

		public abstract AssemblyBase AssemblyInfo {get;}
		//		public abstract string Name {get;}
		//		public abstract string Namespace {get;}

		//		public abstract Type BaseType {get;}
		public abstract bool IsNested {get;}

		//		public abstract bool IsValueType {get;}
		//		public abstract bool IsPrimitive {get;}
		//		public abstract bool IsArray {get;}
		public abstract bool IsByRefPointer {get;}
		public abstract bool IsByValPointer {get;}

		// These below are redefined since original ones refers runtime type.
		public sealed override string FullName {
			get {
				if(this.IsNested) {
					return this.DeclaringType.FullName+"+"+this.Name;
				} else {
					return this.Namespace+"."+this.Name;
				}
			}
		}

		public abstract int InstanceSize {get;}			// ヒープ上でのデータサイズ
		public abstract int VariableSize {get;}			// 変数のサイズ
		public abstract int StaticSize {get;}			// 静的データのサイズ
		public abstract int OffsetToContents {get;}		// ポインタからデータまでのオフセット
		public abstract TypeAttributes AttributeFlags {get;}

		public abstract TypeImpl GetSzArrayType();
		public abstract TypeImpl GetByRefPointerType();
		public abstract TypeImpl GetByValPointerType();
		public abstract TypeImpl GetMnArrayType(int dimension);

		public abstract IEnumerable DeclaredConstructors {get;}
		public abstract IEnumerable DeclaredFields {get;}
		public abstract IEnumerable DeclaredMethods {get;}
		public abstract IEnumerable DeclaredPeoperties {get;}
		public abstract IEnumerable DeclaredEvents {get;}
		public abstract IEnumerable DeclaredNestedTypes {get;}

		public abstract IEnumerable ImplementedInterfaces {get;}

		#region スロット

		public abstract MethodInfoImpl[] ConstructSlots(out InterfaceBase[] ifbases);

		public void PrepareSlots() {
			if(this.slots==null) {
				this.slots = this.ConstructSlots(out this.ifbases);
				if(this.slots==null) throw new UnexpectedException();
			}
		}

		public bool SlotAvailable {
			get {
				return this.slots!=null;
			}
		}

		public int TotalSlotCount {
			get {
				this.PrepareSlots();
				return this.slots.Length;
			}
		}

		public MethodInfoImpl GetSlotMethod(int slotIndex) {
			if(this.slots==null) this.PrepareSlots(); // たぶん要らないはず…
			return this.slots[slotIndex];
		}

		#endregion

		#region インターフェイスマップ

		public int GetInterfaceBaseIndex(RuntimeTypeHandle handle) {
			this.PrepareSlots();
			for(int i=0; i<this.ifbases.Length; ++i) {
				if(this.ifbases[i].Handle.Value==handle.Value) {
					return this.ifbases[i].BaseIndex;
				}
			}
			throw new NotImplementedException();
		}

		public override InterfaceMapping GetInterfaceMap(Type interfaceType) {
			if(interfaceType==null) throw new ArgumentNullException();
			this.PrepareSlots();
			RuntimeTypeHandle handle = interfaceType.TypeHandle;
			for(int i=0; i<this.ifbases.Length; ++i) {
				if(this.ifbases[i].Handle.Value==handle.Value) {
					int baseidx = this.ifbases[i].BaseIndex;
					MethodInfo[] methods = interfaceType.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
					InterfaceMapping ifmap;
					ifmap.TargetType = this;
					ifmap.InterfaceType = interfaceType;
					ifmap.InterfaceMethods = methods;
					ifmap.TargetMethods = new MethodInfoImpl[methods.Length];
					int j = 0;
					foreach(MethodInfoImpl method in methods) {
						ifmap.TargetMethods[j++] = this.GetSlotMethod(baseidx+method.SlotIndex);
					}
					return ifmap;
				}
			}
			throw new ArgumentException();
		}

		#endregion

		/***********************************************************************************************
		 *		Identify
		 ***********************************************************************************************/

		public sealed override bool Equals(object obj) {
			if(obj==null) return false;
			Type type1 = obj as Type;
			if(type1==null) return false;
			return this==type1 || this.FullName==type1.FullName;;
		}

		public sealed override int GetHashCode() {
			return this.FullName.GetHashCode();
		}

		/***********************************************************************************************
		 *		Type Members
		 ***********************************************************************************************/

		#region RuntimeTypeHandle関係

		static Inttable handletable = new Inttable();
		static int handleseed = 1;

		private static unsafe RuntimeTypeHandle GenerateNewHandle(TypeImpl type) {
			int handle;
			for(;;) {
				handle = handleseed++;
				if(!handletable.ContainsKey(handle)) {
					break;
				}
			}
			handletable[handle] = type;
			return *(RuntimeTypeHandle*)&handle;
		}

		public RuntimeTypeHandle Handle {
			get {
				if(this._handle.Value==IntPtr.Zero) {
					this._handle = GenerateNewHandle(this);
				}
				return this._handle;
			}
		}

		public override sealed RuntimeTypeHandle TypeHandle {
			get {
				return this.Handle;
			}
		}

		protected void SetTypeHandle(RuntimeTypeHandle handle) {
			handletable[handle.Value.ToInt32()] = this;
			this._handle = handle;
		}

		public static TypeImpl FindTypeFromHandle(RuntimeTypeHandle handle) {
			return (TypeImpl)handletable[handle.Value.ToInt32()];
		}

		public static TypeImpl FindTypeFromHandle(IntPtr handle) {
			return (TypeImpl)handletable[handle.ToInt32()];
		}

		#endregion

		protected sealed override bool IsPointerImpl() {
			return this.IsByRefPointer || this.IsByValPointer;
		}

		/***********************************************************************************************
		 *		Type Implementing Members
		 ***********************************************************************************************/

		public sealed override Assembly Assembly {
			get {
				return this.AssemblyInfo.RealAssembly;
			}
		}

		public sealed override bool IsSubclassOf(Type c) {
			if(c==null) return false;
			if(c.IsInterface) return false;
			Type x = this;
			do {
				x = x.BaseType;
				if(x==c) return true;
			} while(x!=null);
			return false;
		}

		public override bool IsAssignableFrom(Type c) {
			if(c==null) return false;
			if(c==this) return true;
			if(!this.IsArray || !c.IsArray) {
				return c.IsSubclassOf(this);
			} else {
				if(this.GetArrayRank()!=c.GetArrayRank()) {
					return false;
				} else {
					return this.GetElementType().IsAssignableFrom(c.GetElementType());
				}
			}
		}
 
		public override bool IsInstanceOfType(object o) {
			if(o==null) return false;
			TypeImpl type = (TypeImpl)o.GetType();
			return this.IsAssignableFrom(type);
		}

		protected sealed override TypeAttributes GetAttributeFlagsImpl() {
			return this.AttributeFlags;
		}

		#region Type メンバ

		static Binder default_binder;
		
		new static Binder DefaultBinder {
			get {
				if(default_binder==null) {
					default_binder = new CooS.CodeModels.CLI.StandardBinder();
				}
				return default_binder;
			}
		}

		static bool IsBindable(MethodBase method, BindingFlags bindingAttr) {
			BindingFlags flags = new BindingFlags();
			if(method.IsPublic) flags|=BindingFlags.Public; else flags|=BindingFlags.NonPublic;
			if(method.IsStatic) flags|=BindingFlags.Static; else flags|=BindingFlags.Instance;
			return flags==(flags&bindingAttr);
		}
	
		static bool IsBindable(FieldInfo field, BindingFlags bindingAttr) {
			BindingFlags flags = new BindingFlags();
			if(field.IsPublic) flags|=BindingFlags.Public; else flags|=BindingFlags.NonPublic;
			if(field.IsStatic) flags|=BindingFlags.Static; else flags|=BindingFlags.Instance;
			return flags==(flags&bindingAttr);
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr) {
			if((bindingAttr&BindingFlags.IgnoreCase)!=BindingFlags.Default) {
				return this.FindMembers(type, bindingAttr, Type.FilterNameIgnoreCase, name);
			}
			return this.FindMembers(type, bindingAttr, Type.FilterName, name);
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			if(modifiers!=null) throw new NotImplementedException("modifiers is not null");
			if(callConvention!=CallingConventions.Any) throw new NotImplementedException("call-convention is specified");
			ArrayList list = new ArrayList();
			foreach(ConstructorInfoImpl method in FilteredMemberCollection.Create(this.DeclaredConstructors,bindingAttr)) {
				if(callConvention!=CallingConventions.Any) {
					if(method.CallingConvention!=callConvention) continue;
				}
			}
			if(list.Count==0) return null;
			if(types==null) {
				if(list.Count>1) throw new AmbiguousMatchException();
				return (ConstructorInfo)list[0];
			} else {
				if(binder!=null) binder = DefaultBinder;
				return (ConstructorInfo)binder.SelectMethod(bindingAttr, (ConstructorInfo[])list.ToArray(typeof(ConstructorInfo)), types, modifiers);
			}
		}
	
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
			ArrayList list = new ArrayList();
			foreach(ConstructorInfo method in FilteredMemberCollection.Create(this.DeclaredConstructors,bindingAttr)) {
				list.Add(method);
			}
			return (ConstructorInfo[])list.ToArray(typeof(ConstructorInfo));
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			if(name==null) throw new ArgumentNullException("name");
			if(binder==null) binder = DefaultBinder;
			TypeImpl cur = this;
			ArrayList list = new ArrayList();
			while(cur!=null && list.Count==0) {
				foreach(MethodInfo method in FilteredMemberCollection.Create(cur.DeclaredMethods, name, bindingAttr)) {
					if(callConvention!=CallingConventions.Any) {
						if(method.CallingConvention!=callConvention) continue;
					}
					list.Add(method);
				}
				if(list.Count>0) {
					if(types==null) {
						if(list.Count>1) throw new AmbiguousMatchException();
						return (MethodInfo)list[0];
					} else {
						MethodInfo method = (MethodInfo)binder.SelectMethod(bindingAttr
							, (MethodInfo[])list.ToArray(typeof(MethodInfo))
							, types, modifiers);
						if(method!=null) return method;
						list.Clear();
					}
				}
				cur = (TypeImpl)cur.BaseType;
			}
			return null;
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
			ArrayList list = new ArrayList();
			TypeImpl cur = this;
			while(cur!=null) {
				foreach(MethodInfo method in FilteredMemberCollection.Create(cur.DeclaredMethods, bindingAttr)) {
					list.Add(method);
				}
				if(0!=(bindingAttr&BindingFlags.DeclaredOnly)) {
					break;
				}
				cur = (TypeImpl)cur.BaseType;
			}
			return (MethodInfo[])list.ToArray(typeof(MethodInfo));
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr) {
			if(name==null) throw new ArgumentNullException("name");
			FieldInfo selected = null;
			foreach(FieldInfo field in FilteredMemberCollection.Create(this.DeclaredFields, name, bindingAttr)) {
				if(selected!=null) throw new AmbiguousMatchException();
				selected = field;
			}
			return selected;
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr) {
			ArrayList list = new ArrayList();
			TypeImpl cur = this;
			while(cur!=null) {
				foreach(FieldInfo field in FilteredMemberCollection.Create(cur.DeclaredFields, bindingAttr)) {
					list.Add(field);
				}
				if(0!=(bindingAttr&BindingFlags.DeclaredOnly)) {
					break;
				}
				cur = (TypeImpl)cur.BaseType;
			}
			return (FieldInfo[])list.ToArray(typeof(FieldInfo));
		}

		protected override PropertyInfo GetPropertyImpl(
			string name,
			BindingFlags bindingAttr,
			Binder binder,
			Type returnType,
			Type[] types,
			ParameterModifier[] modifiers)
		{
			if(name==null) throw new ArgumentNullException("name");
			ArrayList list = new ArrayList();
			foreach(PropertyInfo property in FilteredMemberCollection.Create(this.DeclaredPeoperties, name, bindingAttr)) {
				list.Add(property);
			}
			if(list.Count==0) return null;
			if(types==null) {
				if(list.Count>1) throw new AmbiguousMatchException();
				return (PropertyInfo)list[0];
			} else {
				if(binder==null) binder = DefaultBinder;
				return (PropertyInfo)binder.SelectProperty(bindingAttr
					, (PropertyInfo[])list.ToArray(typeof(PropertyInfo))
					, returnType, types, modifiers);
			}
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
			ArrayList list = new ArrayList();
			foreach(FieldInfo field in FilteredMemberCollection.Create(this.DeclaredPeoperties, bindingAttr)) {
				list.Add(field);
			}
			return (PropertyInfo[])list.ToArray(typeof(PropertyInfo));
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr) {
			EventInfo selected = null;
			foreach(EventInfo evt in FilteredMemberCollection.Create(this.DeclaredEvents, name, bindingAttr)) {
				if(selected!=null) throw new AmbiguousMatchException();
				selected = evt;
			}
			return selected;
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr) {
			ArrayList list = new ArrayList();
			foreach(EventInfo evt in FilteredMemberCollection.Create(this.DeclaredEvents, bindingAttr)) {
				list.Add(evt);
			}
			return (EventInfo[])list.ToArray(typeof(EventInfo));
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr) {
			Type selected = null;
			foreach(Type type in FilteredMemberCollection.Create(this.DeclaredNestedTypes, name, bindingAttr)) {
				if(selected!=null) throw new AmbiguousMatchException();
				selected = type;
			}
			return selected;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
			ArrayList list = new ArrayList();
			foreach(Type type in FilteredMemberCollection.Create(this.DeclaredNestedTypes, bindingAttr)) {
				list.Add(type);
			}
			return (Type[])list.ToArray(typeof(Type));
		}

		#endregion

		public override Type[] GetInterfaces() {
			//TODO: check multiple implementation of interface.
			ArrayList list = new ArrayList();
			TypeImpl type = this;
			while(type!=null) {
				foreach(TypeImpl i in type.ImplementedInterfaces) {
					list.Add(i);
				}
				type = (TypeImpl)type.BaseType;
			}
			return (Type[])list.ToArray(typeof(Type));
		}

		#region Not implemented

		public override Guid GUID {
			get {
				return Guid.Empty;
			}
		}

		public override object InvokeMember(
			string name,
			BindingFlags invokeAttr,
			Binder binder,
			object target,
			object[] args,
			ParameterModifier[] modifiers,
			CultureInfo culture,
			string[] namedParameters) {
			throw new NotImplementedException();
		}

		public override Module Module {
			get {
				throw new NotImplementedException();
			}
		}

		public override string AssemblyQualifiedName {
			get {
				throw new NotImplementedException();
			}
		}

		public override object[] GetCustomAttributes(bool inherit) {
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			throw new NotImplementedException();
		}

		public override Type GetInterface(string name, bool ignoreCase) {
			throw new NotImplementedException();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
			throw new NotImplementedException();
		}

		protected override bool IsByRefImpl() {
			return this.IsByRefPointer;
		}

		public override bool IsDefined(Type attributeType, bool inherit) {
			//throw new NotImplementedException();
			//TODO: fix me
			return false;
		}

		protected override bool IsPrimitiveImpl() {
			throw new NotImplementedException();
		}

		protected override bool IsCOMObjectImpl() {
			throw new NotImplementedException();
		}

		protected override bool HasElementTypeImpl() {
			throw new NotImplementedException();
		}

		public override Type UnderlyingSystemType {
			get {
				throw new ExecutionEngineException("このプロパティが使われるとろくなことがない。");
			}
		}

		#endregion

	}

}
