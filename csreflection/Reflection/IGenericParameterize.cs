using System;
using CooS.Formats;

namespace CooS.Reflection {

	public interface IGenericParameterize {

		bool ContainsGenericParameters { get; }
		bool HasGenericParameters { get; }
		int GenericParameterCount { get; }

		TypeBase GetGenericArgumentType(int index);
		TypeBase ResolveGenericParameterType(TypeBase type);

	}

}
