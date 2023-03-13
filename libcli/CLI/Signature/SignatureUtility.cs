using System;
using System.Collections.Generic;

namespace CooS.Formats.CLI.Signature {

	abstract class SignatureUtility {

		public static CustomMod[] ReadCustomMods(SignatureReader reader) {
			List<CustomMod> cmodlist = new List<CustomMod>();
			while(CustomMod.Predict(reader)) {
				cmodlist.Add(new CustomMod(reader));
			}
			return cmodlist.ToArray();
		}

	}

}
