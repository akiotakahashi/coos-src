using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using CooS.Reflection;

namespace CooS.Manipulation {
	using Inttable=Dictionary<int, object>;

	public abstract class Assembler {

		const CodeLevel JIT_COMPILE_LEVEL = CodeLevel.Native;

		public abstract MethodInfoImpl Method {get;}

		//
		// Code Manipulation
		//

		public abstract int Position {get;}
		public abstract CodeInfo GenerateCode();

		public abstract int GetArgumentOffset(int iarg);
		public abstract int GetVariableOffset(int index);
		public abstract int TotalVariableSize {get;}
		public abstract int TotalArgumentSize {get;}
		public abstract int BaseOffsetToArguments {get;}
		public abstract int BaseOffsetToVariables {get;}

		//
		// Stack Operations
		//

		public abstract void Pop(int size);
		public abstract void Duplicate(int size);

		//
		// Methematical Operations
		//

		public abstract void Add32(bool signed, bool overflow);
		public abstract void Add64(bool signed, bool overflow);
		public abstract void Sub32(bool signed, bool overflow);
		public abstract void Sub64(bool signed, bool overflow);
		public abstract void Mul32(bool signed, bool overflow);
		public abstract void Mul64(bool signed, bool overflow);
		public abstract void Div32(bool signed);
		public abstract void Div64(bool signed);
		public abstract void Rem32(bool signed);
		public abstract void Rem64(bool signed);

		public abstract void Negate32();
		public abstract void Negate64();

		//
		// Logical Operations
		//

		public abstract void And32();
		public abstract void And64();
		public abstract void Or32();
		public abstract void Or64();
		public abstract void Xor32();
		public abstract void Xor64();
		public abstract void Not32();
		public abstract void Not64();

		public abstract void Shl32();
		public abstract void Shl64();
		public abstract void Shr32(bool signed);
		public abstract void Shr64(bool signed);

		//
		// Compare Operations
		//

		public abstract void CompareI32(Condition cond, bool signed);
		public abstract void CompareI64(Condition cond, bool signed);
		public abstract void CompareR32(Condition cond);
		public abstract void CompareR64(Condition cond);

		//
		// Branch Operations
		//

		public abstract CodeLabel Branch(IBranchTarget target);
		public abstract CodeLabel BranchI32(Condition cond, bool signed, IBranchTarget target);
		public abstract CodeLabel BranchI64(Condition cond, bool signed, IBranchTarget target);
		public abstract CodeLabel BranchR32(Condition cond, IBranchTarget target);
		public abstract CodeLabel BranchR64(Condition cond, IBranchTarget target);
		public abstract void Switch(IBranchTarget[] targets);

		//
		// Call Operations
		//

		public abstract void Return();
		public abstract void LoadTarget(MethodInfoImpl target);
		public abstract void CallInd(MethodInfoImpl signature);
		protected abstract CodeLabel CallImpl(MethodInfoImpl target);
		protected abstract void CallIfImpl(MethodInfoImpl target, MethodInfoImpl finder);
		protected abstract void CallVirtImpl(MethodInfoImpl target, MethodInfoImpl finder);

		public CodeLabel Call(MethodInfoImpl method) {
			if(!method.IsConstructor) {
				return this.CallImpl(method);
			} else {
				this.LoadTarget(method);
				CodeLabel label = this.CallImpl(method);
				// コンストラクタは void なので、戻り値がプッシュされることはない。
				// そのため、次のようにポップを改めて発行しても動作する。
				this.Pop(IntPtr.Size);
				return label;
			}
		}

		/// <summary>
		/// このアセンブラがDelegate.Invokeと関連づけられているとき、
		/// そのInvokeメソッドの実装を提供します。
		/// </summary>
		/// <param name="target">System.Delegate:targetを表すフィールド情報</param>
		/// <param name="function">System.Delegate:pointerを表すフィールド情報</param>
		public abstract void Invoke(FieldInfoImpl target, FieldInfoImpl function);

		//
		// Try-catch Operations
		//

		public abstract void Throw();
		public abstract void Leave(IBranchTarget target, TypeImpl[] discards);

		//
		// Load and Store Operations
		//

