using System;

namespace CooS.Formats.CLI.Metadata.Heaps {
	using Rows;
	using Indexes;

	public sealed partial class TablesHeap : Heap {

		public override int IndexSize {
			get {
				throw new NotSupportedException();
			}
		}

		public int GetPhysicalSizeOfIndex(HeapIndexes heapIndex) {
			switch(heapIndex) {
			case HeapIndexes.Blob:
				return this.BlobIndexSize;
			case HeapIndexes.Guid:
				return this.GuidIndexSize;
			case HeapIndexes.String:
				return this.StringsIndexSize;
			default:
				throw new ArgumentException();
			}
		}

		public int GetPhysicalSizeOfIndex(TableId tableId) {
			return ((uint)rows[(int)tableId]) < (1 << 16) ? 2 : 4;
		}

		public int GetPhysicalSizeOfIndex(CodedIndexes codedIndex) {
			switch(codedIndex) {
			case CodedIndexes.TypeDefOrRef:
				return TypeDefOrRefCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.HasConstant:
				return HasConstantCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.HasCustomAttribute:
				return HasCustomAttributeCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.HasFieldMarshal:
				return HasFieldMarshalCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.HasDeclSecurity:
				return HasDeclSecurityCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.MemberRefParent:
				return MemberRefParentCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.HasSemantics:
				return HasSemanticsCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.MethodDefOrRef:
				return MethodDefOrRefCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.MemberForwarded:
				return MemberForwardedCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.Implementation:
				return ImplementationCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.CustomAttributeType:
				return CustomAttributeTypeCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.ResolutionScope:
				return ResolutionScopeCodedIndex.GetPhysicalSize(this);
			case CodedIndexes.TypeOrMethodDef:
				return TypeOrMethodDefCodedIndex.GetPhysicalSize(this);
			default:
				throw new ArgumentException();
			}
		}

		internal int GetCodedIndexPhysicalSize(params TableId[] tableIds) {
			int count = 0;
			foreach(TableId tid in tableIds) {
				count = Math.Max(count, this.rows[(int)tid]);
			}
			return (count < (1<<(int)Math.Ceiling(Math.Log(count, 2)))) ? 2 : 4;
		}

	}

}
