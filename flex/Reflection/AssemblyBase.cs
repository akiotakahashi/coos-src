using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace CooS.Reflection {
	
	public abstract class AssemblyBase : IDisposable /*: Assembly*/ {

		public abstract string Name {
			get;
		}

		public abstract CultureInfo Culture {
			get;
		}

		public abstract Version Version {
			get;
		}

		private bool disposed = false;

		~AssemblyBase() {
			if(!disposed) {
				Dispose();
			}
		}

		protected abstract void Dispose();

		#region IDisposable ÉÅÉìÉo

		void IDisposable.Dispose() {
			if(!disposed) {
				disposed = true;
				this.Dispose();
			}
		}

		#endregion

		public abstract IEnumerable<TypeBase> EnumType();
		public abstract TypeBase FindType(string name, string ns);

	}

}
