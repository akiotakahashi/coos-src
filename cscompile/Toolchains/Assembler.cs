using System;
using System.IO;
using System.Collections.Generic;
using CooS.Reflection;
using CooS.Collections;
using CooS.Execution;
using CooS.Toolchains;
using BindingFlags=System.Reflection.BindingFlags;
using MethodImplAttributes=System.Reflection.MethodImplAttributes;

namespace CooS.Toolchains {

	public abstract class Assembler {

		const CodeLevel JIT_COMPILE_LEVEL = CodeLevel.Native;

		public abstract World World { get;}
		public abstract Engine Engine { get;}
		public abstract MethodInfo Method { get;}

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
		public abstract void Pop(TypeInfo type);
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
		public abstract void LoadTarget(MethodInfo target);
		public abstract void CallInd(MethodInfo signature);
		protected abstract CodeLabel CallImpl(MethodInfo target);
		protected abstract void CallIfImpl(MethodInfo target, MethodInfo finder);
		protected abstract void CallVirtImpl(MethodInfo target, MethodInfo finder);

		public CodeLabel Call(MethodInfo method) {
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
		public abstract void Invoke(FieldInfo target, FieldInfo function);

		//
		// Try-catch Operations
		//

		public abstract void Throw();
		public abstract void Leave(IBranchTarget target, TypeInfo[] discards);

		//
		// Load and Store Operations
		//

		public abstract void LoadNull();
		public abstract void LoadConstant(int value);
		public abstract void LoadConstant(float value);
		public abstract void LoadConstant(double value);
		public abstract void LoadConstant(IntPtr value);

		public abstract void LoadInd(TypeInfo type);
		public abstract void StoreInd(TypeInfo type);

		public abstract void LoadArg(int index);
		public abstract void StoreArg(int index);
		public abstract void LoadArgAddress(int index);

		public abstract void LoadVar(int index);
		public abstract void StoreVar(int index);
		public abstract void LoadVarAddress(int index);
		public abstract void LoadWorkspace(TypeInfo type);
		public abstract void LoadWorkspaceAddress();

		public abstract void LoadElement(TypeInfo type);
		public abstract void StoreElement(TypeInfo type);
		public abstract void LoadElementAddress(TypeInfo type);

		//
		// Load Token
		//

		public void LoadToken(TypeInfo type) {
			this.LoadConstant(type.TypeHandle);
		}

		public void LoadToken(FieldInfo field) {
			this.LoadConstant(field.FieldHandle);
		}

		public void LoadToken(MethodInfo method) {
			this.LoadConstant(method.MethodHandle);
		}

		public abstract void LoadEntryPoint(MethodInfo method);

		//
		// Field Access Operations
		//

		public void LoadLength() {
			FieldInfo field = this.Engine.Realize(this.World.SzArray.FindField("Length"));
			if(field==null) throw new UnexpectedException("SzArray:Length is missed");
			this.LoadField(field);
		}

		public abstract void LoadField(FieldInfo field);
		public abstract void StoreField(FieldInfo field);
		public abstract void LoadFieldAddress(FieldInfo field);

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
		/// storetype はコピーの型で、realtype はコピー元の実際の型です。
		/// </summary>
		/// <param name="type"></param>
		public abstract void CopyObject(TypeInfo type);		// ..., [nint] obj, [nint] adr -> ..., [nint] adr
		public abstract void LoadObject(TypeInfo type);		// ..., [nint] adr -> ..., [type] value
		public abstract void StoreObject(TypeInfo storetype, TypeInfo realtype);		// ..., [nint] adr, [nint] obj

		//
		// Conversion
		//

		public abstract void Convert(TypeInfo from, TypeInfo to);

		//
		// Functionalization
		//

		public abstract void Prologue();
		public abstract void Epilogue();
		public abstract void Trap();
		protected abstract void WriteMethodStub(MethodInfo stubmethod);

		//
		//****************************************************************************************
		//
		//		Implementation
		//
		//****************************************************************************************

		#region インスタンスと文字列の生成

		static MethodInfo allocateobjectimpl;

		private MethodInfo RefAllocateObjectImpl {
			get {
				if(allocateobjectimpl==null) {
					allocateobjectimpl = this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "AllocateObjectImpl"));
					//CooS.Execution.CodeManager.RegisterProxyMethod(allocateobjectimpl);
				}
				return allocateobjectimpl;
			}
		}