		public abstract void LoadNull();
		public abstract void LoadConstant(int value);
		public abstract void LoadConstant(float value);
		public abstract void LoadConstant(double value);

		public abstract void LoadInd(TypeImpl type);
		public abstract void StoreInd(TypeImpl type);

		public abstract void LoadArg(int index);
		public abstract void StoreArg(int index);
		public abstract void LoadArgAddress(int index);

		public abstract void LoadVar(int index);
		public abstract void StoreVar(int index);
		public abstract void LoadVarAddress(int index);
		public abstract void LoadWorkspace(TypeImpl type);
		public abstract void LoadWorkspaceAddress();

		public abstract void LoadElement(TypeImpl type);
		public abstract void StoreElement(TypeImpl type);
		public abstract void LoadElementAddress(TypeImpl type);

		//
		// Load Token
		//

		public void LoadToken(TypeImpl type) {
			this.LoadConstant(type.TypeHandle.Value.ToInt32());
		}

		public void LoadToken(FieldInfoImpl field) {
			this.LoadConstant(field.FieldHandle.Value.ToInt32());
		}

		public void LoadToken(MethodInfoImpl method) {
			this.LoadConstant(method.MethodHandle.Value.ToInt32());
		}

		public abstract void LoadEntryPoint(MethodInfoImpl method);

		//
		// Field Access Operations
		//

		public void LoadLength() {
			FieldInfo field = this.Method.Assembly.Manager.SzArray.GetField("Length",BindingFlags.Instance|BindingFlags.NonPublic);
			if(field==null) throw new UnexpectedException("SzArray:Length is missed");
			this.LoadField((FieldInfoImpl)field);
		}

		public abstract void LoadField(FieldInfoImpl field);
		public abstract void StoreField(FieldInfoImpl field);
		public abstract void LoadFieldAddress(FieldInfoImpl field);

		//
		// Heap Access Operations
		//

		public abstract void ClearMemory();					// ..., [nint] adr, [int32] size
		public abstract void ClearMemory(int size);			// ..., [nint] adr
		public abstract void InitializeMemory();			// ..., [nint] adr, [int8] value, [int32] size

		public abstract void CopyBlock();

		/// <summary>
		/// オブジェクト obj をアドレス adr の位置にコピーします。
		/// obj は参照型であればポインタ、値型ではインスタンスです。
		/// </summary>
		/// <param name="type"></param>
		public abstract void CopyObject(TypeImpl type);		// ..., [nint] obj, [nint] adr -> ..., [nint] adr
		public abstract void LoadObject(TypeImpl type);		// ..., [nint] adr -> ..., [type] value

		//
		// Conversion
		//

		public abstract void Convert(TypeImpl from, TypeImpl to);

		//
		// Functionalization
		//

		public abstract void Prologue();
		public abstract void Epilogue();
		public abstract void Trap();
		protected abstract void WriteMethodStub(MethodInfoImpl stubmethod);

		//
		//****************************************************************************************
		//
		//		Implementation
		//
		//****************************************************************************************

		class LoopException : Exception {
		}

		#region インスタンスと文字列の生成

		static Exception loopex = new LoopException();
		static bool running_alloc = false;

		private static object AllocateObjectImpl(int aid, int tid) {
			//if(running_alloc) throw loopex;
			running_alloc = true;
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeImpl type = assembly.GetTypeInfo(tid);
			object ret = Memory.Allocate(type);
			running_alloc = false;
			return ret;
		}

		static MethodInfoImpl allocateobjectimpl;

		private MethodInfoImpl RefAllocateObjectImpl {
			get {
				if(allocateobjectimpl==null) {
					allocateobjectimpl = this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:AllocateObjectImpl",true);
					CooS.Execution.CodeManager.RegisterProxyMethod(allocateobjectimpl);
				}
				return allocateobjectimpl;
			}
		}

		private static Delegate AllocateDelegate(object target, IntPtr ftn, int aid, int tid) {
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeImpl type = assembly.GetTypeInfo(tid);
			Delegate obj = (Delegate)Memory.Allocate(type);
			Assist.ConstructDelegate(obj, target, ftn);
			return obj;
		}

