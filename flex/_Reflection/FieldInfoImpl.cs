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
			// TODO:  MethodInfoImpl.GetCustomAttributes ������ǉ����܂��B
			throw new NotImplementedException();
		}
	
		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			// TODO:  MethodInfoImpl.GetCustomAttributes ������ǉ����܂��B
			throw new NotImplementedException();
		}
	
		public override bool IsDefined(Type attributeType, bool inherit) {
			// TODO:  MethodInfoImpl.IsDefined ������ǉ����܂��B
			throw new NotImplementedException();
		}

		#region RuntimeFieldHandle�֌W

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
		/// ���̃t�B�[���h�Ɋ֘A�Â���ꂽRuntimeFieldHandle��ݒ肵�܂��B
		/// �������A���łɃ}�l�[�W�J�[�l���ɂ���ăn���h�����^�����Ă����ꍇ�ɂ́A
		/// �����̐ݒ�͏㏑�����ꂸ�A�߂�l�͂��łɐݒ肳��Ă����l�ɂȂ�܂��B
		/// �V�����ݒ肳�ꂽ�ꍇ�́A�߂�l��handle�Ɠ������Ȃ�܂��B
		/// </summary>
		/// <param name="handle">�V�����ݒ肳��悤�Ƃ���n���h���̒l</param>
		/// <returns>�V�����ݒ肳�ꂽ�ꍇ��handle�Ɠ���̒l�B
		/// ���łɃn���h�����֘A�Â����Ă����ꍇ�́A���łɐݒ肳��Ă����l�B</returns>
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
