using System;
using System.Reflection;
using System.Collections;
using CooS.Formats.CLI;

namespace CooS.Reflection.CLI.Metatype {

	abstract class ConcreteType : SuperType {

		readonly TypeDefInfo row;
		readonly int fieldCount;
		readonly int methodCount;
		Type baseType;
		string name = null;
		string _namespace = null;
		int isize = -1;
		int ssize = -1;
		byte[] staticheap;
		SuperType[] interfaces = null;

		public ConcreteType(AssemblyDef assembly, TypeDefInfo row) : base(assembly) {
			this.row = row;
			this.fieldCount = row.FieldCount;
			this.methodCount = row.MethodCount;
#if metatype
			assembly.AssignTypeDef(row.Index, this);
			assembly.AssignFieldOwner(this.row.FieldList, this.fieldCount, this);
			assembly.AssignMethodOwner(this.row.MethodList, this.methodCount, this);
			// ランタイムと同期を取る
			if(Engine.Infrastructured) {
				RuntimeTypeHandle handle = Engine.NotifyLoadingType(assembly, row.Index, this);
				this.SetTypeHandle(handle);
			}
			//
#endif
			//this.PrepareSlots();
		}

#if metatype
		public ConcreteType(AssemblyDef assembly, int rowIndex) : this(assembly, (TypeDefRow)assembly.Metadata.Tables[TableId.TypeDef][rowIndex]) {
		}
#endif

		protected unsafe void AssigneStaticHeap(byte[] buf) {
			if(this.staticheap==null) {
				this.staticheap = buf;
				//*
				if(buf!=null) {
					fixed(byte* p = &buf[0]) {
						Console.WriteLine("Assign {0} (0x{1:X8}:{2})", this.FullName, (IntPtr)p, buf.Length);
						//Assist.Dump(buf);
					}
				}
				//*/
			} else if(buf==null) {
				if(this.staticheap.Length>0) {
					throw new InvalidOperationException("buf is null but staticheap.Length is "+this.staticheap.Length);
				}
			} else {
				bool different = buf.Length!=this.staticheap.Length;
				if(!different) {
					for(int i=0; i<buf.Length; ++i) {
						if(buf[i]!=this.staticheap[i]) {
							different = true;
							break;
						}
					}
				}
				if(different) {
					Console.WriteLine("Conflict {0}:", this.FullName);
					Utility.Dump(buf);
					Utility.Dump(this.staticheap);
					//Buffer.BlockCopy(this.staticheap,0,buf,0,Math.Min(this.staticheap.Length,buf.Length));
					throw new InvalidOperationException("Each static heap are conflict.");
				}
			}
		}

		public override void CompleteSetup() {
			base.CompleteSetup();
			/*
			if(this.handle.Value==IntPtr.Zero) {
				Console.WriteLine("EmptyHandle: {0}", this.FullName);
			}
			*/
		}

#if metadata
		public TypeDefInfo Row {
			get {
				return this.row;
			}
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}
#endif

		#region Typeメンバ

		public override string Name {
			get {
				if(name==null) {
					name = this.row.Name;
				}
				return name;
			}
		}

		public override string Namespace {
			get {
				if(_namespace==null) {
					_namespace = this.row.Namespace;
				}
				return _namespace;
			}
		}

		public override Type BaseType {
			get {
				if(this.baseType==null) {
					this.baseType = this.row.BaseType.Realize(this.MyAssembly.Domain);
				}
				return this.baseType;
			}
		}

#if metadata
		public override bool IsNested {
			get {
				return this.row.IsNested;
			}
		}
#endif

		protected override bool IsArrayImpl() {
			return false;
		}

		public override bool IsByRefPointer {
			get {
				return false;
			}
		}

		public override bool IsByValPointer {
			get {
				return false;
			}
		}

		public override Type GetElementType() {
			return null;
		}

		private TypeDefInfo declaringType;

		public override Type DeclaringType {
			get {
				if(!this.declaringType==null) {
					this.declaringType = this.row.EnclosingType();
				}
				return this.declaringType;
			}
		}

		public override TypeAttributes AttributeFlags {
			get {
				return this.row.Flags;
			}
		}

		public override int OffsetToContents {
			get {
				return 0;
			}
		}

		#endregion

		public bool OwnsField(int rowIndex) {
			return this.row.FieldList<=rowIndex && rowIndex<this.row.FieldList+this.fieldCount;
		}

		public bool OwnsMethod(int rowIndex) {
			return this.row.MethodList<=rowIndex && rowIndex<this.row.MethodList+this.methodCount;
		}

		#region クラスサイズの関するメンバ

