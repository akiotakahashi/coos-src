using System;
using System.IO;
using System.Reflection;
using System.Collections;
using CooS.Collections;
using CooS.CodeModels;
using CooS.CodeModels.CLI;
using CooS.CodeModels.CLI.Metadata;
using System.Runtime.InteropServices;

namespace CooS.Reflection {

	public abstract class MethodInfoImpl : MethodInfo, IBranchTarget {

		RuntimeMethodHandle _handle;

		public MethodInfoImpl() {
		}

		public override string ToString() {
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			buf.Append(this.FullName);
			buf.Append("(");
			foreach(ParameterInfo p in this.GetParameters()) {
				if(buf[buf.Length-1]!='(') buf.Append(", ");
				buf.Append(p.ParameterType.Name);
				buf.Append(" ");
				buf.Append(p.Name);
			}
			buf.Append(")");
			return buf.ToString();
		}

		public string FullName {
			get {
				return this.Assembly.GetTypeName(this.DeclaringTypeIndex)+":"+this.Name;
			}
		}

		public abstract AssemblyBase Assembly {get;}
		public abstract int DeclaringTypeIndex {get;}
		public abstract int ParameterCount {get;}
		public abstract TypeImpl GetParameterType(int index);

		public int ArgumentCount {
			get {
				if(this.IsStatic) {
					return this.ParameterCount;
				} else {
					return this.ParameterCount+1;
				}
			}
		}

		public Type GetArgumentType(int index) {
			if(!this.IsStatic) {
				if(index==0) {
					if(this.DeclaringType.IsValueType) {
						return ((TypeImpl)this.DeclaringType).GetByRefPointerType();
					} else {
						return this.DeclaringType;
					}
				}
				--index;
			}
			return this.GetParameterType(index);
		}

		public int GetArgumentSize(int index) {
			int size = ((TypeImpl)this.GetArgumentType(index)).VariableSize;
			return Architecture.GetStackingSize(size);
		}

		public abstract int VariableCount {get;}
		public abstract Type GetVariableType(int index);
		public int GetVariableSize(int index) {
			return ((TypeImpl)this.GetVariableType(index)).VariableSize;
		}

		public abstract MethodInfoImpl[] GetCallings();

		public MethodInfoImpl FindWrappingMethod() {
			string wrapclass = "CooS.Wrap._"+this.DeclaringType.Namespace+"._"+this.DeclaringType.Name;
			string name = this.IsConstructor ? "InternalAllocateInstance" : this.Name;
			MethodInfoImpl[] candidates = CooS.Management.AssemblyResolver.Manager.ResolveMethods(wrapclass+":"+name, false);
			if(candidates==null || candidates.Length==0) return null;
			foreach(MethodInfoImpl cand in candidates) {
				if(cand.ParameterCount!=this.ParameterCount) continue;
				if(this.IsConstructor && !cand.IsStatic) continue;
				if(!this.IsConstructor && cand.IsStatic!=this.IsStatic) continue;
				bool mismatch = false;
				for(int i=0; i<cand.ParameterCount; ++i) {
					//TODO
					if(cand.GetParameterType(i).FullName!=this.GetParameterType(i).FullName) {
						mismatch = true;
						break;
					}
				}
				if(!mismatch) {
					return cand;
				}
			}
			return null;
		}

		#region RuntimeMethodHandle関係

		static readonly Inttable handletable = new Inttable();
		static int handleseed = 1;

		private static unsafe RuntimeMethodHandle GenerateNewHandle(MethodInfoImpl method) {
			int handle;
			for(;;) {
				handle = handleseed++;
				if(!handletable.ContainsKey(handle)) {
					break;
				}
			}
			handletable[handle] = method;
			return *(RuntimeMethodHandle*)&handle;
		}

		public RuntimeMethodHandle Handle {
			get {
				if(this._handle.Value==IntPtr.Zero) {
					this._handle = GenerateNewHandle(this);
				}
				return this._handle;
			}
		}

		public override RuntimeMethodHandle MethodHandle {
			get {
				return this.Handle;
			}
		}

		#endregion

		public override object[] GetCustomAttributes(bool inherit) {
			// TODO:  MethodInfoImpl.GetCustomAttributes 実装を追加します。
			throw new NotImplementedException();
		}
	
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			// TODO:  MethodInfoImpl.GetCustomAttributes 実装を追加します。
			throw new NotImplementedException();
		}
	
		public override bool IsDefined(Type attributeType, bool inherit) {
			// TODO:  MethodInfoImpl.IsDefined 実装を追加します。
			throw new NotImplementedException();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture) {
			// TODO:  MethodInfoImpl.Invoke 実装を追加します。
			throw new NotImplementedException();
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes {
			get {
				// TODO:  MethodInfoImpl.ReturnTypeCustomAttributes getter 実装を追加します。
				return null;
			}
		}

		#region IBranchTarget メンバ

		public IntPtr Address {
			get {
				return CooS.Execution.CodeManager.GetCallableCode(this).EntryPoint;
			}
		}

		#endregion

		public bool IsDelegateInvoke {
			get {
				MethodImplAttributes flags = MethodImplAttributes.Runtime|MethodImplAttributes.ForwardRef;
				return this.GetMethodImplementationFlags()==flags
					&& this.Name=="Invoke"
					&& this.DeclaringType.IsSubclassOf(typeof(Delegate));
			}
		}

		public abstract int RowIndex {get;}
		public abstract CodeInfo GenerateCallableCode(CodeLevel level);
		public abstract CodeInfo GenerateExecutableCode(CodeLevel level);

		public abstract bool IsBlank {get;}
		public abstract void MakeBlank();

		public bool HasNewSlot {
			get {
				if(!this.IsVirtual) return false;
				return 0!=(this.Attributes&MethodAttributes.NewSlot)
					|| (0!=(this.GetMethodImplementationFlags()&MethodImplAttributes.Runtime)
					&& this.Name=="Invoke");
			}
		}

		public bool HasReuseSlot {
			get {
				if(!this.IsVirtual) return false;
				return !this.HasNewSlot;
			}
		}

		public abstract int SlotIndex {get;}

	}

}
