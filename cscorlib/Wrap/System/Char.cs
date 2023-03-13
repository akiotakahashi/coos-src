using System;

namespace CooS.Wrap._System {

	public class _Char {

		static unsafe void GetDataTablePointers(
			out byte* category_data,
			out byte* numeric_data,
			out double* numeric_data_values,
			out ushort* to_lower_data_low,
			out ushort* to_lower_data_high,
			out ushort* to_upper_data_low,
			out ushort* to_upper_data_high) {
			category_data = null;
			numeric_data = null;
			numeric_data_values = null;
			to_lower_data_low = null;
			to_lower_data_high = null;
			to_upper_data_low = null;
			to_upper_data_high = null;
		}

		public static bool IsLetter(char c) {
			if('a'<=c && c<='z') return true;
			if('A'<=c && c<='Z') return true;
			return false;
		}

		public static bool IsDigit(char c) {
			return '0'<=c && c<='9';
		}

		public static bool IsWhiteSpace(char c) {
			switch (c) {
			case '\t':
			case '\n':
			case '\v':
			case '\f':
			case '\r':
			case '\x0085':
			case '\u2028':
			case '\u2029':
				return true;
			}
			return false;
		}

		internal static char ToLowerInvariant(char c) {
			if('A'<=c && c<='Z') {
				return (char)(c + ('a'-'A'));
			}
			return c;
		}

		internal static char ToUpperInvariant(char c) {
			if('a'<=c && c<='z') {
				return (char)(c + ('A'-'a'));
			}
			return c;
		} 
 
	}

}
