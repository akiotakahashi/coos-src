using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace CooS.FileSystem.CDFS {
	/// <summary>
	/// Iso9660Directory ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
	/// </summary>
	public class Iso9660Directory : Iso9660Item, DirectoryAspect {
	
		readonly Hashtable entries = CollectionsUtil.CreateCaseInsensitiveHashtable();

		public Iso9660Directory(DirectoryRecord record, Partition partition) : base(record,partition) {
			uint count = (uint)(this.record.DataLength+this.partition.BlockSize-1)/(uint)this.partition.BlockSize;
			byte[] buf = new byte[count*this.partition.BlockSize];
			ReadExtent(buf, 0, 0, (int)count);
			BinaryReader reader = new BinaryReader(new MemoryStream(buf));
			for(;;) {
				long position = reader.BaseStream.Position;
				DirectoryRecord dr = new DirectoryRecord(reader);
				if(dr.LengthOfDirectoryRecord==0) break;
				string filename = dr.GetFileName();
				if(filename.Length>0) {
					this.entries.Add(filename, dr);
				}
				reader.BaseStream.Position = position+dr.LengthOfDirectoryRecord;
			}
		}

		public override Aspect QueryAspects(Type aspect) {
			if(aspect==typeof(DirectoryAspect)) {
				return this;
			} else {
				return null;
			}
		}

		#region DirectoryAspect ÉÅÉìÉo

		public System.Collections.IEnumerable EnumBookInfo() {
			ArrayList list = new ArrayList();
			foreach(DictionaryEntry e in this.entries) {
				DirectoryRecord record = (DirectoryRecord)e.Value;
				BookInfo bi = record.ToBookInfo();
				list.Add(bi);
			}
			return list;
		}

		public BookInfo GetBookInfo(string name) {
			if(!this.entries.ContainsKey(name)) return null;
			return ((DirectoryRecord)this.entries[name]).ToBookInfo();
		}

		public Book OpenBook(string name) {
			if(!this.entries.ContainsKey(name)) return null;
			DirectoryRecord record = (DirectoryRecord)this.entries[name];
			if(0!=(record.FileFlags&FileFlags.Directory)) {
				return new Iso9660Directory(record, this.partition);
			} else {
				return new Iso9660File(record, this.partition, this);
			}
		}

		#endregion

	}
}
