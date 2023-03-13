using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Security.Policy;
using System.Runtime.InteropServices;
using CooS.Architectures;
using CooS.Reflection;
using CooS.Management;
using CooS.CodeModels.CLI;
using CooS.CodeModels.CLI.Metatype;

namespace CooS {

	public struct ImageLocation {
		public IntPtr Start;
		public int Size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MetaTypeDesc {
		public Type MetaType;
		public Type BaseType;
		public Type Class;
		public Type String;
		public Type Delegate;
		public Type ValueType;
		public Type Primitive;
		public Type Interface;
		public Type SzArray;
		public Type MnArray;
		public Type ByRefPtr;
		public Type ByValPtr;
	}

	class Initializer {

		static AssemblyManager manager = null;

		#region アセンブリ

		static unsafe byte[] ImportImage(ImageLocation image) {
			byte* src = (byte*)image.Start.ToPointer();
			byte[] buf = new byte[image.Size];
			fixed(byte* dst = &buf[0]) {
				CooS.Tuning.Memory.Copy(dst, src, (uint)image.Size);
			}
			return buf;
		}
		
		public static AssemblyBase LoadPEImage(ImageLocation image) {
			if(manager==null) {
				manager = new AssemblyManager();
			}
			byte[] buf = ImportImage(image);
			return Engine.LoadAssemblyInternal(buf);
		}

		#endregion

		#region メタタイプ

		public static MetaTypeDesc GetMetaTypes() {
			MetaTypeDesc desc;
			desc.MetaType	= typeof(ClassType);
			desc.BaseType	= typeof(TypeImpl);
			desc.Class		= desc.MetaType;
			desc.String		= typeof(StringType);
			desc.Delegate	= typeof(DelegateType);
			desc.ValueType	= typeof(StructType);
			desc.Primitive	= typeof(PrimitiveType);
			desc.Interface	= typeof(InterfaceType);
			desc.SzArray	= typeof(SzArrayType);
			desc.MnArray	= typeof(MnArrayType);
			desc.ByRefPtr	= typeof(ByRefPointerType);
			desc.ByValPtr	= typeof(ByValPointerType);
			return desc;
		}

		public static MetaTypeDesc LoadMetaTypes(AssemblyDef assembly) {
			MetaTypeDesc desc;
			desc.BaseType	= new ClassType(assembly, typeof(TypeImpl));
			desc.MetaType	= new ClassType(assembly, typeof(ClassType));
			desc.Class		= desc.MetaType;
			desc.String		= new ClassType(assembly, typeof(StringType));
			desc.Delegate	= new ClassType(assembly, typeof(DelegateType));
			desc.ValueType	= new ClassType(assembly, typeof(StructType));
			desc.Primitive	= new ClassType(assembly, typeof(PrimitiveType));
			desc.Interface	= new ClassType(assembly, typeof(InterfaceType));
			desc.SzArray	= new ClassType(assembly, typeof(SzArrayType));
			desc.MnArray	= new ClassType(assembly, typeof(MnArrayType));
			desc.ByRefPtr	= new ClassType(assembly, typeof(ByRefPointerType));
			desc.ByValPtr	= new ClassType(assembly, typeof(ByValPointerType));
			return desc;
		}

		public static Type IsPreloaded(AssemblyDef assembly, int rowIndex) {
			if(assembly.IsTypeDefLoaded(rowIndex)) {
				return assembly.GetTypeDef(rowIndex);
			} else {
				Console.WriteLine("IsPreloaded: "+rowIndex);
				return null;
			}
		}

		#endregion

#if false

		private static void TestLongCalc(long op1, long op2) {
			Console.WriteLine("{0:X} + {1:X} = {2:X}", op1, op2, op1+op2);
			Console.WriteLine("{0:X} - {1:X} = {2:X}", op1, op2, op1-op2);
			Console.WriteLine("{0:X} * {1:X} = {2:X}", op1, op2, op1*op2);
			Console.WriteLine("{0:X} / {1:X} = {2:X}", op1, op2, op1/op2);
			Console.WriteLine("{0:X} % {1:X} = {2:X}", op1, op2, op1%op2);
			TestLongCalc((ulong)op1, (ulong)op2);
		}

