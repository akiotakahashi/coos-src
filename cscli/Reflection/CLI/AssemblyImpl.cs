using System;
using System.Collections.Generic;
using CooS.Formats.CLI;
using CooS.Formats.CLI.Signature;

namespace CooS.Reflection.CLI {
	
	public class AssemblyImpl : AssemblyBase {

		private readonly World world;
		private readonly AssemblyDefInfo entity;
		private readonly TypeImpl[] types;
		private readonly FieldImpl[] fields;
		private readonly MethodImpl[] methods;
		private readonly MethodBase[] methodspecs;
		private Dictionary<int, FnPtrImpl> fnptrs = null;

		internal AssemblyImpl(World world, AssemblyDefInfo assembly) {
			this.world = world;
			this.entity = assembly;
			this.types = new TypeImpl[assembly.TypeDefCount];
			this.fields = new FieldImpl[assembly.FieldDefCollection.Count];
			this.methods = new MethodImpl[assembly.MethodDefCollection.Count];
			this.methodspecs = new MethodBase[assembly.MethodSpecCollection.Count];
		}

		public override void Dispose() {
			this.entity.Dispose();
		}

		public override World World {
			get {
				return this.world;
			}
		}

		public override string Name {
			get {
				return this.entity.AssemblyName.Name;
			}
		}

		public override Version Version {
			get {
				return this.entity.AssemblyName.Version;
			}
		}

		public override System.Globalization.CultureInfo Culture {
			get {
				return this.entity.AssemblyName.CultureInfo;
			}
		}

		public override MethodBase EntryPoint {
			get {
				MethodDefInfo ep = this.entity.EntryPoint;
				if(ep==null) return null;
				return this.Realize(ep);
			}
		}

		internal AssemblyBase Realize(AssemblyRefInfo assemblyref) {
			AssemblyBase assembly = world.ResolveAssembly(assemblyref.Name, assemblyref.Culture, assemblyref.Version);
			if(assembly==null) { throw new AssemblyNotFoundException(assemblyref.Name); }
			return assembly;
		}

		private TypeImpl Realize(int index, TypeDefInfo typedef) {
			if(types[index]==null) {
				if(typedef==null) {
					typedef = this.entity.GetTypeDefByIndex(index);
				}
				TypeImpl type;
				if(typedef.IsInterface) {
					type = new Metatype.InterfaceType(this, typedef);
				} else {
					if(this.world.IsSystemAssembly(this) && typedef.Namespace=="System") {
						switch(typedef.Name) {
						case "Boolean":
						case "Char":
						case "SByte":
						case "Byte":
						case "Int32":
						case "Int64":
						case "Int16":
						case "IntPtr":
						case "UInt16":
						case "UInt32":
						case "UInt64":
						case "UIntPtr":
						case "Single":
						case "Double":
							type = new Metatype.PrimitiveType(this, typedef);
							break;
						case "Object":
							type = new Metatype.ObjectType(this, typedef);
							break;
						case "Enum":
							type = new Metatype.StructType(this, typedef);
							break;
						case "ValueType":
							type = new Metatype.ClassType(this, typedef);
							break;
						case "Delegate":
							type = new Metatype.ClassType(this, typedef);
							break;
						default:
							type = null;
							break;
						}
					} else {
						type = null;
					}
					if(type==null) {
						TypeBase t = this.Realize(typedef.BaseType);
						if(this.world.Resolve(PrimitiveTypes.Enum).IsAssignableFrom(t)) {
							type = new Metatype.EnumType(this, typedef);
						} else if(this.world.Resolve(PrimitiveTypes.ValueType).IsAssignableFrom(t)) {
							type = new Metatype.StructType(this, typedef);
						} else if(this.world.Resolve(PrimitiveTypes.Delegate).IsAssignableFrom(t)) {
							type = new Metatype.DelegateType(this, typedef);
						} else {
							type = new Metatype.ClassType(this, typedef);
						}
					}
				}
				return types[index] = type;
			}
			return types[index];
		}

