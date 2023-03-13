using System;
using System.IO;
using System.Collections;
using CooS.FileSystem;

namespace CooS.IO {

	public class FileImpl : IDisposable {

		//FIXME: Use weak GCHandle instead of object-reference.
		static readonly Hashtable handles = new Hashtable();
		static int handlecounter = 0;
		IntPtr handle = IntPtr.Zero;
		FileAspect page;
		long position;
		byte[] clusterbuf;
		int consumedsize;

		static IntPtr RegisterFile(FileImpl file) {
			IntPtr handle;
			while(true) {
				lock(typeof(FileImpl)) {
					handle = new IntPtr(++handlecounter);
					try {
						handles.Add(handle, file);
						break;
					} catch(ArgumentException) {
					}
				}
			}
			return handle;
		}

		static void UnregisterFile(IntPtr handle) {
			lock(typeof(FileImpl)) {
				handles.Remove(handle);
			}
		}

		public static FileImpl FromHandle(IntPtr handle) {
			lock(typeof(FileImpl)) {
				return (FileImpl)handles[handle];
			}
		}

		public FileImpl(FileAspect page) {
			this.page = page;
			handle = RegisterFile(this);
			clusterbuf = new byte[this.page.ClusterSize];
			this.consumedsize = clusterbuf.Length;
		}

		~FileImpl() {
			Dispose();
		}

		#region IDisposable メンバ

		public void Dispose() {
			if(handle!=IntPtr.Zero) {
				UnregisterFile(handle);
				handle = IntPtr.Zero;
			}
		}

		#endregion

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		public long Length {
			get {
				return this.page.Length;
			}
		}

		public long Seek() {
			throw new NotImplementedException();
		}

		public int Read(byte[] buffer, int index, int size) {
			if(size<0) throw new ArgumentOutOfRangeException("size");
			int bpc = page.ClusterSize;
			int io = this.clusterbuf.Length-consumedsize;
			if(io>0) {
				if(size<io) io=size;
				Buffer.BlockCopy(this.clusterbuf,this.consumedsize,buffer,index,io);
				consumedsize += io;
				size -= io;
				if(size<=0) return io;
			}
			if(position*bpc+size>=this.page.Length) {
				// 領域外読み込みの場合はサイズを調整
				size = (int)(page.Length-position*bpc);
				if(size<=0) return 0;	// 全部読み込み終ってる
			}
			int count = size/bpc;
			page.Read(buffer, index+io, position, count);
			position += count;
			io += count*bpc;
			size -= count*bpc;
			if(size<=0) {
				consumedsize = clusterbuf.Length;
			} else {
				page.Read(clusterbuf, 0, position, 1);
				++position;
				Buffer.BlockCopy(clusterbuf,0,buffer,index+io,size);
				consumedsize = size;
				io += size;
			}
			return io;
		}

	}

}
