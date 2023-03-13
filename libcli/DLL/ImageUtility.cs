using System;
using System.IO;
using System.Text;

namespace CooS.Formats.DLL {

	public abstract class ImageUtility {

		public static string ReadString(BinaryReader reader, int maxlength) {
			if(maxlength==0) {
				return null;
			} else {
				StringBuilder buf = new StringBuilder();
				for(; maxlength>0; --maxlength) {
					byte ch = reader.ReadByte();
					if(ch==0) { break; }
					buf.Append((char)ch);
				}
				return buf.ToString();
			}
		}

	}

}
