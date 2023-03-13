using System;
using System.IO;
using System.Collections;
using CooS.IO;
using CooS.FileSystem;
using CooS.Management;

namespace CooS.Wrap._System.IO {

	using SPath = Path;

	internal class _MonoIO {

		public static char PathSeparator {
			get {
				return ';';
			}
		}

		public static char DirectorySeparatorChar {
			get {
				return '/';
			}
		}

		public static char AltDirectorySeparatorChar {
			get {
				return '\\';
			}
		}
		
		public static char VolumeSeparatorChar {
			get {
				return ':';
			}
		}

		public static char[] InvalidPathChars {
			get {
				return new char[]{'<','>','\b','\t','\r','\n','\"'};
			}
		}

		public static IntPtr Open(string filepath, FileMode mode, FileAccess access, FileShare share, bool async, out MonoIOError error) {
			//Console.WriteLine("MonoIO:Open({0})", filepath);
			Book fbook = FileSystemManager.GetBook(new PathInfo(filepath));
			FileAspect file = (FileAspect)fbook.QueryAspects(typeof(FileAspect));
			if(file==null) {
				error = MonoIOError.ERROR_FILE_NOT_FOUND;
				return IntPtr.Zero;
			} else {
				FileImpl fileimpl = new FileImpl(file);
				error = MonoIOError.ERROR_SUCCESS;
				return fileimpl.Handle;
			}
		}

		public static bool Close(IntPtr handle, out MonoIOError error) {
			error = MonoIOError.ERROR_INVALID_PARAMETER;
			FileImpl file = FileImpl.FromHandle(handle);
			if(file==null) return false;
			error = MonoIOError.ERROR_SUCCESS;
			file.Dispose();
			return true;
		}

		public static int Read(IntPtr handle, byte[] dest, int dest_offset, int count, out MonoIOError error) {
			error = MonoIOError.ERROR_INVALID_PARAMETER;
			FileImpl file = FileImpl.FromHandle(handle);
			try {
				error = MonoIOError.ERROR_SUCCESS;
				return file.Read(dest, dest_offset, count);
			} catch {
				error = MonoIOError.ERROR_INVALID_PARAMETER;
				return 0;
			}
		}

		public static FileAttributes GetFileAttributes(string path, out MonoIOError error) {
			//Console.WriteLine("MonoIO:GetFileAttributes({0})", path);
			error = MonoIOError.ERROR_FILE_NOT_FOUND;
			PathInfo pi = new PathInfo(Path.GetFullPath(path));
			BookInfo bi = FileSystemManager.GetBookInfo(pi);
			if(bi!=null) {
				error = MonoIOError.ERROR_SUCCESS;
				return bi.Attributes;
			} else {
				return (FileAttributes)(-1);
			}
		}

		public static long GetLength(IntPtr handle, out MonoIOError error) {
			FileImpl file = FileImpl.FromHandle(handle);
			error = MonoIOError.ERROR_SUCCESS;
			return file.Length;
		}

		public static MonoFileType GetFileType(IntPtr handle, out MonoIOError error) {
			error = MonoIOError.ERROR_SUCCESS;
			return MonoFileType.Disk;
		}

		public static bool GetFileStat(string path, out MonoIOStat stat, out MonoIOError error) {
			error = MonoIOError.ERROR_FILE_NOT_FOUND;
			PathInfo pi = new PathInfo(Path.GetFullPath(path));
			BookInfo bi = FileSystemManager.GetBookInfo(pi);
			if(bi==null) {
				stat = new MonoIOStat();
				return false;
			} else {
				error = MonoIOError.ERROR_SUCCESS;
				stat = bi.ToMonoIOStat();
				return true;
			}
		}

		static int count = 0;
		static Hashtable filelists = new Hashtable();

		public static IntPtr FindFirstFile(string path, out MonoIOStat stat, out MonoIOError error) {
			//Console.WriteLine("MonoIO:FindFirstFile({0})", path);
			stat = new MonoIOStat();
			error = MonoIOError.ERROR_PATH_NOT_FOUND;

			PathInfo pi = new PathInfo(Path.GetDirectoryName(Path.GetFullPath(path)));
			Book book = FileSystemManager.GetBook(pi);
			if(book==null) {
				error = MonoIOError.ERROR_PATH_NOT_FOUND;
				return IntPtr.Zero;
			}

			DirectoryAspect dir = (DirectoryAspect)book.QueryAspects(typeof(DirectoryAspect));
			if(dir==null) {
				error = MonoIOError.ERROR_PATH_NOT_FOUND;
				return IntPtr.Zero;
			}

			IEnumerator list = dir.EnumBookInfo().GetEnumerator();
			if(!list.MoveNext()) {
				error = MonoIOError.ERROR_FILE_NOT_FOUND;
				stat = new MonoIOStat();
				return IntPtr.Zero;
			} else {
				IntPtr key;
				lock(typeof(Path)) {
					key = new IntPtr(count++);
					filelists[key] = list;
				}
				BookInfo bi = (BookInfo)list.Current;
				stat = bi.ToMonoIOStat();
				error = MonoIOError.ERROR_SUCCESS;
				return key;
			}
		}

		public static bool FindNextFile(IntPtr find, out MonoIOStat stat, out MonoIOError error) {
			IEnumerator list;
			lock(typeof(Path)) {
				list = (IEnumerator)filelists[find];
			}
			if(!list.MoveNext()) {
				error = MonoIOError.ERROR_FILE_NOT_FOUND;
				stat = new MonoIOStat();
				return false;
			} else {
				BookInfo bi = (BookInfo)list.Current;
				stat = bi.ToMonoIOStat();
				error = MonoIOError.ERROR_SUCCESS;
				return true;
			}
		}

		public static bool FindClose(IntPtr find, out MonoIOError error) {
			lock(typeof(Path)) {
				filelists.Remove(find);
			}
			error = MonoIOError.ERROR_SUCCESS;
			return true;
		}

		static string curdir = "";

		public static string GetCurrentDirectory(out MonoIOError error) {
			error = MonoIOError.ERROR_SUCCESS;
			return curdir;
		}

		public static bool SetCurrentDirectory(string path, out MonoIOError error) {
			error = MonoIOError.ERROR_SUCCESS;
			curdir = path;
			return true;
		}

	}

}
