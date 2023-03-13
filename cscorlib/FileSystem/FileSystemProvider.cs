using System;

namespace CooS.FileSystem {

	public abstract class FileSystemProvider {

		public abstract string Name {get;}
		public abstract string PreferredPrefix {get;}
		public abstract Archive Bind(Partition partition);
		public abstract bool IsSuitable(Partition partition);

	}

}
