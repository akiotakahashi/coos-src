// Auto-generated file - DO NOT EDIT!
// Please edit md-schema.xml or tabs-decoder.xsl if you want to make changes.

using System;

namespace CooS.CodeModels.CLI.Metadata {


	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public sealed class TabsDecoder {

		private TabsDecoder() {
		}

		public static Table CreateTable(TableId id, TablesHeap heap, long offset) {
			switch(id) {

			case TableId.Module:
				return new ModuleTable(heap,offset);
			
			case TableId.TypeRef:
				return new TypeRefTable(heap,offset);
			
			case TableId.TypeDef:
				return new TypeDefTable(heap,offset);
			
			case TableId.FieldPtr:
				return new FieldPtrTable(heap,offset);
			
			case TableId.Field:
				return new FieldTable(heap,offset);
			
			case TableId.MethodPtr:
				return new MethodPtrTable(heap,offset);
			
			case TableId.Method:
				return new MethodTable(heap,offset);
			
			case TableId.ParamPtr:
				return new ParamPtrTable(heap,offset);
			
			case TableId.Param:
				return new ParamTable(heap,offset);
			
			case TableId.InterfaceImpl:
				return new InterfaceImplTable(heap,offset);
			
			case TableId.MemberRef:
				return new MemberRefTable(heap,offset);
			
			case TableId.Constant:
				return new ConstantTable(heap,offset);
			
			case TableId.CustomAttribute:
				return new CustomAttributeTable(heap,offset);
			
			case TableId.FieldMarshal:
				return new FieldMarshalTable(heap,offset);
			
			case TableId.DeclSecurity:
				return new DeclSecurityTable(heap,offset);
			
			case TableId.ClassLayout:
				return new ClassLayoutTable(heap,offset);
			
			case TableId.FieldLayout:
				return new FieldLayoutTable(heap,offset);
			
			case TableId.StandAloneSig:
				return new StandAloneSigTable(heap,offset);
			
			case TableId.EventMap:
				return new EventMapTable(heap,offset);
			
			case TableId.EventPtr:
				return new EventPtrTable(heap,offset);
			
			case TableId.Event:
				return new EventTable(heap,offset);
			
			case TableId.PropertyMap:
				return new PropertyMapTable(heap,offset);
			
			case TableId.PropertyPtr:
				return new PropertyPtrTable(heap,offset);
			
			case TableId.Property:
				return new PropertyTable(heap,offset);
			
			case TableId.MethodSemantics:
				return new MethodSemanticsTable(heap,offset);
			
			case TableId.MethodImpl:
				return new MethodImplTable(heap,offset);
			
			case TableId.ModuleRef:
				return new ModuleRefTable(heap,offset);
			
			case TableId.TypeSpec:
				return new TypeSpecTable(heap,offset);
			
			case TableId.ImplMap:
				return new ImplMapTable(heap,offset);
			
			case TableId.FieldRVA:
				return new FieldRVATable(heap,offset);
			
			case TableId.ENCLog:
				return new ENCLogTable(heap,offset);
			
			case TableId.ENCMap:
				return new ENCMapTable(heap,offset);
			
			case TableId.Assembly:
				return new AssemblyTable(heap,offset);
			
			case TableId.AssemblyProcessor:
				return new AssemblyProcessorTable(heap,offset);
			
			case TableId.AssemblyOS:
				return new AssemblyOSTable(heap,offset);
			
			case TableId.AssemblyRef:
				return new AssemblyRefTable(heap,offset);
			
			case TableId.AssemblyRefProcessor:
				return new AssemblyRefProcessorTable(heap,offset);
			
			case TableId.AssemblyRefOS:
				return new AssemblyRefOSTable(heap,offset);
			
			case TableId.File:
				return new FileTable(heap,offset);
			
			case TableId.ExportedType:
				return new ExportedTypeTable(heap,offset);
			
			case TableId.ManifestResource:
				return new ManifestResourceTable(heap,offset);
			
			case TableId.NestedClass:
				return new NestedClassTable(heap,offset);
			
			case TableId.TypeTyPar:
				return new TypeTyParTable(heap,offset);
			
			case TableId.MethodTyPar:
				return new MethodTyParTable(heap,offset);
			
			default:
				throw new ArgumentException();
			}
		}

		/// <summary>
		/// </summary>
		/// <remarks>
		/// </remarks>
		public static MDToken DecodeToken(CodedTokenId id, int data) {
			MDToken res = new MDToken();
			int tag;
			int rid;
			TokenType tok;

			switch (id) {

			case CodedTokenId.TypeDefOrRef :
				tag = data & 0x03;
				rid = (int) ((uint) data >> 2);
				switch (tag) {

				case 0 :
					tok = TokenType.TypeDef;
					break;

				case 1 :
					tok = TokenType.TypeRef;
					break;

				case 2 :
					tok = TokenType.TypeSpec;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for TypeDefOrRef, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.HasConstant :
				tag = data & 0x03;
				rid = (int) ((uint) data >> 2);
				switch (tag) {

				case 0 :
					tok = TokenType.FieldDef;
					break;

				case 1 :
					tok = TokenType.ParamDef;
					break;

				case 2 :
					tok = TokenType.Property;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for HasConstant, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.HasCustomAttribute :
				tag = data & 0x1F;
				rid = (int) ((uint) data >> 5);
				switch (tag) {

				case 0 :
					tok = TokenType.MethodDef;
					break;

				case 1 :
					tok = TokenType.FieldDef;
					break;

				case 2 :
					tok = TokenType.TypeRef;
					break;

				case 3 :
					tok = TokenType.TypeDef;
					break;

				case 4 :
					tok = TokenType.ParamDef;
					break;

				case 5 :
					tok = TokenType.InterfaceImpl;
					break;

				case 6 :
					tok = TokenType.MemberRef;
					break;

				case 7 :
					tok = TokenType.Module;
					break;

				case 8 :
					tok = TokenType.Permission;
					break;

				case 9 :
					tok = TokenType.Property;
					break;

				case 10 :
					tok = TokenType.Event;
					break;

				case 11 :
					tok = TokenType.Signature;
					break;

				case 12 :
					tok = TokenType.ModuleRef;
					break;

				case 13 :
					tok = TokenType.TypeSpec;
					break;

				case 14 :
					tok = TokenType.Assembly;
					break;

				case 15 :
					tok = TokenType.AssemblyRef;
					break;

				case 16 :
					tok = TokenType.File;
					break;

				case 17 :
					tok = TokenType.ExportedType;
					break;

				case 18 :
					tok = TokenType.ManifestResource;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for HasCustomAttribute, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.HasFieldMarshal :
				tag = data & 0x01;
				rid = (int) ((uint) data >> 1);
				switch (tag) {

				case 0 :
					tok = TokenType.FieldDef;
					break;

				case 1 :
					tok = TokenType.ParamDef;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for HasFieldMarshal, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.HasDeclSecurity :
				tag = data & 0x03;
				rid = (int) ((uint) data >> 2);
				switch (tag) {

				case 0 :
					tok = TokenType.TypeDef;
					break;

				case 1 :
					tok = TokenType.MethodDef;
					break;

				case 2 :
					tok = TokenType.Assembly;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for HasDeclSecurity, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.MemberRefParent :
				tag = data & 0x07;
				rid = (int) ((uint) data >> 3);
				switch (tag) {

				case 0 :
					tok = TokenType.TypeDef;
					break;

				case 1 :
					tok = TokenType.TypeRef;
					break;

				case 2 :
					tok = TokenType.ModuleRef;
					break;

				case 3 :
					tok = TokenType.MethodDef;
					break;

				case 4 :
					tok = TokenType.TypeSpec;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for MemberRefParent, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.HasSemantics :
				tag = data & 0x01;
				rid = (int) ((uint) data >> 1);
				switch (tag) {

				case 0 :
					tok = TokenType.Event;
					break;

				case 1 :
					tok = TokenType.Property;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for HasSemantics, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.MethodDefOrRef :
				tag = data & 0x01;
				rid = (int) ((uint) data >> 1);
				switch (tag) {

				case 0 :
					tok = TokenType.MethodDef;
					break;

				case 1 :
					tok = TokenType.MemberRef;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for MethodDefOrRef, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.MemberForwarded :
				tag = data & 0x01;
				rid = (int) ((uint) data >> 1);
				switch (tag) {

				case 0 :
					tok = TokenType.FieldDef;
					break;

				case 1 :
					tok = TokenType.MethodDef;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for MemberForwarded, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.Implementation :
				tag = data & 0x03;
				rid = (int) ((uint) data >> 2);
				switch (tag) {

				case 0 :
					tok = TokenType.File;
					break;

				case 1 :
					tok = TokenType.AssemblyRef;
					break;

				case 2 :
					tok = TokenType.ExportedType;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for Implementation, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.CustomAttributeType :
				tag = data & 0x07;
				rid = (int) ((uint) data >> 3);
				switch (tag) {

				case 0 :
					tok = TokenType.TypeRef;
					break;

				case 1 :
					tok = TokenType.TypeDef;
					break;

				case 2 :
					tok = TokenType.MethodDef;
					break;

				case 3 :
					tok = TokenType.MemberRef;
					break;

				case 4 :
					tok = TokenType.String;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for CustomAttributeType, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			case CodedTokenId.ResolutionScope :
				tag = data & 0x03;
				rid = (int) ((uint) data >> 2);
				switch (tag) {

				case 0 :
					tok = TokenType.Module;
					break;

				case 1 :
					tok = TokenType.ModuleRef;
					break;

				case 2 :
					tok = TokenType.AssemblyRef;
					break;

				case 3 :
					tok = TokenType.TypeRef;
					break;

				default :
					throw new BadMetadataException("Invalid coded token for ResolutionScope, unknown table tag - " + tag);
				}
				res = new MDToken(tok, rid);
				break;

			default:
				break;
			}
			return res;
		}

		private static int GetBitCount(int number) {
			if(number<=2) return 1;
			if(number<=4) return 2;
			if(number<=8) return 3;
			if(number<=16) return 4;
			if(number<=32) return 5;
			if(number<=64) return 6;
			throw new NotImplementedException();
		}

		private static int GetComposedSize(params int[] sizes) {
			int res = MDUtils.Max(sizes);
			return res < (1 << (16-GetBitCount(sizes.Length))) ? 2 : 4;
		}

		public static int GetCodedIndexSize(CodedTokenId id, int [] rows, int sizeOfStringsIndex) {
			switch (id) {

			case CodedTokenId.TypeDefOrRef :
				return GetComposedSize(rows [(int) TableId.TypeDef], rows [(int) TableId.TypeRef], rows [(int) TableId.TypeSpec]);

			case CodedTokenId.HasConstant :
				return GetComposedSize(rows [(int) TableId.Field], rows [(int) TableId.Param], rows [(int) TableId.Property]);

			case CodedTokenId.HasCustomAttribute :
				return GetComposedSize(rows [(int) TableId.Method], rows [(int) TableId.Field], rows [(int) TableId.TypeRef], rows [(int) TableId.TypeDef], rows [(int) TableId.Param], rows [(int) TableId.InterfaceImpl], rows [(int) TableId.MemberRef], rows [(int) TableId.Module], rows [(int) TableId.DeclSecurity], rows [(int) TableId.Property], rows [(int) TableId.Event], rows [(int) TableId.StandAloneSig], rows [(int) TableId.ModuleRef], rows [(int) TableId.TypeSpec], rows [(int) TableId.Assembly], rows [(int) TableId.AssemblyRef], rows [(int) TableId.File], rows [(int) TableId.ExportedType], rows [(int) TableId.ManifestResource]);

			case CodedTokenId.HasFieldMarshal :
				return GetComposedSize(rows [(int) TableId.Field], rows [(int) TableId.Param]);

			case CodedTokenId.HasDeclSecurity :
				return GetComposedSize(rows [(int) TableId.TypeDef], rows [(int) TableId.Method], rows [(int) TableId.Assembly]);

			case CodedTokenId.MemberRefParent :
				return GetComposedSize(rows [(int) TableId.TypeDef], rows [(int) TableId.TypeRef], rows [(int) TableId.ModuleRef], rows [(int) TableId.Method], rows [(int) TableId.TypeSpec]);

			case CodedTokenId.HasSemantics :
				return GetComposedSize(rows [(int) TableId.Event], rows [(int) TableId.Property]);

			case CodedTokenId.MethodDefOrRef :
				return GetComposedSize(rows [(int) TableId.Method], rows [(int) TableId.MemberRef]);

			case CodedTokenId.MemberForwarded :
				return GetComposedSize(rows [(int) TableId.Field], rows [(int) TableId.Method]);

			case CodedTokenId.Implementation :
				return GetComposedSize(rows [(int) TableId.File], rows [(int) TableId.AssemblyRef], rows [(int) TableId.ExportedType]);

			case CodedTokenId.CustomAttributeType :
				int total = 0;
				for(int i=0; i<rows.Length; ++i) {
					total += rows[i];
				}
				total -= rows[(int)TableId.CustomAttribute];
				return GetComposedSize(total);

			case CodedTokenId.ResolutionScope :
				return GetComposedSize(rows [(int) TableId.Module], rows [(int) TableId.ModuleRef], rows [(int) TableId.AssemblyRef], rows [(int) TableId.TypeRef]);

			default:
				throw new ArgumentException();
			}
		}

		/*
		/// <summary>
		/// </summary>
		unsafe public static int DecodePhysicalTable(Table table, byte [] data, int offs, int [] rows)
		{
			int rowSize; // expanded row size (all indices are dwords)
			int fldSize; // physical field size
			int dest;
			int nRows;
			byte [] buff = new byte[table.LogicalSize];
			int si = table.Root.StringsIndexSize;
			int gi = table.Root.GUIDIndexSize;
			int bi = table.Root.BlobIndexSize;

			if (heap.HasModule) {
				rowSize = 2 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.Module];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ModuleTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Generation, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Mvid, index(#GUID)
					fldSize = gi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EncId, index(#GUID)
					fldSize = gi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EncBaseId, index(#GUID)
					fldSize = gi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeRef) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.TypeRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// ResolutionScope, coded-index(ResolutionScope)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.ResolutionScope, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Namespace, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeDef) {
				rowSize = 4 + 4 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.TypeDef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeDefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Namespace, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Extends, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// FieldList, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MethodList, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.FieldPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Field, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasField) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Field];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.MethodPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Method, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethod) {
				rowSize = RVA.Size + 2 + 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.Method];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// RVA, RVA
					fldSize = RVA.Size;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += RVA.Size;
	
					// ImplFlags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// ParamList, index(Param)
					fldSize = GetIndexSize(TableId.Param, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasParamPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.ParamPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ParamPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Param, index(Param)
					fldSize = GetIndexSize(TableId.Param, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasParam) {
				rowSize = 2 + 2 + 4;
				nRows = rows [(int) TableId.Param];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ParamTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Sequence, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasInterfaceImpl) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.InterfaceImpl];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new InterfaceImplTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Class, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Interface, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMemberRef) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.MemberRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MemberRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Class, coded-index(MemberRefParent)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MemberRefParent, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasConstant) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Constant];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ConstantTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Type, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Parent, coded-index(HasConstant)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasConstant, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Value, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasCustomAttribute) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.CustomAttribute];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new CustomAttributeTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, coded-index(HasCustomAttribute)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasCustomAttribute, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Type, coded-index(CustomAttributeType)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.CustomAttributeType, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Value, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldMarshal) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.FieldMarshal];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldMarshalTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, coded-index(HasFieldMarshal)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasFieldMarshal, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// NativeType, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasDeclSecurity) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.DeclSecurity];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new DeclSecurityTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Action, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Parent, coded-index(HasDeclSecurity)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasDeclSecurity, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PermissionSet, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasClassLayout) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.ClassLayout];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ClassLayoutTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// PackingSize, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// ClassSize, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Parent, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldLayout) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.FieldLayout];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldLayoutTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Offset, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Field, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasStandAloneSig) {
				rowSize = 4;
				nRows = rows [(int) TableId.StandAloneSig];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new StandAloneSigTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasEventMap) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.EventMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new EventMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EventList, index(Event)
					fldSize = GetIndexSize(TableId.Event, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasEventPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.EventPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new EventPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Event, index(Event)
					fldSize = GetIndexSize(TableId.Event, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasEvent) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Event];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new EventTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// EventFlags, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EventType, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasPropertyMap) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.PropertyMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new PropertyMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PropertyList, index(Property)
					fldSize = GetIndexSize(TableId.Property, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasPropertyPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.PropertyPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new PropertyPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Property, index(Property)
					fldSize = GetIndexSize(TableId.Property, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasProperty) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Property];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new PropertyTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Type, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodSemantics) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.MethodSemantics];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodSemanticsTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Semantics, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Method, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Association, coded-index(HasSemantics)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasSemantics, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodImpl) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.MethodImpl];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodImplTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Class, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MethodBody, coded-index(MethodDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MethodDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MethodDeclaration, coded-index(MethodDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MethodDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasModuleRef) {
				rowSize = 4;
				nRows = rows [(int) TableId.ModuleRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ModuleRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeSpec) {
				rowSize = 4;
				nRows = rows [(int) TableId.TypeSpec];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeSpecTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasImplMap) {
				rowSize = 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.ImplMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ImplMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// MappingFlags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// MemberForwarded, coded-index(MemberForwarded)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MemberForwarded, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// ImportName, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// ImportScope, index(ModuleRef)
					fldSize = GetIndexSize(TableId.ModuleRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldRVA) {
				rowSize = RVA.Size + 4;
				nRows = rows [(int) TableId.FieldRVA];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldRVATable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// RVA, RVA
					fldSize = RVA.Size;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += RVA.Size;
	
					// Field, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasENCLog) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.ENCLog];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ENCLogTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Token, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// FuncCode, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasENCMap) {
				rowSize = 4;
				nRows = rows [(int) TableId.ENCMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ENCMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Token, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssembly) {
				rowSize = 4 + 2 + 2 + 2 + 2 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.Assembly];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// HashAlgId, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MajorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// MinorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// BuildNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// RevisionNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PublicKey, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Culture, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyProcessor) {
				rowSize = 4;
				nRows = rows [(int) TableId.AssemblyProcessor];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyProcessorTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Processor, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyOS) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.AssemblyOS];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyOSTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// OSPlatformID, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMajorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMinorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyRef) {
				rowSize = 2 + 2 + 2 + 2 + 4 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.AssemblyRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// MajorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// MinorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// BuildNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// RevisionNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PublicKeyOrToken, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Culture, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// HashValue, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyRefProcessor) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.AssemblyRefProcessor];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyRefProcessorTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Processor, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// AssemblyRef, index(AssemblyRef)
					fldSize = GetIndexSize(TableId.AssemblyRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyRefOS) {
				rowSize = 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.AssemblyRefOS];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyRefOSTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// OSPlatformID, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMajorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMinorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// AssemblyRef, index(AssemblyRef)
					fldSize = GetIndexSize(TableId.AssemblyRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFile) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.File];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FileTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// HashValue, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasExportedType) {
				rowSize = 4 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.ExportedType];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ExportedTypeTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// TypeDefId, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// TypeName, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// TypeNamespace, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Implementation, coded-index(Implementation)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.Implementation, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasManifestResource) {
				rowSize = 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.ManifestResource];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ManifestResourceTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Offset, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Implementation, coded-index(Implementation)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.Implementation, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasNestedClass) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.NestedClass];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new NestedClassTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// NestedClass, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EnclosingClass, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeTyPar) {
				rowSize = 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.TypeTyPar];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeTyParTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Number, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Class, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Bound, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodTyPar) {
				rowSize = 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.MethodTyPar];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodTyParTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Number, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Method, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Bound, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			return offs;
		}
		*/


		/*
		/// <summary>
		/// </summary>
		unsafe public static int DecodePhysicalTables(TablesHeap heap, byte [] data, int offs, int [] rows)
		{
			int rowSize; // expanded row size (all indices are dwords)
			int fldSize; // physical field size
			int dest;
			int nRows;
			byte [] buff = null;
			int si = heap.StringsIndexSize;
			int gi = heap.GUIDIndexSize;
			int bi = heap.BlobIndexSize;

			if (heap.HasModule) {
				rowSize = 2 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.Module];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ModuleTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Generation, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Mvid, index(#GUID)
					fldSize = gi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EncId, index(#GUID)
					fldSize = gi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EncBaseId, index(#GUID)
					fldSize = gi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeRef) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.TypeRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// ResolutionScope, coded-index(ResolutionScope)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.ResolutionScope, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Namespace, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeDef) {
				rowSize = 4 + 4 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.TypeDef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeDefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Namespace, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Extends, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// FieldList, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MethodList, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.FieldPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Field, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasField) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Field];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.MethodPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Method, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethod) {
				rowSize = RVA.Size + 2 + 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.Method];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// RVA, RVA
					fldSize = RVA.Size;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += RVA.Size;
	
					// ImplFlags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// ParamList, index(Param)
					fldSize = GetIndexSize(TableId.Param, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasParamPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.ParamPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ParamPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Param, index(Param)
					fldSize = GetIndexSize(TableId.Param, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasParam) {
				rowSize = 2 + 2 + 4;
				nRows = rows [(int) TableId.Param];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ParamTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Sequence, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasInterfaceImpl) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.InterfaceImpl];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new InterfaceImplTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Class, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Interface, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMemberRef) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.MemberRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MemberRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Class, coded-index(MemberRefParent)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MemberRefParent, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasConstant) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Constant];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ConstantTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Type, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Parent, coded-index(HasConstant)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasConstant, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Value, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasCustomAttribute) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.CustomAttribute];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new CustomAttributeTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, coded-index(HasCustomAttribute)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasCustomAttribute, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Type, coded-index(CustomAttributeType)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.CustomAttributeType, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Value, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldMarshal) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.FieldMarshal];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldMarshalTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, coded-index(HasFieldMarshal)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasFieldMarshal, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// NativeType, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasDeclSecurity) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.DeclSecurity];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new DeclSecurityTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Action, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Parent, coded-index(HasDeclSecurity)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasDeclSecurity, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PermissionSet, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasClassLayout) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.ClassLayout];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ClassLayoutTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// PackingSize, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// ClassSize, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Parent, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldLayout) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.FieldLayout];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldLayoutTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Offset, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Field, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasStandAloneSig) {
				rowSize = 4;
				nRows = rows [(int) TableId.StandAloneSig];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new StandAloneSigTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasEventMap) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.EventMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new EventMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EventList, index(Event)
					fldSize = GetIndexSize(TableId.Event, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasEventPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.EventPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new EventPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Event, index(Event)
					fldSize = GetIndexSize(TableId.Event, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasEvent) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Event];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new EventTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// EventFlags, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EventType, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasPropertyMap) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.PropertyMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new PropertyMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Parent, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PropertyList, index(Property)
					fldSize = GetIndexSize(TableId.Property, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasPropertyPtr) {
				rowSize = 4;
				nRows = rows [(int) TableId.PropertyPtr];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new PropertyPtrTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Property, index(Property)
					fldSize = GetIndexSize(TableId.Property, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasProperty) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.Property];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new PropertyTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Type, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodSemantics) {
				rowSize = 2 + 4 + 4;
				nRows = rows [(int) TableId.MethodSemantics];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodSemanticsTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Semantics, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Method, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Association, coded-index(HasSemantics)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.HasSemantics, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodImpl) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.MethodImpl];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodImplTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Class, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MethodBody, coded-index(MethodDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MethodDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MethodDeclaration, coded-index(MethodDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MethodDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasModuleRef) {
				rowSize = 4;
				nRows = rows [(int) TableId.ModuleRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ModuleRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeSpec) {
				rowSize = 4;
				nRows = rows [(int) TableId.TypeSpec];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeSpecTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Signature, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasImplMap) {
				rowSize = 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.ImplMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ImplMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// MappingFlags, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// MemberForwarded, coded-index(MemberForwarded)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.MemberForwarded, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// ImportName, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// ImportScope, index(ModuleRef)
					fldSize = GetIndexSize(TableId.ModuleRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFieldRVA) {
				rowSize = RVA.Size + 4;
				nRows = rows [(int) TableId.FieldRVA];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FieldRVATable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// RVA, RVA
					fldSize = RVA.Size;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += RVA.Size;
	
					// Field, index(Field)
					fldSize = GetIndexSize(TableId.Field, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasENCLog) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.ENCLog];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ENCLogTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Token, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// FuncCode, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasENCMap) {
				rowSize = 4;
				nRows = rows [(int) TableId.ENCMap];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ENCMapTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Token, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssembly) {
				rowSize = 4 + 2 + 2 + 2 + 2 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.Assembly];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// HashAlgId, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// MajorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// MinorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// BuildNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// RevisionNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PublicKey, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Culture, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyProcessor) {
				rowSize = 4;
				nRows = rows [(int) TableId.AssemblyProcessor];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyProcessorTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Processor, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyOS) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.AssemblyOS];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyOSTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// OSPlatformID, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMajorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMinorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyRef) {
				rowSize = 2 + 2 + 2 + 2 + 4 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.AssemblyRef];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyRefTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// MajorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// MinorVersion, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// BuildNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// RevisionNumber, short
					fldSize = sizeof (short);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// PublicKeyOrToken, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Culture, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// HashValue, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyRefProcessor) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.AssemblyRefProcessor];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyRefProcessorTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Processor, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// AssemblyRef, index(AssemblyRef)
					fldSize = GetIndexSize(TableId.AssemblyRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasAssemblyRefOS) {
				rowSize = 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.AssemblyRefOS];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new AssemblyRefOSTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// OSPlatformID, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMajorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// OSMinorVersion, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// AssemblyRef, index(AssemblyRef)
					fldSize = GetIndexSize(TableId.AssemblyRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasFile) {
				rowSize = 4 + 4 + 4;
				nRows = rows [(int) TableId.File];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new FileTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// HashValue, index(#Blob)
					fldSize = bi;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasExportedType) {
				rowSize = 4 + 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.ExportedType];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ExportedTypeTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// TypeDefId, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// TypeName, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// TypeNamespace, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Implementation, coded-index(Implementation)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.Implementation, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasManifestResource) {
				rowSize = 4 + 4 + 4 + 4;
				nRows = rows [(int) TableId.ManifestResource];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new ManifestResourceTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Offset, int
					fldSize = sizeof (int);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Flags, uint
					fldSize = sizeof (uint);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Implementation, coded-index(Implementation)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.Implementation, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasNestedClass) {
				rowSize = 4 + 4;
				nRows = rows [(int) TableId.NestedClass];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new NestedClassTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// NestedClass, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// EnclosingClass, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasTypeTyPar) {
				rowSize = 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.TypeTyPar];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new TypeTyParTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Number, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Class, index(TypeDef)
					fldSize = GetIndexSize(TableId.TypeDef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Bound, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			if (heap.HasMethodTyPar) {
				rowSize = 2 + 4 + 4 + 4;
				nRows = rows [(int) TableId.MethodTyPar];
				AllocBuff(ref buff, rowSize * nRows);
				dest = 0;

				MDTable tab = new MethodTyParTable(heap);

				for (int i = nRows; --i >= 0;) {
	
					// Number, ushort
					fldSize = sizeof (ushort);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 2;
	
					// Method, index(Method)
					fldSize = GetIndexSize(TableId.Method, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Bound, coded-index(TypeDefOrRef)
					fldSize = GetCodedIndexSize(heap, CodedTokenId.TypeDefOrRef, rows);
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
					// Name, index(#Strings)
					fldSize = si;
					Array.Copy(data, offs, buff, dest, fldSize);
					offs += fldSize;
					dest += 4;
	
				}

				tab.FromRawData(buff, 0, nRows);
			}

			return offs;
		}
		*/

	} // end class
} // end namespace

