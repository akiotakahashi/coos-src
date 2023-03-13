//
// System.Collections.Codetable.cs
//
// Author:
//   Sergey Chaban (serge@wildwestsoftware.com)
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Runtime.Serialization;
using CooS.CodeModels;
using CooS.Reflection;

namespace CooS.Collections {

	public sealed class Codetable {

		internal struct Slot {
			internal object/*MethodInfoImpl*/ key;
			internal CodeInfo value;
			// Hashcode. Chains are also marked through this.
			internal int hashMix;
			internal MethodInfoImpl Key {
				get {
					return (MethodInfoImpl)key;
				}
			}
		}

		internal class KeyMarker {
			public readonly static KeyMarker Removed = new KeyMarker();
		}

		//
		// Private data
		//

		const int CHAIN_MARKER  = ~Int32.MaxValue;

		private int inUse;
		private int modificationCount;
		private int loadFactor100;
		private Slot [] table;
		private int threshold;
	
		private HashKeys hashKeys;
		private HashValues hashValues;

		private static readonly int [] primeTbl = {
													  11,
													  19,
													  37,
													  73,
													  109,
													  163,
													  251,
													  367,
													  557,
													  823,
													  1237,
													  1861,
													  2777,
													  4177,
													  6247,
													  9371,
													  14057,
													  21089,
													  31627,
													  47431,
													  71143,
													  106721,
													  160073,
													  240101,
													  360163,
													  540217,
													  810343,
													  1215497,
													  1823231,
													  2734867,
													  4102283,
													  6153409,
													  9230113,
													  13845163
												  };

		//
		// Constructors
		//

		private Codetable (int capacity, int loadFactor100) {
			if (capacity<0)
				throw new ArgumentOutOfRangeException ("capacity", "negative capacity");

			if (loadFactor100 < 10 || loadFactor100 > 100)
				throw new ArgumentOutOfRangeException ("loadFactor", "load factor");

			if (capacity == 0) ++capacity;
			this.loadFactor100 = 75*loadFactor100/100;
			int tableSize = capacity * 100 / this.loadFactor100;

			if (tableSize > Int32.MaxValue)
				throw new ArgumentException ("Size is too big");

			int size = (int) tableSize;
			size = ToPrime (size);
			this.SetTable (new Slot [size]);

			this.inUse = 0;
			this.modificationCount = 0;

		}

		public Codetable () : this (0, 100) {
		}

		public Codetable (int capacity) : this (capacity, 100) {
		}

		//
		// Properties
		//

		// ICollection

		public int Count {
			get {
				return inUse;
			}
		}

		// IDictionary

		public HashKeys Keys {
			get {
				if (this.hashKeys == null)
					this.hashKeys = new HashKeys (this);
				return this.hashKeys;
			}
		}

		public HashValues Values {
			get {
				if (this.hashValues == null)
					this.hashValues = new HashValues (this);
				return this.hashValues;
			}
		}

		public CodeInfo this [MethodInfoImpl key] {
			get {
				if (key == null)
					throw new ArgumentNullException ("key", "null key");
	
				Slot [] table = this.table;
				uint size = (uint) table.Length;
				int h = this.GetHash (key) & Int32.MaxValue;
				uint indx = (uint)h;
				uint step = (uint) ((h >> 5)+1) % (size-1)+1;
				
	
				for (uint i = size; i > 0; i--) {
					indx %= size;
					Slot entry = table [indx];
					MethodInfoImpl k = entry.Key;
					if (k == null)
						break;
					
					if (k == key || ((entry.hashMix & Int32.MaxValue) == h
						&& this.KeyEquals (key, k))) {
						return entry.value;
					}
	
					if ((entry.hashMix & CHAIN_MARKER) == 0)
						break;
	
					indx += step;
				}
				
				return null;
			}
			
			set {
				PutImpl (key, value, true);
			}
		}

		//
		// Interface methods
		//

