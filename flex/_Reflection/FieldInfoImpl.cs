using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace CooS.Reflection {
	using Inttable=Dictionary<int, object>;

	public abstract class FieldInfoImpl : FieldInfo {

		RuntimeFieldHandle _handle;

		public FieldInfoImpl() {
			this._handle = new RuntimeFieldHandle();
		}

		public string FullName {
			get {
				return this.DeclaringType.FullName+":"+this.Name;
			}
		}

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

		#region RuntimeFieldHandle関係

		static readonly Inttable handletable = new Inttable();

		private static unsafe RuntimeFieldHandle GenerateNewHandle(FieldInfoImpl field) {
			int handle;
			fixed(byte* _p = field.GetStaticBuffer()) {
				byte* p = _p;
				for(;;) {
					handle = (int)p;
					if(!handletable.ContainsKey(handle)) {
						break;
					}
					++p;
				}
				Console.WriteLine("Field [{0:X8}] {1}", handle, field.FullName);
				handletable[handle] = field;
			}
			return *(RuntimeFieldHandle*)&handle;
		}

		public RuntimeFieldHandle Handle {
			get {
				if(this._handle.Value==IntPtr.Zero) {
					this._handle = GenerateNewHandle(this);
				}
				return this._handle;
			}
		}

		public override sealed RuntimeFieldHandle FieldHandle {
			get {
				return this.Handle;
			}
		}

		/// <summary>
		/// このフィールドに関連づけられたRuntimeFieldHandleを設定します。
		/// ただし、すでにマネージカーネルによってハンドルが与えられていた場合には、
		/// 既存の設定は上書きされず、戻り値はすでに設定されていた値になります。
		/// 新しく設定された場合は、戻り値がhandleと等しくなります。
		/// </summary>
		/// <param name="handle">新しく設定されようとするハンドルの値</param>
		/// <returns>新しく設定された場合はhandleと同一の値。
		/// すでにハンドルが関連づけられていた場合は、すでに設定されていた値。</returns>
		internal RuntimeFieldHandle AssociateFieldHandle(RuntimeFieldHandle handle) {
			if(this._handle.Value==IntPtr.Zero) {
				this._handle = handle;
				//Console.WriteLine("Field [{0:X8}] {1}", handle.Value.ToInt32(), this.FullName);
			} else {
				//Console.WriteLine("Field [{0:X8}] {1} as 2nd", this.FieldHandle.Value.ToInt32(), this.FullName);
			}
			handletable[handle.Value.ToInt32()] = this;
			return this._handle;
		}

		public static FieldInfoImpl FindFieldFromHandle(RuntimeFieldHandle handle) {
			return (FieldInfoImpl)handletable[handle.Value.ToInt32()];
		}

		public static FieldInfoImpl FindFieldFromHandle(IntPtr handle) {
			return (FieldInfoImpl)handletable[handle.ToInt32()];
		}

		#endregion

		public abstract int RowIndex {get;}
		public abstract int GetFieldOffset();
		public abstract byte[] GetStaticBuffer();

	}

}
