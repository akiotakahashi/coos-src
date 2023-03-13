using System;
using System.IO;
using System.Text;

namespace CooS.CodeModels.DLL {

	public abstract class ImageUtility {

		/*
		public static unsafe string GetString(sbyte* data, int start, int len, Encoding encoding) {
			byte[] data_array = new byte[len-start];
			for (int i=start; i<len; i++) {
				data_array[i-start] = (byte)*data++;
			}
			return encoding.GetString(data_array);
		}
		*/

		public static string ReadString(BinaryReader reader, int maxlength) {
			StringBuilder builder = new StringBuilder();
			byte ch;
			while(maxlength-->0 && (ch=reader.ReadByte())!=0) {
				builder.Append((char)ch);
			}
			if(maxlength==0) return null;
			return builder.ToString();
		}

	}

}
