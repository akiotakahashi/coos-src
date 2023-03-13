using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CooS;
using CooS.Graphics;
using CooS.Reflection;
using CooS.FileSystem;
using CooS.Management;
using CooS.CodeModels;
using CooS.Architectures;
using CooS.Execution;
using CooS.FileSystem.FAT;
using CooS.FileSystem.CDFS;
using CooS.Drivers.StorageDevices.FDD;
using CooS.Drivers.Controllers;
using CooS.Drivers.PS2;
using CooS.Drivers.ATAPI;

unsafe class MyClass {

	static void Main(string[] args) {
		foreach(string arg in args) {
			Console.WriteLine("arg: {0}", arg);
		}
		Console.WriteLine("Executing assembly is "+Assembly.GetExecutingAssembly().FullName);
		if(Assembly.GetCallingAssembly()!=null) {
			Console.WriteLine("Calling assembly is "+Assembly.GetCallingAssembly().FullName);
		}
		Console.WriteLine("----<application>---------------------------------------------------------");
		MyClass me = new MyClass();
		me.Test(args);
		Console.WriteLine();
		Console.WriteLine("All examination were completed.");
	}

	private MyClass() {
	}

	private void TestThread() {
	}

	private void TestMethod() {
		long l1, l2;
		l1 = -1;
		l2 = 1;
		int i1;
		i1 = -1;
		if(l1+l2!=0) throw new UnexpectedException();
		if(l1-l2!=-2) throw new UnexpectedException();
		if(l2-l1!=2) throw new UnexpectedException();
		if(-1!=(int)l1) throw new UnexpectedException();
		if(l1!=i1) throw new UnexpectedException();
	}

