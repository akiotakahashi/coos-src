using System;
using System.Reflection;
using System.Collections;
using CooS.Reflection;
using CooS.CodeModels.CLI.Metadata;

namespace CooS.CodeModels.CLI.Metatype {

	abstract class ConcreteType : SuperType {

		readonly TypeDefRow row;
		readonly int fieldCount;
		readonly int methodCount;
		Type baseType;
		string name = null;
		string _namespace = null;
		int isize = -1;
		int ssize = -1;
		byte[] staticheap;
		SuperType[] interfaces = null;

		public ConcreteType(AssemblyDef assembly, TypeDefRow row) : base(assembly) {
			TypeDefTable table = (TypeDefTable)assembly.Metadata.Tables[TableId.TypeDef];
			this.row = row;
			this.fieldCount = 0;
			this.methodCount = 0;
			if(row.Index<table.RowCount) {
				TypeDefRow next = (TypeDefRow)table[row.Index+1];
				this.fieldCount = next.FieldList-this.row.FieldList;
				this.methodCount = next.MethodList-this.row.MethodList;
			} else {
				this.fieldCount = assembly.Metadata.Tables.GetRowCount(TableId.Field)+1-this.row.FieldList;
				this.methodCount = assembly.Metadata.Tables.GetRowCount(TableId.Method)+1-this.row.MethodList;
			}
			assembly.AssignTypeDef(row.Index, this);
			assembly.AssignFieldOwner(this.row.FieldList, this.fieldCount, this);
			assembly.AssignMethodOwner(this.row.MethodList, this.methodCount, this);
			// �����^�C���Ɠ��������
			if(Engine.Infrastructured) {
				RuntimeTypeHandle handle = Engine.NotifyLoadingType(assembly, row.Index, this);
				this.SetTypeHandle(handle);
			}
			//
			//this.PrepareSlots();
		}

		public ConcreteType(AssemblyDef assembly, int rowIndex) : this(assembly, (TypeDefRow)assembly.Metadata.Tables[TableId.TypeDef][rowIndex]) {
		}

		protected void AssigneStaticHeap(byte[] buf) {
			if(this.staticheap==null) {
				this.staticheap = buf;
				//*
				if(buf!=null) {
					Console.WriteLine("Assign {0} (0x{1:X8}:{2})", this.FullName, Kernel.ObjectToValue(buf).ToInt32(), buf.Length);
					//Assist.Dump(buf);
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
					Assist.Dump(buf);
					Assist.Dump(this.staticheap);
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

		public TypeDefRow Row {
			get {
				return this.row;
			}
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		#region Type�����o

		public override string Name {
			get {
				if(name==null) {
					name = ((AssemblyDef)MyAssembly).Metadata.Strings[this.row.Name];
				}
				return name;
			}
		}

		public override string Namespace {
			get {
				if(_namespace==null) {
					_namespace = ((AssemblyDef)MyAssembly).Metadata.Strings[this.row.Namespace];
				}
				return _namespace;
			}
		}

		public override Type BaseType {
			get {
				if(this.baseType==null) {
					this.baseType = ((AssemblyDef)MyAssembly).ResolveType(this.row.Extends);
				}
				return this.baseType;
			}
		}

		public override bool IsNested {
			get {
				return TypeUtility.IsNestedFlag(this.row.Flags);
			}
		}

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

		private int declaringTypeIndex = -1;

		public override Type DeclaringType {
			get {
				if(!this.IsNested) throw new InvalidOperationException();
				if(this.declaringTypeIndex<0) {
					this.declaringTypeIndex = this.MyAssembly.GetEnclosingClass(this.row.Index);
					if(this.declaringTypeIndex<0) throw new UnexpectedException();
				}
				return this.MyAssembly.GetTypeDef(this.declaringTypeIndex);
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

		#region �N���X�T�C�Y�̊ւ��郁���o

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

		#region �ÓI�t�B�[���h�Ɋւ�郁���o

		public unsafe byte[] StaticHeap {
			get {
				if(this.staticheap==null) {
					this.LayoutStaticFields();
				}
				return this.staticheap;
			}
		}

		#endregion

		#region �����o���ɃA�N�Z�X���邽�߂̃����o

		int constructorcount = -1;

		private void DetermineConstructorCount() {
			if(this.constructorcount>=0) return;
			int c = 0;
			for(int i=this.methodCount-1; i>=0; --i) {
				MethodDefInfo method = ((AssemblyDef)MyAssembly).GetMethodDef(this.row.MethodList+i);
				method.AssignOwner(this);
				if(method.IsConstructor) {
					++c;
				}
			}
			this.constructorcount = c;
		}

		private MethodDefInfo[] GetAllMethods() {
			MethodDefInfo[] methods = new MethodDefInfo[this.methodCount];
			for(int i=this.methodCount-1; i>=0; --i) {
				methods[i] = this.MyAssembly.GetMethodDef(this.row.MethodList+i);
			}
			return methods;
		}

		#endregion

		#region �����o�R���N�V����

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

		#region �C���^�[�t�F�C�X

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

		#region ���z���\�b�h

		public override MethodInfoImpl[] ConstructSlots(out InterfaceBase[] ifbases) {
			TypeImpl basetype = (TypeImpl)this.BaseType;
			// �X���b�g�̐�������
			int totalslots = 0;
			if(!this.IsInterface && basetype!=null) {
				totalslots = basetype.TotalSlotCount;
			}
			foreach(MethodDefInfo method in this.DeclaredMethods) {
				if(!method.IsVirtual) continue;
				if(!method.HasNewSlot) continue;
				method.AssignSlot(totalslots++);
			}
			// �C���^�[�t�F�C�X�}�b�v�̐�������
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
			// �X���b�g��������
			MethodInfoImpl[] slots = new MethodInfoImpl[totalslots];
			if(!this.IsInterface && basetype!=null) {
				for(int i=0; i<basetype.TotalSlotCount; ++i) {
					slots[i] = basetype.GetSlotMethod(i);
				}
			}
			// �X���b�g������
			foreach(MethodDefInfo method in this.DeclaredMethods) {
				if(!method.IsVirtual) continue;
				slots[method.SlotIndex] = method;
			}
			// �C���^�[�t�F�C�X�}�b�v������
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