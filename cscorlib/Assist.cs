using System;
using System.Reflection;
using System.Collections;

namespace CooS {
	using Reflection;
	using CodeModels;
	using Execution;
	using CodeModels.CLI;

	class Assist {

		#region コマンドライン

		public static unsafe string[] BuildCommandArguments(char* pcmdline, bool oldstyle) {
			if(pcmdline==null) return new string[0];
			string cmdline = new string(pcmdline).Trim();
			if(cmdline.Length==0) return new string[0];
			ArrayList args = new ArrayList();
			int s = 0;
			while(s<cmdline.Length) {
				int l = cmdline.IndexOf(' ',s);
				if(l<0) l=cmdline.Length;
				if(!oldstyle || s>0) {
					// オールドスタイルの一番目はプログラム名だからスキップ
					args.Add(cmdline.Substring(s,l-s));
				}
				s = l+1;
			}
			return (string[])args.ToArray(typeof(string));
		}
		
		#endregion

		#region デレゲート

		public static void ConstructDelegate(Delegate obj, object target, IntPtr ftn) {
			SetDelegateTarget(obj, target);
			SetFunctionPointer(obj, ftn);
		}
		
		static FieldInfoImpl pointer_field;
		static FieldInfoImpl target_field;

		public static FieldInfoImpl DelegatePointerField {
			get {
				if(pointer_field==null) {
					pointer_field = (FieldInfoImpl)typeof(Delegate).GetField("method_ptr",
						BindingFlags.NonPublic|BindingFlags.Instance);
					if(pointer_field==null) throw new FieldNotFoundException();
				}
				return pointer_field;
			}
		}

		public static FieldInfoImpl DelegateTargetField {
			get {
				if(target_field==null) {
					target_field = (FieldInfoImpl)typeof(Delegate).GetField("m_target",
						BindingFlags.NonPublic|BindingFlags.Instance);
					if(target_field==null) throw new FieldNotFoundException();
				}
				return target_field;
			}
		}

		public static IntPtr GetFunctionPointer(Delegate obj) {
            //return obj.Pointer;
			return (IntPtr)DelegatePointerField.GetValue(obj);
		}

		internal static void SetFunctionPointer(Delegate obj, IntPtr pfn) {
			//obj.Pointer = pfn;
			DelegatePointerField.SetValue(obj, pfn);
		}
		
		public static object GetDelegateTarget(Delegate obj) {
			return obj.Target;
		}
		
		internal static void SetDelegateTarget(Delegate obj, object target) {
			DelegateTargetField.SetValue(obj, target);
		}

		#endregion

		#region 例外

		public static void DumpException(Exception ex) {
			Console.Error.WriteLine("*******************************************************************");
			Console.Error.WriteLine("throw: {0}", ex.GetType().FullName);
			Console.Error.WriteLine("  msg: {0}", ex.Message);
		}

		#endregion

		#region フィールド

		public static object GetField(object target, string name) {
			return target.GetType().GetField(name, BindingFlags.NonPublic|BindingFlags.Instance).GetValue(target);
		}

		public static void SetField(object target, string name, object value) {
			target.GetType().GetField(name, BindingFlags.NonPublic|BindingFlags.Instance).SetValue(target, value);
		}

		#endregion

		#region メソッド

		public static RuntimeMethodHandle GetMethodHandle(AssemblyBase assembly, int rowIndex) {
			MethodInfoImpl method = assembly.GetMethodInfo(rowIndex);
			return method.MethodHandle;
		}

		#endregion

	}

}
