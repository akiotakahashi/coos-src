using System;

namespace CooS.Formats.CLI.Signature {

	public class MethodSig : MemberSig {

		public readonly CallingConventionFlags Flags;
		public readonly int GenericParameterCount;
		public readonly RetType RetType;
		public readonly ParamSig[] Params;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal MethodSig(SignatureReader reader) : base(reader) {
			this.Flags = (CallingConventionFlags)reader.ReadByte();
			if(0!=(Flags&CallingConventionFlags.GENERIC)) {
				this.GenericParameterCount = reader.ReadInt32();
			} else {
				this.GenericParameterCount = 0;
			}
			int paramCount = reader.ReadInt32();
			this.RetType = new RetType(reader);
			this.Params = new ParamSig[paramCount];
			for(int i=0; i<paramCount; ++i) {
				this.Params[i] = new ParamSig(reader);
			}
		}

		public bool Generic {
			get {
				return 0!=(Flags & CallingConventionFlags.GENERIC);
			}
		}

		public bool HasThis {
			get {
				return 0!=(Flags & CallingConventionFlags.HASTHIS);
			}
		}

		public bool ExplicitThis {
			get {
				return 0!=(Flags & CallingConventionFlags.EXPLICITTHIS);
			}
		}

		public bool VarArg {
			get {
				return 0!=(Flags & CallingConventionFlags.VARARG);
			}
		}

		public bool CLang {
			get {
				return 0!=(Flags & CallingConventionFlags.C_LANG);
			}
		}

		public bool StdCall {
			get {
				return 0!=(Flags & CallingConventionFlags.STDCALL);
			}
		}

		public bool ThisCall {
			get {
				return 0!=(Flags & CallingConventionFlags.THISCALL);
			}
		}

		public bool FastCall {
			get {
				return 0!=(Flags & CallingConventionFlags.FASTCALL);
			}
		}

		public bool Default {
			get {
				return !VarArg && !CLang && !StdCall && !ThisCall && !FastCall;
			}
		}

		public int ParameterCount {
			get {
				return this.Params.Length;
			}
		}

	}

}
