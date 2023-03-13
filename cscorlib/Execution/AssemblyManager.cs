using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using CooS.Collections;
using CooS.CodeModels.CLI;

namespace CooS.Reflection {
	//using Inttable = System.Collections.Hashtable;
	using Enumerable = Inttable.HashValues;

	public class AssemblyManager {

		static readonly Version EmptyVersion = new Version(0,0,0,0);
		readonly Inttable assemblies = new Inttable();
		readonly Hashtable cache = new Hashtable();
		AssemblyBase primary = null;
		AssemblyBase secondary = null;

		const int PrimaryId = 1;
		const int SecondaryId = 2;
		int nextId = PrimaryId;

		public AssemblyManager() {
		}

		public AssemblyBase LoadAssembly(AssemblyBase assembly) {
			if(assembly.Manager!=null) throw new ArgumentException();
			assembly.Manager = this;
			assembly.Id = this.nextId++;
			Console.WriteLine("Registered [{0}] with ID#{1}", assembly.GetName(false).Name, assembly.Id);
			this.assemblies.Add(assembly.Id, assembly);
			Console.WriteLine(assembly.GetName(false));
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

		public AssemblyBase LoadAssembly(string filename) {
			byte[] buf;
			using(Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				buf = new byte[stream.Length];
				stream.Read(buf, 0, buf.Length);
			}
			return this.LoadAssembly(Engine.AnalyzeAssembly(buf));
		}

		private void MakePrimitive(string name) {
			TypeImpl type = this.ResolveType(name, true);
			type = ((AssemblyDef)type.AssemblyInfo).RestructureAsPrimitive(type);
			this.cache[name] = type;
		}

		public void Setup() {
			this.MakePrimitive("System.Boolean");
			this.MakePrimitive("System.Char");
			this.MakePrimitive("System.SByte");
			this.MakePrimitive("System.Byte");
			this.MakePrimitive("System.Int16");
			this.MakePrimitive("System.UInt16");
			this.MakePrimitive("System.Int32");
			this.MakePrimitive("System.UInt32");
			this.MakePrimitive("System.Int64");
			this.MakePrimitive("System.UInt64");
			this.MakePrimitive("System.IntPtr");
			this.MakePrimitive("System.UIntPtr");
			this.MakePrimitive("System.Single");
			this.MakePrimitive("System.Double");
		}

		public static bool IsSatisfied(AssemblyName target, AssemblyName request) {
			return target.Name==request.Name
				&& (request.Version==EmptyVersion || target.Version==request.Version)
				;
		}

		public Enumerable EnumAssemblies() {
			return this.assemblies.Values;
		}

		public AssemblyBase GetAssembly(int id, bool throwOnMiss) {
			AssemblyBase assembly;
			if(id==PrimaryId) {
				assembly = this.primary;
			} else if(id==SecondaryId) {
				assembly = this.secondary;
			} else {
				assembly = (AssemblyBase)this.assemblies[id];
			}
			if(assembly==null && throwOnMiss) throw new NotFoundException();
			return assembly;
		}

		public AssemblyBase ResolveAssembly(AssemblyName name, bool throwOnMiss) {
			foreach(AssemblyBase assembly in this.assemblies.Values) {
				if(IsSatisfied(assembly.GetName(false), name)) {
					return assembly;
				}
			}
			if(throwOnMiss) throw new NotFoundException();
			return null;
		}

		public AssemblyBase ResolveAssembly(string name, bool throwOnMiss) {
			foreach(AssemblyBase assembly in this.assemblies.Values) {
				if(0==string.Compare(assembly.GetName(false).Name,name,true)) {
					return assembly;
				}
			}
			if(throwOnMiss) throw new NotFoundException();
			return null;
		}

		public TypeImpl ResolveType(string name, bool throwOnMiss) {
			if(this.cache.ContainsKey(name)) {
				return (TypeImpl)cache[name];
			} else {
				foreach(AssemblyBase assembly in this.assemblies.Values) {
					TypeImpl type = assembly.FindType(name, false);
					if(type!=null) {
						this.cache[name] = type;
						return type;
					}
				}
			}
			if(throwOnMiss) throw new NotFoundException();
			return null;
		}

		public MethodInfoImpl[] ResolveMethods(string fullname, bool throwOnMiss) {
			int i = fullname.LastIndexOf(':');
			if(i<0) throw new ArgumentException("name has no colon for separater");
			string typename = fullname.Substring(0,i);
			string methodname = fullname.Substring(i+1);
			TypeImpl type = this.ResolveType(typename, false);
			if(type==null) {
				if(throwOnMiss) throw new TypeNotFoundException(typename);
				return null;
			}
			ArrayList list = new ArrayList();
			if(methodname==".ctor") {
				foreach(MethodInfoImpl method in type.DeclaredConstructors) {
					list.Add(method);
				}
			} else {
				foreach(MethodInfoImpl method in FilteredMemberCollection.Create(type.DeclaredMethods, methodname)) {
					list.Add(method);
				}
			}
			if(list.Count==0 && throwOnMiss) throw new MethodNotFoundException(fullname);
			return (MethodInfoImpl[])list.ToArray(typeof(MethodInfoImpl));
		}

		public MethodInfoImpl ResolveMethod(string name, bool throwOnMiss) {
			int i = name.LastIndexOf(':');
			if(i<0) throw new ArgumentException("name has no colon for separater");
			TypeImpl type = this.ResolveType(name.Substring(0,i), false);
			if(type==null) {
				if(throwOnMiss) throw new TypeNotFoundException(name.Substring(0,i));
				return null;
			}
			MethodInfo method = type.GetMethod(name.Substring(i+1)
				, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static);
			if(method==null) {
				if(throwOnMiss) throw new MethodNotFoundException(name.Substring(i+1));
				return null;
			}
			return (MethodInfoImpl)method;
		}

		public TypeImpl Void {
			get {
				return this.ResolveType("System.Void",true);
			}
		}

		public TypeImpl Boolean {
			get {
				return this.ResolveType("System.Boolean",true);
			}
		}

		public TypeImpl Char {
			get {
				return this.ResolveType("System.Char",true);
			}
		}

		public TypeImpl SByte {
			get {
				return this.ResolveType("System.SByte",true);
			}
		}

		public TypeImpl Byte {
			get {
				return this.ResolveType("System.Byte",true);
			}
		}

		public TypeImpl Int16 {
			get {
				return this.ResolveType("System.Int16",true);
			}
		}

		public TypeImpl UInt16 {
			get {
				return this.ResolveType("System.UInt16",true);
			}
		}

		public TypeImpl Int32 {
			get {
				return this.ResolveType("System.Int32",true);
			}
		}

		public TypeImpl UInt32 {
			get {
				return this.ResolveType("System.UInt32",true);
			}
		}

		public TypeImpl Int64 {
			get {
				return this.ResolveType("System.Int64",true);
			}
		}

		public TypeImpl UInt64 {
			get {
				return this.ResolveType("System.UInt64",true);
			}
		}

		public TypeImpl IntPtr {
			get {
				return this.ResolveType("System.IntPtr",true);
			}
		}

		public TypeImpl UIntPtr {
			get {
				return this.ResolveType("System.UIntPtr",true);
			}
		}

		public TypeImpl Single {
			get {
				return this.ResolveType("System.Single",true);
			}
		}

		public TypeImpl Double {
			get {
				return this.ResolveType("System.Double",true);
			}
		}

		public TypeImpl Object {
			get {
				return this.ResolveType("System.Object",true);
			}
		}

		public TypeImpl String {
			get {
				return this.ResolveType("System.String",true);
			}
		}

		public TypeImpl ValueType {
			get {
				return this.ResolveType("System.ValueType",true);
			}
		}

		public TypeImpl Enum {
			get {
				return this.ResolveType("System.Enum",true);
			}
		}

		public TypeImpl Delegate {
			get {
				return this.ResolveType("System.Delegate",true);
			}
		}

		public TypeImpl SzArray {
			get {
				return this.ResolveType("CooS.Reflection.CLI.SzArray",true);
			}
		}

		public TypeImpl ByRefPointer {
			get {
				return this.ResolveType("CooS.Reflection.CLI.ByRefPointer",true);
			}
		}

		public TypeImpl ByValPointer {
			get {
				return this.ResolveType("CooS.Reflection.CLI.ByValPointer",true);
			}
		}

	}

}