	private void Test(string[] args) {
		if(!Engine.Infrastructured) {
			AssemblyResolver.Manager.LoadAssembly(@"D:\Repository\clios\cdimage\mscorlib.dll");
			AssemblyResolver.Manager.LoadAssembly(@"D:\Repository\clios\cskorlib\Release\cskorlib.dll");
			AssemblyResolver.Manager.LoadAssembly(@"D:\Repository\clios\cscorlib\bin\Release\cscorlib.dll");
			AssemblyResolver.Manager.LoadAssembly(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
			AssemblyResolver.Manager.Setup();
		}
		AssemblyResolver.Manager.ResolveType("CooS.CodeModels.CLI.Metatype.ClassType",true).PrepareSlots();
		AssemblyResolver.Manager.ResolveType("System.Collections.Hashtable",true).PrepareSlots();
		CodeManager manager = new CodeManager();
		/*
		manager.Prepare("System.Engine:TestMethod");
		manager.Prepare("System.Array:CreateInstance");
		manager.Prepare("System.Collections.Hashtable:get_Item");
		manager.Prepare("System.Collections.Hashtable:get_Count");
		manager.Prepare("System.Collections.Hashtable:KeyEquals");
		manager.Prepare("CooS.CodeModels.CLI.MethodDefInfo:GenerateExecutableCode");
		manager.Prepare("CooS.CodeModels.Assembler:AllocateObjectImpl");
		manager.Prepare("CooS.CodeModels.CLI.AssemblyDef:ResolveType");
		manager.Prepare("CooS.CodeModels.CLI.AssemblyDef:LoadFieldDef");
		manager.Prepare("CooS.CodeModels.CLI.Metatype.ConcreteType:LayoutInstanceFields");
		manager.Prepare("CooS.CodeModels.CLI.Metadata.TypeDefTable:get_Item");
		manager.Prepare("CooS.Reflection.AssemblyManager:GetAssembly");
		manager.Prepare("CooS.Reflection.AssemblyManager:ResolveType");
		manager.Prepare("CooS.Management.AssemblyResolver:GetAssemblyInfo");
		manager.Prepare("CooS.Memory:AllocateString");
		manager.Prepare("CooS.Memory:AllocateBlock");
		manager.Prepare("CooS.Memory:AllocateArray");
		manager.Prepare("CooS.Tuning.Memory:Clear");
		*/
		/*
		manager.Prepare("CooS.CodeModels.Assembler:LoadStringImpl");
		*/

		CodeManager.MakeBlankMethod("System.Array:CreateInstance", new Type[]{typeof(Type),typeof(int[])});
		CodeManager.MakeBlankMethod("System.Char:IsLetter");
		CodeManager.MakeBlankMethod("System.Char:IsDigit");
		CodeManager.MakeBlankMethod("System.Char:IsWhiteSpace");
		CodeManager.MakeBlankMethod("System.Char:ToLowerInvariant");
		CodeManager.MakeBlankMethod("System.Char:ToUpperInvariant");

		//manager.Recursive = true;
		manager.Prepare("System.IntPtr:.ctor");
		manager.Prepare("System.IntPtr:op_Explicit");
		manager.Prepare("CooS.Reflection.TypeImpl");
		manager.Prepare("CooS.Reflection.FieldInfoImpl");
		manager.Prepare("CooS.Reflection.MethodInfoImpl");
		manager.Prepare("CooS.CodeModels.CLI.FieldDefInfo");
		manager.Prepare("CooS.CodeModels.CLI.MethodDefInfo");
		CodeManager.Trap = true;
		CodeManager.Trap = false;
		manager.Prepare("CooS.Execution.CodeManager:Prepare");
		manager.Prepare("CooS.Execution.CodeManager:CompileTest");
		manager.Prepare("CooS.Execution.CodeManager:CompileTest2");
		manager.Recursive = false;
		manager.Apply();
		// 一度コンパイルを通す
		Engine.ReloadMethodCode("CooS.Execution.CodeManager:CompileTest");
		Engine.ReloadMethodCode("CooS.Execution.CodeManager:CompileTest2");
		CooS.Execution.CodeManager.CompileTest();
		CooS.Execution.CodeManager.CompileTest2();
		// 残ったメソッドをコンパイルして再リンク
		CodeManager.CompileAllProxyMethods();
		CodeManager.Trap = true;
		CodeManager.Trap = false;
		CodeManager.RelinkAllCodes();
		//
		Memory.PrepareSlotsOfAll();
		// コンパイル
		Engine.ReloadMethodCode("CooS.Execution.CodeManager:Prepare");
		manager.Prepare("CooS.Initializer:Startup");
		/*
		if(Engine.Native) {
			ATAPIController second = Architecture.ATAPISecondaryController;
			CooS.Drivers.ATAPI.DeviceBase device = second.Master;
			Console.WriteLine("Device Features : 0x{0:X}", (int)device.DetermineDeviceFeatures());
			Console.WriteLine("Device MediaType: 0x{0:X}", (int)device.GetMediaType());
			//Console.WriteLine("Device MediaAttr: 0x{0:X}", (int)device.GetMediaAttributes());
			switch(device.GetMediaType()) {
			case MediaType.CDROM:
				CooS.Drivers.ATAPI.CDROM.CdromDevice drive = new CooS.Drivers.ATAPI.CDROM.CdromDevice((ATAPIDevice)device);
				Partition media = drive.GetMedia().GetPartition(0);
				Iso9660 fs = new Iso9660();
				DirectoryAspect dir = (DirectoryAspect)fs.Bind(media).GetBooks()[0].QueryAspects(typeof(DirectoryAspect));
				foreach(BookInfo bi in dir.EnumBookInfo()) {
					Console.WriteLine("{0,-32} {1,8} bytes", bi.Name, bi.Length);
				}
				Book book = dir.OpenBook("alice.txt");
				FileAspect file = (FileAspect)book.QueryAspects(typeof(FileAspect));
				Console.Write("Reading ALICE.TXT ({0} bytes) ...", file.Length);
				int size = ((int)file.Length+file.ClusterSize-1)&~(file.ClusterSize-1);
				byte[] buf = new byte[size];
				file.Read(buf, 0, 0, buf.Length/file.ClusterSize);
				Console.WriteLine("OK");
				string txt = System.Text.Encoding.ASCII.GetString(buf, 0, (int)file.Length);
				Console.WriteLine(txt);
				break;
			}
		}
		//*/
		/*
		using(StreamReader reader = new StreamReader("cd0a:/alice.txt", System.Text.Encoding.ASCII)) {
			string line;
			while((line=reader.ReadLine())!=null) {
				//Console.WriteLine(line);
			}
		}
		Console.WriteLine("COMPLETE");
		//*/
		/*
		if(Engine.Native) {
			mx = dispsz.Width/2;
			my = dispsz.Height/2;
			KeyboardController kbc = Architecture.KeyboardController;
			PS2Keyboard keyboard = kbc.Keyboard;
			PS2Mouse mouse = kbc.Mouse;
			keyboard.OnReceive += new KeyboardEventHandler(keyboard_OnReceive);
			mouse.OnReceive += new MouseEventHandler(mouse_OnReceive);
			keyboard.LetEnabled(true);
			mouse.LetEnabled(true);
		}
		//*/
		/*
		foreach(FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Static|BindingFlags.Public)) {
			OpCode op = (OpCode)fi.GetValue(null);
			Console.WriteLine("{0}: optype={1}", op.Name, (int)op.OperandType);
		}
		Console.WriteLine("{0}: optype={1}", OpCodes.Add.Name, (int)OpCodes.Add.OperandType);
		Console.WriteLine("{0}: optype={1}", OpCodes.Ldarg.Name, (int)OpCodes.Ldarg.OperandType);
		Console.WriteLine("{0}: optype={1}", OpCodes.Ldloc.Name, (int)OpCodes.Ldloc.OperandType);
		Console.WriteLine("{0}: optype={1}", OpCodes.Ldc_I4_S.Name, (int)OpCodes.Ldc_I4_S.OperandType);
		Console.WriteLine("{0}: optype={1}", OpCodes.Starg.Name, (int)OpCodes.Starg.OperandType);
		Console.WriteLine("{0}: optype={1}", OpCodes.Newarr.Name, (int)OpCodes.Newarr.OperandType);
		//*/
		/*
		Console.WriteLine(Math.Pow(2,1));
		Console.WriteLine(Math.Pow(2,2));
		Console.WriteLine(Math.Pow(2,4));
		Console.WriteLine(Math.Pow(2,32));
		Console.WriteLine(Math.Pow(2,32.6));
		//*/
		/*
		new Thread(new ThreadStart(TestThread)).Start();
		//*/
		/*
		if(Engine.Native) {
			CooS.Shell.CommandShell shell = new CooS.Shell.CommandShell();
			Thread thread = new Thread(new ThreadStart(shell.Engage));
			thread.IsBackground = false;
			thread.Start();
		}
		return;
		//*/
		/*
		long a, b;
		a = 0xFFFFFFFF;
		b = 1;
		Console.WriteLine("{0:X}", a+b);
		Console.WriteLine("{0:X}", (a+1)/b);
		a = 0xFFFFFFFF;
		b = 0x10;
		Console.WriteLine("{0:X}", a*b);
		Console.WriteLine("{0:X}", a/b);
		Console.WriteLine("{0:X}", a%b);
		//*/
		/*
		Type type = typeof(ArrayList);
		MemberInfo mi = type.GetMethod("Add");
		Console.WriteLine(mi.Name);
		//Kernel.SetVESDebugMode(true);
		bool match = Type.FilterName(mi, "Add");
		//Kernel.SetVESDebugMode(false);
		Console.WriteLine(match);
		foreach(MemberInfo mi2 in type.GetMethods()) {
			Console.WriteLine(mi2.Name);
		}
		//*/
		//*
		if(Engine.Privileged) {
			Canvas display = new CooS.Graphics.Display();
			Painter painter = display.CreatePainter();
			Size dispsz = display.Size;
			for(int deg = 0; ; deg+=3) {
				double rag = Math.PI*deg/179;
				int radius = (int)(40+30*Math.Cos(rag*0.031415)+30*Math.Sin(rag*0.014142));
				int dx = (int)(radius*Math.Cos(rag));
				int dy = (int)(radius*Math.Sin(rag));
				int px = dispsz.Width/2+dx;
				int py = dispsz.Height/2+dy;
				Color c = Color.FromArgb((int)(127*Math.Cos(rag*2))+127, (int)(127*Math.Sin(rag/2))+127, (int)(127*Math.Sin(rag))+127);
				painter.DrawLine(dispsz.Width/2,dispsz.Height/2, px,py, c);
			}
		}
		//*/
		/*
		DateTime dt = new DateTime(1999,1,2,3,4,5);
		Console.WriteLine("{0:yyyy/MM/dd hh:mm:ss}", dt);
		/*
		byte[] buf = new byte[partition.BlockSize];
		partition.Read(buf, 0, 0, 1);
		int i=partition.BlockSize-48;
		while(i<buf.Length) {
			Console.Write("{0:X02} ",buf[i++]);
			if(i%16==0) Console.WriteLine();
		}
		FATFileSystem fs = new FATFileSystem().Bind(partition);
		DirectoryImpl rootdir = fs.GetRootDirectory();
		foreach(string filename in rootdir.GetFileSystemEntries()) {
			Console.WriteLine(filename);
		}
		//*/
		/*
		foreach(string filepath in Directory.GetFiles(Path.GetDirectoryName(args[0]))) {
			Console.WriteLine(filepath);
		}
		FileStream fs = new FileStream(args[0],FileMode.Open);
		Console.WriteLine(fs.Length);
		byte[] buf = new byte[fs.Length];
		fs.Read(buf,0,buf.Length);
		uint sum = 0;
		for(int i=0; i<buf.Length; ++i) {
			sum += buf[i];
		}
		Console.WriteLine(sum);
		//*/
		/*
		Archive archive = FileSystemManager.GetArchive("fat@fd0a");
		Book book = archive.GetBooks()[0];
		Page page = book.GetPage(0);
		DirectoryAspect rootdir = new FATDirectory(archive, page);
		IEnumerator it = rootdir.EnumBookInfo();
		while(it.MoveNext()) {
			BookInfo bi = (BookInfo)it.Current;
			Console.WriteLine(bi.Name);
		}
		book = rootdir.OpenBook("DRIVER.DLL");
		page = book.GetPage(0);
		byte[] buf = new byte[page.ClusterCount*page.ClusterSize];
		page.Read(buf,0,0,(int)page.ClusterCount);
		int i=0;
		while(i<16*10) {
			Console.Write("{0:X02} ",buf[i++]);
			if(i%16==0) Console.WriteLine();
		}
		//*/
		/*
		Console.WriteLine(typeof(byte).Name);
		Console.WriteLine(typeof(char).Name);
		Console.WriteLine(typeof(short).Name);
		Console.WriteLine(typeof(int).Name);
		Console.WriteLine(typeof(long).Name);
		Console.WriteLine(typeof(sbyte).Name);
		Console.WriteLine(typeof(ushort).Name);
		Console.WriteLine(typeof(uint).Name);
		Console.WriteLine(typeof(ulong).Name);
		//*
		Console.WriteLine(typeof(object).GetType().Name);
		Console.WriteLine(typeof(ValueType).GetType().Name);
		Console.WriteLine(typeof(int).GetType().Name);
		Console.WriteLine(typeof(bool).GetType().Name);
		Console.WriteLine(typeof(MyStruct).Name);
		Console.WriteLine(typeof(MyStruct).GetType().Name);
		Console.WriteLine(typeof(MyStruct).GetType().GetType().Name);
		Console.WriteLine(typeof(MyStruct).BaseType.Name);
		Console.WriteLine(typeof(MyStruct).GetType().BaseType.Name);
		Console.WriteLine(typeof(MyStruct).GetType().GetType().BaseType.Name);
		//*
		byte[] arr = new byte[1];
		Console.WriteLine(arr.GetType().Name);
		Console.WriteLine(arr.GetType().GetType().Name);
		Console.WriteLine(arr.GetType().GetType().GetType().Name);
		Console.WriteLine(arr.GetType().GetType().GetType().GetType().Name);
		//*/
		/*
		Console.WriteLine(typeof(int).GetType().Name);
		//*/
		/*
		Console.WriteLine("one:{0}",1);
		Console.WriteLine("two:{0}",2);
		Console.WriteLine("three:{0}",3);
		//*/
		/*
		ushort[] arr = new ushort[2];
		Console.WriteLine(arr.GetType().FullName);
		Console.WriteLine(arr.GetType().GetType().FullName);
		Console.WriteLine(arr.GetType().GetType().GetType().FullName);
		Console.WriteLine(arr.GetType().GetType().GetType().GetType().FullName);
		//*/
		/*
		ushort[] arr = new ushort[2];
		ArrayList list = new ArrayList();
		list.Add((ushort)0xFFFF);
		list.Add((ushort)0xFF);
		list.CopyTo(arr);
		foreach(ushort n in list) {
			Console.WriteLine("{0}",n);
		}
		//*/
		/*
		Console.WriteLine("hex:{0:X}",15);
		Console.WriteLine("hex:{0:X}",255);
		Console.WriteLine("hex:{0:X}",0x7fffffff);
		Console.WriteLine("hex:{0:X}",0x123456789abcdef);
		//*/
		/*
		int v1 = 7;
		int v2 = 2;
		if(v1+v2==9) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1-v2==5) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1*v2==14) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1/v2==3) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		v1 = -v1;
		if(v1+v2==-5) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1-v2==-9) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1*v2==-14) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1/v2==-3) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		v2 = -v2;
		if(v1+v2==-9) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1-v2==-5) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1*v2==14) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(v1/v2==3) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		byte v3 = 255;
		sbyte v4 = -128;
		if(255==(int)v3) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		if(-128==(int)v4) Console.WriteLine("Pass"); else Console.WriteLine("Fail");
		//*/
		/*
		Console.WriteLine(0.ToString());
		Console.WriteLine(1.ToString());
		Console.WriteLine((-1).ToString());
		Console.WriteLine(8.ToString());
		Console.WriteLine(16.ToString());
		Console.WriteLine(100000.ToString());
		Console.WriteLine(12345678.ToString());
		//*/
		/*
		Console.WriteLine("str1+str2+str3:"+"str1"+"str2"+"str3");
		Console.WriteLine("2args:{0}{1}","str1","str2");
		Console.WriteLine("5args:{0}{1}{2}{3}{4}","s1","s2","s3","s4","s5");
		Console.WriteLine("zero:{0}",0);
		Console.WriteLine("one:{0}",1);
		Console.WriteLine("255:{0}",255);
		Console.WriteLine("-1:{0}",-1);
		Console.WriteLine("-2:{0}",-2);
		//*/
		/*
		Console.WriteLine("short:{0}",(short)-1);
		Console.WriteLine("sbyte:{0}",(sbyte)-1);
		Console.WriteLine("byte:{0}",(byte)255);
		//*/
		/*
		ArrayList list = new ArrayList();
		// フィボナッチ数列
		list.Add(1);
		list.Add(2);
		for(int i=0; i<8; ++i) {
			list.Add((int)list[list.Count-2]+(int)list[list.Count-1]);
		}
		int[] arr = (int[])list.ToArray(typeof(int));
		foreach(int n in arr) {
			Console.WriteLine(n);
		}
		/*
		double dbl = 2;
		dbl = Math.Pow(3, 3);
		/*
		ulong* p = (ulong*)&dbl;
		Console.WriteLine("ulong:{0:X}",p[0]);
		Console.WriteLine("double:{0}",dbl);
		//*/
		/*
		int rem = 9;
		Console.WriteLine(2.3 % rem);
		//*/
		/*
		string teststr = "<p>CooS is <a>here</a>.</p>";
		string pattern = "</?([a-z]+)>";
		Regex re = new Regex(pattern);
		Console.WriteLine("Match \"{0}\" with \"{1}\"", pattern, teststr);
		foreach(Match m in re.Matches(teststr)) {
			Console.WriteLine("Found: {0}", m.Groups[1].Value);
		}
		//*/
		/*
		MyStruct a;
		a = f(new MyStruct(9,8));
		Console.WriteLine(a.x);
		Console.WriteLine(a.y);
		//*/
		/*
		ArrayList list = new ArrayList();
		list.Add(new MyStruct(3,5));
		list.Add(new MyStruct(1,5));
		list.Add(new MyStruct(3,5));
		list.Add(new MyStruct(7,4));
		list.Sort();
		foreach(object x in list) {
			Console.WriteLine(x);
		}
		//*/
		/*
		object[] arr1 = new object[]{ new MyStruct(3,5), new MyStruct(7,4), new MyStruct(1,5) };
		MyStruct[] arr2 = new MyStruct[arr1.Length];
		arr1.CopyTo(arr2, 0);
		foreach(MyStruct n in arr1) {
			Console.WriteLine("x={0}, y={1}", n.x, n.y);
		}
		//*/
		/*
		using(Stream stream = new FileStream("D:\\Repository\\clios\\application\\bin\\Release\\application.exe",FileMode.Open,FileAccess.Read)) {
			PEImage exeimg = new PEImage(stream);
			CorHeader corhdr = exeimg.ReadCorHeader();
			MetadataRoot root = corhdr.ReadMetadata();
			AssemblyRow row = (AssemblyRow)root.Tables[TableId.Assembly][1];
			Console.WriteLine("OK: {0} ({1}.{2}.{3}.{4})",
				root.Strings[row.Name],
				row.MajorVersion, row.MinorVersion,
				row.BuildNumber, row.RevisionNumber);
		}
		//*/
	}

}
