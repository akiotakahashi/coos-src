using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Runtime.CompilerServices;
using CooS.Reflection;
using CooS.CodeModels.DLL;

namespace CooS.CodeModels.CLI {
	using Metatype;
	using Metadata;
	using Signature;
	using Manipulation;

	sealed class MethodDefInfo : MethodSigInfo {

		readonly MethodRow row;
		ConcreteType owner = null;
		string name = null;
		CodeFlags codeFlags;
		int codeOffset;
		int codeSize;
		int maxStack;
		int varCount;
		int slotIndex = -1;
		LocalVarSig localVarSig;
		ArrayList extraSections;

		[Flags]
		enum MethodSectionFlags : byte {
			EHTable		= 0x01,	// Exception handling data.
			OptILTable	= 0x02,	// Reserved, shall be 0.
			FatFormat	= 0x40,	// Data format is of the fat variety, meaning there is a 3 byte length.
			// If not set, the header is small with a  1 byte length
			MoreSects	= 0x80,	// Another data section occurs after this current section
		}

		[Flags]
		enum ExceptionClauseFlags {
			EXCEPTION_CLAUSE_EXCEPTION	= 0x0000,	// A typed exception clause
			EXCEPTION_CLAUSE_FILTER		= 0x0001,	// An exception filter and handler clause
			EXCEPTION_CLAUSE_FINALLY	= 0x0002,	// A finally clause
			EXCEPTION_CLAUSE_FAULT		= 0x0004,	// Fault clause (finally that is called on exception only)
		}

		class MethodSection {
			public MethodSectionFlags Flags;
			public long Position;
			public MethodSection(MethodSectionFlags flags, long pos) {
				this.Flags = flags;
				this.Position = pos;
			}
		}

		public MethodDefInfo(AssemblyDef assembly, MethodRow row) : base(assembly, row.Signature) {
			this.row = row;
			MetadataRoot mdroot = this.assembly.Metadata;
			MethodTable table = (MethodTable)mdroot.Tables[TableId.Method];
			this.codeOffset = 0;
			this.codeFlags = (CodeFlags)0;
			this.codeSize = 0;
			this.maxStack = 0;
			this.varCount = 0;
			if(row.RVA!=0 && MethodImplAttributes.IL==(row.ImplFlags&MethodImplAttributes.CodeTypeMask)) {
				using(Stream stream = mdroot.OpenStream(row.RVA)) {
					BinaryReader reader = new BinaryReader(stream);
					byte fst = reader.ReadByte();
					switch((CodeFlags)fst&CodeFlags.TypeMask) {
					case CodeFlags.Tiny:
						// Tiny format
						codeOffset = 1;
						this.codeFlags = CodeFlags.Tiny;
						this.codeSize = fst>>2;
						this.maxStack = 8;
						break;
					case CodeFlags.Fat:
						// Fat format
						codeOffset = 12;
						int flagsandsize  = fst | ((int)reader.ReadByte()<<8);
						this.codeFlags = (CodeFlags)(flagsandsize & 0x0FFF);
						this.maxStack = reader.ReadUInt16();
						this.codeSize = reader.ReadInt32();
						MDToken localVarSigTok = reader.ReadUInt32();
						if(localVarSigTok>0) {
							if(localVarSigTok.TableId!=TableId.StandAloneSig) throw new BadMetadataException();
							StandAloneSigRow sas = (StandAloneSigRow)mdroot.Tables[localVarSigTok];
							SignatureReader sasr = mdroot.Blob.OpenReader(sas.Signature);
							this.localVarSig = new Signature.LocalVarSig(sasr);
							this.varCount = this.localVarSig.LocalVars.Length;
						}
						break;
					default:
						throw new BadMetadataException("Failed to detect method header type.");
					}
				}
			}
			if(0!=(codeFlags&CodeFlags.MoreSects)) {
				this.extraSections = new ArrayList();
				using(BinaryReader reader = new BinaryReader(mdroot.OpenStream(row.RVA))) {
					reader.BaseStream.Seek(this.codeOffset+this.codeSize, SeekOrigin.Current);
					long position = reader.BaseStream.Position;
					if(position%4 >0) {
						int diff = 4-(int)(position%4);
						reader.BaseStream.Seek(diff, SeekOrigin.Current);
					}
#if false
					MethodSectionFlags sectFlags;
					do {
						sectFlags = (MethodSectionFlags)reader.ReadByte();
						if(0!=(~0xC3&(int)sectFlags)) throw new BadILException(this.FullName);
						extraSections.Add(new MethodSection(sectFlags,position));
						int length;
						if(0!=(sectFlags&MethodSectionFlags.FatFormat)) {
							int fst = reader.ReadByte();
							int snd = reader.ReadByte();
							int trd = reader.ReadByte();
							length = (fst<<16) | (snd<<8) | trd;
							length -= 4;
						} else {
							length = reader.ReadByte();
							length -= 2;
						}
						reader.BaseStream.Seek(length, SeekOrigin.Current);
					} while(0!=(sectFlags&MethodSectionFlags.MoreSects));
#endif
				}
			}
		}