		// ICollection
		public void CopyTo (Array array, int arrayIndex) {
			if (null == array)
				throw new ArgumentNullException ("array");

			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException ("arrayIndex");

			if (array.Rank > 1)
				throw new ArgumentException ("array is multidimensional");

			if ((array.Length > 0) && (arrayIndex >= array.Length))
				throw new ArgumentException ("arrayIndex is equal to or greater than array.Length");

			if (arrayIndex + this.inUse > array.Length)
				throw new ArgumentException ("Not enough room from arrayIndex to end of array for this Codetable");

			Enumerator it = GetEnumerator ();
			int i = arrayIndex;

			while (it.MoveNext ()) {
				array.SetValue (it.Entry, i++);
			}
		}


		// IDictionary

		public void Add (MethodInfoImpl key, CodeInfo value) {
			PutImpl (key, value, false);
		}

		public void Clear () {
			for (int i = 0;i<table.Length;i++) {
				table [i].key = null;
				table [i].value = null;
				table [i].hashMix = 0;
			}

			inUse = 0;
			modificationCount++;
		}

		public Enumerator GetEnumerator () {
			return new Enumerator (this, EnumeratorMode.ENTRY_MODE);
		}

		public void Remove (MethodInfoImpl key) {
			int i = Find (key);
			if (i >= 0) {
				Slot [] table = this.table;
				int h = table [i].hashMix;
				h &= CHAIN_MARKER;
				table [i].hashMix = h;
				table [i].key = (h != 0)
					? KeyMarker.Removed
					: null;
				table [i].value = null;
				--inUse;
				++modificationCount;
			}
		}

		public bool ContainsKey (MethodInfoImpl key) {
			return (Find (key) >= 0);
		}

		public bool ContainsValue (object value) {
			int size = this.table.Length;
			Slot [] table = this.table;
			if (value == null) {
				for (int i = 0; i < size; i++) {
					Slot entry = table [i];
					if (entry.key != null && entry.key!= KeyMarker.Removed
						&& entry.value == null) {
						return true;
					}
				}
			} else { 
				for (int i = 0; i < size; i++) {
					Slot entry = table [i];
					if (entry.key != null && entry.key!= KeyMarker.Removed
						&& value.Equals (entry.value)) {
						return true;
					}
				}
			}
			return false;
		}

		//
		// Protected instance methods
		//

		/// <summary>Returns the hash code for the specified key.</summary>
		private int GetHash (MethodInfoImpl key) {
			return key.Handle.Value.ToInt32();
		}

		/// <summary>
		///  Compares a specific CodeInfo with a specific key
		///  in the Codetable.
		/// </summary>
		private bool KeyEquals (object item, object key) {
			return item==key;
		}

		//
		// Private instance methods
		//

		private void AdjustThreshold () {
			int size = table.Length;

			threshold = (int) (size*loadFactor100/100);
			if (this.threshold >= size)
				threshold = size-1;
		}

		private void SetTable (Slot [] table) {
			if (table == null)
				throw new ArgumentNullException ("table");

			this.table = table;
			AdjustThreshold ();
		}

		private int Find (MethodInfoImpl key) {
			if (key == null)
				throw new ArgumentNullException ("key", "null key");

			Slot [] table = this.table;
			uint size = (uint) table.Length;
			int h = this.GetHash (key) & Int32.MaxValue;
			uint indx = (uint)h;
			uint step = (uint) ((h >> 5)+1) % (size-1)+1;
			

			for (uint i = size; i > 0; i--) {
				indx %= size;
				Slot entry = table [indx];
				MethodInfoImpl k = entry.Key;
				if (k == null)
					break;
				
				if (k == key || ((entry.hashMix & Int32.MaxValue) == h
					&& this.KeyEquals (key, k))) {
					return (int) indx;
				}

				if ((entry.hashMix & CHAIN_MARKER) == 0)
					break;

				indx += step;
			}
			return -1;
		}


