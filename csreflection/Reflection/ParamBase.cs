using System;
using System.Collections.Generic;
using System.Text;

namespace CooS.Reflection {

	public abstract class ParamBase {

		/*
		 * このクラスはパラメータの型に関する情報を提供しないように！
		 * なぜなら総称型の特殊化を行うメソッドにおいて、
		 * クラスをラップする必要が出てきてしまうから。
		 * c.f. SpecializedMethodBase:EnumParameterInfo
		 */

		public abstract MethodBase Method {
			get;
		}

		public abstract int Position {
			get;
		}

		public abstract string Name {
			get;
		}

		public string FullName {
			get {
				return this.Method.FullName+"+"+this.Name;
			}
		}

	}

}
