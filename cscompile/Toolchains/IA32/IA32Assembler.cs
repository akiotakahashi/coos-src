using System;
using System.IO;
using System.Collections;
using CooS.Reflection;
using CooS.Compile;
using CooS.Execution;

namespace CooS.Toolchains.IA32 {
	using IA32Toolkit.Assembler;

	public sealed class IA32Assembler : Assembler {

		readonly ArrayList labels = new ArrayList();
		readonly Compiler Compiler;
		readonly MethodInfo method;
		readonly MemoryStream stream;
		readonly IA32Toolkit.Assembler.IA32Assembler asm;
		readonly int workingsize;

		public IA32Assembler(Compiler compiler, MethodInfo method, int workingsize) {
			this.Compiler = compiler;
			this.method = method;
			this.workingsize = workingsize;
			this.stream = new MemoryStream();
			this.asm = new IA32Toolkit.Assembler.IA32Assembler(this.stream);
		}

		public override World World {
			get {
				return this.Compiler.World;
			}
		}

		public override Engine Engine {
			get { 
				return this.Compiler.Engine;
			}
		}
		
		public override MethodInfo Method {
			get {
				return this.method;
			}
		}

		private int OffsetToSzArrayData {
			get {
				return this.Engine.Realize(this.World.SzArray).InstanceSize;
			}
		}

		public CodeLabel CreateRelativeLabel(IBranchTarget target) {
			CodeLabel lbl = new RelativeLabel((int)this.stream.Position-4, target);
			this.labels.Add(lbl);
			return lbl;
		}

		public CodeLabel CreateAbsoluteLabel(byte[] target, int offset) {
			CodeLabel lbl = new AbsoluteLabel((int)this.stream.Position-4, target, offset+OffsetToSzArrayData);
			this.labels.Add(lbl);
			return lbl;
		}

		private void ChangeMode() {
			this.asm.BaseStream.WriteByte(0x66);
		}

		private void Zero(Register32 reg) {
			this.asm.Xor(reg, (RegMem32)reg);
		}

		#region 型指定されたメモリアクセスのためのメソッド

		private void StoreMemory(Register32 origin, int offset, int size) {
			if(origin==Register32.EBX) throw new UnexpectedException();
			StoreMemory(RegMem.Indirect(origin,offset), size);
		}

		private void StoreMemory(RegMem dst, int size) {
			switch(size) {
			case 0:
				throw new ArgumentException();
			case 1:
			case 2:
			case 4:
				this.asm.Pop(Register32.EBX);
				switch(size) {
				case 1:
					this.asm.Mov(dst, Register8.BL);
					break;
				case 2:
					this.ChangeMode();
					this.asm.Mov(dst, Register16.BX);
					break;
				case 4:
					this.asm.Mov(dst, Register32.EBX);
					break;
				default:
					throw new UnexpectedException();
				}
				break;
			case 8:
				this.asm.Pop(Register32.EBX);
				this.asm.Mov(dst, Register32.EBX);
				this.asm.Pop(Register32.EBX);
				dst.Disp += 4;
				this.asm.Mov(dst, Register32.EBX);
				break;
			default:
				this.asm.Cld();
				this.asm.Lea(Register32.EDI, dst);
				this.asm.Mov((RegMem32)Register32.ESI, Register32.ESP);
				this.asm.Mov(Register32.ECX, size);
				this.asm.Rep_movs_m8_m8();
				this.asm.Add(Register32.ESP, Architecture.GetStackingSize(size));
				break;
			}
		}

		private void LoadMemory_Any(Register32 origin, int offset, int size) {
			if(size<IntPtr.Size) throw new ArgumentException();
			if(size<=IntPtr.Size*4) {
				while(size>0) {
					size -= 4;
					this.asm.Push((RegMem32)RegMem.Indirect(origin, offset+size));
				}
			} else {
				int ssz = Architecture.GetStackingSize(size);
				this.asm.Sub(Register32.ESP, ssz);
				this.asm.Cld();
				this.asm.Mov(Register32.ECX, ssz);
				this.asm.Mov((RegMem32)Register32.EDI, Register32.ESP);
				this.asm.Lea(Register32.ESI, RegMem.Indirect(origin,offset));
				this.asm.Rep_movs_m8_m8();
			}
		}

		private void LoadMemory_Sn(Register32 origin, int offset, int size) {
			if(size>=IntPtr.Size) throw new ArgumentException();
			switch(size) {
			case 2:
				this.asm.Movsx(Register32.EAX, (RegMem16)RegMem.Indirect(origin,offset));
				this.asm.Push(Register32.EAX);
				break;
			case 1:
				this.asm.Movsx(Register32.EAX, (RegMem8)RegMem.Indirect(origin,offset));
				this.asm.Push(Register32.EAX);
				break;
			default:
				throw new NotSupportedException("size="+size);
			}
		}

		private void LoadMemory_Un(Register32 origin, int offset, int size) {
			if(size>=IntPtr.Size) throw new ArgumentException();
			switch(size) {
			case 2:
				this.asm.Movzx(Register32.EAX, (RegMem16)RegMem.Indirect(origin,offset));
				this.asm.Push(Register32.EAX);
				break;
			case 1:
				this.asm.Movzx(Register32.EAX, (RegMem8)RegMem.Indirect(origin,offset));
				this.asm.Push(Register32.EAX);
				break;
			default:
				throw new NotSupportedException("size="+size);
			}
		}

