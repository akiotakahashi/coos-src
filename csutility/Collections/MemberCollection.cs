using System;
using System.Collections.Generic;
using System.Collections;

namespace CooS.Collections {

	public delegate T LoadMember<T,A>(A assembly, int rowIndex);

	public class MemberCollection<T,A> : IEnumerable<T> {

		readonly A assembly;
		readonly int rowIndex;
		readonly int count;
		readonly LoadMember<T,A> loader;

		public MemberCollection(A assembly, int rowIndex, int count, LoadMember<T, A> loader) {
			this.assembly = assembly;
			this.rowIndex = rowIndex;
			this.count = count;
			this.loader = loader;
		}

		public int Count {
			get {
				return this.count;
			}
		}

		public T this[int index] {
			get {
				return this.loader(this.assembly, index);
			}
		}

		#region IEnumerable ƒƒ“ƒo

		public IEnumerator<T> GetEnumerator() {
			for(int i=rowIndex; i<rowIndex+count; ++i) {
				yield return this.loader(this.assembly, i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			for(int i=rowIndex; i<rowIndex+count; ++i) {
				yield return this.loader(this.assembly, i);
			}
		}

		#endregion

	}

}
