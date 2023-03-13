using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CooS.Graphics;

unsafe class MyClass {

	static void Main(string[] args) {
		if(args.Length!=1) {
			Console.WriteLine("draw [1|2]");
			return;
		}
		Canvas display = new CooS.Graphics.Display();
		Painter painter = display.CreatePainter();
		Size dispsz = display.Size;
		Console.WriteLine();
		Console.WriteLine("Press any key to exit");
		switch(int.Parse(args[0])) {
		case 1:
			for(int deg = 0; Console.In.Peek()==-1; deg+=3) {
				double rag = Math.PI*deg/179;
				int radius = (int)(40+30*Math.Cos(rag*0.031415)+30*Math.Sin(rag*0.014142));
				int dx = (int)(radius*Math.Cos(rag));
				int dy = (int)(radius*Math.Sin(rag));
				int px = dispsz.Width/2+dx;
				int py = dispsz.Height/2+dy;
				Color c = Color.FromArgb((int)(127*Math.Cos(rag*2))+127, (int)(127*Math.Sin(rag/2))+127, (int)(127*Math.Sin(rag))+127);
				painter.DrawLine(dispsz.Width/2,dispsz.Height/2, px,py, c);
			}
			break;
		case 2:
			int[] sintbl = new int[360];
			for(int r=0; r<sintbl.Length; ++r) {
				sintbl[r] = (int)(256*256*Math.Sin(Math.PI*r/180));
			}
			int t = 0;
			int[] points = new int[dispsz.Width];
			while(Console.In.Peek()==-1) {
				for(int x=0; x<dispsz.Width/2; ++x) {
					display[x+dispsz.Width/4,points[x]] = Color.Black;
					int p = sintbl[(x*3+t*2)%360]*dispsz.Height/8;
					p >>= 16;
					points[x] = (int)(p*sintbl[(x+t*5)%360])>>16;
					points[x] += dispsz.Height/2;
					display[x+dispsz.Width/4,points[x]] = Color.White;
				}
				++t;
			}
			break;
		default:
			Console.WriteLine("Illegal number: {0}", args[0]);
			break;
		}
	}

}