		private static void TestLongCalc(ulong op1, ulong op2) {
			Console.WriteLine("{0:X} + {1:X} = {2:X}", op1, op2, op1+op2);
			Console.WriteLine("{0:X} - {1:X} = {2:X}", op1, op2, op1-op2);
			Console.WriteLine("{0:X} * {1:X} = {2:X}", op1, op2, op1*op2);
			Console.WriteLine("{0:X} / {1:X} = {2:X}", op1, op2, op1/op2);
			Console.WriteLine("{0:X} % {1:X} = {2:X}", op1, op2, op1%op2);
		}

		private static void TestOutput(string s, ulong value) {
			Console.WriteLine("0x{0:X16} = {0}", (long)value);
			Console.WriteLine("0x{0:X16} = {0}", (ulong)value);
		}

#endif

		static bool finalized = false;

		public static bool Finalized {
			get {
				return finalized;
			}
		}

		static int preloadFieldCount = 0;
		static string[] preloadFields = null;
		static int[] preloadIndices = null;
		static RuntimeFieldHandle[] preloadHandles = null;

		public static void PreloadFieldHandle(string asmname, int rowIndex, RuntimeFieldHandle handle) {
			if(!Finalized) {
				if(preloadFields==null) preloadFields = new string[32];
				if(preloadIndices==null) preloadIndices = new int[32];
				if(preloadHandles==null) preloadHandles = new RuntimeFieldHandle[32];
				preloadFields[preloadFieldCount] = asmname;
				preloadIndices[preloadFieldCount] = rowIndex;
				preloadHandles[preloadFieldCount] = handle;
				++preloadFieldCount;
			} else {
				AssemblyBase assembly = AssemblyResolver.FindAssemblyInfo(asmname);
				assembly.GetFieldInfo(rowIndex).AssociateFieldHandle(handle);
			}
		}

		public static void PrepareSetup() {
			CooS.Management.AssemblyResolver.AssociateAssemblies();
		}

		public static void FinalizeSetup() {
			// フィールドハンドルをリセット
			for(int i=0; i<preloadFieldCount; ++i) {
				AssemblyBase assembly = AssemblyResolver.FindAssemblyInfo(preloadFields[i]);
				assembly.GetFieldInfo(preloadIndices[i]).AssociateFieldHandle(preloadHandles[i]);
			}
			preloadFields = null;
			preloadIndices = null;
			preloadHandles = null;
			preloadFieldCount = 0;
			// Validations
			if(typeof(byte[]).GetType().Name!="SzArrayType") throw new UnexpectedException();
			// Threading
			/*
			Execution.RunState rs;
			AppDomainSetup ads = new AppDomainSetup();
			ads.ApplicationName = "<CooS.Kernel>";
			Evidence evidence = new Evidence();
			evidence.AddAssembly(typeof(Initializer).Assembly);
			ThreadStart ts = new ThreadStart(Startup);
			rs.Thread = new Thread(ts);
			rs.Context = null;
			rs.AppDomain = AppDomain.CreateDomain("CooS Kernel",evidence,ads);
			//ExecutionSystem.RunState.Current = rs;
			*/
			finalized = true;
		}

		public static void Startup() {
			Console.WriteLine("CONFIRMED RECEIVING CONTROL");
			CooS.Management.FileSystemManager.Initialize();
		}

		private static void WriteImpl(char ch) {
			// NOP
			// 少しはコードがないとコンパイルできない…orz
			int i = 0;
			++i;
		}

		private static string ReadLineImpl() {
			return null;
		}

		class StandardOut : TextWriter {
			public override System.Text.Encoding Encoding {
				get {
					return null;
				}
			}
			public override void Write(char ch) {
				WriteImpl(ch);
			}
		}

		class StandardIn : TextReader {
			public override string ReadLine() {
				return ReadLineImpl();
			}
		}

		private static void Setup() {
			StandardOut stdout = new StandardOut();
			Console.SetOut(stdout);
			Console.SetError(stdout);
			Console.SetIn(new StandardIn());
		}

	}

}