		private void Rehash () {
			int oldSize = this.table.Length;

			// From the SDK docs:
			//   Codetable is automatically increased
			//   to the smallest prime number that is larger
			//   than twice the current number of Codetable buckets
			uint newSize = (uint)ToPrime ((oldSize<<1)|1);


			Slot [] newTable = new Slot [newSize];
			Slot [] table = this.table;

			for (int i = 0;i<oldSize;i++) {
				Slot s = table [i];
				if (s.key!= null) {
					int h = s.hashMix & Int32.MaxValue;
					uint spot = (uint)h;
					uint step = ((uint) (h>>5)+1)% (newSize-1)+1;
					for (uint j = spot%newSize;;spot+= step, j = spot%newSize) {
						// No check for KeyMarker.Removed here,
						// because the table is just allocated.
						if (newTable [j].key == null) {
							newTable [j].key = s.key;
							newTable [j].value = s.value;
							newTable [j].hashMix |= h;
							break;
						} else {
							newTable [j].hashMix |= CHAIN_MARKER;
						}
					}
				}
			}

			++this.modificationCount;

			this.SetTable (newTable);
		}


		private void PutImpl (MethodInfoImpl key, CodeInfo value, bool overwrite) {
			if (key == null)
				throw new ArgumentNullException ("key", "null key");

			uint size = (uint)this.table.Length;
			if (this.inUse >= this.threshold) {
				this.Rehash ();
				size = (uint)this.table.Length;
			}

			int h = this.GetHash (key) & Int32.MaxValue;
			uint spot = (uint)h;
			uint step = (uint) ((spot>>5)+1)% (size-1)+1;
			Slot [] table = this.table;
			Slot entry;

			int freeIndx = -1;
			for (int i = 0; i < size; i++) {
				int indx = (int) (spot % size);
				entry = table [indx];

				if (freeIndx == -1
					&& entry.key == KeyMarker.Removed
					&& (entry.hashMix & CHAIN_MARKER) != 0)
					freeIndx = indx;

				if (entry.key == null ||
					(entry.key == KeyMarker.Removed
					&& (entry.hashMix & CHAIN_MARKER) == 0)) {

					if (freeIndx == -1)
						freeIndx = indx;
					break;
				}

				if ((entry.hashMix & Int32.MaxValue) == h && KeyEquals (key, entry.key)) {
					if (overwrite) {
						table [indx].value = value;
						++this.modificationCount;
					} else {
						// Handle Add ():
						// An entry with the same key already exists in the Codetable.
						throw new ArgumentException (
							"Key duplication when adding: " + key);
					}
					return;
				}

				if (freeIndx == -1) {
					table [indx].hashMix |= CHAIN_MARKER;
				}

				spot+= step;

			}

			if (freeIndx!= -1) {
				table [freeIndx].key = key;
				table [freeIndx].value = value;
				table [freeIndx].hashMix |= h;

				++this.inUse;
				++this.modificationCount;
			}

		}

		private void  CopyToArray (Array arr, int i,
			EnumeratorMode mode) {
			Enumerator it = new Enumerator (this, mode);

			while (it.MoveNext ()) {
				arr.SetValue (it.Current, i++);
			}
		}



		//
		// Private static methods
		//
		private static bool TestPrime (int x) {
			if ((x & 1) != 0) {
				for (int n = 3; n< (int)Math.Sqrt (x); n += 2) {
					if ((x % n) == 0)
						return false;
				}
				return true;
			}
			// There is only one even prime - 2.
			return (x == 2);
		}

		private static int CalcPrime (int x) {
			for (int i = (x & (~1))-1; i< Int32.MaxValue; i += 2) {
				if (TestPrime (i)) return i;
			}
			return x;
		}

		private static int ToPrime (int x) {
			for (int i = 0; i < primeTbl.Length; i++) {
				if (x <= primeTbl [i])
					return primeTbl [i];
			}
			return CalcPrime (x);
		}




		//
		// Inner classes
		//

		public enum EnumeratorMode : int {
			KEY_MODE = 0,
			VALUE_MODE,
			ENTRY_MODE,
		};

		public sealed class Enumerator {

