using System;
using System.Collections;
using CooS.Reflection;
using CooS.Management;
using CooS.Collections;

namespace CooS.Execution {
	using CodeInfo = CooS.CodeModels.CodeInfo;
	using CodeLevel = CooS.CodeModels.CodeLevel;

	public sealed class CodeManager {

		public static void CompileTest() {
			Codetable table = new Codetable();
			MethodInfoImpl method = (MethodInfoImpl)typeof(CodeManager).GetMethod("CompileTest");
			table.Add(method, new CodeInfo(IntPtr.Zero));
			table[method] = new CodeInfo(IntPtr.Zero);
			CodeInfo code = table[method];
			foreach(DictionaryEntry e in table) {
				CodeInfo c = (CodeInfo)e.Value;
			}
			Inttable itbl = new Inttable();
			itbl.Add(1, null);
			itbl[2] = null;
			itbl.ContainsKey(2);
			object v = itbl[1];
			AssemblyBase assembly;
			assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(1);
			assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(2);
			assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(3);
			assembly.GetMethodInfo(1);
			string s1 = "abc";
			string s2 = "123";
			string s = s1+s2;
			s.GetHashCode();
			bool b = s=="abc123";
		}

		public static void CompileTest2() {
			Codetable table = new Codetable();
			MethodInfoImpl method = (MethodInfoImpl)typeof(CodeManager).GetMethod("CompileTest");
			table.Add(method, new CodeInfo(IntPtr.Zero));
			table[method] = new CodeInfo(IntPtr.Zero);
			Inttable itbl = new Inttable();
			itbl.Add(1, null);
			itbl[2] = null;
			itbl.ContainsKey(2);
			object v = itbl[1];
			AssemblyBase assembly;
			assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(1);
			assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(2);
			assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(3);
			assembly.GetMethodInfo(1);

			CooS.CodeModels.Assembler.CompileTest();
		}

		public static bool Trap = false;

		static readonly Codetable gect = new Codetable();	// Global Executable Code Table
		static readonly Codetable gcct = new Codetable();	// Global Callable Code Table
		private readonly Hashtable codetable = new Hashtable();
		public bool Recursive = false;

		private static unsafe DictionaryEntry FindEntry(IntPtr ip) {
			void* pip = ip.ToPointer();
			foreach(DictionaryEntry e in gect) {
				CodeInfo code = (CodeInfo)e.Value;
				if(code.CodeBlock!=null) {
					fixed(byte* p = code.CodeBlock) {
						if(p<=pip && pip<p+code.CodeBlock.Length) {
							return e;
						}
					}
				}
			}
			return new DictionaryEntry(null, null);
		}

		public static MethodInfoImpl FindMethod(IntPtr ip) {
			DictionaryEntry e = FindEntry(ip);
			return (MethodInfoImpl)e.Key;
		}

		public static CodeInfo FindCode(IntPtr ip) {
			DictionaryEntry e = FindEntry(ip);
			return (CodeInfo)e.Value;
		}

		public static void RelinkAllCodes() {
			Console.Write("Relink codes...");
			int c = 0;
			foreach(CodeInfo code in gect.Values) {
				code.Link();
				++c;
			}
			Console.WriteLine("OK. {0} codes are re-linked.", c);
		}

		private static unsafe void RegisterExecutableCodeImpl(MethodInfoImpl method, CodeInfo code) {
			int adr;
			fixed(byte* p = code.CodeBlock) {
				adr = (int)p;
			}
			gect.Add(method, code);
			Console.WriteLine("[0x{0:X8}-0x{1:X8}:E] {3}", adr, adr+code.CodeBlock.Length-1
				, method.Handle.Value.ToInt32(), method.FullName
				, Kernel.ObjectToValue(method).ToInt32()
				);
		}

