using System;
using System.Text;
using System.Collections;
using System.Globalization;
using CooS.Reflection;

namespace CooS.Wrap._System {
	using AssemblyResolver = CooS.Management.AssemblyResolver;

	public class _String {
	
		static readonly Hashtable internStrings = new Hashtable();
		int length;
		char start_char;

		public unsafe char get_Chars(int index) {
			fixed(char* p = &this.start_char) {
				return p[index];
			}
		}

		private static string InternalAllocateStr(int length) {
			return Memory.AllocateString((TypeImpl)typeof(string), length);
		}

		private static string InternalIntern(string str) {
			if(internStrings==null) return str;
			lock(internStrings.SyncRoot) {
				if(internStrings.ContainsKey(str)) {
					return (string)internStrings[str];
				} else {
					internStrings[str] = str;
					return str;
				}
			}
		}
		
		private static string InternalIsInterned(string str) {
			if(internStrings==null) return null;
			lock(internStrings.SyncRoot) {
				return (string)internStrings[str];
			}
		}

		private string[] InternalSplit(char[] separator, int count) {
			ArrayList list = new ArrayList();
			string me = (string)(object)this;
			int prev = 0;
			while(count-->1) {
				int next = me.IndexOfAny(separator,prev);
				if(next<0) break;
				list.Add(me.Substring(prev, next-prev));
				prev = next+1;
			}
			list.Add(me.Substring(prev));
			return (string[])list.ToArray(typeof(string));
		}

		private string InternalReplace(string oldValue, string newValue, CompareInfo comp) {
			string me = (string)(object)this;
			if(oldValue.Length==1 && newValue.Length==1) return me.Replace(oldValue[0], newValue[0]);
			int count = 0;
			int index = 0;
			while((index=comp.IndexOf(me, oldValue, index))>=0) {
				++count;
				++index;
			}
			StringBuilder buf = new StringBuilder(me.Length+(newValue.Length-oldValue.Length)*count);
			int prev = 0;
			int next;
			while((next=comp.IndexOf(me, oldValue, prev))>=0) {
				buf.Append(me, prev, next-prev);
				buf.Append(newValue);
				prev = next+oldValue.Length;
			}
			return buf.ToString();
		}

		private unsafe void InternalCopyTo(int sIndex, char[] dest, int destIndex, int count) {
			if(count>0) {
				string me = (string)(object)this;
				int size = sizeof(char)*count;
				fixed(char* d = dest, s = me) {
					Tuning.Memory.Copy(d+destIndex, s+sIndex, size);
				}
			}
		}

		private static unsafe void InternalStrcpy(string dest, int destPos, string src) {
			int size = sizeof(char)*src.Length;
			fixed(char* d = dest, s = src) {
				Tuning.Memory.Copy(d+destPos, s, size);
			}
		}

		private static unsafe void InternalStrcpy(string dest, int destPos, string src, int sPos, int count) {
			if(count>0) {
				int size = count*sizeof(char);
				fixed(char* d = dest, s = src) {
					Tuning.Memory.Copy(d+destPos, s+sPos, size);
				}
			}
		}

		private static unsafe void InternalStrcpy(string dest, int destPos, char[] chars) {
			if(chars.Length>0) {
				int size = chars.Length*sizeof(char);
				fixed(char* d = dest, s = chars) {
					Tuning.Memory.Copy(d+destPos, s, size);
				}
			}
		}

		private static unsafe void InternalStrcpy(string dest, int destPos, char[] chars, int sPos, int count) {
			if(count>0) {
				int size = count*sizeof(char);
				fixed(char* pd = dest, ps = chars) {
					Tuning.Memory.Copy(pd+destPos, ps+sPos, size);
				}
			}
		}

		private unsafe string InternalTrim(char[] chars, int typ) {
			if(chars==null) throw new ArgumentException("chars is null");
			string me = (string)(object)this;
			int ec = me.Length-1;
			if(typ==0 || typ==2) {
				for(; ec>=0; --ec) {
					bool hit = false;
					for(int ich=0; ich<chars.Length; ++ich) {
						if(me[ec]==chars[ich]) {
							hit = true;
							break;
						}
					}
					if(!hit) break;
				}
			}
			int sc = 0;
			if(typ==0 || typ==1) {
				for(; sc<=ec; ++sc) {
					bool hit = false;
					for(int ich=0; ich<chars.Length; ++ich) {
						if(me[sc]==chars[ich]) {
							hit = true;
							break;
						}
					}
					if(!hit) break;
				}
			}
			if(sc==0 && ec==me.Length-1) {
				return me;
			} else {
				fixed(char* p = me) {
					return new string(p,sc,ec-sc+1);
				}
			}
		}