			private Codetable host;
			private int stamp;
			private int pos;
			private int size;
			private EnumeratorMode mode;

			private MethodInfoImpl currentKey;
			private CodeInfo currentValue;

			private readonly static string xstr = "Codetable.Enumerator: snapshot out of sync.";

			public Enumerator (Codetable host, EnumeratorMode mode) {
				this.host = host;
				stamp = host.modificationCount;
				size = host.table.Length;
				this.mode = mode;
				Reset ();
			}

			public Enumerator (Codetable host)
				: this (host, EnumeratorMode.KEY_MODE) {}


			private void FailFast () {
				if (host.modificationCount != stamp) {
					throw new InvalidOperationException (xstr);
				}
			}

			public void Reset () {
				FailFast ();

				pos = -1;
				currentKey = null;
				currentValue = null;
			}

			public bool MoveNext () {
				FailFast ();

				if (pos < size) {
					while (++pos < size) {
						Slot entry = host.table [pos];

						if (entry.key != null && entry.key != KeyMarker.Removed) {
							currentKey = entry.Key;
							currentValue = entry.value;
							return true;
						}
					}
				}

				currentKey = null;
				currentValue = null;
				return false;
			}

			public DictionaryEntry Entry {
				get {
					if (currentKey == null) throw new InvalidOperationException ();
					FailFast ();
					return new DictionaryEntry (currentKey, currentValue);
				}
			}

			public MethodInfoImpl Key {
				get {
					if (currentKey == null) throw new InvalidOperationException ();
					FailFast ();
					return currentKey;
				}
			}

			public CodeInfo Value {
				get {
					if (currentKey == null) throw new InvalidOperationException ();
					FailFast ();
					return currentValue;
				}
			}

			public object Current {
				get {
					if (currentKey == null) throw new InvalidOperationException ();
						
					switch (mode) {
					case EnumeratorMode.KEY_MODE:
						return currentKey;
					case EnumeratorMode.VALUE_MODE:
						return currentValue;
					case EnumeratorMode.ENTRY_MODE:
						return new DictionaryEntry (currentKey, currentValue);
					}
					throw new Exception ("should never happen");
				}
			}
		}

		public sealed class HashKeys {

			private Codetable host;

			public HashKeys (Codetable host) {
				if (host == null)
					throw new ArgumentNullException ();

				this.host = host;
			}

			// ICollection

			public int Count {
				get {
					return host.Count;
				}
			}

			public void CopyTo (Array array, int arrayIndex) {
				if (array == null)
					throw new ArgumentNullException ("array");
				if (array.Rank != 1)
					throw new ArgumentException ("array");
				if (arrayIndex < 0) 
					throw new ArgumentOutOfRangeException ("arrayIndex");
				if (array.Length - arrayIndex < Count)
					throw new ArgumentException ("not enough space");
				
				host.CopyToArray (array, arrayIndex, EnumeratorMode.KEY_MODE);
			}

			// IEnumerable

			public Enumerator GetEnumerator () {
				return new Codetable.Enumerator (host, EnumeratorMode.KEY_MODE);
			}

		}


		public sealed class HashValues {

			private Codetable host;

			public HashValues (Codetable host) {
				if (host == null)
					throw new ArgumentNullException ();

				this.host = host;
			}

			// ICollection

			public int Count {
				get {
					return host.Count;
				}
			}

			public void CopyTo (Array array, int arrayIndex) {
				if (array == null)
					throw new ArgumentNullException ("array");
				if (array.Rank != 1)
					throw new ArgumentException ("array");
				if (arrayIndex < 0) 
					throw new ArgumentOutOfRangeException ("arrayIndex");
				if (array.Length - arrayIndex < Count)
					throw new ArgumentException ("not enough space");
				
				host.CopyToArray (array, arrayIndex, EnumeratorMode.VALUE_MODE);
			}

			// IEnumerable

			public Enumerator GetEnumerator () {
				return new Codetable.Enumerator (host, EnumeratorMode.VALUE_MODE);
			}

		}

	} // Codetable

}
