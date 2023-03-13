using System;
using CooS.Drivers.PS2;
using CooS.Drivers.PS2.Mouse;

namespace CooS.Shell {

	public class ShellMouse {
		public ShellMouse() {
			KeyboardController kbc = Architecture.KeyboardController;
			kbc.Mouse.OnReceive += new MouseEventHandler(mouse_OnReceive);
			kbc.Mouse.LetEnabled(true);
		}

		int x = 0;
		int y = 0;

		private void mouse_OnReceive(MouseData data, int dx, int dy, int dz) {
			x += dx;
			y += dy;
			Console.Write("\rX={0}, Y={1}, L={2}, R={3} ", x, y, data.LeftButtonPressed, data.RightButtonPressed);
		}

		/*
		private void mouse_OnReceive(MouseData data, int dx, int dy, int dz) {
			painter.DrawLine(0,my, dispsz.Width,my, Color.Black);
			painter.DrawLine(mx, 0, mx,dispsz.Height, Color.Black);
			mx += dx;
			my -= dy;
			if(mx<0) mx=0; else if(dispsz.Width<=mx) mx=dispsz.Width-1;
			if(my<0) my=0; else if(dispsz.Height<=my) my=dispsz.Height-1;
			painter.DrawLine(0,my, dispsz.Width,my, Color.Blue);
			painter.DrawLine(mx, 0, mx,dispsz.Height, Color.Blue);
		}
		*/

	}

}