		private static void RegisterExecutableCode(MethodInfoImpl method, CodeInfo code) {
			if(gect.ContainsKey(method)) throw new ArgumentException("Method is already registered.", "method");
			RegisterExecutableCodeImpl(method, code);
			if(gcct.ContainsKey(method)) {
				//TODO: re-link codes that links this removing code.
				gcct.Remove(method);
			}
		}

		private static unsafe void RegisterCallableCode(MethodInfoImpl method, CodeInfo code) {
			if(gect.ContainsKey(method)) throw new ArgumentException("Method is already registered.", "method");
			int adr;
			fixed(byte* p = code.CodeBlock) {
				adr = (int)p;
			}
			gcct[method] = code;
			Console.WriteLine("[0x{0:X8}-0x{1:X8}:C] {3}", adr, adr+code.CodeBlock.Length-1
				, method.Handle.Value.ToInt32(), method.FullName
				, Kernel.ObjectToValue(method).ToInt32()
				);
		}
		
		/*
		private static unsafe void ReportCodeFragments(Codetable ct) {
			foreach(DictionaryEntry e in ct) {
				MethodInfoImpl method = (MethodInfoImpl)e.Key;
				CodeInfo code = (CodeInfo)e.Value;
				if(code.CodeBlock==null) {
					Console.WriteLine("[0x{0:X8}-??????????] {1}", code.EntryPoint, method.FullName);
				} else {
					int adr;
					fixed(byte* p = code.CodeBlock) {
						adr = (int)p;
					}
					Console.WriteLine("[0x{0:X8}-0x{1:X8}] {2}", adr, adr+code.CodeBlock.Length-1, method.FullName);
				}
			}
		}

		public static void ReportCodeFragments() {
			Console.WriteLine("Global Executable Code-fragment Table:");
			ReportCodeFragments(gect);
			Console.WriteLine("Global Callable Code-fragment Table:");
			ReportCodeFragments(gcct);
		}
		*/

		public static CodeInfo GetExecutableCode(MethodInfoImpl method) {
			object value = gect[method];
			if(value!=null) {
				return (CodeInfo)value;
			} else {
				CodeInfo code = method.GenerateExecutableCode(CodeLevel.Native);
				RegisterExecutableCode(method, code);
				code.Link();
				return code;
			}
		}

		public static CodeInfo GetCallableCode(MethodInfoImpl method) {
			object value = gect[method];
			if(value!=null) {
				return (CodeInfo)value;
			} else {
				value = gcct[method];
				if(value!=null) {
					return (CodeInfo)value;
				}
			}
			CodeInfo code = method.GenerateCallableCode(CodeLevel.Stub);
			RegisterCallableCode(method, code);
			code.Link();
			return code;
		}

		public static void Clear(string fullname) {
			if(fullname==null) throw new ArgumentNullException("fullname");
			foreach(MethodInfoImpl method in AssemblyResolver.Manager.ResolveMethods(fullname, true)) {
				if(gect.ContainsKey(method)) gect.Remove(method);
				if(gcct.ContainsKey(method)) gcct.Remove(method);
			}
		}

		public static void PrepareExecutableCode(string fullname) {
			if(fullname==null) throw new ArgumentNullException("fullname");
			foreach(MethodInfoImpl method in AssemblyResolver.Manager.ResolveMethods(fullname, true)) {
				//DumpCallGraph(method, new Hashtable(), 0);
				CodeInfo code = GetExecutableCode(method);
				//CooS.Assist.Dump(code.CodeBlock);
			}
		}

		static ArrayList proxyMethods = new ArrayList();

		public static void RegisterProxyMethod(MethodInfoImpl method) {
			CodeInfo code = new CodeInfo(Engine.GetMethodProxyCode(method));
			gect.Add(method, code);
			gcct.Add(method, code);
			proxyMethods.Add(method);
		}

		public static void CompileAllProxyMethods() {
			foreach(MethodInfoImpl method in proxyMethods) {
				gect.Remove(method);
				gcct.Remove(method);
				Console.WriteLine("Compile {0}", method.FullName);
				GetExecutableCode(method);
			}
			proxyMethods.Clear();
		}

