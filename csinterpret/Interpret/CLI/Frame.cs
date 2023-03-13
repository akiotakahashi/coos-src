using System;
using CooS.Reflection;
using CooS.Reflection.CLI;
using CooS.Formats.CLI.CodeModel;
using System.Collections.Generic;

namespace CooS.Interpret.CLI {

	public class Frame {

		public readonly Machine Machine;
		public readonly MethodImpl Method;
		public readonly int BP = 0;	// Stack Base Pointer
		public readonly List<Block> Variables = new List<Block>();
		public readonly List<Instruction> Code = new List<Instruction>();
		public int IC = 0;	// Instruction Counter

		public Frame(Machine machine, MethodImpl method) {
			this.Machine = machine;
			this.Method = method;
			this.BP = this.Machine.Stack.BasePointer;
			for(int i=0; i<method.VariableCount; ++i) {
				Block block;
				block.Type = machine.Engine.Realize(method.GetVariableType(i));
				block.Data = new byte[block.Type.VariableSize];
				this.Variables.Add(block);
			}
			foreach(Instruction inst in method.EnumInstructions()) {
				this.Code.Add(inst);
			}
		}

		public override string ToString() {
			return this.Method.FullName+Environment.NewLine+this.Current;
		}

		public Instruction Current {
			get {
				return this.Code[this.IC];
			}
		}

		public void Next() {
			++this.IC;
		}

		public void Jump(int target) {
			if(this.Current.Address<target) {
				// go toward
				if(target>this.Code[this.Code.Count-1].Address) {
					throw new InvalidProgramException("制御の宛先がコード範囲外です。");
				} else {
					int j = this.IC+1;
					while(target>this.Code[j].Address) {
						++j;
					}
					if(this.Code[j].Address!=target) {
						throw new InvalidProgramException("制御の宛先が命令境界ではありません。");
					} else {
						this.IC = j;
					}
				}
			} else if(this.Current.Address>target) {
				// go back
				if(target<0) {
					throw new InvalidProgramException("制御の宛先がコード範囲外です。");
				} else {
					int j = this.IC-1;
					while(target<this.Code[j].Address) {
						--j;
					}
					if(this.Code[j].Address!=target) {
						throw new InvalidProgramException("制御の宛先が命令境界ではありません。");
					} else {
						this.IC = j;
					}
				}
			} else {
				// repeat
			}
		}

		public int Throw(TypeBase typeBase) {
			throw new Exception("The method or operation is not implemented.");
		}

	}

}
