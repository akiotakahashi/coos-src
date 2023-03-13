using System;
using System.IO;
using System.Text;
using CooS.Formats.DLL;
using Mono.PEToolkit;
using PEImage=Mono.PEToolkit.Image;
using System.Collections.Generic;

namespace CooS.Formats.CLI.Metadata {
	using Heaps;

	public partial class MetadataRoot : IDisposable {

		public const uint SIGNATURE_BSJB = 0x424A5342;

		public readonly CorHeader Header;
		public readonly PEImage image;
		private readonly Stream stream;

		internal readonly long BasePosition;

		public readonly uint Signature;
		public readonly short MajorVersion;
		public readonly short MinorVersion;
		public readonly string VersionString;
		public readonly short Flags;
		private readonly Dictionary<string, HeapStream> streams = new Dictionary<string,HeapStream>(5);

		public readonly TablesHeap Tables;
		public readonly BlobHeap Blob;
		public readonly GuidHeap Guid;
		public readonly StringsHeap Strings;
		public readonly UserStringsHeap UserStrings;

		// value caches
		public readonly int BlobIndexSize = -1;
		public readonly int GuidIndexSize = -1;
		public readonly int StringsIndexSize = -1;

		public static MetadataRoot LoadMetadataFromFile(string filename) {
			PEImage image = new PEImage(filename);
			image.Open();
			image.ReadHeaders();
			CorHeader corhdr = new CorHeader(image);
			return new MetadataRoot(corhdr);
		}

		public static MetadataRoot LoadMetadataFromStream(Stream stream) {
			PEImage image = new PEImage(stream);
			image.Open();
			image.ReadHeaders();
			CorHeader corhdr = new CorHeader(image);
			return new MetadataRoot(corhdr);
		}

		internal void AlignPosition(BinaryReader reader) {
			long pos = reader.BaseStream.Position;
			pos -= this.BasePosition;
			pos = (pos+3)&~3;
			pos += this.BasePosition;
			reader.BaseStream.Position = pos;
		}

		internal MetadataRoot(CorHeader corhdr) {
			this.Header = corhdr;
			this.image = corhdr.Image;
			this.stream = corhdr.Stream;
			stream.Position = this.image.RVAToVA(corhdr.Metadata.VirtualAddress);
			this.BasePosition = stream.Position;

			BinaryReader reader = new BinaryReader(stream);
			this.Signature = reader.ReadUInt32();
#if DEBUG
			if(this.Signature != SIGNATURE_BSJB) {
				throw new BadImageException("Invalid Metadata Signature: 0x"+Signature.ToString("X08"));
			}
#endif

			this.MajorVersion = reader.ReadInt16();
			this.MinorVersion = reader.ReadInt16();
			/*reserved =*/ reader.ReadUInt32();
			int length = reader.ReadInt32();
			if(length>0) {
				this.VersionString = ImageUtility.ReadString(reader, length);
			} else {
				this.VersionString = string.Empty;
			}
			this.AlignPosition(reader);
			this.Flags = reader.ReadInt16();

			int count = reader.ReadInt16();
			for(int i=0; i<count; ++i) {
				HeapStream s = new HeapStream(this, reader);
				Console.WriteLine("stream: name={0}, size={1}", s.Name, s.Size);
				streams.Add(s.Name, s);
			}
			foreach(HeapStream mds in streams.Values) {
				reader.BaseStream.Position = this.BasePosition+mds.Offset;
				switch(mds.Name) {
				case "#~":
				case "#-":
					if(this.Tables!=null) { throw new BadImageException(); }
					this.Tables = new TablesHeap(this, mds, reader);
					break;
				case "#Blob":
					this.Blob = new BlobHeap(this, mds);
					break;
				case "#GUID":
					this.Guid = new GuidHeap(this, mds);
					break;
				case "#Strings":
					this.Strings = new StringsHeap(this, mds);
					break;
				case "#US":
					this.UserStrings = new UserStringsHeap(this, mds);
					break;
				}
			}
			// cache index sizes
			this.BlobIndexSize = this.Tables.BlobIndexSize;
			this.GuidIndexSize = this.Tables.GuidIndexSize;
			this.StringsIndexSize = this.Tables.StringsIndexSize;
		}

		#region IDisposable ƒƒ“ƒo

		public void Dispose() {
			this.stream.Dispose();
		}

		#endregion

		public string this[StringHeapIndex index] {
			get {
				return this.Strings[index];
			}
		}

		public Stream this[BlobHeapIndex index] {
			get {
				return this.Blob[index];
			}
		}

		public Guid this[GuidHeapIndex index] {
			get {
				return this.Guid[index];
			}
		}

#if true

		internal Stream OpenStream(long offset) {
			Stream stream = new CooS.IO.SharedStream(this.stream);
			stream.Position = BasePosition+offset;
			return stream;
		}

		internal Stream OpenStream(RVA rva) {
			Stream stream = new CooS.IO.SharedStream(this.stream);
			stream.Position = image.RVAToVA(rva);
			return stream;
		}

#endif

		public MethodIL GetMethodCode(Rows.MethodDefRow row) {
			if(row.RVA==0) { throw new ArgumentException(); }
			Stream stream = this.OpenStream(row.RVA);
			return new MethodIL(new BinaryReader(stream));
		}

	}
}
