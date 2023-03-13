using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	sealed class MethodDefInfo : MethodDeclInfo, IMemberRefParent {

		readonly MethodDefRow row;
		string name = null;
		MethodIL methodil;

		CodeFlags codeFlags;
		int codeOffset;
		int codeSize;
		int maxStack;
		int varCount;
		LocalVarSig localVarSig;
		List<MethodSection> extraSections;

		[Flags]
		enum MethodSectionFlags : byte {
			EHTable=0x01,	// Exception handling data.
			OptILTable=0x02,	// Reserved, shall be 0.
			FatFormat=0x40,	// Data format is of the fat variety, meaning there is a 3 byte length.
			// If not set, the header is small with a  1 byte length
			MoreSects=0x80,	// Another data section occurs after this current section
		}

		[Flags]
		enum ExceptionClauseFlags {
			EXCEPTION_CLAUSE_EXCEPTION=0x0000,	// A typed exception clause
			EXCEPTION_CLAUSE_FILTER=0x0001,	// An exception filter and handler clause
			EXCEPTION_CLAUSE_FINALLY=0x0002,	// A finally clause
			EXCEPTION_CLAUSE_FAULT=0x0004,	// Fault clause (finally that is called on exception only)
		}

		class MethodSection {
			public MethodSectionFlags Flags;
			public long Position;
			public MethodSection(MethodSectionFlags flags, long pos) {
				this.Flags = flags;
				this.Position = pos;
			}
		}

		internal MethodDefInfo(AssemblyDefInfo assembly, MethodDefRow row) : base(assembly) {
			this.row = row;
			//MetadataRoot mdroot = this.Assembly.Metadata;
			//MethodTable table = (MethodTable)mdroot.Tables[TableId.Method];
			this.codeOffset = 0;
			this.codeFlags = (CodeFlags)0;
			this.codeSize = 0;
			this.maxStack = 0;
			this.varCount = 0;
			if(row.RVA!=0 && MethodImplAttributes.IL==(row.ImplFlags&MethodImplAttributes.CodeTypeMask)) {
				this.methodil = assembly.OpenMethod(row);
			}
#if false
			// Paramテーブルは return type の情報も含むことがあるので、method signature とセマンティクスが一致しない。
			int paramcount = this.Assembly.GetParameterCount(this.RowIndex);
			if(paramcount!=this.Signature.ParameterCount) {
				Console.Error.WriteLine(this.FullName);
				Console.Error.WriteLine("ParamList ParamCount: {0}", paramcount);
				for(int i=0; i<paramcount; ++i) {
					Console.Error.WriteLine(this.Assembly.GetParameterDef(this.row.ParamList+i).Name);
				}
				Console.Error.WriteLine("Signature ParamCount: {0}", this.Signature.ParameterCount);
				Console.Error.Write("Signature: ");
				this.Assembly.OpenSig(this.row.Signature).Dump(Console.Error);
				if(this.Assembly.GetParameterCount(this.RowIndex)<this.Signature.ParameterCount) {
					throw new BadSignatureException(this.FullName);
				}
			}
#endif
		}

		public override int RowIndex {
			get {
				return this.row.Index;
			}
		}

		public int Index {
			get {
				return this.Assembly.SearchTypeOfMethod(this.RowIndex).GetMethodIndex(this.row.Index);
			}
		}

		public override string ToString() {
			return "Method: "+this.FullName;
		}

		private MethodSig _signature;

		internal override MethodSig Signature {
			get {
				if(_signature==null) {
					_signature = (MethodSig)this.Assembly.LoadSignature(this.row.Signature, MethodSig.Factory);
				}
				return _signature;
			}
		}

		#region 単純なプロパティ

		public override string Name {
			get {
				if(name==null) {
					name = this.Assembly.LoadBlobString(row.Name);
				}
				return name;
			}
		}

		public TypeDefInfo TypeDef {
			get {
				return this.Assembly.SearchTypeOfMethod(this.RowIndex);
			}
		}

		public override TypeDeclInfo Type {
			get {
				return this.TypeDef;
			}
		}

		public bool HasRVA {
			get {
				return this.RVA>0;
			}
		}

		public uint RVA {
			get {
				return this.row.RVA;
			}
		}

		public bool IsBlank {
			get {
				return !this.HasRVA;
			}
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

		public bool IsVirtual {
			get {
				return 0!=(this.row.Flags&MethodAttributes.Virtual);
			}
		}

		public bool HasNewSlot {
			get {
				if(!this.IsVirtual) {
					return false;
				} else {
					return (0!=(this.row.Flags&MethodAttributes.NewSlot))
						|| (0!=(this.row.ImplFlags&MethodImplAttributes.Runtime) && this.Name=="Invoke");
				}
			}
		}

		public bool HasReuseSlot {
			get {
				if(!this.IsVirtual) {
					return false;
				} else {
					return !this.HasNewSlot;
				}
			}
		}

		[Obsolete]
		public MethodAttributes Attributes {
			get {
				return row.Flags;
			}
		}

		public MethodImplAttributes GetMethodImplementationFlags() {
			return row.ImplFlags;
		}

		#endregion

		#region ローカル変数

		public int VariableCount {
			get {
				return this.varCount;
			}
		}

		internal LocalVar GetVariable(int index) {
			return this.localVarSig.LocalVars[index];
		}

		public TypeDeclInfo GetVariableType(int index) {
			return this.Assembly.LookupType(this.GetVariable(index).Type);
		}

		#endregion

		#region コード

#if false
		public Stream OpenCodeBlock() {
			if(!this.HasRVA) { throw new MissingMethodException("No RVA: "+this.FullName); }
			Stream stream = this.Assembly.OpenMethod(this.row);
			return new CooS.IO.BoundStream(stream, stream.Position+this.codeOffset, this.codeSize);
		}
#endif

		public IEnumerable<IL.Instruction> EnumInstructions() {
			foreach(IL.Instruction inst in IL.Instruction.Read(methodil.ByteCode)) {
				yield return inst;
			}
		}

		#endregion

		public ParameterDefInfo GetParameterDef(int index) {
			return this.Assembly.GetParameterDef(this.row.ParamList.Value+index);
		}

		public IEnumerable<ParameterDefInfo> ParameterCollection {
			get {
				return this.Assembly.CreateParamDefCollection(this.row.ParamList.Value, this.Assembly.GetParameterCount(this.RowIndex));
			}
		}

		internal int GetParameterIndex(int rowIndex) {
			return rowIndex-this.row.ParamList.Value;
		}

	}

}
