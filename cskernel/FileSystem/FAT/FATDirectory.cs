using System;
using System.Collections;
using CooS.FileSystem;

namespace CooS.FileSystem.FAT {

	public class FATDirectory : DirectoryAspect {

		readonly FATArchive archive;
		readonly FATContainer page;
		readonly Hashtable entries = new Hashtable(new CaseInsensitiveHashCodeProvider(),new CaseInsensitiveComparer());

		public FATDirectory(FATRootDirectory rootdir) {
			this.archive = rootdir.Archive;
			this.page = rootdir;
			this.Initialize();
		}

		public FATDirectory(FATEntity book) {
			this.archive = book.Archive;
			this.page = book;
			this.Initialize();
		}

		private unsafe void Initialize() {
			byte[] buf = new byte[page.ClusterCount*page.ClusterSize];
			page.Read(buf, 0, 0, (int)page.ClusterCount);
			char[] lnbuf = new char[261];
			bool lastingln = false;
			for(uint i=0; i<512/32; ++i) {
				fixed(byte* pbuf = &buf[0]) {
					DirectoryEntry* entry = (DirectoryEntry*)pbuf+i;
					if((entry->attributes&FATAttributes.LongName)==FATAttributes.LongName) {
						LongNameEntry* longname = (LongNameEntry*)entry;
						// LN Entry
						int index;
						if(!lastingln) {
							index = longname->seq-1;
							if(0!=(index&0x40)) {
								lastingln = true;
								index &= ~0x40;
								Array.Clear(lnbuf,0,lnbuf.Length);
							}
						} else {
							index = longname->seq-1;
						}
						if(lastingln) {
							index *= 13;
							lnbuf[index+0] = longname->name0;
							lnbuf[index+1] = longname->name1;
							lnbuf[index+2] = longname->name2;
							lnbuf[index+3] = longname->name3;
							lnbuf[index+4] = longname->name4;
							lnbuf[index+5] = longname->name5;
							lnbuf[index+6] = longname->name6;
							lnbuf[index+7] = longname->name7;
							lnbuf[index+8] = longname->name8;
							lnbuf[index+9] = longname->name9;
							lnbuf[index+10] = longname->name10;
							lnbuf[index+11] = longname->name11;
							lnbuf[index+12] = longname->name12;
						}
					} else {
						if(entry->name0==0x00) {
							break;
						} else if(entry->name0==0xE5) {
							// NOP: This is free but there may be following entries.
						} else if(0!=(entry->attributes&FATAttributes.VolumeId)) {
							// NOP: We should ignore this entry.
						} else {
							if(!lastingln) {
								// short filename
								// nl:length of name, el:length of extension
								int nl = -1;
								if(entry->name7!=' ') nl=8;
								else if(entry->name0==' ') nl=0;
								else if(entry->name1==' ') nl=1;
								else if(entry->name2==' ') nl=2;
								else if(entry->name3==' ') nl=3;
								else if(entry->name4==' ') nl=4;
								else if(entry->name5==' ') nl=5;
								else if(entry->name6==' ') nl=6;
								else if(entry->name7==' ') nl=7;
								int el = -1;
								if(entry->ext2!=' ') el=3;
								else if(entry->ext0==' ') el=0;
								else if(entry->ext1==' ') el=1;
								else if(entry->ext2==' ') el=2;
								lnbuf[0] = (char)entry->name0;
								lnbuf[1] = (char)entry->name1;
								lnbuf[2] = (char)entry->name2;
								lnbuf[3] = (char)entry->name3;
								lnbuf[4] = (char)entry->name4;
								lnbuf[5] = (char)entry->name5;
								lnbuf[6] = (char)entry->name6;
								lnbuf[7] = (char)entry->name7;
								if(el==0) {
									lnbuf[nl] = '\0';
								} else {
									++el;	// for the period
									lnbuf[nl+0] = '.';
									lnbuf[nl+1] = (char)entry->ext0;
									lnbuf[nl+2] = (char)entry->ext1;
									lnbuf[nl+3] = (char)entry->ext2;
								}
								lnbuf[nl+el] = '\0';
							}
							string filename = new string(lnbuf,0,Array.IndexOf(lnbuf,'\0'));
							entries.Add(filename, *entry);
						}
						lastingln = false;
					}
				}
			}
		}


		#region Aspect ÉÅÉìÉo
	
		public Page Page {
			get {
				return this.page;
			}
		}

		#endregion

		#region DirectoryAspect ÉÅÉìÉo

		public Book OpenBook(string name) {
			if(!entries.ContainsKey(name)) return null;
			return new FATEntity((FATArchive)archive, (DirectoryEntry)entries[name], name);
		}

		public IEnumerable EnumBookInfo() {
			ArrayList list = new ArrayList();
			foreach(DictionaryEntry item in entries) {
				DirectoryEntry entry = (DirectoryEntry)item.Value;
				list.Add(entry.ConvertToBookInfo((string)item.Key));
			}
			return list;
		}
	
		public BookInfo GetBookInfo(string name) {
			if(entries.ContainsKey(name)) {
				return ((DirectoryEntry)entries[name]).ConvertToBookInfo(name);
			} else {
				return null;
			}
		}

		#endregion

	}

}
