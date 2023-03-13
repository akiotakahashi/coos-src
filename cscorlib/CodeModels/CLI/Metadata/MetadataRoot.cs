/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;
using System.IO;
using System.Text;
using System.Collections;
using CooS.CodeModels.DLL;
using Image = CooS.Execution.ExecutableImage;

namespace CooS.CodeModels.CLI.Metadata {

	public class MetadataRoot {

		private CorHeader corhdr;
		private Image image;
		private Stream stream;

		// file position of the first byte of the header
		private long filePos;

		/// <summary>
		/// "BSJB" signature.
		/// </summary>
		public static readonly uint Sig = 0x424A5342;

		// Metadata Root header, see 23.1.1
		private uint sig;
		private short majVer; // currently 1
		private short minVer; // currently 1
		private uint reserved;
		//private int len;
		private string verStr;
		private short flags;
		//private short nStreams;
		private readonly Hashtable streams = new Hashtable(5);

		private int strIdx;
		private int guidIdx;
		private int blobIdx;

		private TablesHeap heapTables;
		private BlobHeap heapBlob;
		private GuidHeap heapGuid;
		private StringsHeap heapStrings;
		private UserStringsHeap heapUserStrings;

		public MetadataRoot(CorHeader corhdr, Stream stream) {
			this.image = corhdr.Image;
			this.corhdr = corhdr;
			this.stream = stream;
			this.filePos = stream.Position;
			this.Read(new BinaryReader(stream));
		}

		unsafe public void Read(BinaryReader reader) {

			sig = reader.ReadUInt32();
			if (sig != Sig) throw new BadImageException("Invalid Metadata Signature.");

			majVer = reader.ReadInt16();
			minVer = reader.ReadInt16();
			reserved = reader.ReadUInt32();
			// Length of version string.
			int len = reader.ReadInt32();
			// Read version string.
			if(len>0) {
				long pos = reader.BaseStream.Position;
				verStr = ImageUtility.ReadString(reader, len);
				reader.BaseStream.Position = pos+len;
				// Round up to dword boundary, relative to header start.
				pos = reader.BaseStream.Position;
				pos -= BasePosition;
				pos = (pos+3)&~3;
				pos += BasePosition;
				// Advance file pointer.
				reader.BaseStream.Position = pos;
			} else {
				verStr = String.Empty;
			}
			// Read Flags
			flags = reader.ReadInt16();
			// load all streams into memory
			int nStreams = reader.ReadInt16();
			for(int i=0; i<nStreams; ++i) {
				MDStream s = new MDStream();
				s.Read(this, reader);
				streams.Add(s.Name, s);
			}
			foreach(MDStream stream in streams.Values) {
				reader.BaseStream.Position = this.BasePosition+stream.Offset;
				switch(stream.Name) {
				case "#~":
				case "#-":
					heapTables = new TablesHeap(this,stream,reader);
					break;
				case "#Strings":
					heapStrings = new StringsHeap(this,stream);
					break;
				case "#GUID":
					heapGuid = new GuidHeap(this,stream);
					break;
				case "#Blob":
					heapBlob = new BlobHeap(this,stream);
					break;
				case "#US":
					heapUserStrings = new UserStringsHeap(this,stream);
					break;
				}
			}
			// cache index sizes
			blobIdx = Tables.BlobIndexSize;
			guidIdx = Tables.GuidIndexSize;
			strIdx = Tables.StringsIndexSize;
		}

		public Stream OpenStream(long offset) {
			Stream stream = new CooS.IO.SharedStream(this.stream);
			stream.Position = BasePosition+offset;
			return stream;
		}

		public Stream OpenStream(RVA rva) {
			Stream stream = new CooS.IO.SharedStream(this.stream);
			stream.Position = image.RVAToVA(rva);
			return stream;
		}

		public long BasePosition {
			get {
				return filePos;
			}
		}

		public CorHeader Header {
			get {
				return this.corhdr;
			}
		}

		public uint Signature {
			get {
				return sig;
			}
		}

		public string Version {
			get {
				return String.Format("{0}.{1}", majVer, minVer);
			}
		}

		public string VersionString {
			get {
				return verStr;
			}
		}

		public int StringsIndexSize {
			get {
				return strIdx;
			}
		}

		public int GuidIndexSize {
			get {
				return guidIdx;
			}
		}

		public int BlobIndexSize {
			get {
				return blobIdx;
			}
		}

		public TablesHeap Tables {
			get {
				return heapTables;
			}
		}

		public BlobHeap Blob {
			get {
				return heapBlob;
			}
		}

		public GuidHeap Guid {
			get {
				return heapGuid;
			}
		}

		public StringsHeap Strings {
			get {
				return heapStrings;
			}
		}

		public UserStringsHeap UserStrings {
			get {
				return heapUserStrings;
			}
		}

	}
}
