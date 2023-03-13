using System;
using System.Collections.Generic;
using CooS.Reflection;
using CooS.Execution;

namespace CooS.Compile.CLI {

	class EvaluationStack : ICloneable {

		List<TypeBase> stack;

		public EvaluationStack() {
			stack = new List<TypeBase>();
		}

		private EvaluationStack(EvaluationStack es) {
			stack = new List<TypeBase>(es.stack);
		}

		public override bool Equals(object obj) {
			EvaluationStack stack = (EvaluationStack)obj;
			if(this.Length!=stack.Length) return false;
			for(int i=0; i<this.Length; ++i) {
				if(this[i]!=stack[i]) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			return this.Length.GetHashCode();
		}

		public bool IsCompatibleWith(EvaluationStack stack) {
			if(this.Length!=stack.Length) return false;
			for(int i=0; i<this.Length; ++i) {
				if(this[i]!=stack[i]) {
					goto next;
				}
			}
			return true;
		next:
			// (1)	Expected: Object
			//		Actual	: String
			// (2)	Expected: Int32
			//		Actual	: Boolean
			for(int i=0; i<this.Length; ++i) {
				if(!this[i].IsAssignableFrom(stack[i])) {
					if(this[i].IsPrimitive && stack[i].IsPrimitive) {
						if(Synthesizer.GetIntrinsicType(this[i])==Synthesizer.GetIntrinsicType(stack[i])) {
							continue;
						}
					}
					return false;
				}
			}
			return true;
		}

		public int Length {
			get {
				return this.stack.Count;
			}
		}
		
		public TypeBase Top {
			get {
				return this.stack[this.stack.Count-1];
			}
		}

		public TypeBase this[int index] {
			get {
				return this.stack[this.stack.Count-1-index];
			}
		}

		#region ICloneable ƒƒ“ƒo

		public object Clone() {
			return new EvaluationStack(this);
		}

		#endregion

		public void Push(TypeBase type) {
			this.stack.Add(type);
		}

		public void Pop(int count) {
			this.stack.RemoveRange(this.stack.Count-count, count);
		}

		public TypeBase Pop() {
			TypeBase type = this.Top;
			this.Pop(1);
			return type;
		}

		public void Change(int count, TypeBase type) {
			this.Pop(count);
			if(type.FullName=="System.Void") throw new Exception();
			this.Push(type);
		}

		public void Change(int count, int index) {
			TypeBase type = this[index];
			this.Pop(count);
			this.Push(type);
		}

		public void LoadArg(int index) {
			this.Push(this.stack[index]);
		}

		public void StoreArg(int index) {
			//TODO: I can't decide to set the type of target argument to the type of stack-top.
			this.Pop();
		}

	}

}
