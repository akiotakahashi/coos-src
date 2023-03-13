using System;
public interface IMyIfc {}
public interface IDerived : IMyIfc {}
public class Class1 : IMyIfc {}
public class MyDerivedClass : Class1 {}
class IsSubclassTest
{
    public static void Main()
    {
        Type imyifcType = typeof(IMyIfc);
        Type imyderivedType = typeof(IDerived);
        Class1 mc = new Class1();
        Type mcType = mc.GetType();
        Class1 mdc = new MyDerivedClass();
        Type mdcType = mdc.GetType();
        int [] array  = new int [10];
        Type arrayOfIntsType = array.GetType();
        Type arrayType = typeof(Array);

        Console.WriteLine("Is Array a derived class of int[]? {0}", arrayType.IsSubclassOf(arrayOfIntsType));
        Console.WriteLine("Is int [] a derived class of Array? {0}", arrayOfIntsType.IsSubclassOf(arrayType));
        Console.WriteLine("Is IDerived a derived class of IMyIfc? {0}", imyderivedType.IsSubclassOf(imyifcType));
        Console.WriteLine("Is IMyIfc a derived class of IDerived? {0}", imyifcType.IsSubclassOf(imyderivedType));
        Console.WriteLine("Is myclass a derived class of Class1? {0}", mcType.IsSubclassOf(mcType));
        Console.WriteLine("Is myclass a derived class of IMyIfc? {0}", mcType.IsSubclassOf(imyifcType));
        Console.WriteLine("Is myderivedclass a derived class of Class1? {0}", mdcType.IsSubclassOf(mcType));

        foreach(Type i in imyderivedType.GetInterfaces()) {
        	Console.WriteLine(i.FullName);
        }

        Console.WriteLine("{0}", imyifcType.IsAssignableFrom(imyderivedType));
        Console.WriteLine("{0}", imyderivedType.BaseType);
    }
}