		internal TypeImpl Realize(TypeDefInfo typedef) {
			if(typedef==null) {
				return null;
			} else {
				return this.Realize(typedef.Index, typedef);
			}
		}

		internal TypeBase Realize(TypeRefInfo typeref) {
			IResolutionScope res = typeref.ResolutionScope;
			if(res is AssemblyRefInfo) {
				AssemblyBase scope = this.Realize((AssemblyRefInfo)res);
				return scope.FindType(typeref.Name, typeref.Namespace);
			} else if(res is TypeRefInfo) {
				TypeBase scope = this.Realize((TypeRefInfo)res);
				if(typeref.Namespace!=null) {
					throw new BadImageFormatException();
				}
				return scope.FindType(typeref.Name);
			} else {
				throw new NotSupportedException(res.GetType().FullName);
			}
		}

		internal TypeBase Realize(TypeSpecInfo typespec) {
			return this.Realize(typespec.TypeSig, null, null);
		}

		internal TypeBase Realize(SpecializedTypeInfo spcltype) {
			TypeDeclInfo generictype = spcltype.GenericType;
			TypeSig[] arguments = spcltype.GenericArguments;
			TypeBase[] args = new TypeBase[arguments.Length];
			for(int i=0; i<arguments.Length; ++i) {
				args[i] = this.Realize(arguments[i], null, null);
			}
			return this.Realize(generictype).Specialize(args);
		}

		List<GenericParameterType> tvars = new List<GenericParameterType>();
		List<GenericParameterType> mvars = new List<GenericParameterType>();

		internal TypeBase Realize(GenericParamInfo type) {
			List<GenericParameterType> vars;
			switch(type.SourceType) {
			case CooS.Formats.GenericSources.Type:
				vars = tvars;
				break;
			case CooS.Formats.GenericSources.Method:
				vars = mvars;
				break;
			default:
				throw new UnexpectedException();
			}
			while(type.Number>=vars.Count) {
				vars.Add(null);
			}
			if(vars[type.Number]!=null) {
				return vars[type.Number];
			} else {
				return vars[type.Number] = new GenericParameterType(this, type.SourceType, type.Number);
			}
		}

		internal TypeBase Realize(TypeDeclInfo type) {
			if(type==null) {
				return null;
			} else if(type is TypeDefInfo) {
				return this.Realize((TypeDefInfo)type);
			} else if(type is TypeRefInfo) {
				return this.Realize((TypeRefInfo)type);
			} else if(type is TypeSpecInfo) {
				return this.Realize((TypeSpecInfo)type);
			} else if(type is SpecializedTypeInfo) {
				return this.Realize((SpecializedTypeInfo)type);
			} else if(type is GenericParamInfo) {
				return this.Realize((GenericParamInfo)type);
			} else if(type is DerivedTypeInfo) {
				DerivedTypeInfo dt = (DerivedTypeInfo)type;
				TypeBase bt = this.Realize(dt.BaseType);
				switch(dt.Kind) {
				case ElementType.ByRef:
					return bt.GetByRefPointerType();
				case ElementType.ByVal:
					return bt.GetByValPointerType();
				case ElementType.SzArray:
					return bt.GetSzArrayType();
				case ElementType.MnArray:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException();
				}
			} else if(type is FnPtrInfo) {
				if(this.fnptrs==null) { this.fnptrs = new Dictionary<int, FnPtrImpl>(); }
				FnPtrInfo fnptr = (FnPtrInfo)type;
				if(fnptrs.ContainsKey(fnptr.RowIndex)) {
					return fnptrs[fnptr.RowIndex];
				} else {
					return fnptrs[fnptr.RowIndex] = new FnPtrImpl(this, fnptr);
				}
			} else {
				throw new NotSupportedException(type.GetType().FullName);
			}
		}