		public void AllocateObject(MethodInfoImpl constructor) {
			if(!constructor.IsConstructor) throw new ArgumentException("Specified method is not a constructor.");
			if(constructor.DeclaringType.IsValueType) {
				this.LoadWorkspaceAddress();
				this.CallImpl(constructor);
				this.LoadWorkspace((TypeImpl)constructor.DeclaringType);
			} else if(constructor.IsBlank) {
				if(0!=(constructor.GetMethodImplementationFlags()&MethodImplAttributes.Runtime)) {
					if(constructor.DeclaringType.IsSubclassOf(typeof(Delegate))) {
						this.LoadConstant(constructor.Assembly.Id);
						this.LoadConstant(((TypeImpl)constructor.DeclaringType).RowIndex);
						this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:AllocateDelegate",true));
					} else {
						throw new NotImplementedException(constructor.FullName);
					}
				} else {
					MethodInfoImpl wrap = constructor.FindWrappingMethod();
					if(wrap==null) throw new MissingMethodException(constructor.FullName);
					this.Call(wrap);
				}
			} else {
				// ..., argN
				this.LoadWorkspaceAddress();
				// ..., argN, wsa
				this.LoadConstant(constructor.Assembly.Id);
				this.LoadConstant(((TypeImpl)constructor.DeclaringType).RowIndex);
				// ..., argN, wsa, aid, tid
				this.Call(RefAllocateObjectImpl);
				// ..., argN, wsa, obj
				this.StoreInd(this.Method.Assembly.Manager.IntPtr);
				// ..., argN
				this.LoadWorkspaceAddress();
				// ..., argN, wsa
				this.LoadInd(this.Method.Assembly.Manager.IntPtr);
				// ..., argN, obj
				this.CallImpl(constructor);
				// ...
				this.LoadWorkspaceAddress();
				// ..., wsa
				this.LoadInd(this.Method.Assembly.Manager.IntPtr);
				// ..., obj
			}
		}

		private static object AllocateSzArrayImpl(int length, int aid, int tid) {
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeImpl type = assembly.GetTypeInfo(tid);
			return Memory.AllocateArray(type, length);
		}

		public void AllocateSzArray(TypeImpl type) {
			this.LoadConstant(type.AssemblyInfo.Id);
			this.LoadConstant(type.RowIndex);
			this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:AllocateSzArrayImpl",true));
		}

		static Inttable stringtable = new Inttable();

		private static string LoadStringImpl(int sid) {
			return (string)stringtable[sid];
		}

		public void LoadString(int sid) {
			this.LoadConstant(sid);
			if(!stringtable.ContainsKey(sid)) {
				stringtable[sid] = string.Intern(this.Method.Assembly.LoadString(sid));
			}
			this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:LoadStringImpl",true));
		}

		#endregion

		#region ボクシングとアンボクシング

		private static object BoxImpl(int aid, int rid) {
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeImpl type = assembly.GetTypeInfo(rid);
			return Memory.Allocate(type);
		}

		public void Box(TypeImpl type) {
			this.LoadConstant(type.AssemblyInfo.Id);
			this.LoadConstant(type.RowIndex);
			this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:BoxImpl",true));
			this.CopyObject(type);
		}

		public void Unbox(TypeImpl type) {
			// Unlike box, which is required to make a copy of a value type for use in the object,
			// unbox is not required to copy the value type from the object. Typically it simply
			// computes the address of the value type that is already present inside of the boxed object.
			// オブジェクトの中に値型の複製を作るboxと異なり、unboxはオブジェクトからの複製を必要としません。
			// 典型的には、unboxは単にオブジェクトの内部にある値型の部分のアドレスを計算するだけです。
		}

		#endregion

		#region キャスト

		private static object IsInstImpl(object obj, int aid, int tid) {
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeImpl type = assembly.GetTypeInfo(tid);
			return type.IsInstanceOfType(obj) ? obj : null;
		}

		public void IsInst(TypeImpl type) {
			this.LoadConstant(type.AssemblyInfo.Id);
			this.LoadConstant(type.RowIndex);
			this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:IsInstImpl",true));
		}

