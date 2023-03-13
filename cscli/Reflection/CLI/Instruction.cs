using System;
using CooS.Formats.CLI;
using OpCode = System.Reflection.Emit.OpCode;
using RawInstruction = CooS.Formats.CLI.IL.Instruction;

namespace CooS.Reflection.CLI {

	public class Instruction {

		private readonly AssemblyImpl assembly;
		private readonly MethodImpl method;
		private readonly RawInstruction rawinst;

		internal Instruction(MethodImpl method, RawInstruction rawinst) {
			this.assembly = method._Assembly;
			this.method = method;
			this.rawinst = rawinst;
		}

		public override string ToString() {
			return this.rawinst.ToString();
		}

		public void Dump(System.IO.TextWriter writer) {
			this.rawinst.Dump(writer);
		}

		public TypeBase OpType {
			get {
				return this.assembly.Realize(this.rawinst.OpType);
			}
		}

		public OpCode OpCode { get { return this.rawinst.OpCode; } }
		public int Address { get { return this.rawinst.Address; } }
		public bool Unsigned { get { return this.rawinst.Unsigned; } }
		public bool Overflow { get { return this.rawinst.Overflow; } }
		public int BrTarget { get { return this.rawinst.BrTarget; } }
		public int[] BrTargets { get { return this.rawinst.BrTargets; } }
		public int Number { get { return this.rawinst.Number; } }
		public T GetNumeric<T>() { return this.rawinst.GetNumeric<T>(); }
		public int StringIndex { get { return this.rawinst.StringIndex; } }
		public string Extension { get { return this.rawinst.Extension; } }
		public string CoreName { get { return this.rawinst.CoreName; } }

		public TypeBase TypeInfo {
			get {
				TypeDeclInfo type = this.rawinst.TypeInfo;
				if(!type.IsGenericType) {
					return this.assembly.Realize(type);
				} else {
					throw new NotImplementedException();
				}
			}
		}

		public FieldBase FieldInfo {
			get {
				return (FieldBase)this.MemberInfo;
			}
		}

		public MethodBase MethodInfo {
			get {
				return (MethodBase)this.MemberInfo;
			}
		}

		public MemberBase MemberInfo {
			get {
				MemberDeclInfo member = this.rawinst.MemberInfo;
				if(!member.ContainsGenericParameters) {
					return this.assembly.Realize(member);
				} else {
					MemberBase mem = this.assembly.Realize(member);
					if(mem is FieldBase) {
						FieldBase f = (FieldBase)mem;
						return f;
					} else if(mem is MethodBase) {
						MethodBase m = (MethodBase)mem;
						return m.Specialize(this.method);
					} else {
						throw new UnexpectedException();
					}
				}
			}
		}

	}

}
