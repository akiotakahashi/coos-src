using System;
using System.Collections.Generic;

namespace CooS.Reflection.Generics {

	public struct SpecializedList<T> {

		private Dictionary<BindingDesc, T> list;

		private struct BindingDesc {
			public IGenericParameterize Type;
			public TypeBase[] Args;
			public override bool Equals(object obj) {
				BindingDesc desc = (BindingDesc)obj;
				if(this.Type!=desc.Type) { return false; }
				if((this.Args==null) != (desc.Args==null)) { return false; }
				if(this.Args!=null) {
					if(this.Args.Length!=desc.Args.Length) { return false; }
					for(int i=0; i<this.Args.Length; ++i) {
						if(this.Args[i]!=desc.Args[i]) {
							return false;
						}
					}
				}
				return true;
			}
			public override int GetHashCode() {
				return (this.Type==null ? 0x10000000 : this.Type.GetHashCode())
					+  (this.Args==null ? 0 : this.Args.Length);
			}
		}

		private void GetReady() {
			if(list==null) {
				list = new Dictionary<BindingDesc, T>();
			}
		}

		public T this[IGenericParameterize type, TypeBase[] args] {
			get {
				if(list==null) { throw new KeyNotFoundException(); }
				BindingDesc desc;
				desc.Type = type;
				desc.Args = args;
				return this.list[desc];
			}
			set {
				BindingDesc desc;
				desc.Type = type;
				desc.Args = args;
				GetReady();
				this.list[desc] = value;
			}
		}

		public T this[IGenericParameterize type] {
			get {
				return this[type, null];
			}
			set {
				this[type, null] = value;
			}
		}

		public T this[TypeBase[] args] {
			get {
				return this[null, args];
			}
			set {
				this[null, args] = value;
			}
		}

		public bool Contains(IGenericParameterize type, TypeBase[] args) {
			if(list==null) { return false; }
			BindingDesc desc;
			desc.Type = type;
			desc.Args = args;
			GetReady();
			return list.ContainsKey(desc);
		}

		public bool TryGetValue(IGenericParameterize type, TypeBase[] args, out T value) {
			if(list==null) { value = default(T); return false; }
			BindingDesc desc;
			desc.Type = type;
			desc.Args = args;
			GetReady();
			return list.TryGetValue(desc, out value);
		}

		public bool TryGetValue(IGenericParameterize type, out T value) {
			return this.TryGetValue(type, null, out value);
		}

		public bool TryGetValue(TypeBase[] args, out T value) {
			return this.TryGetValue(null, args, out value);
		}

		public void Add(IGenericParameterize type, TypeBase[] args, T value) {
			BindingDesc desc;
			desc.Type = type;
			desc.Args = args;
			GetReady();
			this.list.Add(desc, value);
		}

	}

}
