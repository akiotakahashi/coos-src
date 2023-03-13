using System;
using System.Collections;

namespace CooS.FileSystem {

	public interface Archive : DirectoryAspect {

		//int ClusterSize {get;}
		Book[] GetBooks();

	}

	public abstract class ArchiveBase : Archive {

		#region Archive �����o

		public Book[] GetBooks() {
			// TODO:  ArchiveBase.GetBooks ������ǉ����܂��B
			return null;
		}

		#endregion

		#region DirectoryAspect �����o

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

		#region Aspect �����o

		public Page Page {
			get {
				return null;
			}
		}

		#endregion

		protected abstract Book OpenRootBook();

	}

}
