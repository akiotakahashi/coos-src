using System;
using System.Collections;
using CooS.Reflection;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Signature {

	class TypeSig {

		ElementType elementType;
		TypeDefOrRefEncoded typeDefOrRef;
		CustomMod[] customMods;
		TypeSig type;
		bool _void;
		bool _byref;
		bool _pinned;
		bool _typedbyref;
		MethodSig method;

		public TypeSig(SignatureReader reader) {
			//COOS: CLI仕様にはないんだけど、FreeType2のアセンブリに出現したので。
			customMods = SignatureUtility.ReadCustomMods(reader);
			//END OF COSTOMIZE
			elementType = reader.ReadMark();
			switch(elementType) {
			default:
				reader.Dump();
				throw new BadSignatureException("Invalid ElementType: "+(int)elementType);
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
			case ElementType.Ptr:
				//| PTR CustomMod* VOID
				//| PTR CustomMod* TypeSig
				customMods = SignatureUtility.ReadCustomMods(reader);
				type = new TypeSig(reader);
				return;
			case ElementType.FnPtr:
				//| FNPTR MethodDefSig
				//| FNPTR MethodRefSig
				this.method = new MethodSigImpl(reader);
				return;
			case ElementType.Array:
				//| ARRAY TypeSig ArrayShape  (general array, see clause 22.2.13)
				throw new NotImplementedException("Not Implemented: ElementType.ARRAY");
			case ElementType.SzArray:
				//| SZARRAY CustomMod* TypeSig (single dimensional, zero-based array i.e. vector)
				customMods = SignatureUtility.ReadCustomMods(reader);
				type = new TypeSig(reader);
				return;
			case ElementType.ByRef:			//COOS: Is this correct?
				_byref = true;
				type = new TypeSig(reader);
				return;
			case ElementType.Void:			//COOS: Is this correct?
				_void = true;
				return;
			case ElementType.TypedByRef:	//COOS: Is this correct?
				_typedbyref = true;
				return;
			case ElementType.Pinned:		//COOS: Is this correct?
				_pinned = true;
				type = new TypeSig(reader);
				return;
			case ElementType.CModReqd:
			case ElementType.CModOpt:
			case ElementType.Internal:
			case ElementType.Modifier:
			case ElementType.Sentinel:
				reader.Dump();
				throw new BadSignatureException("TypeSig::Parse detects unsupported beginning: 0x"+((int)elementType).ToString("X"));
			}
		}

		public override string ToString() {
			return "TypeSig:0x"+((int)this.elementType).ToString("X2")+","+this.typeDefOrRef;
		}

		public ElementType ElementType {
			get {
				return this.elementType;
			}
		}

		public CustomMod[] CustomMods {
			get {
				return this.customMods;
			}
		}

		public TypeSig Type {
			get {
				return this.type;
			}
		}

		public TypeDefOrRefEncoded TypeDefOrRef {
			get {
				return this.typeDefOrRef;
			}
		}

		public MethodSig Method {
			get {
				return this.method;
			}
		}

		public bool Void {
			get {
				return this._void;
			}
		}

		public bool ByRef {
			get {
				return this._byref;
			}
		}

		public bool TypedByRef {
			get {
				return this._typedbyref;
			}
		}

		public TypeImpl ResolveTypeAt(AssemblyDef assembly) {
			if(assembly==null) throw new ArgumentNullException("assembly");
			switch(elementType) {
			default:
				throw new BadSignatureException();
			case ElementType.Boolean:
				return assembly.Manager.Boolean;
			case ElementType.Char:
				return assembly.Manager.Char;
			case ElementType.I1:
				return assembly.Manager.SByte;
			case ElementType.U1:
				return assembly.Manager.Byte;
			case ElementType.I2:
				return assembly.Manager.Int16;
			case ElementType.U2:
				return assembly.Manager.UInt16;
			case ElementType.I4:
				return assembly.Manager.Int32;
			case ElementType.U4:
				return assembly.Manager.UInt32;
			case ElementType.I8:
				return assembly.Manager.Int64;
			case ElementType.U8:
				return assembly.Manager.UInt64;
			case ElementType.R4:
				return assembly.Manager.Single;
			case ElementType.R8:
				return assembly.Manager.Double;
			case ElementType.I:
				return assembly.Manager.IntPtr;
			case ElementType.U:
				return assembly.Manager.UIntPtr;
			case ElementType.String:
				return assembly.Manager.String;
			case ElementType.Object:
				return assembly.Manager.Object;
			case ElementType.Void:
				return assembly.Manager.Void;
			case ElementType.TypedByRef:
				return assembly.Manager.ResolveType("System.TypedReference",true);
			case ElementType.ValueType:
			case ElementType.Class:
				return this.typeDefOrRef.ResolveTypeAt(assembly);
			case ElementType.ByRef:
				return (SuperType)type.ResolveTypeAt(assembly).GetByRefPointerType();
			case ElementType.Ptr:
				return (SuperType)type.ResolveTypeAt(assembly).GetByValPointerType();
			case ElementType.SzArray:
				return (SuperType)type.ResolveTypeAt(assembly).GetSzArrayType();
			case ElementType.Array:
				throw new NotImplementedException("Not Implemented: ElementType.ARRAY");
			case ElementType.FnPtr:
				throw new NotImplementedException("Not Implemented: ElementType.FNPTR");
			case ElementType.Pinned:
				return this.type.ResolveTypeAt(assembly);
			case ElementType.CModReqd:
			case ElementType.CModOpt:
			case ElementType.Internal:
			case ElementType.Modifier:
			case ElementType.Sentinel:
				throw new BadSignatureException("TypeSig::Parse detects unsupported beginning: "+(int)elementType);
			}
		}

	}

}
