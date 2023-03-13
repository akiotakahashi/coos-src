// Auto-generated file - DO NOT EDIT!
// Please edit md-schema.xml or tabs.xsl if you want to make changes.

using System;
using System.IO;
using CooS.CodeModels.DLL;

namespace CooS.CodeModels.CLI.Metadata {


	public class ModuleTable : TableBase {

		public ModuleTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Module";
			}
		}

		public override TableId Id {
			get {
				return TableId.Module;
			}
		}

		public override int RowLogicalSize {
			get {
				return ModuleRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ModuleRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Generation, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Mvid, index(#GUID)
			stream.Read(buff, offset, Heap.GuidIndexSize);
			offset += 4;
		
			// EncId, index(#GUID)
			stream.Read(buff, offset, Heap.GuidIndexSize);
			offset += 4;
		
			// EncBaseId, index(#GUID)
			stream.Read(buff, offset, Heap.GuidIndexSize);
			offset += 4;
		
			return new ModuleRow(this, rowIndex, buff);
		}

	}

	public class TypeRefTable : TableBase {

		public TypeRefTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "TypeRef";
			}
		}

		public override TableId Id {
			get {
				return TableId.TypeRef;
			}
		}

		public override int RowLogicalSize {
			get {
				return TypeRefRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return TypeRefRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// ResolutionScope, coded-index(ResolutionScope)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.ResolutionScope));
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Namespace, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			return new TypeRefRow(this, rowIndex, buff);
		}

	}

	public class TypeDefTable : TableBase {

		TypeDefRow[] cache;

		public TypeDefTable(TablesHeap heap, long offset) : base(heap,offset) {
			this.cache = new TypeDefRow[this.RowCount];
		}

		public override string Name {
			get {
				return "TypeDef";
			}
		}

		public override TableId Id {
			get {
				return TableId.TypeDef;
			}
		}

		public override int RowLogicalSize {
			get {
				return TypeDefRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return TypeDefRow.EstimateCodedSize(Heap);
			}
		}

		public override Row this[int index] {
			get {
				if(index<0) throw new ArgumentOutOfRangeException();
				if(index==0) return null;
				--index;
				if(this.cache[index]==null) {
					this.cache[index] = (TypeDefRow)base[index+1];
				}
				return this.cache[index];
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Flags, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Namespace, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Extends, coded-index(TypeDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef));
			offset += 4;
		
			// FieldList, index(Field)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Field));
			offset += 4;
		
			// MethodList, index(Method)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Method));
			offset += 4;
		
			return new TypeDefRow(this, rowIndex, buff);
		}

	}

	public class FieldPtrTable : TableBase {

		public FieldPtrTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "FieldPtr";
			}
		}

		public override TableId Id {
			get {
				return TableId.FieldPtr;
			}
		}

		public override int RowLogicalSize {
			get {
				return FieldPtrRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return FieldPtrRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Field, index(Field)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Field));
			offset += 4;
		
			return new FieldPtrRow(this, rowIndex, buff);
		}

	}

	public class FieldTable : TableBase {

		public FieldTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Field";
			}
		}

		public override TableId Id {
			get {
				return TableId.Field;
			}
		}

		public override int RowLogicalSize {
			get {
				return FieldRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return FieldRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Flags, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Signature, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new FieldRow(this, rowIndex, buff);
		}

	}

	public class MethodPtrTable : TableBase {

		public MethodPtrTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "MethodPtr";
			}
		}

		public override TableId Id {
			get {
				return TableId.MethodPtr;
			}
		}

		public override int RowLogicalSize {
			get {
				return MethodPtrRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return MethodPtrRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Method, index(Method)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Method));
			offset += 4;
		
			return new MethodPtrRow(this, rowIndex, buff);
		}

	}

	public class MethodTable : TableBase {

		public MethodTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Method";
			}
		}

		public override TableId Id {
			get {
				return TableId.Method;
			}
		}

		public override int RowLogicalSize {
			get {
				return MethodRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return MethodRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// RVA, RVA
			stream.Read(buff, offset, RVA.Size);
			offset += RVA.Size;
		
			// ImplFlags, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Flags, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Signature, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			// ParamList, index(Param)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Param));
			offset += 4;
		
			return new MethodRow(this, rowIndex, buff);
		}

	}

	public class ParamPtrTable : TableBase {

		public ParamPtrTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ParamPtr";
			}
		}

		public override TableId Id {
			get {
				return TableId.ParamPtr;
			}
		}

		public override int RowLogicalSize {
			get {
				return ParamPtrRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ParamPtrRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Param, index(Param)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Param));
			offset += 4;
		
			return new ParamPtrRow(this, rowIndex, buff);
		}

	}

	public class ParamTable : TableBase {

		public ParamTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Param";
			}
		}

		public override TableId Id {
			get {
				return TableId.Param;
			}
		}

		public override int RowLogicalSize {
			get {
				return ParamRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ParamRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Flags, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Sequence, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			return new ParamRow(this, rowIndex, buff);
		}

	}

	public class InterfaceImplTable : TableBase {

		public InterfaceImplTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "InterfaceImpl";
			}
		}

		public override TableId Id {
			get {
				return TableId.InterfaceImpl;
			}
		}

		public override int RowLogicalSize {
			get {
				return InterfaceImplRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return InterfaceImplRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Class, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// Interface, coded-index(TypeDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef));
			offset += 4;
		
			return new InterfaceImplRow(this, rowIndex, buff);
		}

	}

	public class MemberRefTable : TableBase {

		public MemberRefTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "MemberRef";
			}
		}

		public override TableId Id {
			get {
				return TableId.MemberRef;
			}
		}

		public override int RowLogicalSize {
			get {
				return MemberRefRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return MemberRefRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Class, coded-index(MemberRefParent)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.MemberRefParent));
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Signature, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new MemberRefRow(this, rowIndex, buff);
		}

	}

	public class ConstantTable : TableBase {

		public ConstantTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Constant";
			}
		}

		public override TableId Id {
			get {
				return TableId.Constant;
			}
		}

		public override int RowLogicalSize {
			get {
				return ConstantRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ConstantRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Type, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Parent, coded-index(HasConstant)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.HasConstant));
			offset += 4;
		
			// Value, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new ConstantRow(this, rowIndex, buff);
		}

	}

	public class CustomAttributeTable : TableBase {

		public CustomAttributeTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "CustomAttribute";
			}
		}

		public override TableId Id {
			get {
				return TableId.CustomAttribute;
			}
		}

		public override int RowLogicalSize {
			get {
				return CustomAttributeRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return CustomAttributeRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Parent, coded-index(HasCustomAttribute)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.HasCustomAttribute));
			offset += 4;
		
			// Type, coded-index(CustomAttributeType)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.CustomAttributeType));
			offset += 4;
		
			// Value, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new CustomAttributeRow(this, rowIndex, buff);
		}

	}

	public class FieldMarshalTable : TableBase {

		public FieldMarshalTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "FieldMarshal";
			}
		}

		public override TableId Id {
			get {
				return TableId.FieldMarshal;
			}
		}

		public override int RowLogicalSize {
			get {
				return FieldMarshalRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return FieldMarshalRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Parent, coded-index(HasFieldMarshal)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.HasFieldMarshal));
			offset += 4;
		
			// NativeType, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new FieldMarshalRow(this, rowIndex, buff);
		}

	}

	public class DeclSecurityTable : TableBase {

		public DeclSecurityTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "DeclSecurity";
			}
		}

		public override TableId Id {
			get {
				return TableId.DeclSecurity;
			}
		}

		public override int RowLogicalSize {
			get {
				return DeclSecurityRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return DeclSecurityRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Action, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Parent, coded-index(HasDeclSecurity)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.HasDeclSecurity));
			offset += 4;
		
			// PermissionSet, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new DeclSecurityRow(this, rowIndex, buff);
		}

	}

	public class ClassLayoutTable : TableBase {

		public ClassLayoutTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ClassLayout";
			}
		}

		public override TableId Id {
			get {
				return TableId.ClassLayout;
			}
		}

		public override int RowLogicalSize {
			get {
				return ClassLayoutRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ClassLayoutRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// PackingSize, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// ClassSize, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// Parent, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			return new ClassLayoutRow(this, rowIndex, buff);
		}

	}

	public class FieldLayoutTable : TableBase {

		public FieldLayoutTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "FieldLayout";
			}
		}

		public override TableId Id {
			get {
				return TableId.FieldLayout;
			}
		}

		public override int RowLogicalSize {
			get {
				return FieldLayoutRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return FieldLayoutRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Offset, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// Field, index(Field)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Field));
			offset += 4;
		
			return new FieldLayoutRow(this, rowIndex, buff);
		}

	}

	public class StandAloneSigTable : TableBase {

		public StandAloneSigTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "StandAloneSig";
			}
		}

		public override TableId Id {
			get {
				return TableId.StandAloneSig;
			}
		}

		public override int RowLogicalSize {
			get {
				return StandAloneSigRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return StandAloneSigRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Signature, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new StandAloneSigRow(this, rowIndex, buff);
		}

	}

	public class EventMapTable : TableBase {

		public EventMapTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "EventMap";
			}
		}

		public override TableId Id {
			get {
				return TableId.EventMap;
			}
		}

		public override int RowLogicalSize {
			get {
				return EventMapRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return EventMapRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Parent, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// EventList, index(Event)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Event));
			offset += 4;
		
			return new EventMapRow(this, rowIndex, buff);
		}

	}

	public class EventPtrTable : TableBase {

		public EventPtrTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "EventPtr";
			}
		}

		public override TableId Id {
			get {
				return TableId.EventPtr;
			}
		}

		public override int RowLogicalSize {
			get {
				return EventPtrRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return EventPtrRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Event, index(Event)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Event));
			offset += 4;
		
			return new EventPtrRow(this, rowIndex, buff);
		}

	}

	public class EventTable : TableBase {

		public EventTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Event";
			}
		}

		public override TableId Id {
			get {
				return TableId.Event;
			}
		}

		public override int RowLogicalSize {
			get {
				return EventRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return EventRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// EventFlags, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// EventType, coded-index(TypeDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef));
			offset += 4;
		
			return new EventRow(this, rowIndex, buff);
		}

	}

	public class PropertyMapTable : TableBase {

		public PropertyMapTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "PropertyMap";
			}
		}

		public override TableId Id {
			get {
				return TableId.PropertyMap;
			}
		}

		public override int RowLogicalSize {
			get {
				return PropertyMapRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return PropertyMapRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Parent, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// PropertyList, index(Property)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Property));
			offset += 4;
		
			return new PropertyMapRow(this, rowIndex, buff);
		}

	}

	public class PropertyPtrTable : TableBase {

		public PropertyPtrTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "PropertyPtr";
			}
		}

		public override TableId Id {
			get {
				return TableId.PropertyPtr;
			}
		}

		public override int RowLogicalSize {
			get {
				return PropertyPtrRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return PropertyPtrRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Property, index(Property)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Property));
			offset += 4;
		
			return new PropertyPtrRow(this, rowIndex, buff);
		}

	}

	public class PropertyTable : TableBase {

		public PropertyTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Property";
			}
		}

		public override TableId Id {
			get {
				return TableId.Property;
			}
		}

		public override int RowLogicalSize {
			get {
				return PropertyRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return PropertyRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Flags, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Type, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new PropertyRow(this, rowIndex, buff);
		}

	}

	public class MethodSemanticsTable : TableBase {

		public MethodSemanticsTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "MethodSemantics";
			}
		}

		public override TableId Id {
			get {
				return TableId.MethodSemantics;
			}
		}

		public override int RowLogicalSize {
			get {
				return MethodSemanticsRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return MethodSemanticsRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Semantics, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Method, index(Method)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Method));
			offset += 4;
		
			// Association, coded-index(HasSemantics)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.HasSemantics));
			offset += 4;
		
			return new MethodSemanticsRow(this, rowIndex, buff);
		}

	}

	public class MethodImplTable : TableBase {

		public MethodImplTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "MethodImpl";
			}
		}

		public override TableId Id {
			get {
				return TableId.MethodImpl;
			}
		}

		public override int RowLogicalSize {
			get {
				return MethodImplRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return MethodImplRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Class, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// MethodBody, coded-index(MethodDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.MethodDefOrRef));
			offset += 4;
		
			// MethodDeclaration, coded-index(MethodDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.MethodDefOrRef));
			offset += 4;
		
			return new MethodImplRow(this, rowIndex, buff);
		}

	}

	public class ModuleRefTable : TableBase {

		public ModuleRefTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ModuleRef";
			}
		}

		public override TableId Id {
			get {
				return TableId.ModuleRef;
			}
		}

		public override int RowLogicalSize {
			get {
				return ModuleRefRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ModuleRefRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			return new ModuleRefRow(this, rowIndex, buff);
		}

	}

	public class TypeSpecTable : TableBase {

		public TypeSpecTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "TypeSpec";
			}
		}

		public override TableId Id {
			get {
				return TableId.TypeSpec;
			}
		}

		public override int RowLogicalSize {
			get {
				return TypeSpecRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return TypeSpecRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Signature, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new TypeSpecRow(this, rowIndex, buff);
		}

	}

	public class ImplMapTable : TableBase {

		public ImplMapTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ImplMap";
			}
		}

		public override TableId Id {
			get {
				return TableId.ImplMap;
			}
		}

		public override int RowLogicalSize {
			get {
				return ImplMapRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ImplMapRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// MappingFlags, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// MemberForwarded, coded-index(MemberForwarded)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.MemberForwarded));
			offset += 4;
		
			// ImportName, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// ImportScope, index(ModuleRef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.ModuleRef));
			offset += 4;
		
			return new ImplMapRow(this, rowIndex, buff);
		}

	}

	public class FieldRVATable : TableBase {

		public FieldRVATable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "FieldRVA";
			}
		}

		public override TableId Id {
			get {
				return TableId.FieldRVA;
			}
		}

		public override int RowLogicalSize {
			get {
				return FieldRVARow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return FieldRVARow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// RVA, RVA
			stream.Read(buff, offset, RVA.Size);
			offset += RVA.Size;
		
			// Field, index(Field)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Field));
			offset += 4;
		
			return new FieldRVARow(this, rowIndex, buff);
		}

	}

	public class ENCLogTable : TableBase {

		public ENCLogTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ENCLog";
			}
		}

		public override TableId Id {
			get {
				return TableId.ENCLog;
			}
		}

		public override int RowLogicalSize {
			get {
				return ENCLogRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ENCLogRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Token, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// FuncCode, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			return new ENCLogRow(this, rowIndex, buff);
		}

	}

	public class ENCMapTable : TableBase {

		public ENCMapTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ENCMap";
			}
		}

		public override TableId Id {
			get {
				return TableId.ENCMap;
			}
		}

		public override int RowLogicalSize {
			get {
				return ENCMapRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ENCMapRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Token, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			return new ENCMapRow(this, rowIndex, buff);
		}

	}

	public class AssemblyTable : TableBase {

		public AssemblyTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "Assembly";
			}
		}

		public override TableId Id {
			get {
				return TableId.Assembly;
			}
		}

		public override int RowLogicalSize {
			get {
				return AssemblyRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return AssemblyRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// HashAlgId, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// MajorVersion, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// MinorVersion, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// BuildNumber, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// RevisionNumber, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Flags, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// PublicKey, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Culture, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			return new AssemblyRow(this, rowIndex, buff);
		}

	}

	public class AssemblyProcessorTable : TableBase {

		public AssemblyProcessorTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "AssemblyProcessor";
			}
		}

		public override TableId Id {
			get {
				return TableId.AssemblyProcessor;
			}
		}

		public override int RowLogicalSize {
			get {
				return AssemblyProcessorRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return AssemblyProcessorRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Processor, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			return new AssemblyProcessorRow(this, rowIndex, buff);
		}

	}

	public class AssemblyOSTable : TableBase {

		public AssemblyOSTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "AssemblyOS";
			}
		}

		public override TableId Id {
			get {
				return TableId.AssemblyOS;
			}
		}

		public override int RowLogicalSize {
			get {
				return AssemblyOSRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return AssemblyOSRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// OSPlatformID, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// OSMajorVersion, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// OSMinorVersion, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			return new AssemblyOSRow(this, rowIndex, buff);
		}

	}

	public class AssemblyRefTable : TableBase {

		public AssemblyRefTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "AssemblyRef";
			}
		}

		public override TableId Id {
			get {
				return TableId.AssemblyRef;
			}
		}

		public override int RowLogicalSize {
			get {
				return AssemblyRefRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return AssemblyRefRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// MajorVersion, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// MinorVersion, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// BuildNumber, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// RevisionNumber, short
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Flags, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// PublicKeyOrToken, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Culture, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// HashValue, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new AssemblyRefRow(this, rowIndex, buff);
		}

	}

	public class AssemblyRefProcessorTable : TableBase {

		public AssemblyRefProcessorTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "AssemblyRefProcessor";
			}
		}

		public override TableId Id {
			get {
				return TableId.AssemblyRefProcessor;
			}
		}

		public override int RowLogicalSize {
			get {
				return AssemblyRefProcessorRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return AssemblyRefProcessorRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Processor, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// AssemblyRef, index(AssemblyRef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.AssemblyRef));
			offset += 4;
		
			return new AssemblyRefProcessorRow(this, rowIndex, buff);
		}

	}

	public class AssemblyRefOSTable : TableBase {

		public AssemblyRefOSTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "AssemblyRefOS";
			}
		}

		public override TableId Id {
			get {
				return TableId.AssemblyRefOS;
			}
		}

		public override int RowLogicalSize {
			get {
				return AssemblyRefOSRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return AssemblyRefOSRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// OSPlatformID, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// OSMajorVersion, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// OSMinorVersion, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// AssemblyRef, index(AssemblyRef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.AssemblyRef));
			offset += 4;
		
			return new AssemblyRefOSRow(this, rowIndex, buff);
		}

	}

	public class FileTable : TableBase {

		public FileTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "File";
			}
		}

		public override TableId Id {
			get {
				return TableId.File;
			}
		}

		public override int RowLogicalSize {
			get {
				return FileRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return FileRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Flags, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// HashValue, index(#Blob)
			stream.Read(buff, offset, Heap.BlobIndexSize);
			offset += 4;
		
			return new FileRow(this, rowIndex, buff);
		}

	}

	public class ExportedTypeTable : TableBase {

		public ExportedTypeTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ExportedType";
			}
		}

		public override TableId Id {
			get {
				return TableId.ExportedType;
			}
		}

		public override int RowLogicalSize {
			get {
				return ExportedTypeRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ExportedTypeRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Flags, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// TypeDefId, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// TypeName, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// TypeNamespace, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Implementation, coded-index(Implementation)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.Implementation));
			offset += 4;
		
			return new ExportedTypeRow(this, rowIndex, buff);
		}

	}

	public class ManifestResourceTable : TableBase {

		public ManifestResourceTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "ManifestResource";
			}
		}

		public override TableId Id {
			get {
				return TableId.ManifestResource;
			}
		}

		public override int RowLogicalSize {
			get {
				return ManifestResourceRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return ManifestResourceRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Offset, int
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// Flags, uint
			stream.Read(buff, offset, 4);
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			// Implementation, coded-index(Implementation)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.Implementation));
			offset += 4;
		
			return new ManifestResourceRow(this, rowIndex, buff);
		}

	}

	public class NestedClassTable : TableBase {

		public NestedClassTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "NestedClass";
			}
		}

		public override TableId Id {
			get {
				return TableId.NestedClass;
			}
		}

		public override int RowLogicalSize {
			get {
				return NestedClassRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return NestedClassRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// NestedClass, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// EnclosingClass, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			return new NestedClassRow(this, rowIndex, buff);
		}

	}

	public class TypeTyParTable : TableBase {

		public TypeTyParTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "TypeTyPar";
			}
		}

		public override TableId Id {
			get {
				return TableId.TypeTyPar;
			}
		}

		public override int RowLogicalSize {
			get {
				return TypeTyParRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return TypeTyParRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Number, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Class, index(TypeDef)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.TypeDef));
			offset += 4;
		
			// Bound, coded-index(TypeDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef));
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			return new TypeTyParRow(this, rowIndex, buff);
		}

	}

	public class MethodTyParTable : TableBase {

		public MethodTyParTable(TablesHeap heap, long offset) : base(heap,offset) {
		}

		public override string Name {
			get {
				return "MethodTyPar";
			}
		}

		public override TableId Id {
			get {
				return TableId.MethodTyPar;
			}
		}

		public override int RowLogicalSize {
			get {
				return MethodTyParRow.LogicalSize;
			}
		}

		public override int RowCodedSize {
			get {
				return MethodTyParRow.EstimateCodedSize(Heap);
			}
		}

		public override Row ReadRow(int rowIndex, Stream stream) {
			byte[] buff = new byte[RowLogicalSize];
			int offset = 0;
		
			// Number, ushort
			stream.Read(buff, offset, 2);
			offset += 2;
		
			// Method, index(Method)
			stream.Read(buff, offset, Heap.GetIndexSize(TableId.Method));
			offset += 4;
		
			// Bound, coded-index(TypeDefOrRef)
			stream.Read(buff, offset, Heap.GetCodedIndexSize(CodedTokenId.TypeDefOrRef));
			offset += 4;
		
			// Name, index(#Strings)
			stream.Read(buff, offset, Heap.StringsIndexSize);
			offset += 4;
		
			return new MethodTyParRow(this, rowIndex, buff);
		}

	}


}

