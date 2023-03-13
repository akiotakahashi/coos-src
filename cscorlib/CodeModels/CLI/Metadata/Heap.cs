/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
using CooS.IO;

namespace CooS.CodeModels.CLI.Metadata {

	/// <summary>
	/// Base class for all metadata heaps.
	/// </summary>
	public abstract class Heap {

		private readonly MetadataRoot mdroot;
		private readonly MDStream stream;

		protected Heap(MetadataRoot mdroot, MDStream stream) {
			this.mdroot = mdroot;
			this.stream = stream;
		}

		public MetadataRoot Root {
			get {
				return mdroot;
			}
		}

		public MDStream Stream {
			get {
				return stream;
			}
		}

		public Stream OpenStream(long offset) {
			return this.mdroot.OpenStream(this.stream.Offset+offset);
		}

		public static int ReadCompressedInt(Stream stream) {
			int fst = stream.ReadByte();
			if(fst<0) throw new IOException("First byte is -1/"+stream.Position);
			if((fst&0x80)==0) {
				return fst;
			} else {
				int snd = stream.ReadByte();
				if(snd<0) throw new IOException("Second byte is -1");
				if((fst&0x40)==0) {
					return snd | ((fst&0x3F)<<8);
				} else if((fst&0x20)==0) {
					int trd = stream.ReadByte();
					int fth = stream.ReadByte();
					if(trd<0) throw new IOException("Third byte is -1");
					if(fth<0) throw new IOException("Fourth byte is -1");
					return fth | (trd<<8) | (snd<<16) | ((fst&0x1F)<<24);
				} else {
					throw new FormatException();
				}
			}
		}

		public Stream OpenData(int rowIndex) {
			Stream stream = OpenStream(rowIndex);
			int length = ReadCompressedInt(stream);
			long start = stream.Position;
			return new BoundStream(stream,start,length);
		}

	}
}
