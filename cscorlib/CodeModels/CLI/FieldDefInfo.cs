using System;
using System.Reflection;
using CooS.Reflection;
using System.Runtime.InteropServices;

namespace CooS.CodeModels.CLI {
	using Metatype;
	using Metadata;
	using Signature;

	class FieldDefInfo : FieldInfoImpl {

		readonly AssemblyDef Assembly;
		readonly FieldRow row;
		readonly FieldSig sig;
		readonly SuperType fieldtype;
		ConcreteType owner;
		int offset;
		string name;

		public FieldDefInfo(AssemblyDef assembly, FieldRow fieldrow) {
			this.Assembly = assembly;
			this.row = fieldrow;
			this.sig = new FieldSig(this.Assembly.OpenSig(this.row.Signature));
			this.fieldtype = (SuperType)this.sig.Type.ResolveTypeAt(this.Assembly);
			this.offset = -1;
		}

		public void AssignOwner(ConcreteType type) {
			this.owner = type;
		}

		public void AssignHeap(ref int offset) {
			this.offset = offset;
			offset += this.fieldtype.VariableSize;
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		public ConcreteType OwnerType {
			get {
				if(this.owner==null) {
					this.owner = this.Assembly.GetFieldOwner(this.row.Index);
				}
				return this.owner;
			}
		}
		
		public override Type DeclaringType {
			get {
				return (Type)this.OwnerType;
			}
		}
		
		public override Type ReflectedType {
			get {
				return (Type)this.OwnerType;
			}
		}

		public override string Name {
			get {
				if(name==null) {
					name = this.Assembly.Metadata.Strings[row.Name];
				}
				return name;
			}
		}

		public override FieldAttributes Attributes {
			get {
				return row.Flags;
			}
		}

		public override Type FieldType {
			get {
				return (Type)this.fieldtype;
			}
		}

		public override int GetFieldOffset() {
			if(this.offset<0) {
				if(this.IsStatic) {
					this.OwnerType.LayoutStaticFields();
				} else {
					this.OwnerType.LayoutInstanceFields();
				}
				if(this.offset<0) {
					throw new UnexpectedException();
				}
			}
			return this.offset;
		}

		public override byte[] GetStaticBuffer() {
			if(0!=(this.Attributes&FieldAttributes.HasFieldRVA)) {
				return this.OwnerType.MyAssembly.Metadata.Header.Image.MemoryImage;
			} else {
				return this.OwnerType.StaticHeap;
			}
		}

		public override unsafe object GetValue(object obj) {
			if(this.IsStatic && obj!=null) throw new ArgumentException();
			if(!this.IsStatic && obj==null) throw new ArgumentNullException();
			if(obj==null) {
				return Memory.BoxValue(this.fieldtype, this.OwnerType.StaticHeap, this.GetFieldOffset());
			} else {
				return Memory.BoxValue(this.fieldtype, Kernel.ObjectToValue(obj), this.GetFieldOffset());
			}
		}

		public override unsafe void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, System.Globalization.CultureInfo culture) {
			int size = this.fieldtype.VariableSize;
			if(this.IsStatic) {
				if(obj!=null) throw new ArgumentException();
				GCHandle pin2 = GCHandle.Alloc(value, GCHandleType.Pinned);
				fixed(byte* p1 = this.GetStaticBuffer()) {
					IntPtr p2 = Kernel.ObjectToValue(value);
					void* dst = (p1+this.GetFieldOffset());
					if(this.fieldtype.IsValueType) {
						Tuning.Memory.Copy(dst, p2.ToPointer(), size);
					} else {
						*(void**)dst = p2.ToPointer();
					}
				}
				pin2.Free();
			} else {
				if(obj==null) throw new ArgumentNullException();
				GCHandle pin1 = GCHandle.Alloc(obj, GCHandleType.Pinned);
				GCHandle pin2 = GCHandle.Alloc(value, GCHandleType.Pinned);
				IntPtr p1 = Kernel.ObjectToValue(obj);
				IntPtr p2 = Kernel.ObjectToValue(value);
				void* dst = ((byte*)p1.ToPointer()+this.GetFieldOffset());
				if(this.fieldtype.IsValueType) {
					Tuning.Memory.Copy(dst, p2.ToPointer(), size);
				} else {
					*(void**)dst = p2.ToPointer();
				}
				pin2.Free();
				pin1.Free();
			}
		}

	}
}
