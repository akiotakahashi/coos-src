using System;
using System.IO;
using System.Text;

namespace CooS.FileSystem {

	/// <summary>
	/// (<drive>(@host):)/<path> 構文のパスを解釈します。
	/// </summary>
	public class PathInfo {

		public static readonly char[] PathSeparatorChars = new char[]{
																		 Path.DirectorySeparatorChar,
																		 Path.AltDirectorySeparatorChar
																	 };
		public static readonly string RootDirectoryName = string.Empty;

		string drive;
		string host;
		string location;
		
		public static bool IsDsc(char ch) {
			return ch==Path.DirectorySeparatorChar || ch==Path.AltDirectorySeparatorChar;
		}

		public PathInfo(string path) {
			path = path.Trim(' ');
			int index = path.IndexOf(':');
			if(index<0) {
				drive = null;
				host = null;
				location = path;
				if(location.Length==0) throw new FormatException();
			} else {
				drive = path.Substring(0,index);
				location = path.Substring(index+1);
				index = drive.IndexOf('@');
				if(index<0) {
					host = null;
					if(drive.Length==0) throw new FormatException();
				} else {
					host = drive.Substring(index+1);
					drive = drive.Substring(0,index);
					if(host.Length==0) throw new FormatException();
				}
			}
		}

		public string Drive {
			get {
				return drive;
			}
		}

		public string Host {
			get {
				return host;
			}
		}

		public string Location {
			get {
				return location;
			}
		}

		public string Storage {
			get {
				if(drive==null && host==null) return null;
				if(host==null) return drive;
				return drive+"@"+host;
			}
		}

		public bool IsRooted {
			get {
				if(location==null || location.Length==0) {
					return true;
				} else {
					return IsDsc(location[0]);
				}
			}
		}

		public bool IsRootDir {
			get {
				return this.location.Length==1 && this.IsRooted;
			}
		}

		private void AppendStoragePath(StringBuilder buf) {
			if(drive!=null || host!=null) {
				buf.Append(drive);
				if(host!=null) {
					buf.Append('@');
					buf.Append(host);
				}
				buf.Append(':');
			}
		}

		public string RootPath {
			get {
				if(drive==null && host==null) return "/";
				StringBuilder buf = new StringBuilder();
				AppendStoragePath(buf);
				buf.Append("/");
				return buf.ToString();
			}
		}

		public PathInfo GetFullPath() {
			return new PathInfo(Path.GetFullPath(this.ToString()));
		}

		public override string ToString() {
			if(this.drive!=null) {
				if(this.host!=null) {
					return this.drive+"@"+this.host+":"+this.location;
				} else {
					return this.drive+":"+this.location;
				}
			} else {
				if(this.host!=null) {
					return "@"+this.host+":"+this.location;
				} else {
					return this.location;
				}
			}
		}

	}

}