		private static object CastClassImpl(object obj, int aid, int rid) {
			//throw new NotImplementedException();
			return obj;	// always success
		}

		public void CastClass(TypeImpl type) {
			if(type.IsPointer) return;
			if(type.IsArray) return;
			this.LoadConstant(type.AssemblyInfo.Id);
			this.LoadConstant(type.RowIndex);
			this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:CastClassImpl",true));
		}

		#endregion

		#region メソッドスタブ

		/// <summary>
		/// 指定されたメソッドをコンパイルし、そのエントリポイントアドレスを返します。
		/// このメソッドはJITコンパイルしたCALL機械語から、呼び出し先のメソッドをさらにJITコンパイルするために呼び出されます。
		/// このメソッドの副作用の結果として、呼び出し元の機械語の呼び出し先アドレスが書き換わります。
		/// これは、次回からはコンパイルした機械語を直接実行するようにするために必要です。
		/// この動作のために、このメソッドは必ず相対番地呼出し命令を使用して呼び出される必要があります。
		/// </summary>
		/// <param name="rip">スタブが用意する呼び出し番地を格納したアドレス</param>
		/// <param name="method">コンパイル対象となるメソッド</param>
		/// <returns></returns>
		static unsafe IntPtr PrepareExecutableCode(IntPtr* sp, int assemblyid, int rowIndex) {
			//Console.WriteLine("ParepareExcurable(sp=0x{0:X8}, *sp=0x{3:X8}, aid={1}, rowIndex={2})", (int)sp, assemblyid, rowIndex, sp->ToInt32());
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(assemblyid);
			MethodInfoImpl method = assembly.GetMethodInfo(rowIndex);
			//Console.WriteLine("Preparing method = {0}", method.FullName);
#if false
			Assembler asm = Architecture.CreateAssembler(method, 0);
			if(asm.TotalArgumentSize>0) {
				Assist.Dump(new IntPtr(sp+1), 0, asm.TotalArgumentSize);
			}
			if(!method.IsStatic || method.Name=="FindActualMethod") {
				IntPtr p = *(sp+1+Architecture.GetStackingLength(asm.GetArgumentOffset(0)));
				if(method.DeclaringType.IsValueType) {
					Console.WriteLine("Target: 0x{0:X8}", p.ToInt32());
				} else {
					object obj = Kernel.ValueToObject(p);
					if(obj==null) throw new NullReferenceException("Calling target is null: "+method.FullName);
					Console.WriteLine("Target: {0}", obj.GetType());
					Console.WriteLine("Text  : {0}", obj.ToString());
				}
			}
#endif
			// Generating executable code
			CodeInfo codeinfo = CooS.Execution.CodeManager.GetExecutableCode(method);
#if true
			IntPtr* p = (IntPtr*)sp->ToPointer();
			--p;
			/*
			Console.WriteLine("Rewrite calling address [0x{0:X8}] 0x{1:X8} -> 0x{2:X8}"
				, (int)p, p->ToInt32(), codeinfo.EntryPoint.ToInt32());
			//*/
			*p = new IntPtr(codeinfo.EntryPoint.ToInt32()-sp->ToInt32());
#endif
			//Console.WriteLine("pre-execution> [0x"+codeinfo.EntryPoint.ToInt32().ToString("X8")+"] "+method.FullName);
			return codeinfo.EntryPoint;
		}

		static MethodInfoImpl prepareexecutablecode;

		private MethodInfoImpl RefPrepareExecutableCode {
			get {
				if(prepareexecutablecode==null) {
					prepareexecutablecode = this.Method.Assembly.Manager.ResolveMethod("CooS.Formats.Assembler:PrepareExecutableCode",true);
					CooS.Execution.CodeManager.RegisterProxyMethod(prepareexecutablecode);
				}
				return prepareexecutablecode;
			}
		}

		public void WriteMethodStub() {
			this.WriteMethodStub(RefPrepareExecutableCode);
		}

		#endregion

		#region 仮想メソッド

		static Exception nullex = new NullReferenceException();

