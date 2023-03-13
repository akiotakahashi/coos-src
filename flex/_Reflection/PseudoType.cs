using System;
using System.Reflection;

namespace CooS.Reflection {

	sealed class PseudoType : Type {

		// This field, handle, is referred by name.
		// Keep it sync with mms.cpp in kernel.dll.
		internal RuntimeTypeHandle handle;

		int rowIndex;
		Type baseType;
		Type elementType;
		string name;
		string _namespace;
		
		private PseudoType(RuntimeTypeHandle handle, int rowIndex, string name, string Namespace, Type baseType) {
			this.handle = handle;
			this.rowIndex = rowIndex;
			this.baseType = baseType;
			this.name = name;
			this._namespace = Namespace;
		}

		public int RowIndex {
			get {
				return this.rowIndex;
			}
		}

		public override RuntimeTypeHandle TypeHandle {
			get {
				return this.handle;
			}
		}

		public override string Namespace {
			get {
				return this._namespace;
			}
		}
		
		public override string Name {
			get {
				return name;
			}
		}
			
		public override string FullName {
			get {
				return this.Namespace+"."+this.Name;
			}
		}

		public override Type BaseType {
			get {
				return baseType;
			}
		}
		
		protected override bool IsPointerImpl() {
			return this.name.EndsWith("&") || this.name.EndsWith("*");
		}

		protected override bool IsArrayImpl() {
			return this.name.EndsWith("[]");
		}

		protected override bool IsByRefImpl() {
			return this.name.EndsWith("&");
		}

		public override Type GetElementType() {
			return elementType;
		}
		
		public void SetElementType(Type elementType) {
			this.elementType = elementType;
		}

		/*****************************************************************************************
		 *		Pseudo Methods
		 *****************************************************************************************/

		public override object[] GetCustomAttributes(bool inherit) {
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit) {
			throw new NotSupportedException();
		}

		public override Assembly Assembly {
			get {
				throw new NotSupportedException();
			}
		}

		public override string AssemblyQualifiedName {
			get {
				throw new NotSupportedException();
			}
		}

		protected override TypeAttributes GetAttributeFlagsImpl() {
			throw new NotSupportedException();
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			throw new NotSupportedException();
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override Type GetInterface(string name, bool ignoreCase) {
			throw new NotSupportedException();
		}

		public override Type[] GetInterfaces() {
			throw new NotSupportedException();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
			throw new NotSupportedException();
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
			throw new NotSupportedException();
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {
			throw new NotSupportedException();
		}

		public override Guid GUID {
			get {
				throw new NotSupportedException();
			}
		}

		protected override bool HasElementTypeImpl() {
			throw new NotSupportedException();
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) {
			throw new NotSupportedException();
		}

		protected override bool IsCOMObjectImpl() {
			throw new NotSupportedException();
		}

		protected override bool IsPrimitiveImpl() {
			throw new NotSupportedException();
		}

		public override Module Module {
			get {
				throw new NotSupportedException();
			}
		}

		public override Type UnderlyingSystemType {
			get {
				throw new NotSupportedException();
			}
		}

	}

}