		public void LayoutInstanceFields() {
			if(this.isize>=0) return;
			//if(isize==-2 || ssize==-2) throw new InvalidOperationException("Recursive Operation");
			//isize = -2;
			//ssize = -2;
			int isz = 0;
			ClassLayoutRow row = null;
			if(this.IsExplicitLayout) {
				if(this.fieldCount>0) throw new NotImplementedException();
			} else if(this.IsLayoutSequential) {
				for(int i=1; i<=this.MyAssembly.Metadata.Tables.GetRowCount(TableId.ClassLayout); ++i) {
					row = (ClassLayoutRow)this.MyAssembly.Metadata.Tables[TableId.ClassLayout][i];
					if(row.Parent==this.row.Index) {
						break;
					}
					row = null;
				}
			}
			FieldTable table = (FieldTable)((AssemblyDef)MyAssembly).Metadata.Tables[TableId.Field];
			if(this.BaseType!=null) isz=((TypeImpl)this.BaseType).InstanceSize;
			foreach(FieldDefInfo field in this.DeclaredFields) {
				if(field.IsStatic) continue;
				field.AssignHeap(ref isz);
			}
			if(row!=null) {
				if(row.ClassSize>0) {
					if(isz>row.ClassSize) throw new InvalidProgramException("ClassSize is "+row.ClassSize+" but actually needs "+isz);
					isz = row.ClassSize;
				} else if(row.PackingSize>0) {
					isz = Architecture.AlignOffset(isz, row.PackingSize);
				}
			}
			this.isize = isz;
		}

		public void LayoutStaticFields() {
			if(this.ssize>=0) return;
			int ssz = 0;
			foreach(FieldDefInfo field in this.DeclaredFields) {
				if(!field.IsStatic) continue;
				if(0==(field.Attributes&FieldAttributes.HasFieldRVA)) {
					field.AssignHeap(ref ssz);
				} else {
					FieldRVARow row = null;
					for(int i=1; i<=this.MyAssembly.Metadata.Tables.GetRowCount(TableId.FieldRVA); ++i) {
						row = (FieldRVARow)this.MyAssembly.Metadata.Tables[TableId.FieldRVA][i];
						if(row.Field==field.RowIndex) {
							break;
						}
						row = null;
					}
					if(row==null) throw new InvalidProgramException();
					int tsz = (int)row.RVA.Value;
					field.AssignHeap(ref tsz);
				}
			}
			if(this.staticheap==null) {
				this.staticheap = new byte[ssz];
			} else {
				if(this.staticheap.Length<ssz) {
					throw new SystemException(string.Format("Buffer size is too small: expected={0}, actual={1}", ssz, this.staticheap.Length));
				}
			}
			this.ssize = ssz;
		}

		public override int InstanceSize {
			get {
				if(isize<0) {
					this.LayoutInstanceFields();
				}
				return this.isize;
			}
		}

		public override int StaticSize {
			get {
				if(this.ssize<0) {
					this.LayoutStaticFields();
				}
				return this.ssize;
			}
		}

		#endregion

		#region 静的フィールドに関わるメンバ

		public unsafe byte[] StaticHeap {
			get {
				if(this.staticheap==null) {
					this.LayoutStaticFields();
				}
				return this.staticheap;
			}
		}

		#endregion

		#region メンバ情報にアクセスするためのメンバ

		int constructorcount = -1;

		private void DetermineConstructorCount() {
			if(this.constructorcount>=0) return;
			int c = 0;
			for(int i=this.methodCount-1; i>=0; --i) {
				MethodDefInfo method = ((AssemblyDef)MyAssembly).GetMethodDefInfo(this.row.MethodList+i);
				method.AssignOwner(this);
				if(method.IsConstructor) {
					++c;
				}
			}
			this.constructorcount = c;
		}

		private MethodDef[] GetAllMethods() {
			MethodDefInfo[] methods = new MethodDefInfo[this.methodCount];
			for(int i=this.methodCount-1; i>=0; --i) {
				methods[i] = this.MyAssembly.GetMethodDefInfo(this.row.MethodList+i);
			}
			return methods;
		}

		#endregion

		#region メンバコレクション

		IEnumerable constcol = null;
		IEnumerable fieldcol = null;
		IEnumerable methodcol = null;
		IEnumerable propertycol = null;
		IEnumerable eventcol = null;
		IEnumerable nestedtypecol = null;

		public override IEnumerable DeclaredConstructors {
			get {
				if(this.constcol==null) {
					this.constcol = this.MyAssembly.CreateConstructorCollection(this, this.row.MethodList, this.methodCount);
				}
				return this.constcol;
			}
		}

		public override IEnumerable DeclaredFields {
			get {
				if(this.fieldcol==null) {
					this.fieldcol = this.MyAssembly.CreateFieldCollection(this, this.row.FieldList, this.fieldCount);
				}
				return this.fieldcol;
			}
		}

