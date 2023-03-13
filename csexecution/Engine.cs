using System;
using CooS.Reflection;
using System.Collections.Generic;

namespace CooS {
	using Execution;
	
	public class Engine {

		public readonly World World;

		readonly Dictionary<TypeBase, TypeInfo> typecache = new Dictionary<TypeBase, TypeInfo>();
		readonly Dictionary<FieldBase, FieldInfo> fieldcache = new Dictionary<FieldBase, FieldInfo>();
		readonly Dictionary<MethodBase, MethodInfo> methodcache = new Dictionary<MethodBase, MethodInfo>();

		public Engine(World world) {
			this.World = world;
		}

		public int SzArrayOffsetToContents {
			get {
				return Architecture.Target.AddressSize*2;
			}
		}

		public virtual TypeInfo Resolve(PrimitiveTypes type) {
			return this.Realize(this.World.Resolve(type));
		}

		public virtual TypeInfo Realize(TypeBase type) {
			if(typecache.ContainsKey(type)) {
				return typecache[type];
			}
			return typecache[type] = new TypeInfo(this, type);
		}

		public virtual FieldInfo Realize(FieldBase field) {
			if(fieldcache.ContainsKey(field)) {
				return fieldcache[field];
			}
			return fieldcache[field] = new FieldInfo(this, field);
		}

		public virtual MethodInfo Realize(MethodBase method) {
			if(methodcache.ContainsKey(method)) {
				return methodcache[method];
			}
			return methodcache[method] = new MethodInfo(method);
		}

		public unsafe void MakeSzArrayHeader(byte[] buf, TypeInfo type, int length) {
			fixed(byte* p = buf) {
				*(int*)p = -1;	// Invalid Address
				*(int*)(p+4) = length;
			}
		}

		#region ÉfÉåÉQÅ[Ég

		public void ConstructDelegate(Delegate obj, object target, IntPtr ftn) {
			SetDelegateTarget(obj, target);
			SetFunctionPointer(obj, ftn);
		}

		private FieldInfo pointer_field;
		private FieldInfo target_field;

		private FieldInfo DelegatePointerField {
			get {
				if(pointer_field==null) {
					pointer_field = this.Realize(this.World.Delegate.FindField("method_ptr"));
					if(pointer_field==null) {
						throw new FieldNotFoundException();
					}
				}
				return pointer_field;
			}
		}

		private FieldInfo DelegateTargetField {
			get {
				if(target_field==null) {
					target_field = this.Realize(this.World.Delegate.FindField("m_target"));
					if(target_field==null) {
						throw new FieldNotFoundException();
					}
				}
				return target_field;
			}
		}

		public IntPtr GetFunctionPointer(Delegate obj) {
			//return obj.Pointer;
			return (IntPtr)DelegatePointerField.GetValue(obj);
		}

		internal void SetFunctionPointer(Delegate obj, IntPtr pfn) {
			//obj.Pointer = pfn;
			DelegatePointerField.SetValue(obj, pfn);
		}

		public object GetDelegateTarget(Delegate obj) {
			return obj.Target;
		}

		internal void SetDelegateTarget(Delegate obj, object target) {
			DelegateTargetField.SetValue(obj, target);
		}

		#endregion

	}

}
