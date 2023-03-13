using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace CooS.Reflection {
	
	public abstract class AssemblyBase : IDisposable /*: Assembly*/ {

		private static int idseed = 1;
		private readonly int _id = -1;

		protected AssemblyBase() {
			this._id = ++idseed;
		}

		public int Id {
			get {
				return _id;
			}
		}

		public abstract World World {
			get;
		}

		public abstract string Name {
			get;
		}

		public abstract Version Version {
			get;
		}

		public abstract CultureInfo Culture {
			get;
		}

		private bool disposed = false;

		~AssemblyBase() {
			if(!disposed) {
				Dispose();
			}
		}

		public abstract void Dispose();

		#region IDisposable ÉÅÉìÉo

		void IDisposable.Dispose() {
			if(!disposed) {
				disposed = true;
				this.Dispose();
			}
		}

		#endregion

		public abstract MethodBase EntryPoint {
			get;
		}

		public abstract TypeBase GetTypeById(int id);
		public abstract IEnumerable<TypeBase> EnumTypes();
		public abstract TypeBase FindType(string name);
		public abstract TypeBase FindType(string name, string ns);

	}

}
