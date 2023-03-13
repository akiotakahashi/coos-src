using System;
using System.IO;
using System.Collections;

namespace CooS.CodeModels.CLI.Signature {

	public abstract class SignatureUtility {

		public static CustomMod[] ReadCustomMods(SignatureReader reader) {
			ArrayList cmodlist = new ArrayList();
			while(CustomMod.Predict(reader)) {
				cmodlist.Add(new CustomMod(reader));
			}
			return (CustomMod[])cmodlist.ToArray(typeof(CustomMod));
		}

	}

}
