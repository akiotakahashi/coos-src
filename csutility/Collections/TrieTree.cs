using System;
using System.Collections.Generic;

namespace CooS.Collections {

	public class TrieTree<Value> {

		private Node root;
		private int count = 0;

		public TrieTree() {
			this.root = new Node(string.Empty);
		}

		class Node {

			public string Key;
			public bool Empty;
			public Value value;
			private Dictionary<char,Node> Branches;

			public Node(string key) {
				this.Key = key;
				this.Empty = true;
			}

			public Node(string key, Value value) {
				this.Key = key;
				this.Empty = false;
				this.Value = value;
			}

			public bool IsBlank {
				get {
					return this.Empty;
				}
			}

			public bool HasChildren {
				get {
					return this.Branches!=null && this.Branches.Count>0;
				}
			}

			public void Dump(int depth) {
				Console.WriteLine("{0,"+depth*3+"}{1} = {2}", string.Empty
					, this.Key.Length==0 ? "<root>" : this.Key
					, this.IsBlank ? "<blank>" : (this.Value==null ? "<null>" : this.Value.ToString()));
				if(this.Branches!=null) {
					foreach(Node node in this.Branches.Values) {
						node.Dump(depth+1);
					}
				}
			}

			public Value Value {
				get {
					if(this.Empty) { throw new InvalidOperationException(); }
					return this.value;
				}
				set {
					this.value = value;
					this.Empty = false;
				}
			}

			public void Clear() {
				this.Empty = true;
				this.Value = default(Value);
			}

			public Node Search(string key, ref int start) {
				if(this.Branches==null) {
					return this;
				} else if(!this.Branches.ContainsKey(key[start])) {
					return this;
				} else {
					Node cand = this.Branches[key[start]];
					int keylen = key.Length-start;
					int minlen = Math.Min(keylen, cand.Key.Length);
					int cmp = string.CompareOrdinal(key, start, cand.Key, 0, minlen);
					if(cmp==0) {
						// Ç«ÇøÇÁÇ©Ç…äÆëSàÍív
						if(keylen>cand.Key.Length) {
							// êVãKÉmÅ[ÉhÇÃï˚Ç™ê[Ç¢
							start += cand.Key.Length;
							return cand.Search(key, ref start);
						} else if(keylen<cand.Key.Length) {
							// ä˘ë∂ÉmÅ[ÉhÇÃï˚Ç™ê[Ç¢
							return this;
						} else {
							// è’ìÀ
							start += minlen;
							return cand;
						}
					} else {
						// ìríÜÇ‹Ç≈àÍív
						return this;
					}
				}
			}

			private void AddNode(Node node) {
				if(this.Branches==null) {
					this.Branches = new Dictionary<char,Node>();
				}
				this.Branches[node.Key[0]] = node;
			}
			
			public Node AddBranch(string key, int start) {
				if(this.Branches==null) {
					this.Branches = new Dictionary<char,Node>();
				}
				if(!this.Branches.ContainsKey(key[start])) {
					Node node = new Node(key.Substring(start));
					this.Branches[key[start]] = node;
					return node;
				} else {
					Node cand = this.Branches[key[start]];
					int keylen = key.Length-start;
					int minlen = Math.Min(keylen, cand.Key.Length);
					int i;
					for(i=0; i<minlen; ++i) {
						if(key[i+start]!=cand.Key[i]) break;
					}
					if(i==minlen) {
						if(keylen<cand.Key.Length) {
							cand.Key = cand.Key.Substring(keylen);
							Node node = new Node(key.Substring(start));
							node.AddNode(cand);
							this.Branches[key[start]] = node;
							return node;
						} else if(keylen>cand.Key.Length) {
							// ëŒè€ÇÕÇ±ÇÃÉmÅ[ÉhÇ≈ÇÕÇ»Ç¢
							throw new ArgumentException();
						} else {
							// ëŒè€ÇÕÇ±ÇÃÉmÅ[ÉhÇ≈ÇÕÇ»Ç¢
							throw new ArgumentException();
						}
					} else {
						cand.Key = cand.Key.Substring(i);
						Node node = new Node(key.Substring(start+i));
						Node mid = new Node(key.Substring(start,i));
						mid.AddNode(cand);
						mid.AddNode(node);
						this.Branches[key[start]] = mid;
						return node;
					}
				}
			}

			public void MargeWithChildrenIfBlank() {
				if(!this.IsBlank) return;
				if(this.Key.Length==0) return;
				if(this.Branches.Count==0) {
					throw new Exception();
				}
				if(this.Branches.Count==1) {
					IEnumerator<Node> e = this.Branches.Values.GetEnumerator();
					e.MoveNext();
					Node child = e.Current;
					this.Key += child.Key;
					this.Value = child.Value;
					this.Branches.Clear();
				}
			}

			public void RemoveBranch(string key, int start) {
				if(!this.Branches.ContainsKey(key[start])) {
					throw new ArgumentException();
				} else {
					Node node = this.Branches[key[start]];
					if(node.HasChildren) {
						node.Clear();
						node.MargeWithChildrenIfBlank();
					} else {
						this.Branches.Remove(key[start]);
						this.MargeWithChildrenIfBlank();
					}
				}
			}

		}

		public void Dump() {
			this.root.Dump(0);
		}

		public int Count {
			get {
				return this.count;
			}
		}

		public void Add(string key, Value value) {
			int pos = 0;
			Node target = root.Search(key, ref pos);
			if(pos==key.Length) {
				if(!target.IsBlank) { throw new ArgumentException("Already exists: "+key); }
				target.Value = value;
			} else {
				target.AddBranch(key,pos).Value = value;
				++count;
			}
		}

		public void Remove(string key) {
			int pos = 0;
			Node target = root.Search(key.Substring(0, key.Length-1), ref pos);
			target.RemoveBranch(key, pos);
			--count;
		}

		public bool TryFind(string key, out Value value) {
			int pos = 0;
			Node target = root.Search(key, ref pos);
			if(pos<key.Length) { value = default(Value); return false; }
			if(target.IsBlank) { value = default(Value); return false; }
			value = target.Value;
			return true;
		}

		public Value Find(string key) {
			int pos = 0;
			Node target = root.Search(key, ref pos);
			if(pos<key.Length)
				throw new KeyNotFoundException(key);
			if(target.IsBlank)
				throw new KeyNotFoundException(key);
			return target.Value;
		}

		public bool ContainsKey(string key) {
			int pos = 0;
			Node target = root.Search(key, ref pos);
			if(pos<key.Length) return false;
			if(target.IsBlank) return false;
			return true;
		}

		public Value this[string key] {
			get {
				return this.Find(key);
			}
			set {
				int pos = 0;
				Node target = root.Search(key, ref pos);
				if(pos==key.Length) {
					target.Value = value;
				} else {
					target.AddBranch(key,pos).Value = value;
					++count;
				}
			}
		}

	}

}
