using System;

namespace CooS.Formats.CLI.Metadata {

	public class BadMetadataException : BadImageFormatException {

		public BadMetadataException() : base() {
		}

		public BadMetadataException(string msg) : base(msg) {
		}
	
	}

}
