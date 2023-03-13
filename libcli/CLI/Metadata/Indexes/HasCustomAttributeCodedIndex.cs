using System;
using CooS.Formats.CLI.Metadata.Heaps;

namespace CooS.Formats.CLI.Metadata.Indexes {

	public struct HasCustomAttributeCodedIndex {

		public static TableId[] Mapping = new TableId[] {
			TableId.MethodDef,
			TableId.Field,
			TableId.TypeRef,
			TableId.TypeDef,
			TableId.Param,
			TableId.InterfaceImpl,
			TableId.MemberRef,
			TableId.Module,
			TableId.DeclSecurity,
			TableId.Property,
			TableId.Event,
			TableId.StandAloneSig,
			TableId.ModuleRef,
			TableId.TypeSpec,
			TableId.Assembly,
			TableId.AssemblyRef,
			TableId.File,
			TableId.ExportedType,
			TableId.ManifestResource,
		};

		public const int TagBits = 5;
		public const uint TagMask = (1<<TagBits)-1;

		private readonly int Value;

		internal HasCustomAttributeCodedIndex(int value) {
			this.Value = value;
		}

		public TableId TableId {
			get {
				return Mapping[this.Value&TagMask];
			}
		}

		public RowIndex RowIndex {
			get {
				return new RowIndex(this.Value >> TagBits);
			}
		}

		public static int GetPhysicalSize(TablesHeap heap) {
			return heap.GetCodedIndexPhysicalSize(TableId.MethodDef, TableId.Field, TableId.TypeRef, TableId.TypeDef, TableId.Param, TableId.InterfaceImpl, TableId.MemberRef, TableId.Module, TableId.DeclSecurity, TableId.Property, TableId.Event, TableId.StandAloneSig, TableId.ModuleRef, TableId.TypeSpec, TableId.Assembly, TableId.AssemblyRef, TableId.File, TableId.ExportedType, TableId.ManifestResource);
		}

		public static explicit operator HasCustomAttributeCodedIndex(CodedIndex codedIndex) {
			return new HasCustomAttributeCodedIndex(codedIndex.Value);
		}

	}

}
