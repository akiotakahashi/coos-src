using System;
using System.IO;
using CooS.FileSystem;
using CooS.Management;

namespace CooS.Wrap._System.IO {

	public class _Path {

		public static bool IsPathRooted(string path) {
			return CooS.IO._Path.IsPathRooted(path);
		}

		public static string GetPathRoot(string path) {
			return CooS.IO._Path.GetPathRoot(path);
		}
	
		public static string GetDirectoryName(string path) {
			if(path==string.Empty) throw new ArgumentException();
			if(path==null) return null;
			path = path.Trim();
			int start = path.Length-1;
			if(start<0) {
				return path;
			} else {
				int index = path.LastIndexOfAny(PathInfo.PathSeparatorChars,start);
				if(index<0) return string.Empty;
				return path.Substring(0,index);
			}
		}

		internal static string InsecureGetFullPath(string path) {
			if(path==null) throw new ArgumentNullException("path", "You must specify a path when calling System.IO.Path.GetFullPath");
			path = path.Trim();
			if(path==string.Empty) throw new ArgumentException("The path is not of a legal form", "path");
			if(!Path.IsPathRooted(path)) path = Path.Combine(Directory.GetCurrentDirectory(), path);
			string[] parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			int j = 1;
			for(int i=1; i<parts.Length; ++i) {
				if(parts[i]==".") continue;
				if(parts[i]=="..") {
					if(--j<1) throw new FormatException();
				} else {
					parts[j++] = parts[i];
				}
			}
			if(j==1) {
				return parts[0]+Path.DirectorySeparatorChar;
			} else {
				return string.Join(Path.DirectorySeparatorChar.ToString(), parts, 0, j);
			}
		}
 
	}

}
