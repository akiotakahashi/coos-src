using System;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using CooS.Graphics;
using CooS.Management;
using CooS.Reflection;

namespace CooS.Shell {

	public class CommandShell : ShellBase {

		Canvas display;
		Painter painter;
		Size dispsz;

		public static void Startup() {
			Console.WriteLine("CONFIRM STARTUP");
			CommandShell shell = new CommandShell();
			shell.Engage();
		}

		public CommandShell() {
			if(Engine.Privileged) {
				display = new CooS.Graphics.Display();
			} else {
				display = new CooS.Graphics.Bitmap(PixelFormat.Format24bppRgb,800,600);
			}
			painter = display.CreatePainter();
			dispsz = display.Size;
		}

		public override object Execute() {
			return null;
		}

		const int pen_x_init = 60;
		const int pen_y_init = 86;
		int pen_x = pen_x_init;
		int pen_y = 128;

		private void DrawText(CooS.FontSystem.Font font, string text) {
			foreach(char ch in text) {
				switch(ch) {
				case '\n':
					if(pen_y+this.font.Height*3>=this.dispsz.Height) {
						this.display.Blt(0, pen_y_init, this.display
							, 0, pen_y_init+this.font.Height
							, this.dispsz.Width, this.dispsz.Height-pen_y_init-this.font.Height);
						this.painter.FillRect(0, this.pen_y, this.dispsz.Width, this.dispsz.Height-this.pen_y, Color.Black);
					} else {
						pen_y += font.Height;
					}
					break;
				case '\r':
					pen_x = pen_x_init;
					break;
				case '\t':
					int c = (this.pen_x-pen_x_init)/(this.font.Height*2);
					this.pen_x = pen_x_init+(c+1)*(this.font.Height*2);
					return;
				default:
					if(this.pen_x+this.font.Height>=this.dispsz.Width) {
						this.DrawText(this.font, "\n\r");
					}
					Size sz = font.GetBearingSize(ch);
					pen_x += font.Draw(ch, display, pen_x+sz.Width, pen_y+(this.font.Height-sz.Height));
					break;
				}
			}
		}

		public static byte[] ReadFile(string filename) {
			byte[] buf;
			using(FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
				buf = new byte[stream.Length];
				stream.Read(buf, 0, buf.Length);
			}
			return buf;
		}

		CooS.FontSystem.Font font;

		public override void Output(char ch) {
			switch(ch) {
			case '\n':
				this.Output('\r');
				break;
			}
			if(CooS.Drivers.DisplayAdapters.VBE.ModeInfoBlock.Current.PhysBasePtr==IntPtr.Zero) {
				Console.Write(ch);
			}
			this.DrawText(font, ch.ToString());
		}

		public void DrawInitialLine(int progress) {
			this.painter.DrawLine(dispsz.Width-progress*2, 24, dispsz.Width-progress*2,dispsz.Height-1, Color.DarkGray);
		}

		public void Engage() {
			//painter.DrawLine(0,dispsz.Height/2, dispsz.Width-1,dispsz.Height/2, Color.DarkGray);
			//Engine.GenerateNativeCode(painter.GetType().GetMethod("DrawLine"));
			Directory.SetCurrentDirectory("cd0a:/");

			//
			// フォントを読み込みます。
			//
			this.DrawInitialLine(1);
			Console.WriteLine("READING FONT");
			byte[] buf = ReadFile("ipag.ttf");
			
			//
			// フォントをロードします。
			//
			this.DrawInitialLine(2);
			Console.WriteLine("LOADING FONT");
			font = new CooS.FontSystem.Font(buf, 15);
			
			//
			// フォントをレンダリングします。
			//
			this.DrawInitialLine(3);
			Console.WriteLine("RENDERING...");
			this.DrawText(font, "LOADING FONT COMPLETED\r\n");
			/*
			Console.WriteLine("RENDERING FONT");
			this.DrawText(font, "テキスト出力のためにフォントをレンダリングしています…\r\n");
			this.DrawText(font, "ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\n");
			this.DrawText(font, "abcdefghijklmnopqrstuvwxyz\r\n");
			this.DrawText(font, "\r\n");
			*/

			//
			this.DrawInitialLine(4);
			Console.WriteLine("INITIALIZE KEYBOARD");
			this.DrawText(font, "Initialize console devices\r\n");
			ShellInput input = new ShellInput(this);
			ShellOutput output = new ShellOutput(this);
			this.DrawText(font, "Associate standard I/O with console\r\n");
			if(CooS.Drivers.DisplayAdapters.VBE.ModeInfoBlock.Current.PhysBasePtr!=IntPtr.Zero) {
				Console.SetError(output);
				Console.SetOut(output);
			}
			Console.SetIn(input);
			//
			Console.WriteLine("Load primitive commands");
			this.ProcessCommandLine("load ls.exe");
			this.ProcessCommandLine("load cat.exe");
			//
			this.ProcessCommandLine("clear");
			Console.WriteLine("CooS [Version {0}]", AssemblyResolver.ResolveAssembly("cscorlib",true).GetName().Version);
			Console.WriteLine();
			Console.WriteLine("Type '?' to show operation guide.");
			//
			System.Text.StringBuilder line = new System.Text.StringBuilder();
			Console.Write("{0}> ", Directory.GetCurrentDirectory());
			for(;;) {
				char ch = (char)Console.Read();
				switch(ch) {
				default:
					line.Append(ch);
					this.Output(ch);
					break;
				case '\b':
					if(line.Length>0) {
						this.Output('^');
						line.Remove(line.Length-1,1);
					}
					break;
				case '\n':
					this.Output('\n');
					string cmdline = line.ToString().Trim();
					line = new System.Text.StringBuilder();
					if(cmdline.Length>0) {
						ProcessCommandLine(cmdline);
					}
					Console.Write("{0}> ", Directory.GetCurrentDirectory());
					break;
				}
			}		
		}

