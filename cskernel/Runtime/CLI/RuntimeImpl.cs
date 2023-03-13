using System;
using System.IO;
using CooS.Reflection;
using CooS.Execution;
using System.Globalization;
using System.Collections.Generic;

namespace CooS.Runtime.CLI {

	class LoopException : Exception {
	}

	public static class RuntimeImpl {

		public static string LoadString(int aid, int sid) {
			throw new NotImplementedException();
		}

		public static IntPtr FindActualMethod(object entity, int slotIndex) {
			if(entity==null) throw new NullReferenceException();
			MethodBase method = ((TypeInfo)(object)entity.GetType()).GetSlotMethod(slotIndex);
			//Console.WriteLine("callvirt := "+method.FullName);
			//CodeInfo codeinfo = CooS.Execution.CodeManager.GetExecutableCode(method);
			//return codeinfo.EntryPoint;
			return IntPtr.Zero;
		}

		public static IntPtr FindActualMethodForInterface(object entity, int aid, int tid, int slotIndex) {
#if VMIMPL
			if(entity==null) throw nullex;
			TypeInfo type = entity.GetType();
			int baseidx = type.GetInterfaceBaseIndex(handle);
			MethodInfo method = type.GetSlotMethod(baseidx+slotIndex);
			//Console.WriteLine("callvirtif := "+method.FullName);
			CodeInfo codeinfo = CooS.Execution.CodeManager.GetExecutableCode(method);
			return codeinfo.EntryPoint;
#else
			throw new NotImplementedException();
#endif
		}

		static Exception loopex = new LoopException();
		static bool running_alloc = false;

		private static object AllocateObjectImpl(int aid, int tid) {
#if VMIMPL
			//if(running_alloc) throw loopex;
			running_alloc = true;
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeBase type = assembly.GetTypeById(tid);
			object ret = Memory.Allocate(type);
			running_alloc = false;
			return ret;
#else
			throw new NotImplementedException();
#endif
		}

		private static Delegate AllocateDelegate(object target, IntPtr ftn, int aid, int tid) {
#if VMIMPL
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeInfo type = assembly.GetTypeInfo(tid);
			Delegate obj = (Delegate)Memory.Allocate(type);
			Assist.ConstructDelegate(obj, target, ftn);
			return obj;
#else
			throw new NotImplementedException();
#endif
		}

		private static object AllocateSzArrayImpl(int length, int aid, int tid) {
#if VMIMPL
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeInfo type = assembly.GetTypeInfo(tid);
			return Memory.AllocateArray(type, length);
#else
			throw new NotImplementedException();
#endif
		}

		private static object AllocateSzArrayImplEx(int length, int handle) {
			throw new NotImplementedException();
		}

		private static object BoxImpl(int aid, int rid) {
#if VMIMPL
			AssemblyBase assembly = CooS.Management.AssemblyResolver.GetAssemblyInfo(aid);
			TypeInfo type = assembly.GetTypeInfo(rid);
			return Memory.Allocate(type);
#else
			throw new NotImplementedException();
#endif
		}

		private static object CastClassImpl(object obj, int aid, int rid) {
			//throw new NotImplementedException();
			return obj;	// always success
		}

	}

}