		internal FieldImpl Realize(FieldDefInfo field) {
			if(field==null) {
				return null;
			} else {
				int idx = field.RowIndex-1;
				if(fields[idx]==null) {
					return fields[idx] = new FieldImpl(this.Realize(field.TypeDef), field);
				}
				return fields[idx];
			}
		}

		internal FieldImpl Realize(FieldDeclInfo field) {
			if(field==null) {
				return null;
			} else if(field is FieldDefInfo) {
				return this.Realize((FieldDefInfo)field);
			} else {
				throw new NotImplementedException();
			}
		}

		internal MethodImpl Realize(MethodDefInfo method) {
			if(method==null) {
				return null;
			} else {
				int idx = method.RowIndex-1;
				if(methods[idx]==null) {
					return methods[idx] = new RegularMethodImpl(this.Realize(method.TypeDef), method);
				}
				return methods[idx];
			}
		}

		internal MethodBase Realize(MethodSpecInfo spec) {
			if(spec==null) {
				return null;
			} else if(this.methodspecs[spec.Index]!=null) {
				return this.methodspecs[spec.Index];
			} else {
				TypeBase[] args = new TypeBase[spec.GenericParameterCount];
				int i=0;
				foreach(TypeDeclInfo type in spec.EnumGenericArgumentTypes()) {
					args[i++] = this.Realize(type);
				}
				return this.methodspecs[spec.Index] = this.Realize(spec.Method).Specialize(args);
			}
		}

		internal MethodBase Realize(MethodDeclInfo method) {
			if(method==null) {
				return null;
			} else if(method is MethodDefInfo) {
				return this.Realize((MethodDefInfo)method);
			} else if(method is MethodSpecInfo) {
				return this.Realize((MethodSpecInfo)method);
			} else {
				throw new NotImplementedException();
			}
		}

		internal TypeBase Realize(TypeSig sig, TypeSig[] targs, TypeSig[] margs) {
			switch(sig.ElementType) {
			case ElementType.I:
				return this.world.IntPtr;
			case ElementType.U:
				return this.world.UIntPtr;
			case ElementType.I1:
				return this.world.SByte;
			case ElementType.I2:
				return this.world.Int16;
			case ElementType.I4:
				return this.world.Int32;
			case ElementType.I8:
				return this.world.Int64;
			case ElementType.U1:
				return this.world.Byte;
			case ElementType.U2:
				return this.world.UInt16;
			case ElementType.U4:
				return this.world.UInt32;
			case ElementType.U8:
				return this.world.UInt64;
			case ElementType.R4:
				return this.world.Single;
			case ElementType.R8:
				return this.world.Double;
			case ElementType.Char:
				return this.world.Char;
			case ElementType.Boolean:
				return this.world.Boolean;
			case ElementType.Object:
				return this.world.Object;
			case ElementType.String:
				return this.world.String;
			case ElementType.Void:
				return this.world.Void;
			case ElementType.ValueType:
				return this.Realize(this.entity.LookupType(sig));
			case ElementType.Class:
				return this.Realize(this.entity.LookupType(sig));
			case ElementType.SzArray:
				return this.Realize(sig.Type, targs, margs).GetSzArrayType();
			case ElementType.MnArray:
				//return this.Realize(sig.Type).GetMnArrayType();
				goto default;
			case ElementType.ByRef:
				return this.Realize(sig.Type, targs, margs).GetByRefPointerType();
			case ElementType.ByVal:
				return this.Realize(sig.Type, targs, margs).GetByValPointerType();
			case ElementType.GenericInst:
				return this.Realize(this.entity.LookupType(sig));
			case ElementType.Var:
				if(targs==null) {
					return this.Realize(this.entity.LookupType(sig));
				} else {
					return this.Realize(targs[sig.Number], targs, margs);
				}
			case ElementType.MVar:
				if(margs==null) {
					return this.Realize(this.entity.LookupType(sig));
				} else {
					return this.Realize(margs[sig.Number], targs, margs);
				}
			default:
				throw new NotSupportedException(sig.ElementType.ToString());
			}
		}

