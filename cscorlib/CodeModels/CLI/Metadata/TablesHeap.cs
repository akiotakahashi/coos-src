/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
using System.Collections;

namespace CooS.CodeModels.CLI.Metadata {

	/// <summary>
	/// Metadata tables heap (#~).
	/// </summary>
	/// <remarks>
	/// Partition II; Chapter 21 & 23.1.6
	/// </remarks>
	public class TablesHeap : TablesHeapBase {

		// schema version (currently 1.0)
		byte verMaj;
		byte verMin;

		// bitvector for heap-size flags:
		// bit 1 - if set #Strings heap uses wide indices (dword)
		// bit 2 - if set #GUID heap uses wide indices
		// bit 3 - if set #Blob heap uses wide indices
		// otherwise (particular bit is not set) index size is word.
		byte heapSizes;

		// Tables
		long valid;		// bitvector of valid tables (64-bit, max index = TableId.MAX)
		long sorted;	// bitvector of sorted tables (64-bit)
		int[] rows = new int[(int) TableId.Count];
		Table[] tables = new Table[(int)TableId.Count];

		internal TablesHeap(MetadataRoot mdroot, MDStream stream, BinaryReader reader) : base(mdroot,stream) {

			if(stream.Size<24) throw new BadMetadataException ("Invalid header for #~ heap.");

			long pos = reader.BaseStream.Position;

			reader.ReadUInt32();	// reserved1
			verMaj = reader.ReadByte();
			verMin = reader.ReadByte();
			heapSizes = reader.ReadByte();
			reader.ReadByte();		// reserved2

			valid = reader.ReadInt64();
			sorted = reader.ReadInt64();
			// Calc number of tables from valid bitvector.
			for(int i=0; i<(int)TableId.Count; ++i) {
				if(0!=(valid & (1L << i))) {
					rows[i] = reader.ReadInt32();
				}
			}
			// Allocate tables
			long offset = reader.BaseStream.Position-pos;
			for(int i=0; i<(int)TableId.Count; ++i) {
				if(rows[i]<=0) continue;
				tables[i] = TabsDecoder.CreateTable((TableId)i, this, offset);
				offset += tables[i].RowCodedSize*rows[i];
				//Console.WriteLine("0x{0:X2}-{1}-{2}",i,offset,tables[i].RowCodedSize,rows[i]);
			}

		}

		/// <summary>
		/// Gets or sets bitvector of valid tables (64-bit).
		/// </summary>
		public override long Valid {
			get {
				return valid;
			}
			set {
				valid = value;
			}
		}

		/// <summary>
		/// Gets or sets bitvector of sorted tables (64-bit).
		/// </summary>
		public override long Sorted {
			get {
				return sorted;
			}
			set {
				sorted = value;
			}
		}

		//
		// "Universal" accessors for Valid and Sorted bitvectors.
		//

		public bool IsValid (TableId tab) {
			return (valid & (1L << (int) tab)) != 0;
		}

		public void SetValid (TableId tab, bool b) {
			long mask = 1L << (int) tab;
			if (b) {
				valid |= mask;
			} else {
				valid &= ~mask;
			}
		}


		/// <summary>
		/// True if the given table in this heap is sorted.
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public bool IsSorted (TableId tab) {
			return (sorted & (1L << (int) tab)) != 0;
		}

		/// <summary>
		/// Marks specified table in this heap as sorted or unsorted.
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="b"></param>
		public void SetSorted (TableId tab, bool b) {
			long mask = 1L << (int) tab;
			if (b) {
				sorted |= mask;
			} else {
				sorted &= ~mask;
			}
		}

		public byte HeapSizes {
			get {
				return heapSizes;
			}
			set {
				heapSizes = value;
			}
		}

		public int StringsIndexSize {
			get {
				return 2 + ((heapSizes & 1) << 1);
			}
		}

		public int GuidIndexSize {
			get {
				return 2 + (heapSizes & 2);
			}
		}

		public int BlobIndexSize {
			get {
				return 2 + ((heapSizes & 4) >> 1);
			}
		}

		public int GetRowCount(TableId tid) {
			return rows[(int)tid];
		}

		public Table this[TableId id] {
			get {
				return tables[(int)id];
			}
		}

		public Row this[MDToken token] {
			get {
				return this[token.TableId][token.RID];
			}
		}

		public int GetIndexSize(TableId tid) {
			// Index is 2 bytes wide if table has less than 2^16 rows
			// otherwise it's 4 bytes wide.
			return ((uint) rows [(int)tid]) < (1 << 16) ? 2 : 4;
		}

		public int GetCodedIndexSize(CodedTokenId tid) {
			return TabsDecoder.GetCodedIndexSize(tid, rows, this.StringsIndexSize);
		}
		
		public MethodIL GetMethodBody(MethodRow row) {
			try {
				Stream stream = this.Root.OpenStream(row.RVA);
				MethodIL il = new MethodIL();
				il.Read(new BinaryReader(stream));
				return il;
			} catch {
				return null;
			}
		}

	}

}
