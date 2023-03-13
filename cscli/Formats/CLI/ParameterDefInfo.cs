using System;

namespace CooS.Formats.CLI {
	using Metadata;
	using Signature;
	using Metadata.Rows;

	sealed class ParameterDefInfo : ParameterDeclInfo {

		private AssemblyDefInfo assembly;
		//private ParamSig signature;
		private ParamRow row;

		internal ParameterDefInfo(AssemblyDefInfo assembly, ParamRow row) {
			this.assembly = assembly;
			this.row = row;
		}

		/// <summary>
		/// 4. Sequence shall have a value >= 0 and &lt;= number of parameters in owner method. A Sequence
		/// value of 0 refers to the owner methodÅfs return type; its parameters are then numbered from 1
		/// onwards [ERROR]
		/// </summary>
		public int Sequence {
			get {
				return row.Sequence;
			}
		}

		public override string Name {
			get {
				return this.assembly.LoadBlobString(this.row.Name);
			}
		}

		/// <summary>
		/// 6. If Flags.HasDefault = 1 then this row shall own exactly one row in the Constant table [ERROR]
		/// 7. If Flags.HasDefault = 0, then there shall be no rows in the Constant table owned by this row [ERROR]
		/// 8. parameters cannot be given default values, so Flags.HasDefault shall be 0 [CLS]
		/// 9. if Flags.FieldMarshal = 1 then this row shall own exactly one row in the FieldMarshal table [ERROR]
		/// </summary>
		public ParamAttributes Flags {
			get {
				return this.row.Flags;
			}
		}

		public override MethodDefInfo Method {
			get {
				return this.assembly.SearchMethodOfParam(row.Index);
			}
		}

	}

}
