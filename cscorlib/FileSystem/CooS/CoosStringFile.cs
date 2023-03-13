using System;
using System.IO;

namespace CooS.FileSystem.CooS {

	public class CoosStringFile : FileAspect, Page, Book {

		readonly string filename;
		readonly byte[] buf;

		public CoosStringFile(string filename, string content) {
			this.filename = filename;
			this.buf = System.Text.Encoding.ASCII.GetBytes(content);
		}

		#region Book ƒƒ“ƒo

		public BookInfo BookInfo {
			get {
				return new BookInfo(this.filename,FileAttributes.Normal);
			}
		}

		public Aspect QueryAspects(Type aspect) {
			if(aspect==typeof(FileAspect)) {
				return this;
			} else {
				return null;
			}
		}

		#endregion

		#region FileAspect ƒƒ“ƒo

		public long Length {
			get {
				return buf.Length;
			}
		}

		public int ClusterSize {
			get {
				return 1;
			}
		}

		public void Read(byte[] buffer, int index, long start, int count) {
			Buffer.BlockCopy(this.buf, index, buffer, (int)start, count);
		}

		#endregion

		#region Aspect ƒƒ“ƒo

		public Page Page {
			get {
				return this;
			}
		}

		#endregion

	}

}
