using System;
using System.IO;

namespace CooS.IO {

	public class SharedStream : Stream {
	
		Stream stream;
		long position;
		
		public SharedStream(Stream stream) {
			if(stream is SharedStream) {
				this.stream = ((SharedStream)stream).stream;
			} else {
				this.stream = stream;
			}
			position = 0;
		}
		
		public override string ToString() {
			return "SharedStream/"+this.stream.ToString();
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
				return stream.Length;
			}
		}
		
		public override long Position {
			get {
				return position;
			}
			set {
				if(value<0 || this.stream.Length<value) {
					throw new ArgumentException("Setting Position to "+value+" over "+this.stream.Length);
				}
				position = value;
			}
		}
		
		public override int Read(byte[] buffer, int offset, int count) {
			lock(stream) {
				stream.Position = position;
				int io = stream.Read(buffer, offset, count);
				position = stream.Position;
				return io;
			}
		}
		
		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
			case SeekOrigin.Begin:
				this.Position = offset;
				break;
			case SeekOrigin.End:
				this.Position = Length+offset;
				break;
			case SeekOrigin.Current:
				this.Position += offset;
				break;
			default:
				throw new ArgumentException();
			}
			return position;
		}
		
		public override void SetLength(long value) {
			lock(stream) {
				stream.SetLength(value);
			}
		}
		
		public override void Write(byte[] buffer, int offset, int count) {
			lock(stream) {
				stream.Position = position;
				stream.Write(buffer, offset, count);
				position = stream.Position;
			}
		}

	}

}
