using System;
using System.IO;

namespace CooS.IO {

	public unsafe class RawMemoryStream : System.IO.Stream {
	
		byte* data;
		long length;
		long position;
		
		public RawMemoryStream(IntPtr start, uint size) {
			data = (byte*)start.ToPointer();
			length = size;
			position = 0;
		}
			
		public override string ToString() {
			return "RawMemoryStream(0x"+((int)data).ToString("X8")+")";
		}

		public override bool CanRead {
			get {
				return true;
			}
		}
		public override bool CanSeek {
			get {
				return true;
			}
		}
		public override bool CanWrite {
			get {
				return true;
			}
		}
		public override void Flush() {
			// NOP
		}
		public override long Length {
			get {
				return length;
			}
		}
		public override long Position {
			get {
				return position;
			}
			set {
				position = value;
			}
		}
		public unsafe override int Read(byte[] buffer, int offset, int count) {
			if(position+count>length) count=(int)(length-position);
			if(offset+count>buffer.Length) throw new ArgumentException();
			fixed(byte* p = &buffer[0]) {
				CooS.Tuning.Memory.Copy(p+offset, data+position, (uint)count);
			}
			return count;
		}
		public override long Seek(long offset, System.IO.SeekOrigin origin) {
			switch(origin) {
			case SeekOrigin.Begin:
				position = offset;
				break;
			case SeekOrigin.End:
				position = length+offset;
				break;
			case SeekOrigin.Current:
				position += offset;
				break;
			default:
				throw new ArgumentException();
			}
			return position;
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		public override void Write(byte[] buffer, int offset, int count) {
			if(position+count>length) throw new IOException();
			if(offset+count>buffer.Length) throw new ArgumentException();
			fixed(byte* p = &buffer[0]) {
				CooS.Tuning.Memory.Copy(data+position, p+offset, (uint)count);
			}
		}
	}
}
