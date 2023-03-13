using System;
using System.Reflection;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	class TypeUtility {

		public static bool IsNestedFlag(TypeAttributes flag) {
			switch(flag&TypeAttributes.VisibilityMask) {
			case TypeAttributes.NestedAssembly:
			case TypeAttributes.NestedFamANDAssem:
			case TypeAttributes.NestedFamily:
			case TypeAttributes.NestedFamORAssem:
			case TypeAttributes.NestedPrivate:
			case TypeAttributes.NestedPublic:
				return true;
			default:
				return false;
			}
		}

		public static bool IsPublicFlag(TypeAttributes flag) {
			switch(flag&TypeAttributes.VisibilityMask) {
			case TypeAttributes.Public:
			case TypeAttributes.NestedPublic:
				return true;
			default:
				return false;
			}
		}

		public static AssemblyName ConvertToAssemblyName(AssemblyDefInfo assembly, AssemblyRefRow asmref) {
			AssemblyName asmname = new AssemblyName();
			asmname.Name = assembly.LoadBlobString(asmref.Name);
			asmname.Version = new Version(asmref.MajorVersion, asmref.MinorVersion, asmref.BuildNumber, asmref.RevisionNumber);
			return asmname;
		}

		public static bool MatchesSignature(MethodDeclInfo method, MethodSig signature, AssemblyDefInfo assembly) {
			if(method.IsStatic==signature.HasThis) return false;
			if(method.ParameterCount!=signature.ParameterCount) return false;
			int n = method.ParameterCount;
			for(int i=0; i<n; ++i) {
				TypeDeclInfo paramtype = method.GetParameterType(i);
				if(paramtype!=assembly.LookupType(signature.Params[i].Type)) {
					return false;
				}
			}
			return true;
		}

		public static int GetGenericParameterCount(string name) {
			int i = name.LastIndexOf('`');
			if(i<0) return 0;
			int c;
			string suffix = name.Substring(i+1);
			return int.TryParse(suffix, out c) ? c : 0;
		}

		public static bool IsGenericName(string name) {
			return GetGenericParameterCount(name)>0;
		}

	}

}
