using System;
using System.Reflection;

namespace CooS.Wrap._System {

	public abstract class _Enum {

		private object get_value() {
			return this;
		}

		public static string GetName(Type enumType, object value) {
			foreach(FieldInfo fi in enumType.GetFields()) {
				if(value.Equals(fi.GetValue(null))) {
					return fi.Name;
				}
			}
			throw new ArgumentException(value.ToString());
		}

	}

}
