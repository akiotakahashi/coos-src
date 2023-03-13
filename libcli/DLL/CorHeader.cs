using System;
using System.IO;
using Mono.PEToolkit;
using PEImage = Mono.PEToolkit.Image;
using RowToken = CooS.Formats.CLI.Metadata.RowToken;

namespace CooS.Formats.DLL {

	public class CorHeader {
		
		public readonly PEImage Image;
		public readonly Stream Stream;

		public readonly uint ContentBytes;
		public readonly short MajorRuntimeVersion;
		public readonly short MinorRuntimeVersion;

		public readonly DataDir Metadata;
		public readonly CorFlags Flags;
		public readonly RowToken EntryPointToken;
		public readonly DataDir Resources;
		public readonly DataDir StrongNameSignature;
		public readonly DataDir CodeManagerTable;
		public readonly DataDir VTableFixups;
		public readonly DataDir ExportAddressTableJumps;
		public readonly DataDir EEInfoTable;
		public readonly DataDir HelperTable;
		public readonly DataDir DynamicInfo;
		public readonly DataDir DelayLoadInfo;
		public readonly DataDir ModuleImage;
		public readonly DataDir ExternalFixups;
		public readonly DataDir RidMap;
		public readonly DataDir DebugMap;
		public readonly DataDir IpMap;

		public CorHeader(PEImage image) {
			if(!image.IsCLI) { throw new ArgumentException(); }
			// Foundation
			this.Image = image;
			this.Stream = image.Stream;
			this.Stream.Position = image.RVAToVA(image.Headers.CLIHdrDir.VirtualAddress);
			BinaryReader reader = new BinaryReader(Stream);
			// Headers
			this.ContentBytes = reader.ReadUInt32();
			this.MajorRuntimeVersion = reader.ReadInt16();
			this.MinorRuntimeVersion = reader.ReadInt16();
			// Directories
			this.Metadata = new DataDir(reader);
			this.Flags = (CorFlags)reader.ReadUInt32();
			this.EntryPointToken = new RowToken(reader.ReadUInt32());
			this.Resources  = new DataDir(reader);
			this.StrongNameSignature = new DataDir(reader);
			this.CodeManagerTable = new DataDir(reader);
			this.VTableFixups = new DataDir(reader);
			this.ExportAddressTableJumps = new DataDir(reader);
			this.EEInfoTable = new DataDir(reader);
			this.HelperTable = new DataDir(reader);
			this.DynamicInfo = new DataDir(reader);
			this.DelayLoadInfo = new DataDir(reader);
			this.ModuleImage = new DataDir(reader);
			this.ExternalFixups = new DataDir(reader);
			this.RidMap = new DataDir(reader);
			this.DebugMap = new DataDir(reader);
			this.IpMap = new DataDir(reader);
		}

		public uint Size {
			get {
				return this.ContentBytes;
			}
		}

	}

}
