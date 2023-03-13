using System;
using System.Reflection;
using System.Collections;

namespace CooS.Reflection {

	public class FilteredMemberCollection : IEnumerable {

		readonly IEnumerable c;
		string name = null;
		BindingFlags flags;
		MemberTypes types;

		public static FilteredMemberCollection Create(IEnumerable c, string name) {
			FilteredMemberCollection me = new FilteredMemberCollection(c);
			me.name = name;
			return me;
		}

		public static FilteredMemberCollection Create(IEnumerable c, BindingFlags flags) {
			FilteredMemberCollection me = new FilteredMemberCollection(c);
			me.flags = flags;
			return me;
		}

		public static FilteredMemberCollection Create(IEnumerable c, string name, BindingFlags flags) {
			FilteredMemberCollection me = new FilteredMemberCollection(c);
			me.name = name;
			me.flags = flags;
			return me;
		}

		public FilteredMemberCollection(IEnumerable c) {
			this.c = c;
			this.flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static;
			this.types = MemberTypes.All;
		}

		class FilteredMemberEnumerator : IEnumerator {

			readonly IEnumerator e;
			// Identifier
			readonly string name;
			// Accessibility
			readonly bool include_public;
			readonly bool include_nonpublic;
			// Instance/Static
			readonly bool include_instance;
			readonly bool include_static;
			// Type
			readonly MemberTypes select_types;
			// Position
			readonly bool select_declared;

			// Predicitons
			readonly bool check_accessibility;
			readonly bool check_allocation;

			public FilteredMemberEnumerator(FilteredMemberCollection parent) {
				this.e = parent.c.GetEnumerator();
				this.name = parent.name;
				if(parent.flags==BindingFlags.Default) {
					this.check_accessibility = true;
					this.include_public = true;
				} else {
					if(0!=(parent.flags&BindingFlags.IgnoreCase)) {
						throw new NotImplementedException();
					}
					//
					if(0!=(parent.flags&BindingFlags.Public)) {
						this.include_public = true;
					}
					if(0!=(parent.flags&BindingFlags.NonPublic)) {
						this.include_nonpublic = true;
					}
					if(0!=(parent.flags&BindingFlags.Instance)) {
						this.include_instance = true;
					}
					if(0!=(parent.flags&BindingFlags.Static)) {
						this.include_static = true;
					}
					//
					this.check_accessibility = !this.include_public || !this.include_nonpublic;
					this.check_allocation = !this.include_instance || !this.include_static;
					//
					this.select_types = parent.types;
					if(0!=(parent.flags&BindingFlags.DeclaredOnly)) {
						this.select_declared = true;
					}
					//
				}
			}

			#region IEnumerator ÉÅÉìÉo

			public void Reset() {
				this.e.Reset();
			}

			public object Current {
				get {
					return e.Current;
				}
			}

			public bool MoveNext() {
				while(e.MoveNext()) {
					MemberInfo member = (MemberInfo)e.Current;
					if(this.name!=null && this.name!=member.Name) continue;
					if(0==(this.select_types&member.MemberType)) continue;
					bool is_public;
					bool is_static;
					switch(member.MemberType) {
					case MemberTypes.Method:
						MethodInfo method = (MethodInfo)member;
						is_public = method.IsPublic;
						is_static = method.IsStatic;
						break;
					case MemberTypes.Field:
						FieldInfo field = (FieldInfo)member;
						is_public = field.IsPublic;
						is_static = field.IsStatic;
						break;
					default:
						throw new NotImplementedException("miss: "+member.MemberType);
					}
					if(this.check_accessibility) {
						if(!this.include_public && is_public) continue;
						if(!this.include_nonpublic && !is_public) continue;
					}
					if(this.check_allocation) {
						if(!this.include_static && is_static) continue;
						if(!this.include_nonpublic && !is_public) continue;
					}
					if(this.select_declared) {
						if(member.DeclaringType!=member.ReflectedType) continue;
					}
					return true;
				}
				return false;
			}

			#endregion

		}

		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}

		public BindingFlags BindingFlags {
			get {
				return this.flags;
			}
			set {
				this.flags = value;
			}
		}

		public MemberTypes TargetTypes {
			get {
				return this.types;
			}
			set {
				this.types = value;
			}
		}

		#region IEnumerable ÉÅÉìÉo

		public IEnumerator GetEnumerator() {
			return new FilteredMemberEnumerator(this);
		}

		#endregion

	}

}
