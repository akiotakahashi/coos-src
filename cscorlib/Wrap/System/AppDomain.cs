using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;

namespace CooS.Wrap._System {

	public class _AppDomain {

		static AppDomain current;
	
		internal static Context InternalGetDefaultContext() {
			return null;
		}

		internal static Context InternalGetContext() {
			return null;
		}

		internal static string InternalGetProcessGuid(string newguid) {
			return newguid;
		}

		public static AppDomain CurrentDomain {
			get {
				if(current==null) {
					ConstructorInfo ci = typeof(AppDomain).GetConstructor(
						BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance
						, null, Type.EmptyTypes, null);
					current = (AppDomain)ci.Invoke(null);
				}
				return current;
			}
		}

		private static AppDomain createDomain(string friendlyName, AppDomainSetup info) {
			return new AppDomainImpl(friendlyName, info);
		}
 
		internal static object InvokeInDomain(AppDomain domain, MethodInfo method, object obj, object[] args) {
			AppDomain cd = AppDomain.CurrentDomain;
			//if(domain==null) throw new ArgumentNullException();
			//if(cd!=domain) throw new NotSupportedException();
			Console.WriteLine("InvokeInDomain> {0}", method.Name);
			return method.Invoke(obj, args);
		}

	}

}
