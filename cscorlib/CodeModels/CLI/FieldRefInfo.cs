using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.CodeModels.CLI {
	using Metatype;
	using Metadata;
	using Signature;

#if false

	class FieldRefInfo : FieldInfoImpl {

		readonly AssemblyDef Assembly;
		readonly SuperType reftype;
		readonly FieldInfoImpl original;
		
		public FieldRefInfo(SuperType reftype, FieldInfoImpl original) {
			this.Assembly = reftype.MyAssembly;
			this.reftype = reftype;
			this.original = original;
		}

		public override int RowIndex {
			get {
				return this.original.RowIndex;
			}
		}

		protected override RuntimeFieldHandle GenerateNewHandle(FieldInfoImpl field) {
			return  this.original.Handle;
		}

		public override Type DeclaringType {
			get {
				return this.original.DeclaringType;
			}
		}

		public override Type ReflectedType {
			get {
				return this.reftype;
			}
		}

		public override string Name {
			get {
				return this.original.Name;
			}
		}

		public override FieldAttributes Attributes {
			get {
				return this.original.Attributes;
			}
		}

		public override Type FieldType {
			get {
				return this.original.FieldType;
			}
		}

		public override int GetFieldOffset() {
			return this.original.GetFieldOffset();
		}

		public override object GetValue(object obj) {
			return this.original.GetValue(obj);
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, System.Globalization.CultureInfo culture) {
			this.original.SetValue(obj, value, invokeAttr, binder, culture);
		}

		public override byte[] GetStaticBuffer() {
			return this.original.GetStaticBuffer();
		}

	}

#endif

}
