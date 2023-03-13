using System;
using System.Reflection;
using System.Collections.Generic;

namespace CooS.Development {

	class Program {

		static int Main(string[] args) {
			Dictionary<string,Type> typetable = new Dictionary<string,Type>();
			foreach(Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				typetable.Add(type.Name.ToLower(), type);
			}
			if(args.Length==0) {
				Console.WriteLine("Commands:");
				foreach(string name in typetable.Keys) {
					Console.WriteLine("\t{0}", name);
				}
				return 0;
			} else {
				if(!typetable.ContainsKey(args[0].ToLower())) {
					throw new ArgumentException("Invalid Verb: "+args[0]);
				} else {
					Type type = typetable[args[0].ToLower()];
					ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(string[]) });
					object obj = ctor.Invoke(new object[]{args});
					return (int)type.GetMethod("Execute").Invoke(obj, new object[0]);
				}
			}
		}

	}

}
