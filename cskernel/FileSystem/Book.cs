using System;
using System.Collections;

namespace CooS.FileSystem {

	public interface Book {

		BookInfo BookInfo {get;}
		Aspect QueryAspects(Type aspect);

	}

}
