using System;
using System.IO;

namespace CooS.IO {

	public class BoundStream : Stream {

		Stream stream;
		long begin;
		long length;

		public BoundStream(Stream stream, long begin, long length) {
			this.stream = stream;
			this.begin = begin;
			this.length = length;
			this.stream.Position = begin;
		}
		
		public override string ToString() {
			return "BoundStream/"+this.stream.ToString();
		}

		public override bool CanRead {
			get {
				return stream.CanRead;
			}
		}
		
		public override bool CanSeek {
			get {
				return stream.CanSeek;
			}
		}
		
		public override bool CanWrite {
			get {
				return stream.CanWrite;
			}
		}
		
		public override void Flush() {
			stream.Flush();
		}
		
		public override long Length {
			get {
				return length;
			}
		}
		
		public override long Position {
			get {
				return stream.Position-begin;
			}
			set {
				stream.Position = value+begin;
			}
		}
		
		public override int Read(byte[] buffer, int offset, int count) {
			lock(stream) {
				long position = this.Position;
				if(position+count>length) count=(int)(length-position);
				return stream.Read(buffer, offset, count);
			}
		}
		
		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
			case SeekOrigin.Begin:
				this.Position = offset;
				break;
			case SeekOrigin.End:
				this.Position = length+offset;
				break;
			case SeekOrigin.Current:
				this.Position += offset;
				break;
			default:
				throw new ArgumentException();
			}
			return this.Position;
		}
		
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}

	}

}
