using System;
using System.Reflection;
using CooS.CodeModels.CLI.Metadata;
using CooS.Reflection;
using CooS.CodeModels.CLI;
using CooS.CodeModels.CLI.Signature;

namespace CooS.CodeModels.CLI {

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

		public static AssemblyName ConvertToAssemblyName(AssemblyDef assembly, AssemblyRefRow asmref) {
			AssemblyName asmname = new AssemblyName();
			asmname.Name = assembly.Metadata.Strings[asmref.Name];
			asmname.Version = new Version(asmref.MajorVersion, asmref.MinorVersion, asmref.BuildNumber, asmref.RevisionNumber);
			return asmname;
		}

		public static bool MatchesSignature(MethodBase method, MethodSig signature, AssemblyDef assembly) {
			if(method.IsStatic && signature.HasThis) return false;
			if(!method.IsStatic && !signature.HasThis) return false;
			ParameterInfo[] parameters = method.GetParameters();
			if(parameters.Length!=signature.ParameterCount) return false;
			for(int i=0; i<parameters.Length; ++i) {
				if(parameters[i].ParameterType!=signature.GetParameter(i,assembly).Type.ResolveTypeAt(assembly)) {
					return false;
				}
			}
			return true;
		}

	}

}