		static IntPtr FindActualMethod(object entity, int slotIndex) {
			if(entity==null) throw nullex;
			MethodInfoImpl method = ((TypeImpl)entity.GetType()).GetSlotMethod(slotIndex);
			//Console.WriteLine("callvirt := "+method.FullName);
			CodeInfo codeinfo = CooS.Execution.CodeManager.GetExecutableCode(method);
			return codeinfo.EntryPoint;
		}

		static IntPtr FindActualMethodForInterface(object entity, RuntimeTypeHandle handle, int slotIndex) {
			if(entity==null) throw nullex;
			TypeImpl type = (TypeImpl)entity.GetType();
			int baseidx = type.GetInterfaceBaseIndex(handle);
			MethodInfoImpl method = type.GetSlotMethod(baseidx+slotIndex);
			//Console.WriteLine("callvirtif := "+method.FullName);
			CodeInfo codeinfo = CooS.Execution.CodeManager.GetExecutableCode(method);
			return codeinfo.EntryPoint;
		}

		static MethodInfoImpl findactualmethod;
		static MethodInfoImpl findactualmethod4if;

		private static MethodInfoImpl RefFindActualMethod {
			get {
				if(findactualmethod==null) {
					findactualmethod = CooS.Management.AssemblyResolver.Manager.ResolveMethod("CooS.Formats.Assembler:FindActualMethod",true);
					CooS.Execution.CodeManager.RegisterProxyMethod(findactualmethod);
				}
				return findactualmethod;
			}
		}

		private static MethodInfoImpl RefFindActualMethodForInterface {
			get {
				if(findactualmethod4if==null) {
					findactualmethod4if = CooS.Management.AssemblyResolver.Manager.ResolveMethod("CooS.Formats.Assembler:FindActualMethodForInterface",true);
					CooS.Execution.CodeManager.RegisterProxyMethod(findactualmethod4if);
				}
				return findactualmethod4if;
			}
		}

		public void CallVirt(TypeImpl target, MethodInfoImpl method) {
			if(!method.IsVirtual) {
				// C#でSystem.Object:GetType()を呼ぶと、instance なのになぜか callvirt で実行しようとする。
				switch(method.FullName) {
				case "System.Object:GetType":
					this.LoadConstant(4);
					this.Sub32(false, false);
					this.LoadInd(method.Assembly.Manager.IntPtr);
					break;
				default:
					this.Call(method);
					break;
				}
			} else /*if(0!=(method.GetMethodImplementationFlags()&MethodImplAttributes.Runtime)) {
				switch(method.Name) {
				case "Invoke":
					this.Invoke(method, Assist.DelegateTargetField, Assist.DelegatePointerField);
					break;
				default:
					throw new NotSupportedException(method.FullName);
				}
			} else*/ {
				if(!target.IsInterface && method.DeclaringType.IsInterface) {
					Type[] types = new Type[method.ParameterCount];
					for(int i=0; i<method.ParameterCount; ++i) {
						types[i] = method.GetParameterType(i);
					}
					method = (MethodInfoImpl)target.GetMethod(method.Name, BindingFlags.Public|BindingFlags.Instance, null, types, null);
					if(method==null) throw new MissingMethodException();
				}
				if(method.DeclaringType.IsInterface) {
					this.CallIfImpl(method, RefFindActualMethodForInterface);
				} else if(!target.IsSealed) {
					this.CallVirtImpl(method, RefFindActualMethod);
				} else {
					ArrayList list = new ArrayList(method.ParameterCount);
					foreach(ParameterInfo p in method.GetParameters()) {
						list.Add(p.ParameterType);
					}
					method = (MethodInfoImpl)target.GetMethod(method.Name
						, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic
						, null, method.CallingConvention
						, (Type[])list.ToArray(typeof(Type)), null);
					Console.WriteLine("CALL-OPT: simply call for "+method.FullName);
					this.Call(method);
				}	
			}
		}

		#endregion

		public static void CompileTest() {
			MethodInfoImpl m = CooS.Management.AssemblyResolver.Manager.ResolveMethod("CooS.Formats.CLI.MethodDefInfo:GenerateExecutableCode",true);
			CodeInfo code = CooS.Execution.CodeManager.GetExecutableCode(m);
		}

	}

}
