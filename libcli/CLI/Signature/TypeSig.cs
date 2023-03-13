using System;

namespace CooS.Formats.CLI.Signature {

	public class TypeSig : SignatureBase {

		public readonly ElementType ElementType;
		private readonly TypeDefOrRefEncoded? typeDefOrRef;
		public readonly CustomMod[] CustomMods;
		public readonly TypeSig Type;
		public readonly ArrayShape ArrayShape;
		public readonly MethodSig Method;
		public readonly TypeSig[] BindingTypes;
		private readonly GenericSources? source;
		private readonly int? number;

		public static readonly SignatureFactory Factory = new FactoryImpl();

		internal TypeSig(SignatureReader reader) : base(reader) {
			// CLI仕様にはないんだけど、FreeType2のアセンブリに出現したので。
			/*customMods =*/ SignatureUtility.ReadCustomMods(reader);
			ElementType = reader.ReadMark();
			switch(ElementType) {
			default:
				reader.Dump(Console.Out);
				throw new BadSignatureException("Invalid ElementType: "+(int)ElementType);
			case ElementType.Boolean:
			case ElementType.Char:
			case ElementType.I1:
			case ElementType.U1:
			case ElementType.I2:
			case ElementType.U2:
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R4:
			case ElementType.R8:
			case ElementType.I:
			case ElementType.U:
			case ElementType.String:
			case ElementType.Object:
				return;
			case ElementType.ValueType:
			case ElementType.Class:
				//| VALUETYPE TypeDefOrRefEncoded
				//| CLASS TypeDefOrRefEncoded
				typeDefOrRef = new TypeDefOrRefEncoded(reader);
				return;
			case ElementType.FnPtr:
				//| FNPTR MethodDefInfoSig
				//| FNPTR MethodRefInfoSig
				this.Method = new MethodSig(reader);
				return;
			case ElementType.MnArray:
				//| ARRAY TypeSig ArrayShape  (general array, see clause 22.2.13)
				this.Type = new TypeSig(reader);
				this.ArrayShape = new ArrayShape(reader);
				return;
			case ElementType.SzArray:
				//| SZARRAY CustomMod* TypeSig (single dimensional, zero-based array i.e. vector)
				CustomMods = SignatureUtility.ReadCustomMods(reader);
				Type = new TypeSig(reader);
				return;
			case ElementType.ByRef:			//COOS: Is this correct?
				Type = new TypeSig(reader);
				return;
			case ElementType.ByVal:
				//| PTR CustomMod* VOID
				//| PTR CustomMod* TypeSig
				CustomMods = SignatureUtility.ReadCustomMods(reader);
				Type = new TypeSig(reader);
				return;
			case ElementType.Void:			//COOS: Is this correct?
				return;
			case ElementType.TypedByRef:	//COOS: Is this correct?
				return;
			case ElementType.Pinned:		//COOS: Is this correct?
				Type = new TypeSig(reader);
				return;
			case ElementType.GenericInst:
				switch(reader.ReadMark()) {
				case ElementType.Class:
				case ElementType.ValueType:
					break;
				default:
					throw new BadSignatureException();
				}
				typeDefOrRef = new TypeDefOrRefEncoded(reader);
				BindingTypes = new TypeSig[reader.ReadInt32()];
				for(int i=0; i<BindingTypes.Length; ++i) {
					BindingTypes[i] = new TypeSig(reader);
				}
				return;
			case ElementType.Var:
				source = GenericSources.Type;
				number = reader.ReadInt32();
				return;
			case ElementType.MVar:
				source = GenericSources.Method;
				number = reader.ReadInt32();
				return;
			case ElementType.CModReqd:
			case ElementType.CModOpt:
			case ElementType.Internal:
			case ElementType.Modifier:
			case ElementType.Sentinel:
				reader.Dump(Console.Out);
				throw new BadSignatureException("TypeSig::Parse detects unsupported beginning: 0x"+((int)ElementType).ToString("X"));
			}
		}

		public bool Void {
			get {
				return this.ElementType==ElementType.Void;
			}
		}

		public bool ByRef {
			get {
				return this.ElementType==ElementType.ByRef;
			}
		}

		public bool ByVal {
			get {
				return this.ElementType==ElementType.ByVal;
			}
		}

		public bool TypedByRef {
			get {
				return this.ElementType==ElementType.TypedByRef;
			}
		}

		public bool Pinned {
			get {
				return this.ElementType==ElementType.Pinned;
			}
		}

		public TypeDefOrRefEncoded TypeDefOrRef {
			get {
				return this.typeDefOrRef.Value;
			}
		}

		public int Number {
			get {
				return this.number.Value;
			}
		}

		public GenericSources TypeSource {
			get {
				return this.source.Value;
			}
		}

		class FactoryImpl : SignatureFactory {

			internal override object Parse(SignatureReader reader) {
				return new TypeSig(reader);
			}

		}

	}

}