		private TypeBase Realize(RetType sig, TypeSig[] targs, TypeSig[] margs) {
			if(sig.Void) {
				return this.World.Void;
			} else {
				return this.Realize(sig.Type, targs, margs);
			}
		}

		private MemberRefDesc ConvertToDescriptor(MemberRefInfo member) {
			MemberRefDesc desc;
			desc.name = member.Name;
			TypeSig[] gargs = member.Type.GenericArguments;
			MemberSig sig = member.Signature;
			switch(member.Kind) {
			case System.Reflection.MemberTypes.Field:
				FieldSig fsig = (FieldSig)sig;
				desc.returntype = this.Realize(fsig.Type, gargs, null);
				desc.parameters = null;
				break;
			case System.Reflection.MemberTypes.Constructor:
			case System.Reflection.MemberTypes.Method:
				MethodSig msig = (MethodSig)sig;
				desc.returntype = this.Realize(msig.RetType, gargs, null);
				desc.parameters = new TypeBase[msig.ParameterCount];
				for(int i=0; i<msig.ParameterCount; ++i) {
					desc.parameters[i] = this.Realize(msig.Params[i].Type, member.Type.GenericArguments, null);
				}
				break;
			default:
				throw new UnexpectedException(sig.GetType().FullName);
			}
			return desc;
		}

		internal MemberBase Realize(MemberRefInfo member) {
			if(member==null) return null;
			IMemberRefParent res = member.Class;
			if(res is MethodDefInfo) {
				return this.Realize((MethodDefInfo)res);
			} else {
				TypeBase type;
				if(res is TypeDefInfo) {
					type = this.Realize((TypeDefInfo)res);
				} else if(res is TypeRefInfo) {
					type = this.Realize((TypeRefInfo)res);
				} else if(res is TypeSpecInfo) {
					type = this.Realize((TypeSpecInfo)res);
				} else {
					throw new NotSupportedException(res.GetType().FullName);
				}
				MemberBase ret = type.FindMember(ConvertToDescriptor(member));
				if(ret==null) { throw new MemberNotFoundException(res.Name+":"+member.Name); }
				return ret;
			}
		}

		internal MemberBase Realize(MemberDeclInfo member) {
			if(member==null) return null;
			if(member is MethodDefInfo) {
				return this.Realize((MethodDefInfo)member);
			} else if(member is FieldDefInfo) {
				return this.Realize((FieldDefInfo)member);
			} else if(member is MemberRefInfo) {
				return this.Realize((MemberRefInfo)member);
			} else if(member is MethodSpecInfo) {
				return this.Realize((MethodSpecInfo)member);
			} else {
				throw new NotImplementedException(member.GetType().FullName);
			}
		}

		public override TypeBase GetTypeById(int id) {
			return this.Realize(id, null);
		}

		public override IEnumerable<TypeBase> EnumTypes() {
			foreach(TypeDefInfo typedef in this.entity.TypeDefCollection) {
				yield return this.Realize(typedef);
			}
		}

		public IEnumerable<TypeBase> EnumNestedTypeCandidatesIn(string name) {
			foreach(TypeDefInfo typedef in this.entity.EnumNestedTypeCandidatesIn(name)) {
				yield return this.Realize(typedef);
			}
		}

		public override TypeBase FindType(string name) {
			TypeDefInfo typedef = this.entity.SearchTypeDef(name);
			if(typedef==null) {
				return null;
			}
			return Realize(typedef);
		}

		public override TypeBase FindType(string name, string ns) {
			TypeDefInfo typedef = this.entity.SearchTypeDef(name, ns);
			if(typedef==null) {
				return null;
			}
			return Realize(typedef);
		}

		internal AssemblyBase Resolve(AssemblyRefInfo asmref, World world) {
			return world.ResolveAssembly(asmref.Name, asmref.Culture, asmref.Version);
		}

	}

}
