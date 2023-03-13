using System;
using CooS.Graphics;

namespace CooS.WindowSystem {

	public class WindowManager {

		public static Canvas display;

		static WindowManager() {
			display = new CooS.Graphics.Display();
		}

		public WindowManager() {
		}

	}

}
