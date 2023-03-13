using System;
using System.IO;
using System.Collections.Generic;

namespace CooS.Formats.CLI.Metadata.Heaps {
	using Rows;
	using Indexes;

	public sealed partial class TablesHeap : Heap {

		internal byte ReadByte(byte[] buf, ref int position) {
			int p = position;
			position += 1;
			return buf[p];
		}

		internal UInt16 ReadUInt16(byte[] buf, ref int position) {
			int p = position;
			position += 2;
			return BitConverter.ToUInt16(buf, p);
		}

		internal UInt32 ReadUInt32(byte[] buf, ref int position) {
			int p = position;
			position += 4;
			return BitConverter.ToUInt32(buf, p);
		}

		internal Int16 ReadEnum16(byte[] buf, ref int position) {
			return (Int16)this.ReadUInt16(buf, ref position);
		}

		internal Int32 ReadEnum32(byte[] buf, ref int position) {
			return (Int32)this.ReadUInt32(buf, ref position);
		}

		internal StringHeapIndex ReadStringHeapIndex(byte[] buf, ref int position) {
			int p = position;
			switch(this.StringsIndexSize) {
			case 2:
				position += 2;
				return new StringHeapIndex(BitConverter.ToUInt16(buf, p));
			case 4:
				position += 4;
				return new StringHeapIndex(BitConverter.ToInt32(buf, p));
			default:
				throw new InvalidOperationException();
			}
		}

		internal BlobHeapIndex ReadBlobHeapIndex(byte[] buf, ref int position) {
			int p = position;
			switch(this.BlobIndexSize) {
			case 2:
				position += 2;
				return new BlobHeapIndex(BitConverter.ToUInt16(buf, p));
			case 4:
				position += 4;
				return new BlobHeapIndex(BitConverter.ToInt32(buf, p));
			default:
				throw new InvalidOperationException();
			}
		}

		internal GuidHeapIndex ReadGuidHeapIndex(byte[] buf, ref int position) {
			int p = position;
			switch(this.GuidIndexSize) {
			case 2:
				position += 2;
				return new GuidHeapIndex(BitConverter.ToUInt16(buf, p));
			case 4:
				position += 4;
				return new GuidHeapIndex(BitConverter.ToInt32(buf, p));
			default:
				throw new InvalidOperationException();
			}
		}

		internal RowIndex ReadRowIndex(TableId tableId, byte[] buf, ref int position) {
			int count = this.GetRowCount(tableId);
			if(count<0xffff) {
				return new RowIndex((int)this.ReadUInt16(buf, ref position));
			} else {
				return new RowIndex((int)this.ReadUInt32(buf, ref position));
			}
		}

		private CodedIndex ReadCodedIndex(int size, int bits, TableId[] mapping, byte[] buf, ref int position) {
			uint value;
			switch(size) {
			case 1:
				value = this.ReadByte(buf, ref position);
				break;
			case 2:
				value = this.ReadUInt16(buf, ref position);
				break;
			case 4:
				value = this.ReadUInt32(buf, ref position);
				break;
			default:
				throw new ArgumentException();
			}
			return new CodedIndex((int)(value>>bits)|(int)mapping[value&(uint)((1<<bits)-1)<<24]);
		}

		internal CodedIndex ReadCodedIndex(CodedIndexes codedIndexes, byte[] buf, ref int position) {
			switch(codedIndexes) {
			case CodedIndexes.TypeDefOrRef:
				return this.ReadCodedIndex(TypeDefOrRefCodedIndex.GetPhysicalSize(this), TypeDefOrRefCodedIndex.TagBits, TypeDefOrRefCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.HasConstant:
				return this.ReadCodedIndex(HasConstantCodedIndex.GetPhysicalSize(this), HasConstantCodedIndex.TagBits, HasConstantCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.HasCustomAttribute:
				return this.ReadCodedIndex(HasCustomAttributeCodedIndex.GetPhysicalSize(this), HasCustomAttributeCodedIndex.TagBits, HasCustomAttributeCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.HasFieldMarshal:
				return this.ReadCodedIndex(HasFieldMarshalCodedIndex.GetPhysicalSize(this), HasFieldMarshalCodedIndex.TagBits, HasFieldMarshalCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.HasDeclSecurity:
				return this.ReadCodedIndex(HasDeclSecurityCodedIndex.GetPhysicalSize(this), HasDeclSecurityCodedIndex.TagBits, HasDeclSecurityCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.MemberRefParent:
				return this.ReadCodedIndex(MemberRefParentCodedIndex.GetPhysicalSize(this), MemberRefParentCodedIndex.TagBits, MemberRefParentCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.HasSemantics:
				return this.ReadCodedIndex(HasSemanticsCodedIndex.GetPhysicalSize(this), HasSemanticsCodedIndex.TagBits, HasSemanticsCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.MethodDefOrRef:
				return this.ReadCodedIndex(MethodDefOrRefCodedIndex.GetPhysicalSize(this), MethodDefOrRefCodedIndex.TagBits, MethodDefOrRefCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.MemberForwarded:
				return this.ReadCodedIndex(MemberForwardedCodedIndex.GetPhysicalSize(this), MemberForwardedCodedIndex.TagBits, MemberForwardedCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.Implementation:
				return this.ReadCodedIndex(ImplementationCodedIndex.GetPhysicalSize(this), ImplementationCodedIndex.TagBits, ImplementationCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.CustomAttributeType:
				return this.ReadCodedIndex(CustomAttributeTypeCodedIndex.GetPhysicalSize(this), CustomAttributeTypeCodedIndex.TagBits, CustomAttributeTypeCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.ResolutionScope:
				return this.ReadCodedIndex(ResolutionScopeCodedIndex.GetPhysicalSize(this), ResolutionScopeCodedIndex.TagBits, ResolutionScopeCodedIndex.Mapping, buf, ref position);
			case CodedIndexes.TypeOrMethodDef:
				return this.ReadCodedIndex(TypeOrMethodDefCodedIndex.GetPhysicalSize(this), TypeOrMethodDefCodedIndex.TagBits, TypeOrMethodDefCodedIndex.Mapping, buf, ref position);
			default:
				throw new InvalidOperationException();
			}
		}

	}

}
