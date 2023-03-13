using System;
using System.Collections.Generic;
using CooS.Formats.Java;
using CooS.Formats.Java.Description;

namespace CooS.Reflection.Java {
	
	public class AssemblyImpl : AssemblyBase {

		private readonly World world;
		private readonly AssemblyDefInfo entity;
		private readonly TypeImpl[] types;
		private readonly FieldImpl[] fields;
		private readonly MethodImpl[] methods;

		internal AssemblyImpl(World world, AssemblyDefInfo assembly) {
			this.world = world;
			this.entity = assembly;
			this.types = new TypeImpl[assembly.TypeCount];
			this.fields = new FieldImpl[assembly.FieldCount];
			this.methods = new MethodImpl[assembly.MethodCount];
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
				return this.entity.Name;
			}
		}

		public override Version Version {
			get {
				return new Version();
			}
		}

		public override System.Globalization.CultureInfo Culture {
			get {
				return System.Globalization.CultureInfo.InvariantCulture;
			}
		}

		public override MethodBase EntryPoint {
			get {
				return null;
			}
		}

		private TypeImpl Realize(int index, TypeDefInfo typedef) {
			if(types[index]==null) {
				if(typedef==null) {
					typedef = this.entity.GetTypeInfo(index);
				}
				TypeImpl type;
				if(typedef.IsInterface) {
					type = new Metatype.InterfaceType(this, typedef);
				} else {
					type = new Metatype.ClassType(this, typedef);
				}
				return types[index] = type;
			}
			return types[index];
		}

		public TypeImpl Realize(TypeDefInfo typedef) {
			return this.Realize(typedef.Index, typedef);
		}

		public TypeImpl Realize(TypeDeclInfo typedef) {
			throw new NotImplementedException();
		}

		public FieldImpl Realize(FieldDefInfo field) {
			if(fields[field.Index]==null) {
				return fields[field.Index] = new FieldImpl(this.Realize(field.Type), field);
			}
			return fields[field.Index];
		}

		public MethodImpl Realize(MethodDefInfo method) {
			if(methods[method.Index]==null) {
				return methods[method.Index] = new MethodImpl(this.Realize(method.Type), method);
			}
			return methods[method.Index];
		}

		private MemberRefDesc ConvertToDescriptor(MemberRefInfo member) {
			throw new Exception("The method or operation is not implemented.");
		}

		public MemberBase Realize(MemberRefInfo member) {
			TypeBase type;
			string res = member.Class.Replace('/','.');
			int index = res.LastIndexOf('.');
			if(index<0) {
				type = this.world.ResolveType(res, null);
			} else {
				type = this.world.ResolveType(res.Substring(index+1), res.Substring(0,index));
			}
			return type.FindMember(ConvertToDescriptor(member));
		}

		public MemberBase Realize(MemberDeclInfo member) {
			if(member is MethodDefInfo) {
				return this.Realize((MethodDefInfo)member);
			} else if(member is FieldDefInfo) {
				return this.Realize((FieldDefInfo)member);
			} else if(member is MemberRefInfo) {
				return this.Realize((MemberRefInfo)member);
			} else {
				throw new NotImplementedException();
			}
		}

		public MemberBase RealizeMember(FieldSig sig) {
			throw new NotImplementedException();
		}

		public override TypeBase GetTypeById(int id) {
			return this.Realize(id, null);
		}

		public override IEnumerable<TypeBase> EnumTypes() {
			foreach(TypeDefInfo typedef in this.entity.EnumTypes(false)) {
				yield return this.Realize(typedef);
			}
		}

		public IEnumerable<TypeBase> EnumNestedTypes(TypeImpl enclosing) {
			foreach(TypeDefInfo typedef in this.entity.EnumTypes(false)) {
				if(this.Realize(typedef.EnclosingType)==enclosing) {
					yield return this.Realize(typedef);
				}
			}
		}

		public override TypeBase FindType(string name, string ns) {
			TypeDefInfo typedef = this.entity.FindType(name, ns);
			if(typedef==null) {
				return null;
			}
			return Realize(typedef);
		}

		public override TypeBase FindType(string name) {
			throw new NotImplementedException();
		}

	}

}
