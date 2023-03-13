using System;
using System.IO;

namespace CooS.FileSystem {

	public interface FileAspect : Aspect {

		long Length {get;}
		int ClusterSize {get;}
		void Read(byte[] buffer, int index, long start, int count);

	}

}