		public override ParameterInfo[] GetParameters() {
			ParameterInfo[] list = new ParameterInfo[this.ParameterCount];
			int i = 0;
			foreach(ParamSig param in this.methodSig.GetParameters(this.assembly)) {
				ParamRow row;
				if(i<this.ParameterCount) {
					row = (ParamRow)this.row.Table.Heap[TableId.Param][this.row.ParamList+i];
				} else {
					row = null;
				}
				list[i] = new ParameterDefInfo(this,param,row);
				++i;
			}
			return list;
		}

		public override MethodInfoImpl[] GetCallings() {
			if(!this.HasRVA) return new MethodInfoImpl[0];
			ArrayList callings = new ArrayList();
			ILStream ils = new ILStream(this.MyAssembly, this.OpenCodeBlock());
			while(!ils.AtEndOfStream) {
				Instruction inst = ils.Read();
				if(inst.OpCode.Value==System.Reflection.Emit.OpCodes.Call.Value
				|| inst.OpCode.Value==System.Reflection.Emit.OpCodes.Callvirt.Value)
				{
					callings.Add(this.assembly.ResolveMethod((MDToken)inst.Operand));
				} else if(inst.OpCode.Value==System.Reflection.Emit.OpCodes.Ldtoken.Value) {
					MDToken token = (MDToken)inst.Operand;
					switch(token.TableId) {
					case TableId.Method:
					case TableId.MemberRef:
						MethodInfo mi = this.assembly.ResolveMethod(token);
						callings.Add(mi);
						break;
					}
				}
			}
			ils.Close();
			return (MethodInfoImpl[])callings.ToArray(typeof(MethodInfoImpl));
		}

