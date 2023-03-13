using System;
using System.IO;
using System.Collections;

namespace CooS.FileSystem.CooS {

	public class CoosFileSystem : FileSystemProvider, Archive, Book, Page, DirectoryAspect {

		private Hashtable books = new Hashtable();

		public CoosFileSystem() {
			books["coos"] = new CoosSystemDirectory();
		}
		
		public override string Name {
			get {
				return "coos";
			}
		}

		public override string PreferredPrefix {
			get {
				return "cs";
			}
		}

		public override Archive Bind(Partition partition) {
			return this;
		}

		public override bool IsSuitable(Partition partition) {
			return false;
		}

		#region Archive メンバ

		public Book[] GetBooks() {
			return new Book[]{this};
		}

		#endregion

		#region Book メンバ

		public BookInfo BookInfo {
			get {
				return BookInfo.RootDirectoryInfo;
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

		#region Page メンバ

		#endregion

		#region DirectoryAspect メンバ

		public IEnumerable EnumBookInfo() {
			BookInfo[] bis = new BookInfo[this.books.Count];
			int i = 0;
			foreach(DictionaryEntry e in this.books) {
				bis[i++] = new BookInfo((string)e.Key, FileAttributes.Directory);
			}
			return bis;
		}

		public BookInfo GetBookInfo(string name) {
			if(!this.books.ContainsKey(name)) return null;
			return ((Book)books[name]).BookInfo;
		}

		public Book OpenBook(string name) {
			return (Book)this.books[name];
		}

		#endregion

		#region Aspect メンバ

		public Page Page {
			get {
				return this;
			}
		}

		#endregion

	}

}