		private Assembly LoadAssemblyFile(string filepath) {
			AssemblyBase assembly = Engine.LoadAssembly(ReadFile(filepath));
			AssemblyName name = assembly.GetName(false);
			Console.WriteLine("OK: {0} (Version {1})", name.Name, name.Version);
			return assembly.RealAssembly;
		}

		private void ExecuteAssembly(Assembly assembly, string[] args) {
			if(assembly==null) throw new ArgumentNullException();
			MethodInfo method = assembly.EntryPoint;
			if(method==null) {
				Console.WriteLine("No entrypoint");
				return;
			}
			assembly.EntryPoint.Invoke(null, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static, null, new object[]{args}, null);
		}

		private void ProcessCommandLine(string cmdline) {
			cmdline = cmdline.Trim();
			int i = cmdline.IndexOf(' ');
			string cmd;
			string[] args;
			if(i<0) {
				cmd = cmdline;
				args = new string[0];
			} else {
				cmd = cmdline.Substring(0,i);
				cmdline = cmdline.Substring(i+1).TrimStart();
				args = cmdline.Split(' ','\t');
			}
			switch(cmd) {
			default:
				if(cmd.ToLower().EndsWith(".exe") && File.Exists(cmd)) {
					this.ExecuteAssembly(this.LoadAssemblyFile(cmd), args);
				} else {
					Assembly assembly = AssemblyResolver.ResolveAssembly(cmd, false);
					if(assembly!=null) {
						this.ExecuteAssembly(assembly, args);
					} else {
						Console.WriteLine("Invalid command: "+cmd);
					}
				}
				break;
			case "?":
				if(args.Length==0) {
					Console.WriteLine(@"CooS Command-based Shell

Available inner commands:

?		: Show this help
cd		: Change Directory
		  Set current directory to specified one.
ls		: List file entries
		  Show list of files in specified directory.
cat		: Print contents to console
		  You already know this as UNIX cat.

How to use:

You can run a executable assembly (.exe file) by simply type it's whole filename.
In details, this operation can separate into two prcesses.
	1) Load assembly by 'load' command.
	   When CooS finished successfully, loaded assembly is registered into CooS Assembly Repository.
	2) Execute the assembly with arguments (if needed).

CooS Command-based Shell trys to resolve undefined command as a Assembly Name.
If there is an Assembly having the name in CooS Assembly Repository, the shell execute it.
So you must type whole filename to execute it first, but secondary type assembly name simply to execute.
ex.		cd0a:/> application.exe		(OK)
		cd0a:/> application			(OK) ");
				} else {
					Console.WriteLine("Help integration not supported");
				}
				break;
			case "cd":
				if(args.Length!=1) {
					Console.WriteLine("cd <dirpath>");
				} else {
					string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), args[0]));
					if(!Directory.Exists(path)) {
						Console.WriteLine("Directory not found");
					} else {
						Directory.SetCurrentDirectory(path);
					}
				}
				break;
			case "load":
				if(args.Length!=1) {
					Console.WriteLine("load <filepath>");
				} else {
					this.LoadAssemblyFile(args[0]);
				}
				break;
			case "clear":
				this.pen_y = pen_y_init;
				this.painter.FillRect(0, this.pen_y, 98*this.dispsz.Width/100, this.dispsz.Height-this.pen_y, Color.Black);
				Console.WriteLine();
				break;
			case "font":
				if(args.Length!=2) {
					Console.WriteLine("font <font filename> <pixel height>");
				} else {
					if(!File.Exists(args[0])) {
						Console.WriteLine("No such file: {0}", args[0]);
					} else {
						Console.Write("Reading font file...");
						byte[] buf = ReadFile(args[0]);
						Console.WriteLine("OK");
						Console.Write("Loading font...");
						this.font = new CooS.FontSystem.Font(buf, int.Parse(args[1]));
						this.ProcessCommandLine("clear");
					}
				}
				break;
			}
		}

	}

}