		public override string ToString() {
			return "Method: "+this.FullName;
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		#region オーナークラス

		public void AssignOwner(ConcreteType type) {
			this.owner = type;
		}

		public ConcreteType OwnerType {
			get {
				if(this.owner==null) {
					this.owner = this.assembly.GetMethodOwner(this.row.Index);
				}
				return this.owner;
			}
		}

		public override int DeclaringTypeIndex {
			get {
				return this.assembly.GetMethodOwnerIndex(this.row.Index);
			}
		}

		#endregion

		#region 単純なプロパティ

		public override string Name {
			get {
				if(name==null) {
					name = this.assembly.Metadata.Strings[row.Name];
				}
				return name;
			}
		}

		public override Type DeclaringType {
			get {
				return this.OwnerType;
			}
		}
		
		public override Type ReflectedType {
			get {
				return this.OwnerType;
			}
		}
		
		public bool HasRVA {
			get {
				return this.RVA>0;
			}
		}

		public RVA RVA {
			get {
				return this.row.RVA;
			}
		}

		public override bool IsBlank {
			get {
				return !this.HasRVA;
			}
		}

		public override void MakeBlank() {
			this.row.RVA = 0;
		}

		public int MaxStack {
			get {
				return this.maxStack;
			}
		}

		public bool IsNative {
			get {
				return 0!=(this.row.ImplFlags&MethodImplAttributes.Native);
			}
		}

		public override MethodAttributes Attributes {
			get {
				return row.Flags;
			}
		}

		public override MethodImplAttributes GetMethodImplementationFlags() {
			return row.ImplFlags;
		}

		#endregion

		#region ローカル変数

		public override int VariableCount {
			get {
				return this.varCount;
			}
		}

		public LocalVar GetVariable(int index) {
			return this.localVarSig.LocalVars[index];
		}

		public override Type GetVariableType(int index) {
			return this.GetVariable(index).Type.ResolveTypeAt(this.assembly);
		}

		#endregion

		#region コード

		public Stream OpenCodeBlock() {
			if(!this.HasRVA) throw new MissingMethodException("No RVA: "+this.FullName);
			MetadataRoot mdroot = this.assembly.Metadata;
			Stream stream = mdroot.OpenStream(this.RVA);
			stream = new CooS.IO.BoundStream(stream,stream.Position+this.codeOffset,this.codeSize);
			return stream;
		}

		public override CodeInfo GenerateCallableCode(CodeLevel level) {
			switch(level) {
			case CodeLevel.DontCare:
			case CodeLevel.IL:
				return this.GenerateExecutableCode(level);
			case CodeLevel.Stub:
				Assembler assembler = CooS.Architecture.CreateAssembler(this,0);
				assembler.WriteMethodStub();
				return assembler.GenerateCode();
			case CodeLevel.Native:
				return this.GenerateExecutableCode(level);
			default:
				throw new ArgumentException();
			}
		}

		public override CodeInfo GenerateExecutableCode(CodeLevel level) {
			switch(level) {
			case CodeLevel.DontCare:
			case CodeLevel.IL:
				return new CodeInfo(Engine.GetMethodProxyCode(this));
			case CodeLevel.Native:
				if(this.IsBlank) {
					if(0!=(this.GetMethodImplementationFlags()&MethodImplAttributes.Runtime)) {
						switch(this.Name) {
						case "Invoke": {
							Assembler asm = Architecture.CreateAssembler(this, 0);
							asm.Invoke(Assist.DelegateTargetField, Assist.DelegatePointerField);
							CodeInfo code = asm.GenerateCode();
							return code;
						}
						default:
							throw new NotSupportedException("Unknown runtime method: "+this.Name);
						}
					}
					MethodInfoImpl method = this.FindWrappingMethod();
					if(method==null) throw new MethodNotFoundException("Requested blank method: "+this.FullName);
					Console.WriteLine(method.FullName);
					return method.GenerateExecutableCode(level);
				}
				if(this.IsNative) {
					return new CodeInfo(this.MyAssembly.Metadata.Header.Image.MemoryImage, (int)this.RVA.Value);
				} else {
					//Console.WriteLine("<required to compile lv{1}: {0}>", this.FullName, (int)level);
					Stream stream = this.OpenCodeBlock();
					Instruction[] insts = Synthesizer.Perform(this, stream);
					CodeInfo code = Compiler.Compile(this, insts, CooS.Execution.CodeManager.Trap);
					return code;
				}
			default:
				throw new ArgumentException();
			}
		}

		#endregion

		#region 継承関係

		internal void AssignSlot(int slotindex) {
			if(this.slotIndex>=0 && this.slotIndex!=slotindex) {
				throw new InvalidOperationException("SlotIndex was already assigned to "+this.slotIndex+" (trying "+slotindex+")");
			}
			this.slotIndex = slotindex;
		}

		public override int SlotIndex {
			get {
				if(this.slotIndex<0) {
					if(this.HasNewSlot) {
						// NewSlot
						((TypeImpl)this.DeclaringType).PrepareSlots();
						if(this.slotIndex<0) throw new UnexpectedException();
					} else if(this.HasReuseSlot) {
						// ReuseSlot
						return this.slotIndex = ((MethodInfoImpl)this.GetBaseDefinition()).SlotIndex;
					} else {
						if(this.IsVirtual) {
							throw new InvalidOperationException("Don't have a slot");
						} else {
							throw new InvalidOperationException("Not virtual method: "+this.FullName);
						}
					}
				}
				return this.slotIndex;
			}
		}

		public override MethodInfo GetBaseDefinition() {
			if(!this.IsVirtual) {
				return this;
			} else {
				if(this.HasNewSlot) {
					return this;
				} else if(this.HasReuseSlot) {
					Type parent = this.DeclaringType.BaseType;
					Type[] parameters = new Type[this.ParameterCount];
					for(int i=0; i<parameters.Length; ++i) {
						parameters[i] = this.GetParameterType(i);
					}
					MethodInfoImpl method = (MethodInfoImpl)parent.GetMethod(this.Name
						, (this.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic)
						| (this.IsStatic ? BindingFlags.Static : BindingFlags.Instance)
						, null, parameters, null);
					if(method==null) {
						Console.WriteLine("this     : "+this);
						Console.WriteLine("Declared : "+this.DeclaringType);
						Console.WriteLine("Reflected: "+this.ReflectedType);
						Console.WriteLine("Owner    : "+this.owner);
						Console.WriteLine("Base     : "+this.owner.BaseType);
						throw new UnexpectedException("Base definition is not found: "+this);
					}
					return method;
				} else {
					throw new UnexpectedException();
				}
			}
		}

		#endregion

	}
}
