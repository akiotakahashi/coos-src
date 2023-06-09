using System;
using System.Reflection;
using System.Globalization;

//COOS: This class, StandardBinder, comes from Mono's System.Reflection.Binder.DefaultBinder.

namespace CooS.CodeModels.CLI {

	internal sealed class StandardBinder : Binder {

		internal static int GetDerivedLevel (Type type) {
			Type searchType = type;
			int level = 1;

			while (searchType.BaseType != null) {
				level++;
				searchType = searchType.BaseType;
			}

			return level;
		}

		public override FieldInfo BindToField (BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture) {
			if (match == null)
				throw new ArgumentNullException ("match");
			foreach (FieldInfo f in match) {
				if (check_type (value.GetType (), f.FieldType))
					return f;
			}
			return null;
		}

		public override MethodBase BindToMethod (BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state) {
			Type[] types;
			if (args == null)
				types = Type.EmptyTypes;
			else {
				types = new Type [args.Length];
				for (int i = 0; i < args.Length; ++i) {
					if (args [i] != null)
						types [i] = args [i].GetType ();
				}
			}
			MethodBase selected = SelectMethod (bindingAttr, match, types, modifiers);
			state = null;
			return selected;
		}

		static bool IsArrayAssignable (Type object_type, Type target_type) {
			if (object_type.IsArray && target_type.IsArray)
				return IsArrayAssignable (object_type.GetElementType (), target_type.GetElementType ());
						
			if (target_type.IsAssignableFrom (object_type))
				return true;

			return false;
		}
			
		public override object ChangeType (object value, Type type, CultureInfo culture) {
			if (value == null)
				return null;
			Type vtype = value.GetType ();
			if (type.IsByRef)
				type = type.GetElementType ();
			if (vtype == type || type.IsInstanceOfType (value))
				return value;
			if (vtype.IsArray && type.IsArray){
				if (IsArrayAssignable (vtype.GetElementType (), type.GetElementType ()))
					return value;
			}

			if (check_type (vtype, type))
				return Convert.ChangeType (value, type);
			return null;
		}

		public override void ReorderArgumentArray (ref object[] args, object state) {
			//do nothing until we support named arguments
			//throw new NotImplementedException ();
		}

		private static bool check_type (Type from, Type to) {
			if (from == to)
				return true;

			if (from == null)
				return !to.IsValueType;

			TypeCode fromt = Type.GetTypeCode (from);
			TypeCode tot = Type.GetTypeCode (to);

			if (to.IsByRef != from.IsByRef)
				return false;

			switch (fromt) {
			case TypeCode.Char:
				switch (tot) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object);
			case TypeCode.Byte:
				switch (tot) {
				case TypeCode.Char:
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.SByte:
				switch (tot) {
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.UInt16:
				switch (tot) {
				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.Int16:
				switch (tot) {
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.UInt32:
				switch (tot) {
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.Int32:
				switch (tot) {
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.UInt64:
			case TypeCode.Int64:
				switch (tot) {
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				return to == typeof (object) || (from.IsEnum && to == typeof (Enum));
			case TypeCode.Single:
				return tot == TypeCode.Double || to == typeof (object);
			default:
				/* TODO: handle valuetype -> byref */
				if (to == typeof (object) && from.IsValueType)
					return true;

				return to.IsAssignableFrom (from);
			}
		}

		private static bool check_arguments (Type[] types, ParameterInfo[] args) {
			for (int i = 0; i < types.Length; ++i) {
				if (!check_type (types [i], args [i].ParameterType))
					return false;
			}
			return true;
		}

		public override MethodBase SelectMethod (BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers) {
			MethodBase m;
			int i, j;

			if (match == null)
				throw new ArgumentNullException ("match");

			/* first look for an exact match... */
			for (i = 0; i < match.Length; ++i) {
				m = match [i];
				ParameterInfo[] args = m.GetParameters ();
				if (args.Length != types.Length)
					continue;
				for (j = 0; j < types.Length; ++j) {
					if (types [j] != args [j].ParameterType)
						break;
				}
				if (j == types.Length)
					return m;
			}

			MethodBase result = null;
			for (i = 0; i < match.Length; ++i) {
				m = match [i];
				ParameterInfo[] args = m.GetParameters ();
				if (args.Length != types.Length)
					continue;
				if (!check_arguments (types, args))
					continue;

				if (result != null)
					throw new AmbiguousMatchException ();

				result = m;
			}

			return result;
		}

		public override PropertyInfo SelectProperty (BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers) {
			if (match == null || match.Length == 0)
				throw new ArgumentException ("No properties provided", "match");

			bool haveRet = (returnType != null);
			int idxlen = (indexes != null) ? indexes.Length : -1;
			PropertyInfo result = null;
			int i;
			int best_score = Int32.MaxValue - 1;
			int fail_score = Int32.MaxValue;
			int level = 0;
				
			for (i = match.Length - 1; i >= 0; i--) {
				PropertyInfo p = match [i];
				ParameterInfo[] args = p.GetIndexParameters ();
				if (idxlen >= 0 && idxlen != args.Length)
					continue;

				if (haveRet && !check_type (p.PropertyType, returnType))
					continue;

				int score = Int32.MaxValue - 1;
				if (idxlen > 0) {
					score = check_arguments_with_score (indexes, args);
					if (score == -1)
						continue;
				}

				int new_level = GetDerivedLevel (p.DeclaringType);
				if (result != null) {
					if (best_score < score)
						continue;

					if (best_score == score) {
						if (level == new_level) {
							// Keep searching. May be there's something
							// better for us.
							fail_score = score;
							continue;
						}

						if (level > new_level)
							continue;
					}
				}

				result = p;
				best_score = score;
				level = new_level;
			}

			if (fail_score <= best_score)
				throw new AmbiguousMatchException ();

			return result;
		}

		static int check_arguments_with_score (Type [] types, ParameterInfo [] args) {
			int worst = -1;

			for (int i = 0; i < types.Length; ++i) {
				int res = check_type_with_score (types [i], args [i].ParameterType);
				if (res == -1)
					return -1;

				if (worst < res)
					worst = res;
			}

			return worst;
		}

		// 0 -> same type or null and !valuetype
		// 1 -> to == Enum
		// 2 -> value type that don't lose data
		// 3 -> to == IsAssignableFrom
		// 4 -> to == object
		static int check_type_with_score (Type from, Type to) {
			if (from == null)
				return to.IsValueType ? -1 : 0;

			if (from == to)
				return 0;

			if (to == typeof (object))
				return 4;

			TypeCode fromt = Type.GetTypeCode (from);
			TypeCode tot = Type.GetTypeCode (to);

			switch (fromt) {
			case TypeCode.Char:
				switch (tot) {
				case TypeCode.UInt16:
					return 0;

				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return -1;
			case TypeCode.Byte:
				switch (tot) {
				case TypeCode.Char:
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.SByte:
				switch (tot) {
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.UInt16:
				switch (tot) {
				case TypeCode.UInt32:
				case TypeCode.Int32:
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.Int16:
				switch (tot) {
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.UInt32:
				switch (tot) {
				case TypeCode.UInt64:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.Int32:
				switch (tot) {
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.UInt64:
			case TypeCode.Int64:
				switch (tot) {
				case TypeCode.Single:
				case TypeCode.Double:
					return 2;
				}
				return (from.IsEnum && to == typeof (Enum)) ? 1 : -1;
			case TypeCode.Single:
				return tot == TypeCode.Double ? 2 : -1;
			default:
				return (to.IsAssignableFrom (from)) ? 3 : -1;
			}
		}
	}

}
