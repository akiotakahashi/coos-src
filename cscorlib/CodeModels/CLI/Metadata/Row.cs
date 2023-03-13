/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;

namespace CooS.CodeModels.CLI.Metadata {

	/// <summary>
	/// Metadata row interface.
	/// </summary>
	public interface Row {
		Table Table {get;}
		int Size {get;}
		int Index {get;}
		void Dump(TextWriter writer);
	}

	public sealed class NullRow : Row {
		public static readonly NullRow Instance;

		static NullRow() {
			Instance = new NullRow();
		}

		private NullRow() {
		}

		public int Size {
			get {
				return 0;
			}
		}

		public int Index {
			get {
				return 0;
			}
		}

		public Table Table {
			get {
				return null;
			}
		}

		public void Dump(TextWriter writer) {
			writer.WriteLine("Null row.");
		}
	}

	public abstract class RowBase : Row {

		private readonly Table table;
		private readonly int rowIndex;
	
		protected RowBase(Table table, int rowIndex) {
			this.table = table;
			this.rowIndex = rowIndex;
		}

		#region Row ÉÅÉìÉo

		public Table Table {
			get {
				return this.table;
			}
		}

		public int Index {
			get {
				return this.rowIndex;
			}
		}

		public abstract int Size {get;}
		public abstract void Dump(TextWriter writer);
		
		#endregion

	}

}
