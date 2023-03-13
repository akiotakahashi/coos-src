using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CooS.Reflection;
using CooS.CodeModels;
using CooS.CodeModels.DLL;
using CooS.CodeModels.CLI;
using CooS.CodeModels.CLI.Metadata;
using CooS.Drivers.Controllers;

namespace System {
	using Hashtable = System.Collections.Hashtable;

	public abstract class Engine {

		static Hashtable tbl;

		public static int TestMethod() {
			string s1 = "abc";
			string s2 = "def";
			string s3 = "g";
			string s4 = "";
			/*
			switch(s1+s2+s3+s4) {
			case "abc":
				return 0;
			case "abcdefgh":
				return 1;
			case "abcdefg":
				return 2;
			default:
				return 3;
			}
			*/
			if(tbl==null) {
				tbl = new Hashtable();
				tbl.Add("abc", 0);
				tbl.Add("abcdefgh", 1);
				tbl.Add("abcdefg", 2);
			}
			object v = tbl[s1+s2+s3+s4];
			return v==null ? -1 : (int)v;
		}

		/// <summary>
		/// CooSランタイムによる実行サポートがあることを示します。
		/// </summary>
		public static bool Infrastructured {
			get {
				return false;
			}
		}

		/// <summary>
		/// ソフトウェアが特権モードで動作していることを示します。
		/// </summary>
		public static bool Privileged {
			get {
				return false;
			}
		}

		public static int OffsetToSzArrayData {
			get {
				return IntPtr.Size*3;
			}
		}

		public static void SetDebugMode(bool enabled) {
			//throw new SystemException("VESデバッグモードを設定しようとしましたが、システムは現在VESから切り離されています。");
		}

		internal static void ReloadMethodCode(Type type, int rowIndex, byte[] code) {
			throw new SystemException("VESに対してメソッドのコードを設定しようとしましたが、システムは現在VESから切り離されています。");
		}

		private static unsafe IntPtr GetFramePointer(IntPtr dummy) {
			IntPtr* p = &dummy;
			return *(p-2);
		}

		public static unsafe object GetExecutingAssembly(int depth) {
			IntPtr* p = (IntPtr*)GetFramePointer(IntPtr.Zero).ToPointer();
			IntPtr ip = p[1];
			return CooS.Execution.CodeManager.FindMethod(ip).Assembly.RealAssembly;
		}

		public static object Invoke(MethodInfo mi, object target, object[] args) {
			return InternalInvoke(GetMethodProxyCode(mi), target, args);
		}

		public static IntPtr GetMethodProxyCode(MethodInfo mi) {
			// This is overridden by myself.
			throw new NotSupportedException("This must be overriden.");
		}

		internal static IntPtr GetMethodProxyCode(string asmname, int methodrid) {
			// This is used for me to override again as above.
			throw new NotSupportedException("This must be overriden.");
		}

		private static object InternalInvoke(IntPtr fp, object target, object[] args) {
			byte[] buf = BuildStackImage(target, args);
			/*
			if(args!=null) {
				Console.WriteLine("arguments: {0}", args.Length);
				foreach(object arg in args) {
					Console.WriteLine("arg: {0} ({1})", arg, arg.GetType().Name);
				}
			}
			int i = 0;
			foreach(byte e in buf) {
				Console.Write("{0:X2} ", e);
				if(i++==16) {
					Console.WriteLine();
					i = 0;
				}
			}
			Console.WriteLine();
			//*/
			return InternalInvoke(fp, buf);
		}

		private static unsafe byte[] BuildStackImage(object target, object[] args) {
			int size = target!=null ? IntPtr.Size : 0;
			if(args!=null) {
				foreach(object arg in args) {
					SuperType type = (SuperType)arg.GetType();
					if(type.IsValueType) {
						size += type.VariableSize;
					} else {
						size += IntPtr.Size;
					}
				}
			}
			byte[] buf = new byte[size];
			MemoryStream stream = new MemoryStream(buf);
			BinaryWriter writer = new BinaryWriter(stream);
			if(target!=null) writer.Write(Kernel.ObjectToValue(target).ToInt32());
			if(args!=null) {
				foreach(object arg in args) {
					SuperType type = (SuperType)arg.GetType();
					if(type.IsValueType) {
						IntPtr n = Kernel.ObjectToValue(arg);
						byte* p = (byte*)n.ToPointer();
						for(int i=0; i<type.VariableSize; ++i) {
							writer.Write(p[i]);
						}
					} else {
						writer.Write(Kernel.ObjectToValue(arg).ToInt32());
					}
				}
			}
			writer.Close();
			stream.Close();
			return buf;
		}

		private static object InternalInvoke(IntPtr fp, byte[] stack) {
			// This is overridden.
			throw new NotImplementedException();
		}

		public static RuntimeTypeHandle NotifyLoadingTypeImpl(string asmname, int rowIndex, object obj) {
			// NOP
			return new RuntimeTypeHandle();
		}

		public static RuntimeTypeHandle NotifyLoadingType(AssemblyBase assem, int rowIndex, object obj) {
			return NotifyLoadingTypeImpl(assem.GetName(false).Name, rowIndex, obj);
		}

		public static void ReloadMethodCode(MethodInfoImpl method, CodeInfo code) {
			ReloadMethodCode(method.DeclaringType, method.RowIndex, code.CodeBlock);
		}

		public static void ReloadMethodCode(string fullname) {
			if(fullname==null) throw new ArgumentNullException("fullname");
			foreach(MethodInfoImpl method in CooS.Management.AssemblyResolver.Manager.ResolveMethods(fullname, true)) {
				ReloadMethodCode(method, CooS.Execution.CodeManager.GetExecutableCode(method));
			}
		}

		public static MetadataRoot AnalyzePEImage(byte[] buf) {
			MemoryStream stream = new MemoryStream(buf,false);
			PEImage exeimg = new PEImage(stream);
			CorHeader corhdr = exeimg.ReadCorHeader();
			MetadataRoot root = corhdr.ReadMetadata();
			/*
			if(root.Tables.GetRowCount(TableId.Assembly)==1) {
				AssemblyRow row = (AssemblyRow)root.Tables[TableId.Assembly][1];
				Console.WriteLine("Analyzed PE Image: {0} ({1}.{2}.{3}.{4})",
					root.Strings[row.Name],
					row.MajorVersion, row.MinorVersion,
					row.BuildNumber, row.RevisionNumber);
			}
			*/
			return root;
		}

		public static AssemblyBase AnalyzeAssembly(byte[] buf) {
			MetadataRoot root = AnalyzePEImage(buf);
			return new AssemblyDef(root);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RuntimeLoadAssembly(object assembly, byte[] buf);

		internal static AssemblyBase LoadAssemblyInternal(byte[] buf) {
			return CooS.Management.AssemblyResolver.RegisterAssembly(AnalyzeAssembly(buf));
		}

		public static AssemblyBase LoadAssembly(byte[] buf) {
			AssemblyBase assembly = LoadAssemblyInternal(buf);
			RuntimeLoadAssembly(assembly, buf);
			return assembly;
		}

	}

}
