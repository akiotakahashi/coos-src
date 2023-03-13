using System;
using CooS.Reflection;

namespace CooS.Wrap._System.Runtime.CompilerServices {

	public class _RuntimeHelpers {

		public static int OffsetToStringData {
			get {
				return 4;
			}
		}

		static unsafe void InitializeArray(Array array, IntPtr handle) {
			if(Initializer.Finalized) {
				SzArrayData* arr = (SzArrayData*)Kernel.ObjectToValue(array).ToPointer();
				FieldInfoImpl field = FieldInfoImpl.FindFieldFromHandle(handle);
				if(field==null) throw new FieldNotFoundException(handle.ToInt32().ToString("X"));
				if(!field.IsStatic) throw new ArgumentException();
				fixed(byte* p = field.GetStaticBuffer()) {
					Tuning.Memory.Copy(&arr->first, p+field.GetFieldOffset(), arr->length*arr->elemsize);
				}
			} else {
				SzArrayData* arr = (SzArrayData*)Kernel.ObjectToValue(array).ToPointer();
				Tuning.Memory.Copy(&arr->first, handle.ToPointer(), arr->length*arr->elemsize);
			}
		}

	}

}
