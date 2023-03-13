using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CooS.Formats.CLI.Metadata.Heaps {
	using Rows;

	public sealed partial class TablesHeap : Heap, IEnumerable<ITable> {

		public readonly byte MajorVersion;
		public readonly byte MinorVersion;

		public readonly byte HeapSizes;
		public readonly int StringsIndexSize;
		public readonly int BlobIndexSize;
		public readonly int GuidIndexSize;

		private readonly long Valid;
		private readonly long Sorted;
		private readonly int[] rows = new int[(int)TableId.MAXVALUE+1];

		public readonly Table<AssemblyRow, AssemblyRowIndex> Assembly;
		public readonly Table<AssemblyOSRow, AssemblyOSRowIndex> AssemblyOS;
		public readonly Table<AssemblyProcessorRow, AssemblyProcessorRowIndex> AssemblyProcessor;
		public readonly Table<AssemblyRefRow, AssemblyRefRowIndex> AssemblyRef;
		public readonly Table<AssemblyRefOSRow, AssemblyRefOSRowIndex> AssemblyRefOS;
		public readonly Table<AssemblyRefProcessorRow, AssemblyRefProcessorRowIndex> AssemblyRefProcessor;
		public readonly Table<ClassLayoutRow, ClassLayoutRowIndex> ClassLayout;
		public readonly Table<ConstantRow, ConstantRowIndex> Constant;
		public readonly Table<CustomAttributeRow, CustomAttributeRowIndex> CustomAttribute;
		public readonly Table<DeclSecurityRow, DeclSecurityRowIndex> DeclSecurity;
		public readonly Table<EventRow, EventRowIndex> Event;
		public readonly Table<EventMapRow, EventMapRowIndex> EventMap;
		public readonly Table<ExportedTypeRow, ExportedTypeRowIndex> ExportedType;
		public readonly Table<FieldRow, FieldRowIndex> FieldDef;
		public readonly Table<FieldLayoutRow, FieldLayoutRowIndex> FieldLayout;
		public readonly Table<FieldMarshalRow, FieldMarshalRowIndex> FieldMarshal;
		public readonly Table<FieldRVARow, FieldRVARowIndex> FieldRVA;
		public readonly Table<FileRow, FileRowIndex> File;
		public readonly Table<GenericParamRow, GenericParamRowIndex> GenericParam;
		public readonly Table<GenericParamConstraintRow, GenericParamConstraintRowIndex> GenericParamConstraint;
		public readonly Table<ImplMapRow, ImplMapRowIndex> ImplMap;
		public readonly Table<InterfaceImplRow, InterfaceImplRowIndex> InterfaceImpl;
		public readonly Table<ManifestResourceRow, ManifestResourceRowIndex> ManifestResource;
		public readonly Table<MemberRefRow, MemberRefRowIndex> MemberRef;
		public readonly Table<MethodDefRow, MethodDefRowIndex> MethodDef;
		public readonly Table<MethodImplRow, MethodImplRowIndex> MethodImpl;
		public readonly Table<MethodSemanticsRow, MethodSemanticsRowIndex> MethodSemantics;
		public readonly Table<MethodSpecRow, MethodSpecRowIndex> MethodSpec;
		public readonly Table<ModuleRow, ModuleRowIndex> Module;
		public readonly Table<ModuleRefRow, ModuleRefRowIndex> ModuleRef;
		public readonly Table<NestedClassRow, NestedClassRowIndex> NestedClass;
		public readonly Table<ParamRow, ParamRowIndex> Param;
		public readonly Table<PropertyRow, PropertyRowIndex> Property;
		public readonly Table<PropertyMapRow, PropertyMapRowIndex> PropertyMap;
		public readonly Table<StandAloneSigRow, StandAloneSigRowIndex> StandAloneSig;
		public readonly Table<TypeDefRow, TypeDefRowIndex> TypeDef;
		public readonly Table<TypeRefRow, TypeRefRowIndex> TypeRef;
		public readonly Table<TypeSpecRow, TypeSpecRowIndex> TypeSpec;

		private readonly ITable[] tables = new ITable[(int)TableId.MAXVALUE+1];

		private void RegisterTable(ITable table) {
			this.tables[(int)table.TableId] = table;
		}

		internal TablesHeap(MetadataRoot mdroot, HeapStream stream, BinaryReader reader) : base(mdroot,stream) {

			if(stream.Size<24) throw new BadMetadataException();

			long pos = reader.BaseStream.Position;
			reader.ReadUInt32();	// reserved1
			this.MajorVersion = reader.ReadByte();
			this.MinorVersion = reader.ReadByte();
			this.HeapSizes = reader.ReadByte();
			this.StringsIndexSize = ((this.HeapSizes & 1)!=0) ? 4 : 2;
			this.BlobIndexSize = ((this.HeapSizes & 4)!=0) ? 4 : 2;
			this.GuidIndexSize = ((this.HeapSizes & 2)!=0) ? 4 : 2;
			reader.ReadByte();		// reserved2

			this.Valid = reader.ReadInt64();
			this.Sorted = reader.ReadInt64();
			// Calc number of tables from valid bitvector.
			for(int i=0; i<=(int)TableId.MAXVALUE; ++i) {
				if(0!=(Valid & (1L << i))) {
					this.rows[i] = reader.ReadInt32();
				}
			}

			RegisterTable(this.Assembly = new Table<AssemblyRow, AssemblyRowIndex>(new AssemblyRowFactory(this)));
			RegisterTable(this.AssemblyOS = new Table<AssemblyOSRow, AssemblyOSRowIndex>(new AssemblyOSRowFactory(this)));
			RegisterTable(this.AssemblyProcessor = new Table<AssemblyProcessorRow, AssemblyProcessorRowIndex>(new AssemblyProcessorRowFactory(this)));
			RegisterTable(this.AssemblyRef = new Table<AssemblyRefRow, AssemblyRefRowIndex>(new AssemblyRefRowFactory(this)));
			RegisterTable(this.AssemblyRefOS = new Table<AssemblyRefOSRow, AssemblyRefOSRowIndex>(new AssemblyRefOSRowFactory(this)));
			RegisterTable(this.AssemblyRefProcessor = new Table<AssemblyRefProcessorRow, AssemblyRefProcessorRowIndex>(new AssemblyRefProcessorRowFactory(this)));
			RegisterTable(this.ClassLayout = new Table<ClassLayoutRow, ClassLayoutRowIndex>(new ClassLayoutRowFactory(this)));
			RegisterTable(this.Constant = new Table<ConstantRow, ConstantRowIndex>(new ConstantRowFactory(this)));
			RegisterTable(this.CustomAttribute = new Table<CustomAttributeRow, CustomAttributeRowIndex>(new CustomAttributeRowFactory(this)));
			RegisterTable(this.DeclSecurity = new Table<DeclSecurityRow, DeclSecurityRowIndex>(new DeclSecurityRowFactory(this)));
			RegisterTable(this.Event = new Table<EventRow, EventRowIndex>(new EventRowFactory(this)));
			RegisterTable(this.EventMap = new Table<EventMapRow, EventMapRowIndex>(new EventMapRowFactory(this)));
			RegisterTable(this.ExportedType = new Table<ExportedTypeRow, ExportedTypeRowIndex>(new ExportedTypeRowFactory(this)));
			RegisterTable(this.FieldDef = new Table<FieldRow, FieldRowIndex>(new FieldRowFactory(this)));
			RegisterTable(this.FieldLayout = new Table<FieldLayoutRow, FieldLayoutRowIndex>(new FieldLayoutRowFactory(this)));
			RegisterTable(this.FieldMarshal = new Table<FieldMarshalRow, FieldMarshalRowIndex>(new FieldMarshalRowFactory(this)));
			RegisterTable(this.FieldRVA = new Table<FieldRVARow, FieldRVARowIndex>(new FieldRVARowFactory(this)));
			RegisterTable(this.File = new Table<FileRow, FileRowIndex>(new FileRowFactory(this)));
			RegisterTable(this.GenericParam = new Table<GenericParamRow, GenericParamRowIndex>(new GenericParamRowFactory(this)));
			RegisterTable(this.GenericParamConstraint = new Table<GenericParamConstraintRow, GenericParamConstraintRowIndex>(new GenericParamConstraintRowFactory(this)));
			RegisterTable(this.ImplMap = new Table<ImplMapRow, ImplMapRowIndex>(new ImplMapRowFactory(this)));
			RegisterTable(this.InterfaceImpl = new Table<InterfaceImplRow, InterfaceImplRowIndex>(new InterfaceImplRowFactory(this)));
			RegisterTable(this.ManifestResource = new Table<ManifestResourceRow, ManifestResourceRowIndex>(new ManifestResourceRowFactory(this)));
			RegisterTable(this.MemberRef = new Table<MemberRefRow, MemberRefRowIndex>(new MemberRefRowFactory(this)));
			RegisterTable(this.MethodDef = new Table<MethodDefRow, MethodDefRowIndex>(new MethodDefRowFactory(this)));
			RegisterTable(this.MethodImpl = new Table<MethodImplRow, MethodImplRowIndex>(new MethodImplRowFactory(this)));
			RegisterTable(this.MethodSemantics = new Table<MethodSemanticsRow, MethodSemanticsRowIndex>(new MethodSemanticsRowFactory(this)));
			RegisterTable(this.MethodSpec = new Table<MethodSpecRow, MethodSpecRowIndex>(new MethodSpecRowFactory(this)));
			RegisterTable(this.Module = new Table<ModuleRow, ModuleRowIndex>(new ModuleRowFactory(this)));
			RegisterTable(this.ModuleRef = new Table<ModuleRefRow, ModuleRefRowIndex>(new ModuleRefRowFactory(this)));
			RegisterTable(this.NestedClass = new Table<NestedClassRow, NestedClassRowIndex>(new NestedClassRowFactory(this)));
			RegisterTable(this.Param = new Table<ParamRow, ParamRowIndex>(new ParamRowFactory(this)));
			RegisterTable(this.Property = new Table<PropertyRow, PropertyRowIndex>(new PropertyRowFactory(this)));
			RegisterTable(this.PropertyMap = new Table<PropertyMapRow, PropertyMapRowIndex>(new PropertyMapRowFactory(this)));
			RegisterTable(this.StandAloneSig = new Table<StandAloneSigRow, StandAloneSigRowIndex>(new StandAloneSigRowFactory(this)));
			RegisterTable(this.TypeDef = new Table<TypeDefRow, TypeDefRowIndex>(new TypeDefRowFactory(this)));
			RegisterTable(this.TypeRef = new Table<TypeRefRow, TypeRefRowIndex>(new TypeRefRowFactory(this)));
			RegisterTable(this.TypeSpec = new Table<TypeSpecRow, TypeSpecRowIndex>(new TypeSpecRowFactory(this)));

			// Allocate tables
			for(int i=0; i<=(int)TableId.MAXVALUE; ++i) {
				if(tables[i]==null) { continue; }
				tables[i].RowCount = rows[i];
			}

			// Allocate tables
			//long pos = reader.BaseStream.Position-pos;
			for(int i=0; i<=(int)TableId.MAXVALUE; ++i) {
				if(rows[i]<=0) { continue; }
				tables[i].RowData = reader.ReadBytes(tables[i].RowFactory.PhysicalSize*rows[i]);
				tables[i].StartPosition = 0;
				//Console.WriteLine("0x{0:X2}-{1}-{2}",i,offset,tables[i].RowCodedSize,rows[i]);
			}

		}

		public ITable this[TableId id] {
			get {
				return tables[(int)id];
			}
		}

		#region IEnumerable<ITable> ÉÅÉìÉo

		public IEnumerator<ITable> GetEnumerator() {
			foreach(ITable table in this.tables) {
				if(table==null) { continue; }
				yield return table;
			}
		}

		#endregion

		#region IEnumerable ÉÅÉìÉo

		IEnumerator IEnumerable.GetEnumerator() {
			foreach(ITable table in this.tables) {
				if(table==null) { continue; }
				yield return table;
			}
		}

		#endregion

		public int GetRowCount(TableId tableId) {
			return this.rows[(int)tableId];
		}

		public bool IsValid (TableId tableId) {
			return (this.Valid & (1L << (int) tableId)) != 0;
		}

		public bool IsSorted (TableId tableId) {
			return (Sorted & (1L << (int) tableId)) != 0;
		}

	}

}
