// Auto-generated file - DO NOT EDIT!
// Please edit md-schema.xml or rows.xsl if you want to make changes.

using System;
using System.IO;
using CooS.CodeModels.DLL;
using System.Runtime.InteropServices;

namespace CooS.CodeModels.CLI.Metadata {



	/// <summary>
	///  Represents row in Module table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.27
	/// </remarks>
	public class ModuleRow : RowBase {
		
		public ushort Generation;
		public int Name;
		public int Mvid;
		public int EncId;
		public int EncBaseId;

		public ModuleRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Generation = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Mvid = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.EncId = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.EncBaseId = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Module table has 5 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 5;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.StringsIndexSize + Heap.GuidIndexSize + Heap.GuidIndexSize + Heap.GuidIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Generation        : {0}", this.Generation);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Mvid              : {0}", this.Mvid);
			writer.WriteLine("EncId             : {0}", this.EncId);
			writer.WriteLine("EncBaseId         : {0}", this.EncBaseId);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in TypeRef table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.35
	/// </remarks>
	public class TypeRefRow : RowBase {
		
		public MDToken ResolutionScope;
		public int Name;
		public int Namespace;

		public TypeRefRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.ResolutionScope = TabsDecoder.DecodeToken(CodedTokenId.ResolutionScope, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Namespace = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in TypeRef table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetCodedIndexSize(CodedTokenId.ResolutionScope) + Heap.StringsIndexSize + Heap.StringsIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("ResolutionScope   : {0}", this.ResolutionScope);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Namespace         : {0}", this.Namespace);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in TypeDef table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.34
	/// </remarks>
	public class TypeDefRow : RowBase {
		
		public System.Reflection.TypeAttributes Flags;
		public int Name;
		public int Namespace;
		public MDToken Extends;
		public int FieldList;
		public int MethodList;

		public TypeDefRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Flags = (System.Reflection.TypeAttributes) LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Namespace = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Extends = TabsDecoder.DecodeToken(CodedTokenId.TypeDefOrRef, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.FieldList = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.MethodList = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in TypeDef table has 6 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 6;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + Heap.StringsIndexSize + Heap.StringsIndexSize + Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef) + Heap.GetIndexSize(TableId.Field) + Heap.GetIndexSize(TableId.Method);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Namespace         : {0}", this.Namespace);
			writer.WriteLine("Extends           : {0}", this.Extends);
			writer.WriteLine("FieldList         : {0}", this.FieldList);
			writer.WriteLine("MethodList        : {0}", this.MethodList);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in FieldPtr table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class FieldPtrRow : RowBase {
		
		public int Field;

		public FieldPtrRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Field = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in FieldPtr table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.Field);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Field             : {0}", this.Field);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Field table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.15
	/// </remarks>
	public class FieldRow : RowBase {
		
		public System.Reflection.FieldAttributes Flags;
		public int Name;
		public int Signature;

		public FieldRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Flags = (System.Reflection.FieldAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Signature = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Field table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.StringsIndexSize + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Signature         : {0}", this.Signature);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in MethodPtr table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class MethodPtrRow : RowBase {
		
		public int Method;

		public MethodPtrRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Method = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in MethodPtr table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.Method);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Method            : {0}", this.Method);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Method table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.24
	/// </remarks>
	public class MethodRow : RowBase {
		
		public RVA RVA;
		public System.Reflection.MethodImplAttributes ImplFlags;
		public System.Reflection.MethodAttributes Flags;
		public int Name;
		public int Signature;
		public int ParamList;

		public MethodRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.RVA = LEBitConverter.ToUInt32(buff, offs);
			offs += RVA.Size;
			this.ImplFlags = (System.Reflection.MethodImplAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Flags = (System.Reflection.MethodAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Signature = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.ParamList = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Method table has 6 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 6;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return RVA.Size + 2 + 2 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return RVA.Size + 2 + 2 + Heap.StringsIndexSize + Heap.BlobIndexSize + Heap.GetIndexSize(TableId.Param);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("RVA               : {0}", this.RVA);
			writer.WriteLine("ImplFlags         : {0}", this.ImplFlags);
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Signature         : {0}", this.Signature);
			writer.WriteLine("ParamList         : {0}", this.ParamList);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ParamPtr table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class ParamPtrRow : RowBase {
		
		public int Param;

		public ParamPtrRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Param = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in ParamPtr table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.Param);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Param             : {0}", this.Param);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Param table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.30
	/// </remarks>
	public class ParamRow : RowBase {
		
		public System.Reflection.ParameterAttributes Flags;
		public ushort Sequence;
		public int Name;

		public ParamRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Flags = (System.Reflection.ParameterAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Sequence = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Param table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 2 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + 2 + Heap.StringsIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Sequence          : {0}", this.Sequence);
			writer.WriteLine("Name              : {0}", this.Name);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in InterfaceImpl table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.21
	/// </remarks>
	public class InterfaceImplRow : RowBase {
		
		public int Class;
		public MDToken Interface;

		public InterfaceImplRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Class = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Interface = TabsDecoder.DecodeToken(CodedTokenId.TypeDefOrRef, LEBitConverter.ToInt32(buff, offs));
			
		}

		/// <summary>
		///  Row in InterfaceImpl table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.TypeDef) + Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Class             : {0}", this.Class);
			writer.WriteLine("Interface         : {0}", this.Interface);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in MemberRef table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.23
	/// </remarks>
	public class MemberRefRow : RowBase {
		
		public MDToken Class;
		public int Name;
		public int Signature;

		public MemberRefRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Class = TabsDecoder.DecodeToken(CodedTokenId.MemberRefParent, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Signature = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in MemberRef table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetCodedIndexSize(CodedTokenId.MemberRefParent) + Heap.StringsIndexSize + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Class             : {0}", this.Class);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Signature         : {0}", this.Signature);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Constant table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.9
	/// </remarks>
	public class ConstantRow : RowBase {
		
		public ElementType Type;
		public MDToken Parent;
		public int Value;

		public ConstantRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Type = (ElementType) LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.Parent = TabsDecoder.DecodeToken(CodedTokenId.HasConstant, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Value = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Constant table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.GetCodedIndexSize(CodedTokenId.HasConstant) + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Type              : {0}", this.Type);
			writer.WriteLine("Parent            : {0}", this.Parent);
			writer.WriteLine("Value             : {0}", this.Value);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in CustomAttribute table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.10
	/// </remarks>
	public class CustomAttributeRow : RowBase {
		
		public MDToken Parent;
		public MDToken Type;
		public int Value;

		public CustomAttributeRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Parent = TabsDecoder.DecodeToken(CodedTokenId.HasCustomAttribute, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Type = TabsDecoder.DecodeToken(CodedTokenId.CustomAttributeType, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Value = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in CustomAttribute table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetCodedIndexSize(CodedTokenId.HasCustomAttribute) + Heap.GetCodedIndexSize(CodedTokenId.CustomAttributeType) + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Parent            : {0}", this.Parent);
			writer.WriteLine("Type              : {0}", this.Type);
			writer.WriteLine("Value             : {0}", this.Value);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in FieldMarshal table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.17
	/// </remarks>
	public class FieldMarshalRow : RowBase {
		
		public MDToken Parent;
		public int NativeType;

		public FieldMarshalRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Parent = TabsDecoder.DecodeToken(CodedTokenId.HasFieldMarshal, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.NativeType = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in FieldMarshal table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetCodedIndexSize(CodedTokenId.HasFieldMarshal) + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Parent            : {0}", this.Parent);
			writer.WriteLine("NativeType        : {0}", this.NativeType);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in DeclSecurity table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.11
	/// </remarks>
	public class DeclSecurityRow : RowBase {
		
		public short Action;
		public MDToken Parent;
		public int PermissionSet;

		public DeclSecurityRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Action = LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.Parent = TabsDecoder.DecodeToken(CodedTokenId.HasDeclSecurity, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.PermissionSet = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in DeclSecurity table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.GetCodedIndexSize(CodedTokenId.HasDeclSecurity) + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Action            : {0}", this.Action);
			writer.WriteLine("Parent            : {0}", this.Parent);
			writer.WriteLine("PermissionSet     : {0}", this.PermissionSet);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ClassLayout table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.8
	/// </remarks>
	public class ClassLayoutRow : RowBase {
		
		public short PackingSize;
		public int ClassSize;
		public int Parent;

		public ClassLayoutRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.PackingSize = LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.ClassSize = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Parent = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in ClassLayout table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + 4 + Heap.GetIndexSize(TableId.TypeDef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("PackingSize       : {0}", this.PackingSize);
			writer.WriteLine("ClassSize         : {0}", this.ClassSize);
			writer.WriteLine("Parent            : {0}", this.Parent);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in FieldLayout table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.16
	/// </remarks>
	public class FieldLayoutRow : RowBase {
		
		public int Offset;
		public int Field;

		public FieldLayoutRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Offset = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Field = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in FieldLayout table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + Heap.GetIndexSize(TableId.Field);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Offset            : {0}", this.Offset);
			writer.WriteLine("Field             : {0}", this.Field);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in StandAloneSig table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.33
	/// </remarks>
	public class StandAloneSigRow : RowBase {
		
		public int Signature;

		public StandAloneSigRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Signature = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in StandAloneSig table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Signature         : {0}", this.Signature);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in EventMap table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.12
	/// </remarks>
	public class EventMapRow : RowBase {
		
		public int Parent;
		public int EventList;

		public EventMapRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Parent = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.EventList = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in EventMap table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.TypeDef) + Heap.GetIndexSize(TableId.Event);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Parent            : {0}", this.Parent);
			writer.WriteLine("EventList         : {0}", this.EventList);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in EventPtr table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class EventPtrRow : RowBase {
		
		public int Event;

		public EventPtrRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Event = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in EventPtr table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.Event);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Event             : {0}", this.Event);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Event table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.13
	/// </remarks>
	public class EventRow : RowBase {
		
		public System.Reflection.EventAttributes EventFlags;
		public int Name;
		public MDToken EventType;

		public EventRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.EventFlags = (System.Reflection.EventAttributes) LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.EventType = TabsDecoder.DecodeToken(CodedTokenId.TypeDefOrRef, LEBitConverter.ToInt32(buff, offs));
			
		}

		/// <summary>
		///  Row in Event table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.StringsIndexSize + Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("EventFlags        : {0}", this.EventFlags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("EventType         : {0}", this.EventType);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in PropertyMap table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.32
	/// </remarks>
	public class PropertyMapRow : RowBase {
		
		public int Parent;
		public int PropertyList;

		public PropertyMapRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Parent = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.PropertyList = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in PropertyMap table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.TypeDef) + Heap.GetIndexSize(TableId.Property);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Parent            : {0}", this.Parent);
			writer.WriteLine("PropertyList      : {0}", this.PropertyList);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in PropertyPtr table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class PropertyPtrRow : RowBase {
		
		public int Property;

		public PropertyPtrRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Property = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in PropertyPtr table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.Property);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Property          : {0}", this.Property);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Property table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.30
	/// </remarks>
	public class PropertyRow : RowBase {
		
		public System.Reflection.PropertyAttributes Flags;
		public int Name;
		public int Type;

		public PropertyRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Flags = (System.Reflection.PropertyAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Type = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Property table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.StringsIndexSize + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Type              : {0}", this.Type);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in MethodSemantics table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.26
	/// </remarks>
	public class MethodSemanticsRow : RowBase {
		
		public MethodSemanticsAttributes Semantics;
		public int Method;
		public MDToken Association;

		public MethodSemanticsRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Semantics = (MethodSemanticsAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Method = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Association = TabsDecoder.DecodeToken(CodedTokenId.HasSemantics, LEBitConverter.ToInt32(buff, offs));
			
		}

		/// <summary>
		///  Row in MethodSemantics table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.GetIndexSize(TableId.Method) + Heap.GetCodedIndexSize(CodedTokenId.HasSemantics);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Semantics         : {0}", this.Semantics);
			writer.WriteLine("Method            : {0}", this.Method);
			writer.WriteLine("Association       : {0}", this.Association);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in MethodImpl table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.25
	/// </remarks>
	public class MethodImplRow : RowBase {
		
		public int Class;
		public MDToken MethodBody;
		public MDToken MethodDeclaration;

		public MethodImplRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Class = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.MethodBody = TabsDecoder.DecodeToken(CodedTokenId.MethodDefOrRef, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.MethodDeclaration = TabsDecoder.DecodeToken(CodedTokenId.MethodDefOrRef, LEBitConverter.ToInt32(buff, offs));
			
		}

		/// <summary>
		///  Row in MethodImpl table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.TypeDef) + Heap.GetCodedIndexSize(CodedTokenId.MethodDefOrRef) + Heap.GetCodedIndexSize(CodedTokenId.MethodDefOrRef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Class             : {0}", this.Class);
			writer.WriteLine("MethodBody        : {0}", this.MethodBody);
			writer.WriteLine("MethodDeclaration : {0}", this.MethodDeclaration);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ModuleRef table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.28
	/// </remarks>
	public class ModuleRefRow : RowBase {
		
		public int Name;

		public ModuleRefRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Name = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in ModuleRef table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.StringsIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Name              : {0}", this.Name);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in TypeSpec table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.36
	/// </remarks>
	public class TypeSpecRow : RowBase {
		
		public int Signature;

		public TypeSpecRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Signature = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in TypeSpec table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Signature         : {0}", this.Signature);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ImplMap table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.20
	/// </remarks>
	public class ImplMapRow : RowBase {
		
		public PInvokeAttributes MappingFlags;
		public MDToken MemberForwarded;
		public int ImportName;
		public int ImportScope;

		public ImplMapRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.MappingFlags = (PInvokeAttributes) LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.MemberForwarded = TabsDecoder.DecodeToken(CodedTokenId.MemberForwarded, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.ImportName = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.ImportScope = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in ImplMap table has 4 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.GetCodedIndexSize(CodedTokenId.MemberForwarded) + Heap.StringsIndexSize + Heap.GetIndexSize(TableId.ModuleRef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("MappingFlags      : {0}", this.MappingFlags);
			writer.WriteLine("MemberForwarded   : {0}", this.MemberForwarded);
			writer.WriteLine("ImportName        : {0}", this.ImportName);
			writer.WriteLine("ImportScope       : {0}", this.ImportScope);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in FieldRVA table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.18
	/// </remarks>
	public class FieldRVARow : RowBase {
		
		public RVA RVA;
		public int Field;

		public FieldRVARow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.RVA = LEBitConverter.ToUInt32(buff, offs);
			offs += RVA.Size;
			this.Field = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in FieldRVA table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return RVA.Size + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return RVA.Size + Heap.GetIndexSize(TableId.Field);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("RVA               : {0}", this.RVA);
			writer.WriteLine("Field             : {0}", this.Field);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ENCLog table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class ENCLogRow : RowBase {
		
		public uint Token;
		public uint FuncCode;

		public ENCLogRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Token = LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.FuncCode = LEBitConverter.ToUInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in ENCLog table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + 4;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Token             : {0}", this.Token);
			writer.WriteLine("FuncCode          : {0}", this.FuncCode);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ENCMap table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class ENCMapRow : RowBase {
		
		public uint Token;

		public ENCMapRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Token = LEBitConverter.ToUInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in ENCMap table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Token             : {0}", this.Token);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in Assembly table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.2
	/// </remarks>
	public class AssemblyRow : RowBase {
		
		public System.Configuration.Assemblies.AssemblyHashAlgorithm HashAlgId;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public ushort BuildNumber;
		public ushort RevisionNumber;
		public AssemblyFlags Flags;
		public int PublicKey;
		public int Name;
		public int Culture;

		public AssemblyRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.HashAlgId = (System.Configuration.Assemblies.AssemblyHashAlgorithm) LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.MajorVersion = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.MinorVersion = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.BuildNumber = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.RevisionNumber = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Flags = (AssemblyFlags) LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.PublicKey = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Culture = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in Assembly table has 9 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 9;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 2 + 2 + 2 + 2 + 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + 2 + 2 + 2 + 2 + 4 + Heap.BlobIndexSize + Heap.StringsIndexSize + Heap.StringsIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("HashAlgId         : {0}", this.HashAlgId);
			writer.WriteLine("MajorVersion      : {0}", this.MajorVersion);
			writer.WriteLine("MinorVersion      : {0}", this.MinorVersion);
			writer.WriteLine("BuildNumber       : {0}", this.BuildNumber);
			writer.WriteLine("RevisionNumber    : {0}", this.RevisionNumber);
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("PublicKey         : {0}", this.PublicKey);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Culture           : {0}", this.Culture);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in AssemblyProcessor table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.4
	/// </remarks>
	public class AssemblyProcessorRow : RowBase {
		
		public int Processor;

		public AssemblyProcessorRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Processor = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in AssemblyProcessor table has 1 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 1;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Processor         : {0}", this.Processor);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in AssemblyOS table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.3
	/// </remarks>
	public class AssemblyOSRow : RowBase {
		
		public int OSPlatformID;
		public int OSMajorVersion;
		public int OSMinorVersion;

		public AssemblyOSRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.OSPlatformID = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.OSMajorVersion = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.OSMinorVersion = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in AssemblyOS table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + 4 + 4;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("OSPlatformID      : {0}", this.OSPlatformID);
			writer.WriteLine("OSMajorVersion    : {0}", this.OSMajorVersion);
			writer.WriteLine("OSMinorVersion    : {0}", this.OSMinorVersion);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in AssemblyRef table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.5
	/// </remarks>
	public class AssemblyRefRow : RowBase {
		
		public short MajorVersion;
		public short MinorVersion;
		public short BuildNumber;
		public short RevisionNumber;
		public AssemblyFlags Flags;
		public int PublicKeyOrToken;
		public int Name;
		public int Culture;
		public int HashValue;

		public AssemblyRefRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.MajorVersion = LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.MinorVersion = LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.BuildNumber = LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.RevisionNumber = LEBitConverter.ToInt16(buff, offs);
			offs += 2;
			this.Flags = (AssemblyFlags) LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.PublicKeyOrToken = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Culture = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.HashValue = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in AssemblyRef table has 9 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 9;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 2 + 2 + 2 + 4 + 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + 2 + 2 + 2 + 4 + Heap.BlobIndexSize + Heap.StringsIndexSize + Heap.StringsIndexSize + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("MajorVersion      : {0}", this.MajorVersion);
			writer.WriteLine("MinorVersion      : {0}", this.MinorVersion);
			writer.WriteLine("BuildNumber       : {0}", this.BuildNumber);
			writer.WriteLine("RevisionNumber    : {0}", this.RevisionNumber);
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("PublicKeyOrToken  : {0}", this.PublicKeyOrToken);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Culture           : {0}", this.Culture);
			writer.WriteLine("HashValue         : {0}", this.HashValue);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in AssemblyRefProcessor table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.7
	/// </remarks>
	public class AssemblyRefProcessorRow : RowBase {
		
		public int Processor;
		public int AssemblyRef;

		public AssemblyRefProcessorRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Processor = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.AssemblyRef = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in AssemblyRefProcessor table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + Heap.GetIndexSize(TableId.AssemblyRef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Processor         : {0}", this.Processor);
			writer.WriteLine("AssemblyRef       : {0}", this.AssemblyRef);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in AssemblyRefOS table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.6
	/// </remarks>
	public class AssemblyRefOSRow : RowBase {
		
		public int OSPlatformID;
		public int OSMajorVersion;
		public int OSMinorVersion;
		public int AssemblyRef;

		public AssemblyRefOSRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.OSPlatformID = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.OSMajorVersion = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.OSMinorVersion = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.AssemblyRef = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in AssemblyRefOS table has 4 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + 4 + 4 + Heap.GetIndexSize(TableId.AssemblyRef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("OSPlatformID      : {0}", this.OSPlatformID);
			writer.WriteLine("OSMajorVersion    : {0}", this.OSMajorVersion);
			writer.WriteLine("OSMinorVersion    : {0}", this.OSMinorVersion);
			writer.WriteLine("AssemblyRef       : {0}", this.AssemblyRef);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in File table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.19
	/// </remarks>
	public class FileRow : RowBase {
		
		public System.IO.FileAttributes Flags;
		public int Name;
		public int HashValue;

		public FileRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Flags = (System.IO.FileAttributes) LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.HashValue = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in File table has 3 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 3;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + Heap.StringsIndexSize + Heap.BlobIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("HashValue         : {0}", this.HashValue);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ExportedType table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.14
	/// </remarks>
	public class ExportedTypeRow : RowBase {
		
		public System.Reflection.TypeAttributes Flags;
		public int TypeDefId;
		public int TypeName;
		public int TypeNamespace;
		public MDToken Implementation;

		public ExportedTypeRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Flags = (System.Reflection.TypeAttributes) LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.TypeDefId = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.TypeName = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.TypeNamespace = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Implementation = TabsDecoder.DecodeToken(CodedTokenId.Implementation, LEBitConverter.ToInt32(buff, offs));
			
		}

		/// <summary>
		///  Row in ExportedType table has 5 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 5;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + Heap.GetIndexSize(TableId.TypeDef) + Heap.StringsIndexSize + Heap.StringsIndexSize + Heap.GetCodedIndexSize(CodedTokenId.Implementation);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("TypeDefId         : {0}", this.TypeDefId);
			writer.WriteLine("TypeName          : {0}", this.TypeName);
			writer.WriteLine("TypeNamespace     : {0}", this.TypeNamespace);
			writer.WriteLine("Implementation    : {0}", this.Implementation);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in ManifestResource table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.22
	/// </remarks>
	public class ManifestResourceRow : RowBase {
		
		public int Offset;
		public ManifestResourceAttributes Flags;
		public int Name;
		public MDToken Implementation;

		public ManifestResourceRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Offset = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Flags = (ManifestResourceAttributes) LEBitConverter.ToUInt32(buff, offs);
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Implementation = TabsDecoder.DecodeToken(CodedTokenId.Implementation, LEBitConverter.ToInt32(buff, offs));
			
		}

		/// <summary>
		///  Row in ManifestResource table has 4 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 4 + 4 + Heap.StringsIndexSize + Heap.GetCodedIndexSize(CodedTokenId.Implementation);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Offset            : {0}", this.Offset);
			writer.WriteLine("Flags             : {0}", this.Flags);
			writer.WriteLine("Name              : {0}", this.Name);
			writer.WriteLine("Implementation    : {0}", this.Implementation);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in NestedClass table.
	/// </summary>
	/// <remarks>
	///  See Partition II, Metadata; section 21.29
	/// </remarks>
	public class NestedClassRow : RowBase {
		
		public int NestedClass;
		public int EnclosingClass;

		public NestedClassRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.NestedClass = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.EnclosingClass = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in NestedClass table has 2 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 2;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return Heap.GetIndexSize(TableId.TypeDef) + Heap.GetIndexSize(TableId.TypeDef);
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("NestedClass       : {0}", this.NestedClass);
			writer.WriteLine("EnclosingClass    : {0}", this.EnclosingClass);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in TypeTyPar table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class TypeTyParRow : RowBase {
		
		public ushort Number;
		public int Class;
		public MDToken Bound;
		public int Name;

		public TypeTyParRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Number = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Class = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Bound = TabsDecoder.DecodeToken(CodedTokenId.TypeDefOrRef, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in TypeTyPar table has 4 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.GetIndexSize(TableId.TypeDef) + Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef) + Heap.StringsIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Number            : {0}", this.Number);
			writer.WriteLine("Class             : {0}", this.Class);
			writer.WriteLine("Bound             : {0}", this.Bound);
			writer.WriteLine("Name              : {0}", this.Name);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



	/// <summary>
	///  Represents row in MethodTyPar table.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public class MethodTyParRow : RowBase {
		
		public ushort Number;
		public int Method;
		public MDToken Bound;
		public int Name;

		public MethodTyParRow(Table parent, int rowIndex, byte[] buff) : base(parent,rowIndex) {
			int offs = 0;
		
			this.Number = LEBitConverter.ToUInt16(buff, offs);
			offs += 2;
			this.Method = LEBitConverter.ToInt32(buff, offs);
			offs += 4;
			this.Bound = TabsDecoder.DecodeToken(CodedTokenId.TypeDefOrRef, LEBitConverter.ToInt32(buff, offs));
			offs += 4;
			this.Name = LEBitConverter.ToInt32(buff, offs);
			
		}

		/// <summary>
		///  Row in MethodTyPar table has 4 columns.
		/// </summary>
		public virtual int NumberOfColumns {
			get {
				return 4;
			}
		}

		/// <summary>
		///  Logical size of this instance in bytes.
		/// </summary>
		public override int Size {
			get {
				return LogicalSize;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int LogicalSize {
			get {
				return 2 + 4 + 4 + 4;
			}
		}

		/// <summary>
		///  Logical size of this type of row in bytes.
		/// </summary>
		public static int EstimateCodedSize(TablesHeap Heap) {
			return 2 + Heap.GetIndexSize(TableId.Method) + Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef) + Heap.StringsIndexSize;
		}

		

		/// <summary>
		/// </summary>
		public override void Dump(TextWriter writer) {
			
			writer.WriteLine("Number            : {0}", this.Number);
			writer.WriteLine("Method            : {0}", this.Method);
			writer.WriteLine("Bound             : {0}", this.Bound);
			writer.WriteLine("Name              : {0}", this.Name);
		}

		/// <summary>
		/// </summary>
		public override string ToString() {
			StringWriter sw = new StringWriter();
			Dump(sw);
			return sw.ToString();
		}

	}



}

