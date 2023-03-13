using System;
using System.Reflection;
using CooS.Reflection;

namespace CooS.Wrap._System {

	public sealed class _Type {

		public bool Equals(Type type) {
			return Object.ReferenceEquals(this, type);
		}

		internal static bool type_is_subtype_of(Type a, Type b, bool check_interfaces) {
			if(check_interfaces) throw new NotSupportedException();
			// TypeImpl.IsSubclassOf Ç™ê≥ãKÇÃíËã`
			return a.IsSubclassOf(b);
		}

		internal static bool type_is_assignable_from(Type a, Type b) {
			throw new UnexpectedException();
		}

		private static Type internal_from_name(string name, bool throwOnError, bool ignoreCase) {
			CooS.Utility.TypePath typepath = CooS.Utility.TypePath.Parse(name);
			AssemblyName asmname = typepath.GetAssemblyName();
			if(asmname==null) {
				foreach(Assembly asm in CooS.Management.AssemblyResolver.EnumAssemblies()) {
					Type type = asm.GetType(name,false,ignoreCase);
					if(type!=null) return type;
				}
				if(throwOnError) throw new TypeLoadException(name);
				return null;
			} else {
				Assembly asm = CooS.Management.AssemblyResolver.ResolveAssembly(asmname, true);
				return asm.GetType(name, throwOnError, ignoreCase);
			}
		}

		private static Type internal_from_handle(IntPtr handle) {
			return TypeImpl.FindTypeFromHandle(handle);
		}

		public bool IsEnum {
			get {
				TypeImpl me = (TypeImpl)(object)this;
				return me.IsSubclassOf(me.AssemblyInfo.Manager.Enum);
			}
		}

		internal bool IsSystemType {
			get {
				//MONO: Mono sometimes requires this returning true.
				//	by System.Array:CreateInstance(Type elementType, int[] lengths)
				return true;
			}
		}

		public static TypeCode GetTypeCode(Type type) {
			if(type==null) {
				return TypeCode.Empty;
			} else if(type.Namespace!="System") {
				return TypeCode.Object;
			} else if(type.IsPrimitive) {
				switch(type.Name) {
				case "Boolean":
					return TypeCode.Boolean;
				case "Byte":
					return TypeCode.Byte;
				case "Char":
					return TypeCode.Char;
				case "Decimal":
					return TypeCode.Decimal;
				case "Double":
					return TypeCode.Double;
				case "Int16":
					return TypeCode.Int16;
				case "Int32":
					return TypeCode.Int32;
				case "Int64":
					return TypeCode.Int64;
				case "SByte":
					return TypeCode.SByte;
				case "Single":
					return TypeCode.Single;
				case "UInt16":
					return TypeCode.UInt16;
				case "UInt32":
					return TypeCode.UInt32;
				case "UInt64":
					return TypeCode.UInt64;
				default:
					throw new NotImplementedException();
				}
			} else {
				switch(type.Name) {
				case "DateTime":
					return TypeCode.DateTime;
				case "DBNull":
					return TypeCode.DBNull;
				case "String":
					return TypeCode.String;
				case "Object":
				default:
					return TypeCode.Object;
				}
			}
		} 

	}

}
