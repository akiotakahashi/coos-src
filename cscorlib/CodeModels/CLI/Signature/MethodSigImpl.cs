using System;
using System.Collections;
using CooS.CodeModels.CLI.Metatype;

namespace CooS.CodeModels.CLI.Signature {

	class MethodSigImpl : MethodSig {

		CallingConventionFlags flags;
		RetType retType;
		ParamSig[] paramlist;
	
		public MethodSigImpl(SignatureReader reader) {
			flags = (CallingConventionFlags)reader.ReadByte();
			int paramCount = reader.ReadInt32();
			retType = new RetType(reader);
			ArrayList list = new ArrayList();
			for(int i=0; i<paramCount; ++i) {
				list.Add(new ParamSig(reader));
			}
			paramlist = (ParamSig[])list.ToArray(typeof(ParamSig));
		}

		public CallingConventionFlags Flags {
			get {
				return this.flags;
			}
		}

		public override bool HasThis {
			get {
				return 0!=(flags & CallingConventionFlags.HASTHIS);
			}
		}

		public bool ExplicitThis {
			get {
				return 0!=(flags & CallingConventionFlags.EXPLICITTHIS);
			}
		}

		public bool VarArg {
			get {
				return 0!=(flags & CallingConventionFlags.VARARG);
			}
		}

		public bool CLang {
			get {
				return 0!=(flags & CallingConventionFlags.C_LANG);
			}
		}

		public bool StdCall {
			get {
				return 0!=(flags & CallingConventionFlags.STDCALL);
			}
		}

		public bool ThisCall {
			get {
				return 0!=(flags & CallingConventionFlags.THISCALL);
			}
		}

		public bool FastCall {
			get {
				return 0!=(flags & CallingConventionFlags.FASTCALL);
			}
		}

		public bool Default {
			get {
				return !VarArg && !CLang && !StdCall && !ThisCall && !FastCall;
			}
		}

		public RetType RetType {
			get {
				return this.retType;
			}
		}

		public ParamSig[] Params {
			get {
				return this.paramlist;
			}
		}

		public override int ParameterCount {
			get {
				return this.paramlist.Length;
			}
		}

		public override SuperType GetReturnType(AssemblyDef assembly) {
			return (SuperType)this.RetType.Type.ResolveTypeAt((AssemblyDef)assembly);
		}

		public override ParamSig[] GetParameters(AssemblyDef assembly) {
			return this.paramlist;
		}

		public override ParamSig GetParameter(int index, AssemblyDef assembly) {
			return this.paramlist[index];
		}

	}

}
