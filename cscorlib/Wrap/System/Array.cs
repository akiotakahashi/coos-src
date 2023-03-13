using System;
using CooS.Reflection;

struct SzArrayData {
	public IntPtr elemtype;
	public int elemsize;
	public int length;
	public byte first;
}

namespace CooS.Wrap._System {

	class _Array {

		TypeImpl elemtype;
		int elemsize;
		int length;
		byte first;

		public static Array CreateInstance(Type elementType, int[] lengths) {
			if(elementType==null) throw new ArgumentNullException("elementType");
			if(lengths==null) throw new ArgumentNullException("lengths");
			if(lengths.Length>255) throw new TypeLoadException();
			return CreateInstanceImpl(elementType, lengths, null);
		}
 
		static Array CreateInstanceImpl(Type elementType, int[] lengths, int[] bounds) {
			if(elementType==null) throw new ArgumentNullException();
			if(lengths.Length!=1) throw new NotImplementedException();
			if(bounds!=null) throw new NotImplementedException();
			return Memory.AllocateArray((TypeImpl)elementType, lengths[0]);
		}

		static unsafe void ClearInternal(Array a, int index, int count) {
			SzArrayData* p = (SzArrayData*)Kernel.ObjectToValue(a).ToPointer();
			int size = p->elemsize;
			Tuning.Memory.Clear(&p->first+size*index, size*count);
		}

		static unsafe bool FastCopy(Array source, int source_idx, Array dest, int dest_idx, int length) {
			SzArrayData* src = (SzArrayData*)Kernel.ObjectToValue(source).ToPointer();
			SzArrayData* dst = (SzArrayData*)Kernel.ObjectToValue(dest).ToPointer();
			Tuning.Memory.Copy(
				&dst->first+dst->elemsize*dest_idx,
				&src->first+src->elemsize*source_idx,
				dst->elemsize*length);
			return true;
		}

		int GetRank() {
			return 1;
		}

		int GetLength(int dimension) {
			if(dimension<0) throw new IndexOutOfRangeException();
			if(dimension>0) return 0;
			return length;
		}

		int GetLowerBound(int dimension) {
			if(dimension<0) throw new IndexOutOfRangeException();
			if(dimension>0) return 0;
			return 0;
		}
		
		int GetUpperBound(int dimension) {
			if(dimension<0) throw new IndexOutOfRangeException();
			if(dimension>0) return 0;
			return length-1;
		}

		unsafe object GetValueImpl(int pos) {
			if(elemtype.IsValueType) {
				fixed(byte* p = &first) {
					return Memory.BoxValue(elemtype, new IntPtr(p), elemsize*pos);
				}
			} else {
				return ((object[])(object)this)[pos];
			}
		}

		unsafe void SetValueImpl(object value, int pos) {
			if(elemtype.IsValueType) {
				fixed(byte* p = &first) {
					if(value!=null) {
						Tuning.Memory.Copy(p+elemsize*pos,
							Kernel.ObjectToValue(value).ToPointer(),
							elemsize);
					} else {
						Tuning.Memory.Clear(p+elemsize*pos, elemsize);
					}
				}
			} else {
				((object[])(object)this)[pos] = value;
			}
		}

	}

}
