using System;

namespace CooS.FileSystem {

	public interface Partition {

		Storage Storage {get;}
		int BlockSize {get;}
		long BlockCount {get;}
		void Read(byte[] buffer, int index, long start, int count);

	}

}
