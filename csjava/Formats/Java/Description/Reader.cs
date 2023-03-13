using System;
using System.IO;
using System.Collections.Generic;

namespace CooS.Formats.Java.Description {

	public class Reader {

		private string text;
		private int pos;

		public Reader(string text) {
			this.text = text;
			this.pos = 0;
		}

		public char Peek() {
			return this.text[pos];
		}

		public char Read() {
			return this.text[pos++];
		}

		public string Read(char delimeter) {
			int i = this.text.IndexOf(delimeter, pos);
			if(i<0) throw new ArgumentException();
			string ret = this.text.Substring(pos, i-pos);
			this.pos = i+1;
			return ret;
		}

	}

}
