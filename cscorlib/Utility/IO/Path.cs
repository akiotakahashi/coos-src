using System;
using System.IO;
using CooS.FileSystem;

namespace CooS.IO {

	public class _Path {
		
		public static bool IsPathRooted(string path) {
			if(path==null || path.Length==0) return false;
			if(path.IndexOfAny(System.IO.Path.InvalidPathChars)>=0) {
				throw new ArgumentException("Illegal characters in path", "path");
			}
			return new PathInfo(path).IsRooted;
		}

		public static string GetPathRoot(string path) {
			if(path==string.Empty) throw new ArgumentException("This specified path is invalid.");
			if(path==null) return null;
			return new PathInfo(path).RootPath;
		}

	}

}