		public void ProcessBlankMethodToAllocate(MethodInfo constructor) {
			if(0!=(constructor.ImplFlags&MethodImplAttributes.Runtime)) {
				if(constructor.Type.IsSubclassOf(this.World.Delegate)) {
					this.LoadConstant(constructor.Assembly.Id);
					this.LoadConstant(constructor.Type.Id);
					this.Call(this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "AllocateDelegate")));
					return;
				}
			}
			MethodBase wrap = constructor.Base.FindWrappingMethod();
			if(wrap!=null) {
				this.Call(this.Engine.Realize(wrap));
			} else {
				//throw new MissingMethodException(constructor.FullName);
				Console.WriteLine("FAILED TO EMBED ALLOCATION CALLING: ImplFlags={0}", constructor.ImplFlags);
				for(int i=constructor.ParameterCount-1; i>=0; --i) {
					this.Pop(this.Engine.Realize(constructor.GetParameterType(i)));
				}
				this.LoadNull();
			}
		}

		public void AllocateObject(MethodInfo constructor) {
			if(!constructor.IsConstructor) { throw new ArgumentException("Specified method is not a constructor."); }
			if(constructor.Type.IsValueType) {
				this.LoadWorkspaceAddress();
				this.CallImpl(constructor);
				this.LoadWorkspace(this.Engine.Realize(constructor.Type));
			} else if(constructor.IsBlank) {
				this.ProcessBlankMethodToAllocate(constructor);
			} else {
				// ..., argN
				this.LoadWorkspaceAddress();
				// ..., argN, wsa
				this.LoadConstant(constructor.Assembly.Id);
				this.LoadConstant((constructor.Type).Id);
				// ..., argN, wsa, aid, tid
				this.Call(RefAllocateObjectImpl);
				// ..., argN, wsa, obj
				this.StoreInd(this.Engine.Realize(this.World.IntPtr));
				// ..., argN
				this.LoadWorkspaceAddress();
				// ..., argN, wsa
				this.LoadInd(this.Engine.Realize(this.World.IntPtr));
				// ..., argN, obj
				this.CallImpl(constructor);
				// ...
				this.LoadWorkspaceAddress();
				// ..., wsa
				this.LoadInd(this.Engine.Realize(this.World.IntPtr));
				// ..., obj
			}
		}

		public void AllocateSzArray(TypeInfo type) {
			if(type.Base is CooS.Reflection.Derived.DerivedType) {
				this.LoadConstant(type.TypeHandle);
				this.Call(this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "AllocateSzArrayImplEx")));
			} else {
				this.LoadConstant(type.Assembly.Id);
				this.LoadConstant(type.Id);
				this.Call(this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "AllocateSzArrayImpl")));
			}
		}

		static Inttable<string> stringtable = new Inttable<string>();

		private static string LoadStringImpl(int sid) {
			return stringtable[sid];
		}

		public void LoadString(int sid) {
			this.LoadConstant(this.Method.Assembly.Id);
			this.LoadConstant(sid);
			this.Call(this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "LoadString")));
		}

		#endregion

		#region ボクシングとアンボクシング

		public void Box(TypeInfo type) {
			this.LoadConstant(type.Assembly.Id);
			this.LoadConstant(type.Id);
			this.Call(this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "BoxImpl")));
			this.CopyObject(type);
		}

		public void Unbox(TypeInfo type) {
			// Unlike box, which is required to make a copy of a value type for use in the object,
			// unbox is not required to copy the value type from the object. Typically it simply
			// computes the address of the value type that is already present inside of the boxed object.
			// オブジェクトの中に値型の複製を作るboxと異なり、unboxはオブジェクトからの複製を必要としません。
			// 典型的には、unboxは単にオブジェクトの内部にある値型の部分のアドレスを計算するだけです。
		}

		#endregion

		#region キャスト

		private static object IsInstImpl(object obj, int aid, int tid) {
#if VMIMPL
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeInfo type = assembly.GetTypeInfo(tid);
			return type.IsInstanceOfType(obj) ? obj : null;
#else
			throw new NotImplementedException();
#endif
		}

		public void IsInst(TypeInfo type) {
#if VMIMPL
			this.LoadConstant(type.Assembly.Id);
			this.LoadConstant(type.Id);
			this.Call(this.Method.Assembly.Manager.ResolveMethod("CooS.Compile.Assembler:IsInstImpl",true));
#else 
			throw new NotImplementedException();
#endif
		}

		public void CastClass(TypeInfo type) {
			if(type.IsPointer) return;
			if(type.IsArray) return;
			this.LoadConstant(type.Assembly.Id);
			this.LoadConstant(type.Id);
			this.Call(this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "CastClassImpl")));
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
#if VMPL
			//Console.WriteLine("ParepareExcurable(sp=0x{0:X8}, *sp=0x{3:X8}, aid={1}, rowIndex={2})", (int)sp, assemblyid, rowIndex, sp->ToInt32());
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(assemblyid);
			MethodInfo method = assembly.GetMethodInfo(rowIndex);
			//Console.WriteLine("Preparing method = {0}", method.FullName);
