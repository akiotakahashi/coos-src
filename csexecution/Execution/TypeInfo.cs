using System;
using CooS.Reflection;
using System.Collections.Generic;

namespace CooS.Execution {

	public sealed partial class TypeInfo {

		private readonly Engine engine;
		public readonly TypeBase Base;

		private byte[] heap;
		private int isize = -1;
		private int ssize = -1;
		private IntPtr _handle;

		internal TypeInfo(Engine engine, TypeBase type) {
			this.engine = engine;
			this.Base = type;
		}

		#region RuntimeTypeHandle関係

		static readonly CooS.Collections.Inttable<TypeInfo> handletable = new CooS.Collections.Inttable<TypeInfo>();
		static int handleseed = 1;

		private static IntPtr GenerateNewHandle(TypeInfo type) {
			int handle;
			for(; ; ) {
				handle = handleseed++;
				if(!handletable.ContainsKey(handle)) {
					break;
				}
			}
			handletable[handle] = type;
			return (IntPtr)handle;
		}

		public IntPtr Handle {
			get {
				if(this._handle==IntPtr.Zero) {
					this._handle = GenerateNewHandle(this);
				}
				return this._handle;
			}
		}

		void SetTypeHandle(IntPtr handle) {
			handletable[handle.ToInt32()] = this;
			this._handle = handle;
		}

		public static TypeInfo FindTypeFromHandle(RuntimeTypeHandle handle) {
			return handletable[handle.Value.ToInt32()];
		}

		public static TypeInfo FindTypeFromHandle(IntPtr handle) {
			return handletable[handle.ToInt32()];
		}

		#endregion

		public IntPtr TypeHandle {
			get {
				return this.Handle;
			}
		}

#if DEBUG
		bool recursive = false;
#endif

		public byte[] Heap {
			get {
				if(ssize<0) {
					this.LayoutStaticFields();
				}
				return heap;
			}
		}

		public void LayoutInstanceFields() {
			if(this.isize>=0) { throw new InvalidOperationException(); }
			//if(isize==-2 || ssize==-2) throw new InvalidOperationException("Recursive Operation");
			//isize = -2;
			//ssize = -2;
#if false
			ClassLayoutRow row = null;
			if(this.Base.IsExplicitLayout) {
				if(this.Base.FieldCount>0)
					throw new NotImplementedException();
			} else if(this.IsLayoutSequential) {
				for(int i=1; i<=this.MyAssembly.Metadata.Tables.GetRowCount(TableId.ClassLayout); ++i) {
					row = (ClassLayoutRow)this.MyAssembly.Metadata.Tables[TableId.ClassLayout][i];
					if(row.Parent==this.row.Index) {
						break;
					}
					row = null;
				}
			}
#endif
			int isz = 0;
			if(this.BaseType!=null) {
				isz = this.engine.Realize(this.BaseType).InstanceSize;
			}
			foreach(FieldBase field in this.Base.EnumFields()) {
				if(field.IsStatic) { continue; }
				isz += this.engine.Realize(field).AssignHeap(isz);
			}
#if false
			if(row!=null) {
				if(row.ClassSize>0) {
					if(isz>row.ClassSize)
						throw new InvalidProgramException("ClassSize is "+row.ClassSize+" but actually needs "+isz);
					isz = row.ClassSize;
				} else if(row.PackingSize>0) {
					isz = Architecture.AlignOffset(isz, row.PackingSize);
				}
			}
#endif
			this.isize = isz;
		}

		public void LayoutStaticFields() {
			if(this.ssize>=0) { throw new InvalidOperationException(); }
			int ssz = 0;
			foreach(FieldBase field in this.Base.EnumFields()) {
				if(!field.IsStatic) { continue; }
//				if(!field.HasRVA) {
					ssz += this.engine.Realize(field).AssignHeap(ssz);
//				} else {
//					throw new NotImplementedException();
#if false
					FieldRVARow row = null;
					for(int i=1; i<=this.MyAssembly.Metadata.Tables.GetRowCount(TableId.FieldRVA); ++i) {
						row = (FieldRVARow)this.MyAssembly.Metadata.Tables[TableId.FieldRVA][i];
						if(row.Field==field.RowIndex) {
							break;
						}
						row = null;
					}
					if(row==null)
						throw new InvalidProgramException();
					int tsz = (int)row.RVA.Value;
					field.AssignHeap(ref tsz);
#endif
//				}
			}
			if(ssz>0) {
				if(this.heap==null) {
					this.heap = new byte[ssz];
				} else {
					if(this.heap.Length<ssz) {
						throw new SystemException(string.Format("Buffer size is too small: expected={0}, actual={1}", ssz, this.heap.Length));
					}
				}
			}
			this.ssize = ssz;
		}

