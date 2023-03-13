using System;
using System.IO;
using System.Collections;

namespace CooS.FileSystem.CooS {

	public class CoosSystemDirectory : Book, Page, DirectoryAspect {
	
		private Hashtable books = new Hashtable();
		
		public CoosSystemDirectory() {
			this.books["machine.config"] = new CoosStringFile("machine.config", "<?xml version='1.0' ?><configuration />");
		}

		#region Book ÉÅÉìÉo

		public BookInfo BookInfo {
			get {
				return new BookInfo("coos", FileAttributes.Directory);
			}
		}

		public Aspect QueryAspects(Type aspect) {
			if(aspect==typeof(DirectoryAspect)) {
				return this;
			} else {
				return null;
			}
		}

		#endregion

		#region DirectoryAspect ÉÅÉìÉo

		public System.Collections.IEnumerable EnumBookInfo() {
			BookInfo[] bis = new BookInfo[books.Count];
			int i = 0;
			foreach(DictionaryEntry e in books) {
				bis[i++] = new BookInfo((string)e.Key, FileAttributes.Normal);
			}
			return bis;
		}

		public BookInfo GetBookInfo(string name) {
			throw new NotImplementedException();
		}

		public Book OpenBook(string name) {
			return (Book)this.books[name];
		}

		#endregion

		#region Aspect ÉÅÉìÉo

		public Page Page {
			get {
				return this;
			}
		}

		#endregion

	}

}