		private void LoadMemory(TypeInfo type, Register32 origin, int offset) {
			int size = type.VariableSize;
			if(size>=IntPtr.Size) {
				this.LoadMemory_Any(origin, offset, size);
			} else {
				if(type.IsSigned) {
					this.LoadMemory_Sn(origin, offset, size);
				} else {
					this.LoadMemory_Un(origin, offset, size);
				}
			}
		}

		#endregion

		#region Code Manipulation
		
		public override int Position {
			get {
				return (int)this.stream.Position;
			}
		}

		public override CodeInfo GenerateCode() {
			this.stream.Flush();
			CodeInfo ci = new CodeInfo(this.stream.ToArray(), 0);
			ci.Labels = (CodeLabel[])this.labels.ToArray(typeof(CodeLabel));
			return ci;
		}

		private int GetArgumentSize(MethodBase method, int index) {
			return this.Engine.Realize(method.GetArgumentType(index)).VariableSize;
		}

		private int GetNaturalArgumentOffset(MethodBase method, int iarg) {
			int offset = 0;
			if(this.Engine.Realize(method.ReturnType).VariableSize>8) {
				// 8バイトを超える構造体の戻り値受け渡し用
				offset += IntPtr.Size;
				if(iarg==method.ArgumentCount) {
					return 0;
				}
			}
			for(int i=method.ArgumentCount-1; i>iarg; --i) {
				int size = this.GetArgumentSize(method, i);
				offset += Architecture.GetStackingSize(size);
			}
			return offset;
		}

		public override int GetArgumentOffset(int iarg) {
			int offset = 0;
			if(method.IsConstructor) {
				// コンストラクタの場合、インスタンスへの参照が一番最後にプッシュされる
				// 参照型も値型も参照はアドレスとして渡される
				if(iarg==0) {
					return 0;
				} else {
					offset += IntPtr.Size;
				}
			}
			return offset+GetNaturalArgumentOffset(this.method.Base, iarg);
		}

		private int GetVariableSize(MethodBase method, int index) {
			return this.Engine.Realize(method.GetVariableType(index)).VariableSize;
		}

		public override int GetVariableOffset(int index) {
			int offset = 0;
			for(int i=0; i<index; ++i) {
				int size = this.GetVariableSize(this.method.Base, i);
				offset = Architecture.AlignOffset(offset, size);
				offset += size;
			}
			return Architecture.AlignOffset(offset, this.GetVariableSize(this.method.Base, index));
		}

		private int CalcTotalArgumentSize(MethodBase method) {
			if(method.ArgumentCount==0) {
				return 0;
			} else {
				int offset = GetNaturalArgumentOffset(method, 0);
				int size = this.GetArgumentSize(method, 0);
				return Architecture.GetStackingSize(offset+size);
			}
		}

		public override int TotalArgumentSize {
			get {
				return CalcTotalArgumentSize(this.method.Base);
			}
		}

		public override int TotalVariableSize {
			get {
				if(this.method.VariableCount==0) {
					return 0;
				} else {
					int offset = 0;
					for(int i=0; i<this.method.VariableCount; ++i) {
						int size = this.GetVariableSize(this.method.Base, i);
						offset = Architecture.AlignOffset(offset, size);
						offset += size;
					}
					return Architecture.GetStackingSize(offset);
				}
			}
		}

		public override int BaseOffsetToArguments {
			get {
				return IntPtr.Size*2;
			}
		}

		public override int BaseOffsetToVariables {
			get {
				return -this.TotalVariableSize-this.workingsize;
			}
		}

		#endregion

		#region Stack Operations

		public override void Pop(int size) {
			size = Architecture.GetStackingSize(size);
			this.asm.Add(Register32.ESP, size);
		}

		public override void Pop(TypeInfo type) {
			this.Pop(type.VariableSize);
		}

		public override void Duplicate(int size) {
			if(size<=IntPtr.Size) {
				this.asm.Push((RegMem32)RegMem.Indirect(Register32.ESP));
			} else {
				throw new NotImplementedException();
			}
		}

		#endregion

		#region Methematical Operations

		public override void Add32(bool signed, bool overflow) {
			this.asm.Pop(Register32.EAX);
			this.asm.Add(RegMem.Indirect(Register32.ESP), Register32.EAX);
			if(overflow) {
				if(signed) {
					this.asm.Into();
				} else {
					throw new NotImplementedException();
				}
			}
		}
		
		public override void Add64(bool signed, bool overflow) {
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.ECX);
			this.asm.Add(RegMem.Indirect(Register32.ESP), Register32.EAX);
			this.asm.Adc(RegMem.Indirect(Register32.ESP,+4), Register32.ECX);
			if(overflow) {
				if(signed) {
					this.asm.Into();
				} else {
					throw new NotImplementedException();
				}
			}
		}

		public override void Sub32(bool signed, bool overflow) {
			this.asm.Pop(Register32.EAX);
			this.asm.Sub(RegMem.Indirect(Register32.ESP), Register32.EAX);
			if(overflow) {
				if(signed) {
					this.asm.Into();
				} else {
					throw new NotImplementedException();
				}
			}
		}

