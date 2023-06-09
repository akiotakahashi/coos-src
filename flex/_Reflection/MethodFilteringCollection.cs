using System;
using System.Collections;

namespace CooS.Reflection {

	public class MethodFilteringCollection : IEnumerable {

		readonly IEnumerable c;
		readonly bool for_constructor;

		public MethodFilteringCollection(IEnumerable c, bool for_constructor) {
			this.c = c;
			this.for_constructor = for_constructor;
		}

		class FilteringEnumerator : IEnumerator {

			readonly MethodFilteringCollection parent;
			readonly IEnumerator e;

			public FilteringEnumerator(MethodFilteringCollection parent) {
				this.parent = parent;
				this.e = parent.c.GetEnumerator();
			}

			#region IEnumerator メンバ

			public void Reset() {
				this.e.Reset();
			}

			public object Current {
				get {
					return this.e.Current;
				}
			}

			public bool MoveNext() {
				while(this.e.MoveNext()) {
					object method = (MethodInfoImpl)this.e.Current;
					if(this.parent.for_constructor) {
						if(!method.IsConstructor) continue;
						return true;
					} else {
						if(method.IsConstructor) continue;
						return true;
					}
				}
				return false;
			}

			#endregion

		}

		#region IEnumerable メンバ

		public IEnumerator GetEnumerator() {
			return new FilteringEnumerator(this);
		}

		#endregion

	}

}
