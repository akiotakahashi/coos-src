using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CooS.FileSystem.CDFS {
	using u32l = UInt32;
	using u32m = UInt32;
	using u16l = UInt16;
	using u16m = UInt16;

	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct VolumeDescriptor {
		public VolumeDescriptorBase VDB;
		public byte		VolumeFlags;			// Only SVD
		public byte[]	SystemIdentifier;
		public byte[]	VolumeIdentifier;
		public uint		UnusedField0;
		public uint		UnusedField1;
		public u32b		VolumeSpaceSize;
		public byte[]	EscapeSequences;		// Only SVD
		public u16b		VolumeSetSize;
		public u16b		VolumeSequenceNumber;
		public u16b		LogicalBlockSize;
		public u32b		PathTableSize;
		public u32l		LocationOfTypeLPathTable;
		public u32l		OptionalLocationOfTypeLPathTable;
		public u32m		LocationOfTypeMPathTable;
		public u32m		OptionalLocationOfTypeMPathTable;
		public DirectoryRecord RootDirectory;
		//public byte		RootDirectoryFileIdentifier;
		public byte[]	VolumeSetIdentifier;
		public byte[]	PublisherIdentifier;
		public byte[]	DataPreparerIdentifier;
		public byte[]	ApplicationIdentifier;
		public byte[]	CopyrightFileIdentifier;
		public byte[]	AbstractFileIdentifier;
		public byte[]	BibliographicFileIdentifier;
		public LongDateTime CreationDateTime;
		public LongDateTime ModificationDateTime;
		public LongDateTime ExpirationDateTime;
		public LongDateTime EffectiveDateTime;
		public byte		FileStructureVersion;
		public byte		Reserved1;
		public byte[]	ApplicationUse;
		//public byte Reserved2[653];

		public VolumeDescriptor(BinaryReader reader) {
			this.VDB = new VolumeDescriptorBase(reader);
			this.VolumeFlags = reader.ReadByte();
			this.SystemIdentifier = reader.ReadBytes(32);
			this.VolumeIdentifier = reader.ReadBytes(32);
			this.UnusedField0 = reader.ReadUInt32();
			this.UnusedField1 = reader.ReadUInt32();
			this.VolumeSpaceSize = new u32b(reader);
			this.EscapeSequences = reader.ReadBytes(32);
			this.VolumeSetSize = new u16b(reader);
			this.VolumeSequenceNumber = new u16b(reader);
			this.LogicalBlockSize = new u16b(reader);
			this.PathTableSize = new u32b(reader);
			this.LocationOfTypeLPathTable = reader.ReadUInt32();
			this.OptionalLocationOfTypeLPathTable = reader.ReadUInt32();
			this.LocationOfTypeMPathTable = reader.ReadUInt32();
			this.OptionalLocationOfTypeMPathTable = reader.ReadUInt32();
			this.RootDirectory = new DirectoryRecord(reader);
			//this.RootDirectoryFileIdentifier = reader.ReadByte();
			this.VolumeSetIdentifier = reader.ReadBytes(128);
			this.PublisherIdentifier = reader.ReadBytes(128);
			this.DataPreparerIdentifier = reader.ReadBytes(128);
			this.ApplicationIdentifier = reader.ReadBytes(128);
			this.CopyrightFileIdentifier = reader.ReadBytes(37);
			this.AbstractFileIdentifier = reader.ReadBytes(37);
			this.BibliographicFileIdentifier = reader.ReadBytes(37);
			this.CreationDateTime = new LongDateTime(reader);
			this.ModificationDateTime = new LongDateTime(reader);
			this.ExpirationDateTime = new LongDateTime(reader);
			this.EffectiveDateTime = new LongDateTime(reader);
			this.FileStructureVersion = reader.ReadByte();
			this.Reserved1 = reader.ReadByte();
			this.ApplicationUse = reader.ReadBytes(512);
		}

		public VolumeDescriptor(byte[] buf) : this(new BinaryReader(new MemoryStream(buf))) {
		}

	}

}