		public override IEnumerable DeclaredMethods {
			get {
				if(this.methodcol==null) {
					this.methodcol = this.MyAssembly.CreateMethodCollection(this, this.row.MethodList, this.methodCount);
				}
				return this.methodcol;
			}
		}

		public override IEnumerable DeclaredPeoperties {
			get {
				if(this.propertycol==null) {
					//TODO: fix me
					this.propertycol = new PropertyInfo[0];
				}
				return this.propertycol;
			}
		}

		public override IEnumerable DeclaredEvents {
			get {
				if(this.eventcol==null) {
					//TODO: fix me
					this.eventcol = new EventInfo[0];
				}
				return this.eventcol;
			}
		}

		public override IEnumerable DeclaredNestedTypes {
			get {
				if(this.nestedtypecol==null) {
					//TODO: fix me
					this.nestedtypecol = new TypeImpl[0];
				}
				return this.nestedtypecol;
			}
		}


		#endregion

		#region インターフェイス

		public override IEnumerable ImplementedInterfaces {
			get {
				if(this.interfaces==null) {
					ArrayList list = null;
					MetadataRoot metadata = this.MyAssembly.Metadata;
					Table table = metadata.Tables[TableId.InterfaceImpl];
					if(table==null) {
						this.interfaces = new SuperType[0];
					} else {
						for(int i=1; i<=table.RowCount; ++i) {
							InterfaceImplRow row = (InterfaceImplRow)table[i];
							if(row.Class==this.row.Index) {
								if(list==null) list = new ArrayList();
								list.Add(this.MyAssembly.ResolveType(row.Interface));
							}
						}
						if(list==null) {
							this.interfaces = new SuperType[0];
						} else {
							this.interfaces = (SuperType[])list.ToArray(typeof(SuperType));
						}
					}
				}
				return this.interfaces;
			}
		}

		#endregion

		#region 仮想メソッド

		public override MethodInfoImpl[] ConstructSlots(out InterfaceBase[] ifbases) {
			TypeImpl basetype = (TypeImpl)this.BaseType;
			// スロットの数を決定
			int totalslots = 0;
			if(!this.IsInterface && basetype!=null) {
				totalslots = basetype.TotalSlotCount;
			}
			foreach(MethodDefInfo method in this.DeclaredMethods) {
				if(!method.IsVirtual) continue;
				if(!method.HasNewSlot) continue;
				method.AssignSlot(totalslots++);
			}
			// インターフェイスマップの数を決定
			int ifbaseidx = totalslots;
			Type[] iftypes = null;
			if(!this.IsInterface) {
				iftypes = this.GetInterfaces();
				ifbases = new InterfaceBase[iftypes.Length];
				for(int i=0; i<iftypes.Length; ++i) {
					TypeImpl type = (TypeImpl)iftypes[i];
					ifbases[i].Handle = type.TypeHandle;
					ifbases[i].BaseIndex = totalslots;
					totalslots += type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly).Length;
				}
			} else {
				ifbases = null;
			}
			// スロットを初期化
			MethodInfoImpl[] slots = new MethodInfoImpl[totalslots];
			if(!this.IsInterface && basetype!=null) {
				for(int i=0; i<basetype.TotalSlotCount; ++i) {
					slots[i] = basetype.GetSlotMethod(i);
				}
			}
			// スロットを決定
			foreach(MethodDefInfo method in this.DeclaredMethods) {
				if(!method.IsVirtual) continue;
				slots[method.SlotIndex] = method;
			}
			// インターフェイスマップを決定
			if(!this.IsInterface) {
				foreach(TypeImpl type in iftypes) {
					int inc = 0;
					foreach(MethodInfoImpl method in type.DeclaredMethods) {
						Type[] prms = new Type[method.ParameterCount];
						for(int i=0; i<prms.Length; ++i) {
							prms[i] = method.GetParameterType(i);
						}
						MethodInfo impl = this.GetMethod(type.FullName+"."+method.Name
							, BindingFlags.NonPublic|BindingFlags.Instance, null, prms, null);
						if(impl==null) {
							impl = this.GetMethod(method.Name, BindingFlags.Public|BindingFlags.Instance, null, prms, null);
							if(impl==null) {
								if(!this.IsAbstract && !this.IsInterface) {
									throw new MissingMethodException(this.FullName, method.Name);
								}
							}
						}
						slots[ifbaseidx+method.SlotIndex] = (MethodInfoImpl)impl;
						++inc;
					}
					ifbaseidx += inc;
				}
			}
			//
			return slots;
		}

		#endregion

	}
}