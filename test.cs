using System;
using System.Reflection;

	public class E<U,V> {
		public U vU;
		public V vV;
	}

	public class C<S> {
		public S fC;
		public void f<X>(S s, X x) {
		}
		public class D<T> : E<S,T> {
			public S vDs;
			public T vDt;
			public void g<Y>(S s, T t, Y[] y) {
			}
		}
		public class F {
		}
	}

class Test {

	private static void Print(Type type) {
		Console.WriteLine(">> "+type);
		Console.WriteLine("ContainsGenericParameters: "+type.ContainsGenericParameters);
		foreach(Type arg in type.GetGenericArguments()) {
			Console.WriteLine("arg: "+arg);
		}
		Console.WriteLine(type.BaseType);
		Console.WriteLine(type.IsGenericParameter);
		Console.WriteLine(type.IsGenericType);
		Console.WriteLine(type.IsGenericTypeDefinition);
		if(type.IsGenericParameter) {
			Console.WriteLine(type.GenericParameterAttributes);
			Console.WriteLine(type.GenericParameterPosition);
			foreach(Type arg in type.GetGenericParameterConstraints()) {
				Console.WriteLine(arg);
			}
		} else {
			Console.WriteLine(type.GetGenericTypeDefinition());
		}
		foreach(FieldInfo field in type.GetFields()) {
			Console.WriteLine("f: {0} {1} ({2})", field.FieldType, field.Name, field.DeclaringType);
		}
		foreach(MethodInfo method in type.GetMethods()) {
			Console.Write("m: {0} {1} << ", method.ReturnType, method.Name);
			foreach(ParameterInfo p in method.GetParameters()) {
				Console.Write(", {0} {1}", p.ParameterType, p.Name);
			}
			Console.WriteLine(" ({0})", method.DeclaringType);
		}
		Console.WriteLine();
	}

	public static void Main() {
		Print(typeof(C<int>.F));
		Print(typeof(C<int>.D<long>).GetGenericTypeDefinition().MakeArrayType());
		Print(typeof(C<int>.D<long>[]));
		Print(typeof(C<int>.D<long>).DeclaringType);
		Print(typeof(C<int>.D<long>).GetGenericTypeDefinition());
		Print(typeof(C<int>.D<long>).GetGenericTypeDefinition().GetField("vDt").FieldType);
	}

}
