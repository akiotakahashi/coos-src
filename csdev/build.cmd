@echo off
set FDEV=bin\Debug\fdev.exe

%FDEV% createproxy D:\Repository\clios\fref\bin\Debug\fref.exe CooS.Reflection.TypeBase CooS.Execution.TypeInfo > ..\fves\VES\TypeInfoProxy.cs
%FDEV% createproxy D:\Repository\clios\fref\bin\Debug\fref.exe CooS.Reflection.FieldBase CooS.Execution.FieldInfo > ..\fves\VES\FieldInfoProxy.cs
%FDEV% createproxy D:\Repository\clios\fref\bin\Debug\fref.exe CooS.Reflection.MethodBase CooS.Execution.MethodInfo > ..\fves\VES\MethodInfoProxy.cs

%FDEV% createsynthesizerdispatch > ..\fc\Compile\CLI\Synthesizer2.cs

RAM %FDEV% GenerateInterpretMethodTemplate > ..\fint\Interpret\CLI\InterpreterImpl2.cs
