using System;

namespace CooS.Formats.CLI.Metadata {

	public struct BlobHeapIndex {

		public readonly int RawIndex;

		internal BlobHeapIndex(int value) {
			this.RawIndex = value;
		}

	}

}