		public override void Sub64(bool signed, bool overflow) {
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.ECX);
			this.asm.Sub(RegMem.Indirect(Register32.ESP), Register32.EAX);
			this.asm.Sbb(RegMem.Indirect(Register32.ESP,+4), Register32.ECX);
			if(overflow) {
				if(signed) {
					this.asm.Into();
				} else {
					throw new NotImplementedException();
				}
			}
		}

		public override void Mul32(bool signed, bool overflow) {
			this.asm.Pop(Register32.EAX);
			if(signed) {
				this.asm.Imul((RegMem32)RegMem.Indirect(Register32.ESP));
			} else {
				this.asm.Mul((RegMem32)RegMem.Indirect(Register32.ESP));
			}
			this.asm.Mov(RegMem.Indirect(Register32.ESP), Register32.EAX);
			if(overflow) this.asm.Into();
		}

		public override void Mul64(bool signed, bool overflow) {
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.ESI);
			this.asm.Pop(Register32.EDI);
			if(signed) {
				if(overflow) {
					this.asm.Mov((RegMem32)Register32.EBX, Register32.ECX);
					this.asm.Imul(Register32.EBX, Register32.EDI);
					this.asm.Into();
				}
				this.asm.Imul(Register32.ESI, Register32.ECX);
				this.asm.Imul(Register32.EDI, Register32.EBX);
				this.asm.Imul(Register32.ESI);
				this.asm.Add((RegMem32)Register32.EDX, Register32.ECX);
				if(overflow) this.asm.Into();
				this.asm.Add((RegMem32)Register32.EDX, Register32.EDI);
				if(overflow) this.asm.Into();
				this.asm.Push(Register32.EDX);
				this.asm.Push(Register32.EAX);
			} else {
				throw new NotImplementedException();
			}
		}

		public override void Div32(bool signed) {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EAX);
			if(signed) {
				this.asm.Cdq();
				this.asm.Idiv(Register32.ECX);
			} else {
				this.Zero(Register32.EDX);
				this.asm.Div(Register32.ECX);
			}
			this.asm.Push(Register32.EAX);
		}

		public override void Div64(bool signed) {
			this.asm.Fld((Mem64F)RegMem.Indirect(Register32.ESP,8));
			this.asm.Fdiv((Mem64F)RegMem.Indirect(Register32.ESP));
			this.asm.Fst((Mem64F)RegMem.Indirect(Register32.ESP,8));
			this.asm.Add(Register32.ESP, 8);
		}

		public override void Rem32(bool signed) {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EAX);
			if(signed) {
				this.asm.Cdq();
				this.asm.Idiv(Register32.ECX);
			} else {
				this.Zero(Register32.EDX);
				this.asm.Div(Register32.ECX);
			}
			this.asm.Push(Register32.EDX);
		}

		public override void Rem64(bool signed) {
			throw new NotImplementedException();
		}

		public override void Negate32() {
			this.asm.Neg((RegMem32)RegMem.Indirect(Register32.ESP));
		}

		public override void Negate64() {
			throw new NotImplementedException();
		}

		#endregion

		#region Logical Operations

		public override void And32() {
			this.asm.Pop(Register32.EAX);
			this.asm.And(RegMem.Indirect(Register32.ESP), Register32.EAX);
		}

		public override void And64() {
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.EDX);
			this.asm.And(RegMem.Indirect(Register32.ESP), Register32.EAX);
			this.asm.And(RegMem.Indirect(Register32.ESP,4), Register32.EDX);
		}

		public override void Or32() {
			this.asm.Pop(Register32.EAX);
			this.asm.Or(RegMem.Indirect(Register32.ESP), Register32.EAX);
		}

		public override void Or64() {
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.EDX);
			this.asm.Or(RegMem.Indirect(Register32.ESP), Register32.EAX);
			this.asm.Or(RegMem.Indirect(Register32.ESP,4), Register32.EDX);
		}

		public override void Xor32() {
			this.asm.Pop(Register32.EAX);
			this.asm.Xor(RegMem.Indirect(Register32.ESP), Register32.EAX);
		}

		public override void Xor64() {
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.EDX);
			this.asm.Xor(RegMem.Indirect(Register32.ESP), Register32.EAX);
			this.asm.Xor(RegMem.Indirect(Register32.ESP,4), Register32.EDX);
		}

		public override void Not32() {
			this.asm.Not((RegMem32)RegMem.Indirect(Register32.ESP));
		}

		public override void Not64() {
			throw new NotImplementedException();
		}

		public override void Shl32() {
			this.asm.Pop(Register32.ECX);
			this.asm.Shl_CL((RegMem32)RegMem.Indirect(Register32.ESP));
		}

		public override void Shl64() {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EAX);
			this.asm.Shld_CL(RegMem.Indirect(Register32.ESP), Register32.EAX);
			this.asm.Shl_CL(Register32.EAX);
			this.asm.Push(Register32.EAX);
		}

		public override void Shr32(bool signed) {
			this.asm.Pop(Register32.ECX);
			if(signed) {
				this.asm.Sar_CL((RegMem32)RegMem.Indirect(Register32.ESP));
			} else {
				this.asm.Shr_CL((RegMem32)RegMem.Indirect(Register32.ESP));
			}
		}

		public override void Shr64(bool signed) {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.EDX);
			if(signed) {
				this.asm.Shrd_CL(Register32.EAX, Register32.EDX);
				this.asm.Sar_CL(Register32.EDX);
			} else {
				this.asm.Shrd_CL(Register32.EAX, Register32.EDX);
				this.asm.Shr_CL(Register32.EDX);
			}
			this.asm.Push(Register32.EDX);
			this.asm.Push(Register32.EAX);
		}

		#endregion

		#region Compare Operations

		public override void CompareI32(Condition cond, bool signed) {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EAX);
			this.Zero(Register32.EDX);
			this.asm.Cmp((RegMem32)Register32.EAX, Register32.ECX);
			switch(cond) {
			case Condition.Equal:
				this.asm.Sete(Register8.DL);
				break;
			case Condition.GreaterThan:
				if(signed) {
					this.asm.Setg(Register8.DL);
				} else {
					this.asm.Seta(Register8.DL);
				}
				break;
			case Condition.LessThan:
				if(signed) {
					this.asm.Setl(Register8.DL);
				} else {
					this.asm.Setb(Register8.DL);
				}
				break;
			default:
				throw new NotSupportedException();
			}
			this.asm.Push(Register32.EDX);
		}

		public override void CompareI64(Condition cond, bool signed) {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EDI);
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.ESI);
			this.Zero(Register32.EDX);
			// たぶんこれでいいと思うんだけど…。
			this.asm.Sub((RegMem32)Register32.EAX, Register32.ECX);
			this.asm.Sbb((RegMem32)Register32.ESI, Register32.EDI);
			switch(cond) {
			case Condition.Equal:
				this.asm.Sete(Register8.DL);
				break;
			case Condition.NotEqual:
				this.asm.Setne(Register8.DL);
				break;
			case Condition.GreaterThan:
				if(signed) {
					this.asm.Setg(Register8.DL);
				} else {
					this.asm.Seta(Register8.DL);
				}
				break;
			case Condition.GreaterOrEqual:
				if(signed) {
					this.asm.Setge(Register8.DL);
				} else {
					this.asm.Setae(Register8.DL);
				}
				break;
			case Condition.LessThan:
				if(signed) {
					this.asm.Setl(Register8.DL);
				} else {
					this.asm.Setb(Register8.DL);
				}
				break;
			case Condition.LessOrEqual:
				if(signed) {
					this.asm.Setle(Register8.DL);
				} else {
					this.asm.Setbe(Register8.DL);
				}
				break;
			default:
				throw new NotSupportedException();
			}
			this.asm.Push(Register32.EDX);
		}

		public override void CompareR32(Condition cond) {
			throw new NotImplementedException();
		}

		public override void CompareR64(Condition cond) {
			throw new NotImplementedException();
		}

		#endregion

		#region Branch Operations

		public override CodeLabel Branch(IBranchTarget target) {
			this.asm.Jmp(0x00EFBE00);
			return this.CreateRelativeLabel(target);
		}

		public override CodeLabel BranchI32(Condition cond, bool signed, IBranchTarget target) {
			switch(cond) {
			case Condition.True:
			case Condition.False:
				this.asm.Pop(Register32.EAX);
				this.asm.Test(Register32.EAX,Register32.EAX);
				switch(cond) {
				case Condition.True:
					this.asm.Jnz(0x00EFBE00);
					break;
				case Condition.False:
					this.asm.Jz(0x00EFBE00);
					break;
				default:
					throw new UnexpectedException();
				}
				break;
			default:
				this.asm.Pop(Register32.ECX);
				this.asm.Pop(Register32.EAX);
				this.asm.Cmp((RegMem32)Register32.EAX, Register32.ECX);
				switch(cond) {
				case Condition.GreaterOrEqual:
					if(signed) {
						this.asm.Jge(0x00EFBE00);
					} else {
						this.asm.Jae(0x00EFBE00);
					}
					break;
				case Condition.GreaterThan:
					if(signed) {
						this.asm.Jg(0x00EFBE00);
					} else {
						this.asm.Ja(0x00EFBE00);
					}
					break;
				case Condition.LessOrEqual:
					if(signed) {
						this.asm.Jle(0x00EFBE00);
					} else {
						this.asm.Jbe(0x00EFBE00);
					}
					break;
				case Condition.LessThan:
					if(signed) {
						this.asm.Jl(0x00EFBE00);
					} else {
						this.asm.Jb(0x00EFBE00);
					}
					break;
				case Condition.Equal:
					this.asm.Je(0x00EFBE00);
					break;
				case Condition.NotEqual:
					this.asm.Jne(0x00EFBE00);
					break;
				default:
					throw new UnexpectedException();
				}
				break;
			}
			return this.CreateRelativeLabel(target);
		}

		public override CodeLabel BranchI64(Condition cond, bool signed, IBranchTarget target) {
			this.CompareI64(cond, signed);
			return this.BranchI32(Condition.True, false, target);
		}

		public override CodeLabel BranchR32(Condition cond, IBranchTarget target) {
			throw new NotImplementedException();
		}

		public override CodeLabel BranchR64(Condition cond, IBranchTarget target) {
			throw new NotImplementedException();
		}

		public override void Switch(IBranchTarget[] targets) {
			this.asm.Pop(Register32.EAX);
			this.asm.Test((RegMem32)Register32.EAX, Register32.EAX);
			this.asm.Jz(0x00EFBE00);
			this.CreateRelativeLabel(targets[0]);
			for(int i=1; i<targets.Length; ++i) {
				this.asm.Dec(Register32.EAX);
				this.asm.Jz(0x00EFBE00);
				this.CreateRelativeLabel(targets[i]);
			}
		}

		#endregion

		#region Call Operations

		public override void Return() {
			int argsize = this.TotalArgumentSize;
			int retsize = this.Engine.Realize(this.method.ReturnType).VariableSize;
			// pop returning values
			if(retsize>0) {
				if(retsize>8) {
					this.LoadReturningAddress();
					this.StoreInd(this.Engine.Realize(this.method.ReturnType), true);
				} else {
					if(retsize>0) this.asm.Pop(Register32.EAX);
					if(retsize>4) this.asm.Pop(Register32.EDX);
				}
			}
			// leave
			// retd argsize
			this.asm.Leave_ESP();
			if(argsize==0) {
				this.asm.Ret();
			} else {
				this.asm.Ret((ushort)argsize);
			}
		}

		public override void LoadTarget(MethodInfo method) {
			this.asm.Push((RegMem32)RegMem.Indirect(Register32.ESP, GetNaturalArgumentOffset(method.Base, 0)));
		}

		private void PushReturnValue(TypeInfo rettype) {
			// push <retval> if not void
			if(!rettype.IsPrimitive && rettype.Name!="Void") {
				int size = rettype.VariableSize;
				if(size>8) throw new NotSupportedException();
				if(size>4) this.asm.Push(Register32.EDX);
				if(size>0) this.asm.Push(Register32.EAX);
			}
		}

		public override void CallInd(MethodInfo signature) {
			//this.Trap();
			this.asm.Pop(Register32.EAX);
			this.asm.Call(Register32.EAX);
			this.PushReturnValue(this.Engine.Realize(signature.ReturnType));
		}

		protected override CodeLabel CallImpl(MethodInfo method) {
			int retsize = this.Engine.Realize(method.ReturnType).VariableSize;
			if(retsize>8) {
				this.LoadWorkspaceAddress();
			}
			this.asm.Call(0x00EFBE00);
			CodeLabel lbl = this.CreateRelativeLabel(method);
			if(retsize>8) {
				this.LoadWorkspace(this.Engine.Realize(method.ReturnType));
			} else {
				this.PushReturnValue(this.Engine.Realize(method.ReturnType));
			}
			return lbl;
		}

		protected override void CallIfImpl(MethodInfo target, MethodInfo finder) {
			if(!target.Type.IsInterface) throw new ArgumentException("Argument is not an interface type.");
			this.asm.Push((RegMem32)RegMem.Indirect(Register32.ESP, CalcTotalArgumentSize(target.Base)-IntPtr.Size));
			this.asm.Push(target.Assembly.Id);
			this.asm.Push(target.Type.Id);
			this.asm.Push(target.SlotIndex);
			this.CallImpl(finder);
			this.CallInd(target);
		}

		protected override void CallVirtImpl(MethodInfo target, MethodInfo finder) {
			if(target.IsDelegateInvoke) {
				int objpos = this.TotalArgumentSize-IntPtr.Size;
				this.asm.Mov(Register32.EAX, RegMem.Indirect(Register32.ESP, objpos));
				this.asm.Mov(Register32.ECX, RegMem.Indirect(Register32.EAX, 4));	// Must be sync with Delegate.m_target
				this.asm.Test(Register32.ECX, Register32.ECX);
				this.asm.Jnz(0xA);
				this.asm.Call(Register32.ECX);
				this.asm.Add(Register32.ESP, 4);
				this.asm.Jmp(0x0F);
				this.asm.Mov(RegMem.Indirect(Register32.ESP,objpos), Register32.ECX);
				this.asm.Mov(Register32.ECX, RegMem.Indirect(Register32.EAX,12));
				this.asm.Call(Register32.ECX);
				this.PushReturnValue(this.Engine.Realize(target.ReturnType));
			} else if(target.Type.IsInterface) {
				throw new NotSupportedException();
			} else {
				this.asm.Push((RegMem32)RegMem.Indirect(Register32.ESP, CalcTotalArgumentSize(target.Base)-IntPtr.Size));
				this.asm.Push(target.SlotIndex);
				this.CallImpl(finder);
				this.CallInd(target);
				//this.asm.Pop(Register32.EAX);
				//this.asm.Call(Register32.EAX);
				//this.PushReturnValue(method.ReturnType);
			}
		}

		public override void Invoke(FieldInfo target, FieldInfo function) {
			this.asm.Enter(0, 0);
			// argN, eip, ebp
			this.LoadArg(0);
			this.LoadField(function);
			// argN, eip, ebp, ftn
			this.LoadArg(0);
			this.LoadField(target);
			// argN, eip, ebp, ftn, obj
			this.asm.Pop(Register32.EAX);
			// argN, eip, ebp, ftn
			this.asm.Test(Register32.EAX, Register32.EAX);
			long p0 = this.asm.BaseStream.Position;
			this.asm.Jz(0xBEEF);
			//---> obj, arg1, ..., argN, eip, ebp, ftn
			long p1 = this.asm.BaseStream.Position;
			this.asm.Mov(RegMem.Indirect(Register32.ESP,GetArgumentOffset(0)+IntPtr.Size*3), Register32.EAX);
			this.asm.Pop(Register32.EAX);	// ftn
			this.asm.Pop(Register32.EBP);	// ebp
			this.asm.Jmp(Register32.EAX);
			long p2 = this.asm.BaseStream.Position;
			//---> obj, arg0, ..., argN, eip, ebp, ftn
			this.asm.Pop(Register32.EAX);	// ftn
			this.asm.Pop(Register32.EBP);	// ebp
			this.asm.Pop(Register32.ECX);	// eip
			// obj, arg0, ..., argN
			this.asm.Mov(RegMem.Indirect(Register32.ESP,GetArgumentOffset(0)), Register32.ECX);
			// eip, arg0, ..., argN
			this.asm.Call(Register32.EAX);
			// eip
			this.asm.Ret();
			//---<
			this.asm.BaseStream.Position = p0;
			this.asm.Jz((int)(p2-p1));
		}

		#endregion

		#region Try-catch Operations

		public override void Throw() {
			//TODO: fixme: Throw assembling
			this.asm.Int();
		}

		public override void Leave(IBranchTarget target, TypeInfo[] discards) {
			int size = 0;
			foreach(TypeInfo type in discards) {
				size += Architecture.GetStackingSize(type.VariableSize);
			}
			this.Pop(size);
			this.Branch(target);
		}

		#endregion

		#region Load and Store Operations

		public override void LoadNull() {
			this.asm.Push(0);
		}

		public override void LoadConstant(int value) {
			this.asm.Push(value);
		}

		public override void LoadConstant(float value) {
			this.asm.Push(BitConverter.ToInt32(BitConverter.GetBytes(value),0));
		}

		public override void LoadConstant(double value) {
			byte[] data = BitConverter.GetBytes(value);
			this.asm.Push(BitConverter.ToInt32(data,4));
			this.asm.Push(BitConverter.ToInt32(data,0));
		}

		public override void LoadConstant(IntPtr value) {
			this.LoadConstant(value.ToInt32());
		}

		public override void LoadInd(TypeInfo type) {
			this.asm.Pop(Register32.EAX);
			this.LoadMemory(type, Register32.EAX, 0);
		}

		private void StoreInd(TypeInfo type, bool address_is_last) {
			if(address_is_last) {
				this.asm.Pop(Register32.EAX);
			} else {
				this.asm.Mov(Register32.EAX, RegMem.Indirect(Register32.ESP, Architecture.GetStackingSize(type.VariableSize)));
			}
			this.StoreMemory(Register32.EAX, 0, type.VariableSize);
			if(!address_is_last) {
				this.asm.Add(Register32.ESP, 4);
			}
		}

		public override void StoreInd(TypeInfo type) {
			this.StoreInd(type, false);
		}

		public override void LoadArg(int index) {
			TypeBase type = this.method.GetArgumentType(index);
			int offset = this.BaseOffsetToArguments+GetArgumentOffset(index);
			this.LoadMemory(this.Engine.Realize(type), Register32.EBP, offset);
		}

		private void LoadReturningAddress() {
			TypeBase type = this.method.ReturnType.GetByRefPointerType();
			int offset = this.BaseOffsetToArguments+GetArgumentOffset(this.method.ArgumentCount);
			this.LoadMemory(this.Engine.Realize(type), Register32.EBP, offset);
		}

		public override void StoreArg(int index) {
			int offset = this.BaseOffsetToArguments+GetArgumentOffset(index);
			this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EBP, offset));
			this.StoreMemory(Register32.EAX, 0, this.GetArgumentSize(this.method.Base, index));
		}

		public override void LoadArgAddress(int index) {
			int offset = this.BaseOffsetToArguments+GetArgumentOffset(index);
			this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EBP, offset));
			this.asm.Push(Register32.EAX);
		}

		public override void LoadVar(int index) {
			TypeBase type = this.method.GetVariableType(index);
			int offset = this.BaseOffsetToVariables+GetVariableOffset(index);
			this.LoadMemory(this.Engine.Realize(type), Register32.EBP, offset);
		}

		public override void StoreVar(int index) {
			TypeBase type = this.method.GetVariableType(index);
			int offset = this.BaseOffsetToVariables+this.GetVariableOffset(index);
			this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EBP, offset));
			this.StoreMemory(Register32.EAX, 0, this.Engine.Realize(type).VariableSize);
		}

		public override void LoadVarAddress(int index) {
			int offset = this.BaseOffsetToVariables+this.GetVariableOffset(index);
			this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EBP, offset));
			this.asm.Push(Register32.EAX);
		}

		public override void LoadWorkspace(TypeInfo type) {
			int offset = this.BaseOffsetToVariables+this.TotalVariableSize;
			this.LoadMemory(type, Register32.EBP, offset);
		}

		public override void LoadWorkspaceAddress() {
			int offset = this.BaseOffsetToVariables+this.TotalVariableSize;
			this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EBP, offset));
			this.asm.Push(Register32.EAX);
		}

		public override void LoadElement(TypeInfo type) {
			int offset = this.OffsetToSzArrayData;
			int size = type.VariableSize;
			bool signed = type.IsSigned;
			this.asm.Pop(Register32.ESI);
			this.asm.Pop(Register32.EAX);
			switch(size) {
			case 0:
				throw new ArgumentException();
			case 1:
				this.asm.Mov(Register8.AL, (RegMem8)RegMem.Indirect(Register32.EAX,Register32.ESI,size,offset));
				if(!signed) {
					this.asm.And(Register32.EAX, 0xFF);
				} else {
					this.asm.Cbw_AX();
					this.asm.Cwde_EAX();
				}
				this.asm.Push(Register32.EAX);
				break;
			case 2:
				this.asm.Mov(Register16.AX, (RegMem16)RegMem.Indirect(Register32.EAX,Register32.ESI,size,offset));
				if(!signed) {
					this.asm.And(Register32.EAX, 0xFFFF);
				} else {
					this.asm.Cwde_EAX();
				}
				this.asm.Push(Register32.EAX);
				break;
			case 4:
				this.asm.Push((RegMem32)RegMem.Indirect(Register32.EAX,Register32.ESI,size,offset));
				break;
			case 8:
				this.asm.Push((RegMem32)RegMem.Indirect(Register32.EAX,Register32.ESI,size,4+offset));
				this.asm.Push((RegMem32)RegMem.Indirect(Register32.EAX,Register32.ESI,size,offset));
				break;
			default:
				this.asm.Imul(Register32.ESI, size);
				size = Architecture.AlignOffset(size, 4);
				while(size>0) {
					size -= 4;
					this.asm.Push((RegMem32)RegMem.Indirect(Register32.EAX,Register32.ESI,1,size+offset));
				}
				break;
			}
		}

		public override void StoreElement(TypeInfo type) {
			int offset = Architecture.GetStackingSize(type.VariableSize);
			this.asm.Mov(Register32.EAX, RegMem.Indirect(Register32.ESP, offset));
			this.asm.Mov(Register32.EDX, RegMem.Indirect(Register32.ESP, offset+IntPtr.Size));
			this.StoreMemory(
				RegMem.Indirect(Register32.EDX,
					Register32.EAX,
					type.VariableSize,
					this.OffsetToSzArrayData
				)
				, type.VariableSize);
			this.Pop(IntPtr.Size*2);
		}

		public override void LoadElementAddress(TypeInfo type) {
			int offset = this.OffsetToSzArrayData;
			switch(type.VariableSize) {
			case 0:
				throw new ArgumentException();
			case 1:
			case 2:
			case 4:
			case 8:
				this.asm.Pop(Register32.ESI);	// index
				this.asm.Pop(Register32.EAX);	// array
				this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EAX,Register32.ESI,type.VariableSize,offset));
				this.asm.Push(Register32.EAX);
				break;
			default:
				this.asm.Pop(Register32.EAX);	// index
				this.asm.Pop(Register32.ECX);	// array
				this.asm.Mov(Register32.EDX, type.VariableSize);
				this.asm.Mul(Register32.EDX);
				this.asm.Lea(Register32.EAX, RegMem.Indirect(Register32.EAX,Register32.ECX,1,offset));
				this.asm.Push(Register32.EAX);
				break;
			}
		}

		#endregion

		#region Load Token

		public override void LoadEntryPoint(MethodInfo method) {
			//CodeInfo code = CooS.Execution.CodeManager.GetCallableCode(method);
			//this.LoadConstant(code.EntryPoint.ToInt32());
			this.CreateRelativeLabel(method);
		}

		#endregion

		#region Field Access Operations

		public override void LoadField(FieldInfo field) {
			if(field.IsStatic) {
				this.asm.Mov(Register32.EAX, 0x00EFBE00);
				CodeLabel label = this.CreateAbsoluteLabel(this.Engine.Realize(field.Type).Heap, field.Offset);
			} else {
				this.asm.Pop(Register32.EAX);
			}
			this.LoadMemory(this.Engine.Realize(field.ReturnType), Register32.EAX, field.Offset);
		}

		public override void StoreField(FieldInfo field) {
			TypeInfo type = this.Engine.Realize(field.ReturnType);
			if(field.IsStatic) {
				this.asm.Mov(Register32.EAX, 0x00EFBE00);
				//CodeLabel label = this.CreateAbsoluteLabel(field.GetStaticBuffer(), 0);
				this.StoreMemory(Register32.EAX, field.Offset, type.VariableSize);
			} else {
				this.asm.Mov(Register32.EAX, RegMem.Indirect(Register32.ESP, Architecture.GetStackingSize(type.VariableSize)));
				this.StoreMemory(Register32.EAX, field.Offset, type.VariableSize);
				this.asm.Add(Register32.ESP, IntPtr.Size);
			}
		}

		public override unsafe void LoadFieldAddress(FieldInfo field) {
			if(field.IsStatic) {
				throw new NotImplementedException();
				/*
				fixed(byte* p = field.GetStaticBuffer()) {
					this.asm.Push(new IntPtr(p+field.Offset).ToInt32());
				}
				*/
			} else {
				this.asm.Add(RegMem.Indirect(Register32.ESP), field.Offset);
			}
		}

		#endregion

		#region Heap Access Operations

		public override void ClearMemory() {
			this.Zero(Register32.EAX);
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EDI);
			this.asm.Rep_stos_m8();
		}

		public override void ClearMemory(int size) {
			switch(size) {
			case 0:
				throw new ArgumentException();
			case 4:
			case 8:
			case 12:
			case 16:
				this.Zero(Register32.ECX);
				this.asm.Pop(Register32.EAX);
				for(int i=0; i<size; i+=4) {
					this.asm.Mov(RegMem.Indirect(Register32.EAX, i), Register32.ECX);
				}
				break;
			default:
				this.asm.Push(size);
				this.ClearMemory();
				break;
			}
		}

		public override void InitializeMemory() {
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.EAX);
			this.asm.Pop(Register32.EDI);
			this.asm.Rep_stos_m8();
		}

		public override void CopyBlock() {
			this.asm.Cld();
			this.asm.Pop(Register32.ECX);
			this.asm.Pop(Register32.ESI);
			this.asm.Pop(Register32.EDI);
			this.asm.Rep_movs_m8_m8();
		}

		public override void CopyObject(TypeInfo type) {
			this.asm.Pop(Register32.EDX);
			this.StoreMemory(Register32.EDX, 0, type.VariableSize);
			this.asm.Push(Register32.EDX);
		}

		public override void LoadObject(TypeInfo type) {
			if(!type.IsValueType) throw new ArgumentException("Target type must be a kind of ValueType.", "type");
			this.asm.Pop(Register32.EAX);
			this.LoadMemory(type, Register32.EAX, type.OffsetToContents);
		}

		public override void StoreObject(TypeInfo storetype, TypeInfo realtype) {
			this.asm.Mov(Register32.EDX, RegMem.Indirect(Register32.ESP,realtype.VariableSize));
			this.StoreMemory(Register32.EDX, 0, storetype.VariableSize);
			this.Pop(4);
		}

		#endregion

		#region Conversion

		public override void Convert(TypeInfo from, TypeInfo to) {
			int fsz = from.VariableSize;
			int tsz = to.VariableSize;
			if((to.IsPrimitive && (to.Name=="Single" || to.Name=="Double"))
			|| (from.IsPrimitive && (from.Name=="Single" || from.Name=="Double")))
			{
				//TODO: Floating-point value conversion
				//throw new NotImplementedException("real numbers");
				Console.Error.WriteLine("Converting from/to floating point value is not supported.");
				this.Trap();
			} else {
				// Integer value conversion
				if(fsz==tsz) {
					return;
				} else if(fsz>tsz) {
					// Truncation
					if(fsz==1) throw new UnexpectedException("8bit integer can't be truncated");
					if(tsz==8) throw new UnexpectedException("64bit integer can't be the target where truncate");
					if(fsz==8) {
						this.asm.Pop(Register32.EAX);
						this.asm.Mov(RegMem.Indirect(Register32.ESP), Register32.EAX);
						fsz = 4;
						if(fsz<=tsz) return;
					}
					switch(tsz) {
					case 1:
						this.asm.And(RegMem.Indirect(Register32.ESP), 0xFF);
						break;
					case 2:
						this.asm.And(RegMem.Indirect(Register32.ESP), 0xFFFF);
						break;
					default:
						throw new UnexpectedException("Tryed to truncate to "+tsz+" byte from "+fsz+" byte");
					}
				} else {
					// Extention
					if(fsz==8) throw new UnexpectedException("64bit integer can't be extended");
					if(tsz==1) throw new UnexpectedException("8bit integer can't be the target where extend");
					bool signed;
					switch(from.VariableSize) {
					case 1:
						if(from.Name=="SByte") {
							this.asm.Movsx(Register32.EAX, (RegMem8)RegMem.Indirect(Register32.ESP));
							this.asm.Mov(RegMem.Indirect(Register32.ESP), Register32.EAX);
							signed = true;
						} else {
							this.asm.Movzx(Register32.EAX, (RegMem8)RegMem.Indirect(Register32.ESP));
							this.asm.Mov(RegMem.Indirect(Register32.ESP), Register32.EAX);
							signed = false;
						}
						break;
					case 2:
						if(from.Name=="Int16") {
							this.asm.Movsx(Register32.EAX, (RegMem16)RegMem.Indirect(Register32.ESP));
							this.asm.Mov(RegMem.Indirect(Register32.ESP), Register32.EAX);
							signed = true;
						} else {
							this.asm.Movzx(Register32.EAX, (RegMem16)RegMem.Indirect(Register32.ESP));
							this.asm.Mov(RegMem.Indirect(Register32.ESP), Register32.EAX);
							signed = false;
						}
						break;
					case 4:
						if(from.Name=="Int32") {
							signed = true;
						} else {
							signed = false;
						}
						break;
					default:
						throw new UnexpectedException("Extending conversion from "+from.Name);
					}
					switch(to.Name) {
					case "Int64":
						if(signed) {
							this.Zero(Register32.EAX);
							this.asm.Bt((RegMem32)RegMem.Indirect(Register32.ESP), 31);
							this.asm.Cmc();
							this.asm.Setc(Register8.AL);
							this.asm.Sub(Register32.EAX, 1);
							this.asm.Pop(Register32.ECX);
							this.asm.Push(Register32.EAX);
							this.asm.Push(Register32.ECX);
						} else {
							this.asm.Push((RegMem32)RegMem.Indirect(Register32.ESP));
							this.asm.Mov(RegMem.Indirect(Register32.ESP,4),0);
						}
						break;
					case "UInt64":
						this.asm.Pop(Register32.EAX);
						this.asm.Push(0);
						this.asm.Push(Register32.EAX);
						break;
					default:
						throw new UnexpectedException();
					}
				}
			}
		}

		#endregion

		#region Functionalization

		public override void Prologue() {
			this.asm.Enter((ushort)(this.TotalVariableSize+this.workingsize), 0);
		}

		public override void Epilogue() {
			this.asm.Int();
		}

		public override void Trap() {
			this.asm.Int();
		}

		protected override void WriteMethodStub(MethodInfo stubmethod) {
#if VMIMPL
			CodeInfo codeinfo = CooS.Execution.CodeManager.GetExecutableCode(stubmethod);
			this.asm.Push(Register32.ESP);
			this.LoadConstant(this.method.Assembly.Id);
			this.LoadConstant(this.method.Id);
			this.Call(stubmethod);
			this.asm.Pop(Register32.EAX);
			this.asm.Jmp(Register32.EAX);
#else
			throw new NotImplementedException();
#endif
		}

		#endregion

	}

}
