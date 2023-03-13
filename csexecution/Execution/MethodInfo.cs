using System;
using CooS.Reflection;
using System.Collections.Generic;
using MethodImplAttributes=System.Reflection.MethodImplAttributes;

namespace CooS.Execution {

	public partial class MethodInfo : MemberInfo, IBranchTarget {

		public readonly MethodBase Base;
		public int SlotIndex;

		internal MethodInfo(MethodBase method) {
			this.Base = method;
		}

		public override string ToString() {
			return this.Base.ToString();
		}

		public override System.Reflection.MemberTypes Kind {
			get {
				return System.Reflection.MemberTypes.Method;
			}
		}

		public System.Reflection.MethodImplAttributes ImplFlags {
			get {
				return this.Base.ImplFlags;
			}
		}

		public bool IsDelegateInvoke {
			get {
				//MethodImplAttributes flags = MethodImplAttributes.Runtime|MethodImplAttributes.ForwardRef;
				return this.Base.IsBlank && this.Name=="Invoke" && this.Base.Type.IsSubclassOf(this.Assembly.World.Delegate);
			}
		}

		private static readonly Dictionary<IntPtr, MethodInfo> handles = new Dictionary<IntPtr, MethodInfo>();
		private static uint seed = 1;
		private IntPtr handle;

		public IntPtr MethodHandle {
			get {
				if(handle==IntPtr.Zero) {
					handle = (IntPtr)seed++;
					handles[handle] = this;
				}
				return handle;
			}
		}

		// Method Stubs

		public IEnumerable<TypeBase> EnumArguments() {
			return this.Base.EnumArguments();
		}

		public IEnumerable<object> EnumInstructions() {
			return this.Base.EnumInstructions();
		}

		#region IBranchTarget ÉÅÉìÉo

		public IntPtr Address {
			get {
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion

	}

}
