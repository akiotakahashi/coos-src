// This file is automatically generated by fdev.exe.
// Don't edit this file manually.

using System;

namespace CooS.Execution {

public partial class TypeInfo {

	public CooS.Reflection.AssemblyBase Assembly {
		get { return this.Base.Assembly; }
	}

	public Int32 Id {
		get { return this.Base.Id; }
	}

	public String Name {
		get { return this.Base.Name; }
	}

	public String Namespace {
		get { return this.Base.Namespace; }
	}

	public Boolean ContainsGenericParameters {
		get { return this.Base.ContainsGenericParameters; }
	}

	public Boolean IsGenericParam {
		get { return this.Base.IsGenericParam; }
	}

	public Int32 GenericParamPosition {
		get { return this.Base.GenericParamPosition; }
	}

	public Boolean IsInterface {
		get { return this.Base.IsInterface; }
	}

	public Boolean IsSealed {
		get { return this.Base.IsSealed; }
	}

	public Boolean IsNested {
		get { return this.Base.IsNested; }
	}

	public Boolean IsPrimitive {
		get { return this.Base.IsPrimitive; }
	}

	public Boolean IsSigned {
		get { return this.Base.IsSigned; }
	}

	public Boolean IsArray {
		get { return this.Base.IsArray; }
	}

	public Boolean IsValueType {
		get { return this.Base.IsValueType; }
	}

	public Boolean IsEnum {
		get { return this.Base.IsEnum; }
	}

	public Boolean IsByRefPointer {
		get { return this.Base.IsByRefPointer; }
	}

	public Boolean IsByValPointer {
		get { return this.Base.IsByValPointer; }
	}

	public Boolean IsPointer {
		get { return this.Base.IsPointer; }
	}

	public String FullName {
		get { return this.Base.FullName; }
	}

	public CooS.Reflection.TypeBase BaseType {
		get { return this.Base.BaseType; }
	}

	public CooS.Reflection.TypeBase ElementType {
		get { return this.Base.ElementType; }
	}

	public CooS.Reflection.TypeBase EnclosingType {
		get { return this.Base.EnclosingType; }
	}

	public CooS.Reflection.IntrinsicTypes IntrinsicType {
		get { return this.Base.IntrinsicType; }
	}

	public Int32 IntrinsicSize {
		get { return this.Base.IntrinsicSize; }
	}

	public System.Int32 GetArrayRank()
	{ return this.Base.GetArrayRank(); }

	public System.Boolean IsSubclassOf(CooS.Reflection.TypeBase c)
	{ return this.Base.IsSubclassOf(c); }

	public System.Boolean IsAssignableFrom(CooS.Reflection.TypeBase c)
	{ return this.Base.IsAssignableFrom(c); }

	public CooS.Reflection.TypeBase MakeSzArrayType()
	{ return this.Base.GetSzArrayType(); }

	public CooS.Reflection.TypeBase MakeMnArrayType(System.Int32 rank)
	{ return this.Base.GetMnArrayType(rank); }

	public CooS.Reflection.TypeBase MakeByRefPointerType()
	{ return this.Base.GetByRefPointerType(); }

	public CooS.Reflection.TypeBase MakeByValPointerType()
	{ return this.Base.GetByValPointerType(); }

	public CooS.Reflection.TypeBase FindType(System.String name)
	{ return this.Base.FindType(name); }

	public CooS.Reflection.FieldBase FindField(System.String name)
	{ return this.Base.FindField(name); }

	public CooS.Reflection.MethodBase FindMethod(System.String name)
	{ return this.Base.FindMethod(name); }

	public CooS.Reflection.MethodBase[] FindMethods(System.String name)
	{ return this.Base.FindMethods(name); }

	}

}