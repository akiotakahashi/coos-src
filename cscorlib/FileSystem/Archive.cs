using System;
using System.Collections;

namespace CooS.FileSystem {

	public interface Archive : DirectoryAspect {

		//int ClusterSize {get;}
		Book[] GetBooks();

	}

	public abstract class ArchiveBase : Archive {

		#region Archive メンバ

		public Book[] GetBooks() {
			// TODO:  ArchiveBase.GetBooks 実装を追加します。
			return null;
		}

		#endregion

		#region DirectoryAspect メンバ

		public System.Collections.IEnumerable EnumBookInfo() {
			return new BookInfo[]{BookInfo.RootDirectoryInfo};
		}

		public BookInfo GetBookInfo(string name) {
			if(name==PathInfo.RootDirectoryName) {
				return BookInfo.RootDirectoryInfo;
			} else {
				return null;
			}
		}

		public Book OpenBook(string name) {
			if(name==PathInfo.RootDirectoryName) {
				return this.OpenRootBook();
			} else {
				return null;
			}
		}

		#endregion

		#region Aspect メンバ

		public Page Page {
			get {
				return null;
			}
		}

		#endregion

		protected abstract Book OpenRootBook();

	}

}
