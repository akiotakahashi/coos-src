using System;
using System.IO;
using CooS.Reflection;
using System.Globalization;
using System.Collections.Generic;

namespace CooS.Execution.CLI {
	
	public sealed partial class WorldImpl : World {

		private static readonly Dictionary<string, ILoader> loaders = new Dictionary<string,ILoader>();

		static WorldImpl() {
			loaders["CLI"] = new CooS.Reflection.CLI.LoaderImpl();
			//loaders["Java/.class"] = new CooS.Reflection.Java.SimpleLoader();
		}

	}

	public sealed partial class WorldImpl : IDisposable {

		private readonly List<AssemblyBase> assemblies = new List<AssemblyBase>();
		private readonly int primaryCount = -1;

		public WorldImpl(int c) {
			this.primaryCount = c;
		}

		#region IDisposable ÉÅÉìÉo

		public void Dispose() {
			foreach(AssemblyBase assembly in this.assemblies) {
				assembly.Dispose();
			}
		}

		#endregion

		public AssemblyBase ResolveAssembly(string name) {
			return ResolveAssembly(name, null, ZeroVersion);
		}

		public AssemblyBase ResolveAssembly(string name, CultureInfo culture) {
			return ResolveAssembly(name, culture, ZeroVersion);
		}

		public override AssemblyBase ResolveAssembly(string name, CultureInfo culture, Version version) {
			foreach(AssemblyBase assembly in assemblies) {
				if(IsMatch(assembly, name, culture, version)) {
					return assembly;
				}
			}
			return null;
		}

		private static Version ZeroVersion = new Version();

		private bool IsMatch(AssemblyBase assembly, string name, CultureInfo culture, Version version) {
			if(assembly.Name!=name) {
				return false;
			}
			if(culture!=null && !culture.IsNeutralCulture && !assembly.Culture.IsNeutralCulture) {
				if(!assembly.Culture.Equals(culture))
					return false;
			}
			if(version!=ZeroVersion) {
				if(assembly.Version!=version) {
					return false;
				}
			}
			return true;
		}

		public override TypeBase ResolveType(string fullname) {
			throw new Exception("The method or operation is not implemented.");
		}

		public override TypeBase ResolveType(string name, string ns) {
			foreach(AssemblyBase assembly in assemblies) {
				TypeBase type = assembly.FindType(name, ns);
				if(type!=null) {
					return type;
				}
			}
			return null;
		}

		public override FieldBase ResolveField(string ns, string type, string name) {
			TypeBase t = this.ResolveType(type, ns);
			FieldBase f = t.FindField(name);
			if(f==null) { throw new MethodNotFoundException(ns+"."+type+":"+name); }
			return f;
		}

		public override MethodBase ResolveMethod(string ns, string type, string name) {
			TypeBase t = this.ResolveType(type, ns);
			MethodBase m = t.FindMethod(name);
			if(m==null) { throw new MethodNotFoundException(ns+"."+type+":"+name); }
			return m;
		}

		public override TypeBase Resolve(IntrinsicTypes it) {
			switch(it) {
			case IntrinsicTypes.Int4:
				return this.Resolve(PrimitiveTypes.I4);
			case IntrinsicTypes.Int8:
				return this.Resolve(PrimitiveTypes.I8);
			case IntrinsicTypes.NInt:
				return this.Resolve(PrimitiveTypes.I);
			case IntrinsicTypes.Objt:
				return this.Resolve(PrimitiveTypes.Object);
			case IntrinsicTypes.Fp32:
				return this.Resolve(PrimitiveTypes.R4);
			case IntrinsicTypes.Fp64:
				return this.Resolve(PrimitiveTypes.R8);
			case IntrinsicTypes.Pter:
				return this.Resolve(PrimitiveTypes.ByRef);
			case IntrinsicTypes.Any:
				throw new NotImplementedException();
			default:
				throw new NotSupportedException();
			}
		}

		public override TypeBase Resolve(PrimitiveTypes pt) {
			switch(pt) {
			case PrimitiveTypes.Void:
				return this.ResolveType("Void", "System");
			case PrimitiveTypes.I:
				return this.ResolveType("IntPtr", "System");
			case PrimitiveTypes.I1:
				return this.ResolveType("SByte", "System");
			case PrimitiveTypes.I2:
				return this.ResolveType("Int16", "System");
			case PrimitiveTypes.I4:
				return this.ResolveType("Int32", "System");
			case PrimitiveTypes.I8:
				return this.ResolveType("Int64", "System");
			case PrimitiveTypes.U:
				return this.ResolveType("UIntPtr", "System");
			case PrimitiveTypes.U1:
				return this.ResolveType("Byte", "System");
			case PrimitiveTypes.U2:
				return this.ResolveType("UInt16", "System");
			case PrimitiveTypes.U4:
				return this.ResolveType("UInt32", "System");
			case PrimitiveTypes.U8:
				return this.ResolveType("UInt64", "System");
			case PrimitiveTypes.R4:
				return this.ResolveType("Single", "System");
			case PrimitiveTypes.R8:
				return this.ResolveType("Double", "System");
			case PrimitiveTypes.Boolean:
				return this.ResolveType("Boolean", "System");
			case PrimitiveTypes.Enum:
				return this.ResolveType("Enum", "System");
			case PrimitiveTypes.Object:
				return this.ResolveType("Object", "System");
			case PrimitiveTypes.String:
				return this.ResolveType("String", "System");
			case PrimitiveTypes.ValueType:
				return this.ResolveType("ValueType", "System");
			case PrimitiveTypes.Delegate:
				return this.ResolveType("Delegate", "System");
			case PrimitiveTypes.Array:
				return this.ResolveType("Array", "System");
			case PrimitiveTypes.ByRef:
				return this.ResolveType("ByRefPointer", "CooS.Runtime");
			default:
				throw new NotSupportedException(pt.ToString());
			}
		}

		public override AssemblyBase LoadAssembly(string type, Stream stream) {
			AssemblyBase assembly = loaders[type].LoadAssembly(this, stream);
			assemblies.Add(assembly);
			return assembly;
		}

		public override IEnumerable<TypeBase> EnumTypes() {
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool IsSystemAssembly(AssemblyBase assembly) {
			int index = this.assemblies.IndexOf(assembly);
			return index<this.primaryCount;
		}

	}

}
