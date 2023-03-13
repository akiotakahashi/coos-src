using System;
using System.IO;
using System.Collections;
using CooS.FileSystem;
using CooS.Drivers.StorageDevices.FDD;
using CooS.Drivers.ATAPI;
using CooS.Drivers.ATAPI.CDROM;

namespace CooS.Core {

	public class FileSystemManager {

		static readonly FileSystemProvider cfs = null;
		static readonly Hashtable fss = new Hashtable();
		static readonly Hashtable dev = new Hashtable();
		static readonly Hashtable archives = new Hashtable();

		static FileSystemManager() {
			cfs = new FileSystem.CooS.CoosFileSystem();
		}

		public static void Initialize() {
			RegisterFileSystem(cfs);
			RegisterFileSystem(new FileSystem.FAT.FATFileSystem());
			RegisterFileSystem(new FileSystem.CDFS.Iso9660());
			archives.Add("", cfs.Bind(null));
			Console.WriteLine("--- MOUNTING DEVICES ---");
			//MountStorage(RegisterStorage(new FloppyDisk(new FloppyDiskDrive(FloppyDiskController.Master,0))));
			ATAPIDevice dev = (ATAPIDevice)CooS.Machine.ATAPISecondaryController.Master;
			Storage media = new CdromDevice(dev).GetMedia();
			Console.WriteLine("Mounting device");
			MountStorage(RegisterStorage(media));
		}

		public static void RegisterFileSystem(FileSystemProvider fsp) {
			lock(fss.SyncRoot) {
				fss.Add(fsp.PreferredPrefix, fsp);
			}
		}

		public static void UnregisterFileSystem(string name) {
			lock(fss.SyncRoot) {
				fss.Remove(name);
			}
		}

		public static FileSystemProvider GetFileSystem(string name) {
			return (FileSystemProvider)fss[name.ToLower()];
		}

		public static string RegisterStorage(Storage storage) {
			lock(dev.SyncRoot) {
				for(int i=0; i<100; ++i) {
					string name = storage.PrefixName+i;
					if(!dev.ContainsKey(name)) {
						dev.Add(name, storage);
						return name;
					}
				}
			}
			throw new Exception("The number of devices having same prefix is maximum.");
		}

		public static void UnregisterStorage(string name) {
			lock(dev.SyncRoot) {
				dev.Remove(name);
			}
		}

		public static Storage GetStorage(string devname) {
			return (Storage)dev[devname.ToLower()];
		}

		public static void MountStorage(string name) {
			Storage storage = GetStorage(name);
			for(int i=0; i<storage.PartitionCount; ++i) {
				Partition partition = storage.GetPartition(i);
				foreach(FileSystemProvider fsp in fss.Values) {
					if(fsp.IsSuitable(partition)) {
						string drivename = name+(char)(i+'a');
						Archive archive = fsp.Bind(partition);
						archives.Add(drivename, archive);
						Console.WriteLine("Mount archive: {0}", drivename);
						return;
					}
				}
			}
			throw new SystemException();
		}

		/*
		public static Partition GetPartition(string name) {
			int order = name[name.Length-1]-'a';
			if(order<0 || order>25) throw new ArgumentException();
			Storage storage = GetStorage(name.Substring(0,name.Length-1));
			return storage.GetPartition(order);
		}
		*/

		public static Archive GetArchive(string storage) {
			if(storage==null) storage="";
			return (Archive)archives[storage];
		}

		public static DirectoryAspect GetRootDirectory(string storage) {
			foreach(Book book in FileSystemManager.GetArchive(storage).GetBooks()) {
				DirectoryAspect dir = (DirectoryAspect)book.QueryAspects(typeof(DirectoryAspect));
				if(dir!=null) return dir;
			}
			return null;
		}

		public static Book GetBook(PathInfo path) {
			path = path.GetFullPath();
			//Console.WriteLine("path = {0}", path);
			DirectoryAspect dir = GetArchive(path.Storage);
			if(dir==null) return null;
			if(path.IsRootDir) {
				return dir.OpenBook(PathInfo.RootDirectoryName);
			} else {
				string dirname = TrimSuffix(path.Location);
				string[] paths = dirname.Split(PathInfo.PathSeparatorChars);
				Book book = null;
				foreach(string name in paths) {
					if(dir==null) throw new DirectoryNotFoundException(name);
					book = dir.OpenBook(name);
					if(book==null) throw new DirectoryNotFoundException(name);
					dir = (DirectoryAspect)book.QueryAspects(typeof(DirectoryAspect));
				}
				return book;
			}
		}

		static string TrimSuffix(string path) {
			return path.TrimEnd(PathInfo.PathSeparatorChars);
		}

		public static BookInfo GetBookInfo(PathInfo path) {
			//Console.WriteLine("GetBookInfo({0})", path);
			path = path.GetFullPath();
			DirectoryAspect dir = GetArchive(path.Storage);
			if(dir==null) throw new DirectoryNotFoundException("Storage not found: "+path);
			if(path.IsRootDir) {
				return dir.GetBookInfo(PathInfo.RootDirectoryName);
			} else {
				string canpath = TrimSuffix(path.ToString());
				Book book = GetBook(new PathInfo(Path.GetDirectoryName(canpath)));
				dir = (DirectoryAspect)book.QueryAspects(typeof(DirectoryAspect));
				if(dir==null) throw new DirectoryNotFoundException(path.ToString());
				return dir.GetBookInfo(Path.GetFileName(canpath));
			}
		}

	}
}
