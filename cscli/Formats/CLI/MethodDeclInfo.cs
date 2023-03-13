using System;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using MethodAttributes=System.Reflection.MethodAttributes;
	using MethodImplAttributes=System.Reflection.MethodImplAttributes;

	abstract class MethodDeclInfo : MemberDeclInfo {

		public MethodDeclInfo(AssemblyDefInfo assembly) : base(assembly) {
		}

		public override System.Reflection.MemberTypes Kind {
			get {
				return System.Reflection.MemberTypes.Method;
			}
		}

		public string FullName {
			get {
				return this.Type.FullName+":"+this.Name;
			}
		}

		public bool IsStatic {
			get {
				return !this.Signature.HasThis;
			}
		}

		public bool IsGeneric {
			get {
				return this.Signature.Generic;
			}
		}

		public int GenericParameterCount {
			get {
				return this.Signature.GenericParameterCount;
			}
		}

		public override bool ContainsGenericParameters {
			get {
				return this.IsGeneric;
			}
		}

		#region シグニチャ

		internal abstract MethodSig Signature {
			get;
		}

		public virtual TypeDeclInfo ReturnType {
			get {
				if(this.Signature.RetType.Void) {
					return this.Assembly.LookupPrimitiveType(ElementType.Void);
				} else if(this.Signature.RetType.TypedByRef) {
					return this.Assembly.LookupPrimitiveType(ElementType.TypedByRef);
				} else {
					return this.Assembly.LookupType(this.Signature.RetType.Type);
				}
			}
		}

		#endregion

		#region パラメタ

		public virtual int ParameterCount {
			get {
				return this.Signature.ParameterCount;
			}
		}

		public virtual TypeDeclInfo GetParameterType(int index) {
			return this.Assembly.LookupType(this.Signature.Params[index].Type);
		}

		#endregion

	}

}
