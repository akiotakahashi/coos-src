using System;
using System.Collections;

namespace CooS.Utility {

	public class TypePath {
	
		public readonly string Namespace;
		public readonly string[] TypeName;
		public readonly string Assembly;

		public static TypePath Parse(string path) {
			return new TypePath(path);
		}

		public TypePath(string path) {
			int comma = path.LastIndexOf(',');
			if(comma<0) {
				Assembly = null;
			} else {
				Assembly = path.Substring(comma+1);
				path = path.Substring(0,comma);
			}
			ArrayList nests = new ArrayList();
			int plus = 0;
			while((plus=path.LastIndexOf('+'))>=0) {
				nests.Add(path.Substring(plus+1));
				path = path.Substring(0,plus);
			}
			TypeName = new string[nests.Count+1];
			for(int i=0; i<nests.Count; ++i) {
				TypeName[i+1] = (string)nests[i];
			}
			int period = path.LastIndexOf('.');
			if(period<0) {
				Namespace = null;
				TypeName[0] = path;
			} else {
				Namespace = path.Substring(0,period);
				TypeName[0] = path.Substring(period+1);
			}
		}

		public override string ToString() {
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			if(this.Namespace!=null) buf.Append(Namespace);
			for(int i=this.TypeName.Length-1; i>=0; --i) {
				if(buf.Length>0) buf.Append('.');
				buf.Append(this.TypeName[i]);
			}
			if(this.Assembly!=null) {
				buf.Append(',');
				buf.Append(this.Assembly);
			}
			return buf.ToString();
		}

		public System.Reflection.AssemblyName GetAssemblyName() {
			if(this.Assembly==null) {
				return null;
			} else {
				System.Reflection.AssemblyName asmname = new System.Reflection.AssemblyName();
				asmname.Name = this.Assembly;
				return asmname;
			}
		}

	}

}
