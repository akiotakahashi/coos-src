using System;
using System.Globalization;

class CompareInfoHelper {
	public static char GetLower(char ch) {
		if(ch>'Z') return ch;
		if(ch<'A') return ch;
		return (char)(ch-'A'+'a');
	}
}

namespace CooS.Wrap._System.Globalization {

	public class _CompareInfo {

		private void construct_compareinfo(string locale) {
			//TODO: ?
		}

		private int internal_compare(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options) {
			bool ignorecase = 0!=(options&CompareOptions.IgnoreCase);
			int i1=offset1;
			int i2=offset2;
			while(i1<offset1+length1 && i2<offset2+length2) {
				char ch1 = str1[i1];
				char ch2 = str2[i2];
				if(options==CompareOptions.None || options==CompareOptions.Ordinal) {
					if(ch1!=ch2) {
						if(ch1<ch2) return -1;
						if(ch1>ch2) return +1;
					}
				} else if(ignorecase) {
					ch1 = CompareInfoHelper.GetLower(ch1);
					ch2 = CompareInfoHelper.GetLower(ch2);
					if(ch1!=ch2) {
						if(ch1<ch2) return -1;
						if(ch1>ch2) return +1;
					}
				} else {
					throw new NotImplementedException("Options="+(int)options);
				}
				++i1;
				++i2;
			}
			return length1.CompareTo(length2);
		}

	}

}
