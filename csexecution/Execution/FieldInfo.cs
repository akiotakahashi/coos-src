using System;
using CooS.Reflection;
using System.Collections.Generic;

namespace CooS.Execution {

	public sealed partial class FieldInfo : MemberInfo {

		private readonly Engine engine;
		public readonly FieldBase Base;
		private int offset = -1;

		internal FieldInfo(Engine engine, FieldBase field) {
			this.engine = engine;
			this.Base = field;
		}

		public override System.Reflection.MemberTypes Kind {
			get {
				return System.Reflection.MemberTypes.Field;
			}
		}

		public int Offset {
			get {
				if(this.offset<0) {
					TypeInfo owner = this.engine.Realize(this.Type);
					if(this.offset<0) {
						if(this.IsStatic) {
							owner.LayoutStaticFields();
						} else {
							owner.LayoutInstanceFields();
						}
						if(this.offset<0) {
							throw new UnexpectedException();
						}
					}
					return this.offset;
				}
				return this.offset;
			}
			set {
				if(offset>=0) { throw new InvalidOperationException(); }
				this.offset = value;
			}
		}

		public int BufferIndex {
			get {
				throw new NotImplementedException();
			}
		}

		public int AssignHeap(int offset) {
			this.offset = offset;
			return this.engine.Realize(this.ReturnType).VariableSize;
		}

#if f
		public byte[] GetStaticBuffer() {
			throw new Exception("The method or operation is not implemented.");
		}
#endif

		private static readonly Dictionary<IntPtr, FieldInfo> handles = new Dictionary<IntPtr, FieldInfo>();
		private static uint seed = 1;
		private IntPtr handle;

		public IntPtr FieldHandle {
			get {
				if(handle==IntPtr.Zero) {
					handle = (IntPtr)seed++;
					handles[handle] = this;
				}
				return handle;
			}
		}

		public object GetValue(object target) {
			//return target.GetType().GetField(name, BindingFlags.NonPublic|BindingFlags.Instance).GetValue(target);
			throw new NotImplementedException();
		}

		public void SetValue(object target, object value) {
			//target.GetType().GetField(name, BindingFlags.NonPublic|BindingFlags.Instance).SetValue(target, value);
			throw new NotImplementedException();
		}

	}

}