		/// <summary>
		/// ヒープで占めるメモリサイズを取得します。
		/// </summary>
		public int InstanceSize {
			get {
				if(this.IsPrimitive) {
					return this.Base.IntrinsicSize;
				} else if(this.IsPointer) {
					return Architecture.Target.AddressSize;
				} else {
#if DEBUG
					if(recursive) throw new Exception(this.FullName);
					recursive = true;
#endif
					int size = 0;
					foreach(FieldBase field in this.Base.EnumFields()) {
						if(field.IsStatic) continue;
						TypeInfo ft = this.engine.Realize(field.ReturnType);
						size = Architecture.AlignOffset(size, ft.VariableSize);
						size += ft.VariableSize;
					}
#if DEBUG
					recursive = false;
#endif
					return size;
				}
			}
		}

		/// <summary>
		/// スタックで占めるメモリサイズを取得します。
		/// </summary>
		public int VariableSize {
			get {
				if(IsValueType) {
					return this.InstanceSize;
				} else {
					return Architecture.Target.AddressSize;
				}
			}
		}

		public int OffsetToContents {
			get {
				return 0;
			}
		}

		// Method Stubs

		internal bool IsAssignableFrom(TypeInfo typeInfo) {
			return this.Base.IsAssignableFrom(typeInfo.Base);
		}

		#region スロット

		private MethodBase[] slots;
		private InterfaceBase[] ifbases;

		public MethodBase[] ConstructSlots(out InterfaceBase[] ifbases) {
			TypeInfo basetype;
			// スロットの数を決定
			int totalslots;
			if(this.BaseType==null || this.IsInterface) {
				basetype = null;
				totalslots = 0;
			} else {
				basetype = this.engine.Realize(this.BaseType);
				totalslots = basetype.TotalSlotCount;
			}
			foreach(MethodBase method in this.Base.EnumMethods()) {
				if(!method.IsVirtual) { continue; }
				if(!method.HasNewSlot) { continue; }
				this.engine.Realize(method).SlotIndex = totalslots++;
			}
			// インターフェイスマップの数を決定
			int ifbaseidx = totalslots;
			List<TypeBase> iftypes = new List<TypeBase>();
			if(!this.IsInterface) {
				List<InterfaceBase> list = new List<InterfaceBase>();
				foreach(TypeBase iftype in this.Base.EnumInterfaces()) {
					iftypes.Add(iftype);
					InterfaceBase ifbase;
					ifbase.Handle = iftype.Id;
					ifbase.BaseIndex = totalslots;
					list.Add(ifbase);
					totalslots += iftype.MethodCount;
				}
				ifbases = list.ToArray();
			} else {
				ifbases = null;
			}
			// スロットを初期化
			MethodBase[] slots = new MethodBase[totalslots];
			if(basetype!=null) {
				if(!this.IsInterface && basetype!=null) {
					for(int i=0; i<basetype.TotalSlotCount; ++i) {
						slots[i] = basetype.GetSlotMethod(i);
					}
				}
			}
			// スロットを決定
			foreach(MethodBase method in this.Base.EnumMethods()) {
				if(!method.IsVirtual) { continue; }
				MethodInfo m = this.engine.Realize(method);
				slots[m.SlotIndex] = method;
			}
			// インターフェイスマップを決定
			if(!this.IsInterface) {
				foreach(TypeBase type in iftypes) {
					int inc = 0;
					foreach(MethodBase method in type.EnumMethods()) {
						MethodInfo mi = this.engine.Realize(method);
						TypeBase[] prms = new TypeBase[method.ParameterCount];
						for(int i=0; i<prms.Length; ++i) {
							prms[i] = method.GetParameterType(i);
						}
						MemberRefDesc desc = new MemberRefDesc(method);
						desc.name = type.FullName+"."+method.Name;
						MethodBase impl = this.Base.FindMethod(desc);
						if(impl==null) {
							desc.name = method.Name;
							impl = this.Base.FindMethod(desc);
							if(impl==null) {
								if(!this.Base.IsAbstract && !this.IsInterface) {
									throw new MissingMethodException(this.FullName, method.Name);
								}
							}
						}
						slots[ifbaseidx+mi.SlotIndex] = impl;
						++inc;
					}
					ifbaseidx += inc;
				}
			}
			//
			return slots;
		}
	
		public void PrepareSlots() {
			if(this.slots==null) {
				this.slots = this.ConstructSlots(out this.ifbases);
			}
		}

		public bool SlotAvailable {
			get {
				return this.slots!=null;
			}
		}

		public int TotalSlotCount {
			get {
				this.PrepareSlots();
				return this.slots.Length;
			}
		}

		public MethodBase GetSlotMethod(int slotIndex) {
			if(this.slots==null)
				this.PrepareSlots(); // たぶん要らないはず…
			return this.slots[slotIndex];
		}

		#endregion

	}

	public struct InterfaceBase {
		public int Handle;
		public int BaseIndex;
	}

}
