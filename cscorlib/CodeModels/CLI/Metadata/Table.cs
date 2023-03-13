/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace CooS.CodeModels.CLI.Metadata {

	public interface Table {
		TablesHeap Heap {get;}
		string Name {get;}
		TableId Id {get;}
		int RowCount {get;}
		int RowLogicalSize {get;}
		int RowCodedSize {get;}
		Row this[int index] {get;}
		void Dump(TextWriter writer);
	}

	public abstract class TableBase : Table {

		protected TablesHeap heap;    // base heap
		protected long offset;

		public TableBase(TablesHeap heap, long offset) {
			this.heap = heap;
			this.offset = offset;
		}

		public virtual TablesHeap Heap {
			get {
				return heap;
			}
		}

		public MetadataRoot Root {
			get {
				return heap.Root;
			}
		}

		#region Table ÉÅÉìÉo

		public abstract string Name {get;}
		public abstract TableId Id {get;}
		public abstract int RowLogicalSize {get;}
		public abstract int RowCodedSize {get;}

		public int RowCount {
			get {
				return Heap.GetRowCount(Id);
			}
		}
		
		public virtual Row this[int index] {
			get {
				if(index==0) return null;
				if(index<0) throw new ArgumentOutOfRangeException();
				return ReadRow(index, Heap.OpenStream(offset+RowCodedSize*(index-1)));
			}
		}

		#endregion

		public abstract Row ReadRow(int rowIndex, Stream stream);

		public virtual void Dump(TextWriter writer) {
			writer.WriteLine("=========================================");
			writer.WriteLine("Table '{0}', id = {1} (0x{2}), rows = {3}",
				Name, Id, ((int) Id).ToString("X"), RowCount);
			for(int n=1; n<=RowCount; ++n) {
				Row row = this[n];
				writer.WriteLine();
				writer.WriteLine("Row #{0}", n);
				writer.WriteLine("-------------");
				row.Dump(writer);
				writer.WriteLine();
			}
		}

	}

}