		private static unsafe string InternalJoin(string separator, string[] value, int sIndex, int count) {
			if(count<0) throw new ArgumentException();
			if(count==0) return string.Empty;

			int length = separator.Length*(count-1);
			for(int i=sIndex; i<sIndex+count; ++i) {
				length += value[i].Length;
			}
			fixed(char* p = value[sIndex]) {
				string str = new string(p, 0, length);
				int offset = value[sIndex].Length;
				int len;
				for(int i=sIndex+1; i<sIndex+count; ++i) {
					len = separator.Length;
					InternalStrcpy(str, offset, separator, 0, len);
					offset += len;
					len = value[i].Length;
					InternalStrcpy(str, offset, value[i], 0, len);
					offset += len;
				}
				return str;
			}
		}

		private unsafe int InternalIndexOfAny(char[] arr, int sIndex, int count) {
			string me = (string)(object)this;
			fixed(char* p = me) {
				for(int i=sIndex; i<sIndex+count; ++i) {
					for(int j=0; j<arr.Length; ++j) {
						if(p[i]==arr[j]) return i;
					}
				}
			}
			return -1;
		}

		private unsafe int InternalLastIndexOfAny(char[] arr, int sIndex, int count) {
			string me = (string)(object)this;
			fixed(char* p = me) {
				for(int i=sIndex; i>sIndex-count; --i) {
					for(int j=0; j<arr.Length; ++j) {
						if(p[i]==arr[j]) return i;
					}
				}
			}
			return -1;
		}

		private unsafe string InternalReplace(char oldChar, char newChar) {
			string s;
			string me = (string)(object)this;
			fixed(char* p = me) {
				s = new string(p, 0, me.Length);
			}
			fixed(char* q = s) {
				for(int i=0; i<s.Length; ++i) {
					if(q[i]==oldChar) {
						q[i] = newChar;
					}
				}
				return s;
			}
		}

		private static unsafe string InternalAllocateInstance(char[] val) {
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, val.Length);
			fixed(char* dst = s, src = val) {
				Tuning.Memory.Copy(dst, src, val.Length*2);
			}
			return s;
		}

		private unsafe static string InternalAllocateInstance(char* value) {
			int length = 0;
			while(value[length]!=0) {
				++length;
			}
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, length);
			fixed(char* dst = s) {
				Tuning.Memory.Copy(dst, value, length*2);
			}
			return s;
		}

		private unsafe static string InternalAllocateInstance(sbyte* value) {
			int length = 0;
			while(value[length]!=0) {
				++length;
			}
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, length);
			fixed(char* dst = s) {
				for(int i=0; i<length; ++i) {
					dst[i] = (char)(byte)value[i];
				}
			}
			return s;
		}

		private static unsafe string InternalAllocateInstance(char ch, int count) {
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, count);
			fixed(char* dst = s) {
				for(int i=0; i<count; ++i) {
					dst[i] = ch;
				}
			}
			return s;
		}

		private static unsafe string InternalAllocateInstance(char[] val, int startIndex, int length) {
			if(startIndex<0) throw new ArgumentOutOfRangeException();
			if(length<0) throw new ArgumentOutOfRangeException();
			if(val.Length>startIndex+length) throw new ArgumentException();
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, length);
			fixed(char* dst = s, src = val) {
				Tuning.Memory.Copy(dst, src+startIndex, length*2);
			}
			return s;
		}

		private static unsafe string InternalAllocateInstance(char* value, int startIndex, int length) {
			if(startIndex<0) throw new ArgumentOutOfRangeException();
			if(length<0) throw new ArgumentOutOfRangeException();
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, length);
			fixed(char* dst = s) {
				Tuning.Memory.Copy(dst, value+length, length*2);
			}
			return s;
		}

		private static unsafe string InternalAllocateInstance(sbyte* value, int startIndex, int length) {
			if(startIndex<0) throw new ArgumentOutOfRangeException();
			if(length<0) throw new ArgumentOutOfRangeException();
			string s = Memory.AllocateString(AssemblyResolver.Manager.String, length);
			fixed(char* dst = s) {
				for(int i=0; i<length; ++i) {
					dst[i] = (char)(byte)value[startIndex+i];
				}
			}
			return s;
		}

	}

}
