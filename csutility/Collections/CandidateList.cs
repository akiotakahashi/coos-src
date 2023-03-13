using System;
using System.Collections.Generic;

namespace CooS.Collections {
	
	public class CandidateList<Value> where Value : class {

		private Value cand = default(Value);
		private List<Value> cands = null;

		public CandidateList() {
		}

		public void Add(Value value) {
			if(cand==default(Value)) {
				cand = value;
			} else {
				if(cands==null) {
					cands = new List<Value>(2);
					cands.Add(value);
				}
				cands.Add(value);
			}
		}

		public bool Unique {
			get {
				return cands==null;
			}
		}

		public bool Ambiguous {
			get {
				return cands!=null;
			}
		}

		public Value First {
			get {
				return this.cand;
			}
		}

		public Value[] ToArray() {
			if(cands==null) {
				return new Value[] { this.cand };
			} else {
				return this.cands.ToArray();
			}
		}

	}

}
