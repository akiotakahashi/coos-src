using System;
using System.IO;
using CooS.Reflection;
using CooS.Collections;
using System.Collections.Generic;
using AssemblyName = System.Reflection.AssemblyName;

namespace CooS.Core {
	using IntTable = CooS.Collections.Inttable<int>;
	using Enumerable = CooS.Collections.Inttable<int>.HashValues;

	public static class AssemblyManager {

		static readonly Version EmptyVersion = new Version(0,0,0,0);
		static readonly Inttable<AssemblyBase> assemblies = new Inttable<AssemblyBase>();
		static readonly Dictionary<string, TypeBase> cache = new Dictionary<string, TypeBase>();
		static AssemblyBase primary = null;
		static AssemblyBase secondary = null;

		const int PrimaryId = 1;
		const int SecondaryId = 2;
		static int nextId = PrimaryId;

		public static AssemblyBase LoadAssembly(string type, Stream stream) {
			AssemblyBase assembly = Kernel.World.LoadAssembly(type, stream);
			//assembly.Id = nextId++;
			Console.WriteLine("Registered [{0}] with ID#{1}", assembly.Name, assembly.Id);
			assemblies.Add(assembly.Id, assembly);
			switch(assembly.Id) {
			case PrimaryId:
				primary = assembly;
				break;
			case SecondaryId:
				secondary = assembly;
				break;
			}
			return assembly;
		}

		public static AssemblyBase LoadAssembly(string type, byte[] buf) {
			using(MemoryStream stream = new MemoryStream(buf, false)) {
				return LoadAssembly(type, stream);
			}
		}

		public static AssemblyBase LoadAssembly(string type, string filename) {
			using(Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				return LoadAssembly(type, stream);
			}
		}

		public static bool IsSatisfied(AssemblyBase target, AssemblyName request) {
			return target.Name==request.Name
				&& (request.Version==EmptyVersion || target.Version==request.Version)
				;
		}

		public static AssemblyBase GetAssembly(int id, bool throwOnMiss) {
			AssemblyBase assembly;
			if(id==PrimaryId) {
				assembly = primary;
			} else if(id==SecondaryId) {
				assembly = secondary;
			} else {
				assembly = (AssemblyBase)assemblies[id];
			}
			if(assembly==null && throwOnMiss) throw new NotFoundException();
			return assembly;
		}

		public static AssemblyBase ResolveAssembly(AssemblyName name, bool throwOnMiss) {
			foreach(AssemblyBase assembly in assemblies.Values) {
				if(IsSatisfied(assembly, name)) {
					return assembly;
				}
			}
			if(throwOnMiss) throw new NotFoundException();
			return null;
		}

		public static AssemblyBase ResolveAssembly(string name, bool throwOnMiss) {
			foreach(AssemblyBase assembly in assemblies.Values) {
				if(0==string.Compare(assembly.Name,name,true)) {
					return assembly;
				}
			}
			if(throwOnMiss) throw new NotFoundException();
			return null;
		}

		private static TypeBase ResolveType(string ns, string name, bool throwOnMiss) {
			if(cache.ContainsKey(name)) {
				return cache[name];
			} else {
				foreach(AssemblyBase assembly in assemblies.Values) {
					TypeBase type = assembly.FindType(name, ns);
					if(type!=null) {
						cache[name] = type;
						return type;
					}
				}
			}
			if(throwOnMiss) throw new NotFoundException();
			return null;
		}

		public static TypeBase Void {
			get {
				return ResolveType("System", "Void",true);
			}
		}

		public static TypeBase Boolean {
			get {
				return ResolveType("System", "Boolean",true);
			}
		}

		public static TypeBase Char {
			get {
				return ResolveType("System", "Char",true);
			}
		}

		public static TypeBase SByte {
			get {
				return ResolveType("System", "SByte",true);
			}
		}

		public static TypeBase Byte {
			get {
				return ResolveType("System", "Byte",true);
			}
		}

		public static TypeBase Int16 {
			get {
				return ResolveType("System", "Int16",true);
			}
		}

		public static TypeBase UInt16 {
			get {
				return ResolveType("System", "UInt16",true);
			}
		}

		public static TypeBase Int32 {
			get {
				return ResolveType("System", "Int32",true);
			}
		}

		public static TypeBase UInt32 {
			get {
				return ResolveType("System", "UInt32",true);
			}
		}

		public static TypeBase Int64 {
			get {
				return ResolveType("System", "Int64",true);
			}
		}

		public static TypeBase UInt64 {
			get {
				return ResolveType("System", "UInt64",true);
			}
		}

		public static TypeBase IntPtr {
			get {
				return ResolveType("System", "IntPtr",true);
			}
		}

		public static TypeBase UIntPtr {
			get {
				return ResolveType("System", "UIntPtr",true);
			}
		}

		public static TypeBase Single {
			get {
				return ResolveType("System", "Single",true);
			}
		}

		public static TypeBase Double {
			get {
				return ResolveType("System", "Double",true);
			}
		}

		public static TypeBase Object {
			get {
				return ResolveType("System", "Object",true);
			}
		}

		public static TypeBase String {
			get {
				return ResolveType("System", "String",true);
			}
		}

		public static TypeBase ValueType {
			get {
				return ResolveType("System", "ValueType",true);
			}
		}

		public static TypeBase Enum {
			get {
				return ResolveType("System", "Enum",true);
			}
		}

		public static TypeBase Delegate {
			get {
				return ResolveType("System", "Delegate",true);
			}
		}

		public static TypeBase SzArray {
			get {
				return ResolveType("CooS.Reflection.CLI", "SzArray",true);
			}
		}

		public static TypeBase ByRefPointer {
			get {
				return ResolveType("CooS.Reflection.CLI", "ByRefPointer",true);
			}
		}

		public static TypeBase ByValPointer {
			get {
				return ResolveType("CooS.Reflection.CLI", "ByValPointer",true);
			}
		}

	}

}
