using System;
using System.Collections;

namespace CooS.FileSystem {

	public interface DirectoryAspect : Aspect {
		BookInfo GetBookInfo(string name);
		IEnumerable EnumBookInfo();
		Book OpenBook(string name);
	}

}
