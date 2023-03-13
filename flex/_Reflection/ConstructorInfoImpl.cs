using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Reflection {

	public abstract class ConstructorInfoImpl : ConstructorInfo {

		public ConstructorInfoImpl() {
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit) {
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit) {
			throw new NotImplementedException();
		}

		public override MethodAttributes Attributes {
			get {
				throw new NotImplementedException();
			}
		}

		public override MethodImplAttributes GetMethodImplementationFlags() {
			return new MethodImplAttributes ();
		}

		public abstract int ParameterCount {get;}

		public override ParameterInfo[] GetParameters() {
			throw new NotImplementedException();
		}

		public abstract TypeImpl GetParameterType(int index);

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		public override RuntimeMethodHandle MethodHandle {
			get {
				throw new NotImplementedException();
			}
		}

	}

}