		public static void MakeBlankMethod(string fullname, Type[] types) {
			MethodInfoImpl[] methods = AssemblyResolver.Manager.ResolveMethods(fullname,true);
			foreach(MethodInfoImpl method in methods) {
				if(method.ParameterCount!=types.Length) continue;
				//Console.WriteLine(method.FullName);
				bool mismatch = false;
				for(int i=0; i<types.Length; ++i) {
					//Console.WriteLine("expected={0}, actual={1}", types[i].FullName, method.GetParameterType(i).FullName);
					//Console.WriteLine("expected={0}, actual={1}", Kernel.ObjectToValue(types[i]), Kernel.ObjectToValue(method.GetParameterType(i)));
					// 派生型がランタイムと同期していないため、同一型に複数のインスタンスがある。
					if(method.GetParameterType(i).FullName!=types[i].FullName) {
						mismatch = true;
						break;
					}
				}
				if(!mismatch) {
					method.MakeBlank();
					return;
				}
			}
			throw new MethodNotFoundException(fullname);
		}

		public static void MakeBlankMethod(string fullname) {
			foreach(MethodInfoImpl method in AssemblyResolver.Manager.ResolveMethods(fullname,true)) {
				method.MakeBlank();
			}
		}

		public CodeManager() {
		}

		private CodeInfo PrepareImpl(MethodInfoImpl method) {
			CodeInfo code = method.GenerateExecutableCode(CodeLevel.Native);
			//Console.WriteLine("Prepare: "+method.FullName);
			//Assist.Dump(code.CodeBlock);
			this.codetable[method] = code;
			return code;
		}

		public CodeInfo Prepare(MethodInfoImpl method) {
			if(this.codetable.ContainsKey(method)) {
				return (CodeInfo)this.codetable[method];
			} else if(gect.ContainsKey(method)) {
				return gect[method];
			} else {
				if(method.IsBlank) {
					Console.WriteLine("Blank method: {0}", method.FullName);
					return null;
				} else {
					CodeInfo code = this.PrepareImpl(method);
					if(this.Recursive) {
						foreach(MethodInfoImpl calling in method.GetCallings()) {
							if(!calling.IsBlank) this.Prepare(calling);
						}
					}
					return code;
				}
			}
		}

		private void DumpCallGraph(MethodInfoImpl method, Hashtable list, int depth) {
			list.Add(method, depth);
			Console.WriteLine("{2}{1,"+(depth*3)+"}{0}", method.FullName, string.Empty, method.IsVirtual ? "*" : " ");
			foreach(MethodInfoImpl calling in method.GetCallings()) {
				if(!list.ContainsKey(calling)) {
					DumpCallGraph(calling, list, depth+1);
				}
			}
		}

		public void Prepare(string fullname) {
			if(fullname==null) throw new ArgumentNullException("fullname");
			if(fullname.IndexOf(':')<0) {
				foreach(MethodInfoImpl method in AssemblyResolver.Manager.ResolveType(fullname, true).DeclaredMethods) {
					//DumpCallGraph(method, new Hashtable(), 0);
					CodeInfo code = this.Prepare(method);
					//CooS.Assist.Dump(code.CodeBlock);
				}
			} else {
				foreach(MethodInfoImpl method in AssemblyResolver.Manager.ResolveMethods(fullname, true)) {
					//DumpCallGraph(method, new Hashtable(), 0);
					CodeInfo code = this.Prepare(method);
					//CooS.Assist.Dump(code.CodeBlock);
				}
			}
		}

		public void Apply() {
			foreach(DictionaryEntry e in this.codetable) {
				RegisterExecutableCodeImpl((MethodInfoImpl)e.Key, (CodeInfo)e.Value);
			}
			foreach(DictionaryEntry e in this.codetable) {
				((CodeInfo)e.Value).Link();
			}
			this.codetable.Clear();
		}

	}

}
