using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.CodeModels.CLI {

	class ConstructorDefInfo : ConstructorInfoImpl {

		MethodDefInfo methoddef;

		public ConstructorDefInfo(MethodDefInfo methoddef) {
			this.methoddef = methoddef;
		}

		public override Type DeclaringType {
			get {
				return methoddef.DeclaringType;
			}
		}

		public override Type ReflectedType {
			get {
				return methoddef.DeclaringType;
			}
		}

		public override string Name {
			get {
				return this.methoddef.Name;
			}
		}

		public override MethodAttributes Attributes {
			get {
				return this.methoddef.Attributes;
			}
		}

		public override int ParameterCount {
			get {
				return this.methoddef.ParameterCount;
			}
		}

		public override ParameterInfo[] GetParameters() {
			return this.methoddef.GetParameters();
		}

		public override TypeImpl GetParameterType(int index) {
			return this.methoddef.GetParameterType(index);
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture) {
			object obj = Memory.Allocate((TypeImpl)this.ReflectedType);
			object ret = this.methoddef.Invoke(obj, invokeAttr, binder, parameters, culture);
			if(ret!=null) throw new Exception("Constructor return non-null value.");
			Console.WriteLine(".ctor allocates {0}", obj.GetType().FullName);
			return obj;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

	}

}