#if false
			Assembler asm = Architecture.CreateAssembler(method, 0);
			if(asm.TotalArgumentSize>0) {
				Assist.Dump(new IntPtr(sp+1), 0, asm.TotalArgumentSize);
			}
			if(!method.IsStatic || method.Name=="FindActualMethod") {
				IntPtr p = *(sp+1+Architecture.GetStackingLength(asm.GetArgumentOffset(0)));
				if(method.Type.IsValueType) {
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
#else
			throw new NotImplementedException();
#endif
		}

		static MethodInfo prepareexecutablecode;

		private MethodInfo RefPrepareExecutableCode {
			get {
#if VMIMPL
				if(prepareexecutablecode==null) {
					prepareexecutablecode = this.Method.Assembly.Manager.ResolveMethod("CooS.Compile.Assembler:PrepareExecutableCode",true);
					CooS.Execution.CodeManager.RegisterProxyMethod(prepareexecutablecode);
				}
				return prepareexecutablecode;
#else
				throw new NotImplementedException();
#endif
			}
		}

		public void WriteMethodStub() {
			this.WriteMethodStub(RefPrepareExecutableCode);
		}

		#endregion

		#region 仮想メソッド

		static Exception nullex = new NullReferenceException();

		MethodInfo findactualmethod;
		MethodInfo findactualmethod4if;

		private MethodInfo RefFindActualMethod {
			get {
				if(findactualmethod==null) {
					findactualmethod = this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "FindActualMethod"));
					//CooS.Execution.CodeManager.RegisterProxyMethod(findactualmethod);
				}
				return findactualmethod;
			}
		}

		private MethodInfo RefFindActualMethodForInterface {
			get {
				if(findactualmethod4if==null) {
					findactualmethod4if = this.Engine.Realize(this.World.ResolveMethod("CooS.Runtime.CLI", "RuntimeImpl", "FindActualMethodForInterface"));
					//CooS.Execution.CodeManager.RegisterProxyMethod(findactualmethod4if);
				}
				return findactualmethod4if;
			}
		}

		public void CallVirt(TypeInfo target, MethodInfo method) {
			if(!method.IsVirtual) {
				// C#でSystem.Object:GetType()を呼ぶと、instance なのになぜか callvirt で実行しようとする。
				switch(method.FullName) {
				case "System.Object:GetType":
					this.LoadConstant(4);
					this.Sub32(false, false);
					this.LoadInd(this.Engine.Realize(this.World.IntPtr));
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
				if(!target.IsInterface && method.Type.IsInterface) {
					TypeBase[] types = new TypeBase[method.ParameterCount];
					for(int i=0; i<method.ParameterCount; ++i) {
						types[i] = method.GetParameterType(i);
					}
					MemberRefDesc desc = new MemberRefDesc(method.Base);
					method = this.Engine.Realize(target.Base.FindMethod(desc));
					if(method==null) throw new MissingMethodException();
				}
				if(method.Type.IsInterface) {
					this.CallIfImpl(method, RefFindActualMethodForInterface);
				} else if(!target.IsSealed) {
					this.CallVirtImpl(method, RefFindActualMethod);
				} else {
					int slotindex = method.SlotIndex;
					MemberRefDesc desc = new MemberRefDesc(method.Base);
					method = this.Engine.Realize(target.GetSlotMethod(slotindex));
					Console.WriteLine("CALL-OPT: simply call for "+method.FullName);
					this.Call(method);
				}	
			}
		}

		#endregion

	}

}
