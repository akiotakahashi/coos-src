/*
 * Copyright (c) 2002 Sergey Chaban <serge@wildwestsoftware.com>
 */

using System;

namespace CooS.CodeModels.CLI.Metadata {

	public class BadMetadataException : BadImageFormatException {

		public BadMetadataException() : base() {
		}

		public BadMetadataException(string msg) : base(msg) {
		}
	
	}

}
