# Interoperability
MolangSharp has the ability to expose class properties, fields & methods to the runtime.

## Defining a property
In the following example we will show howto expose a property or field to MoLang.

```c#
[MoProperty("myProperty")]
public double MyProperty { get; set; }

//The following property only has a GET accessor and will be readonly
[MoProperty("myReadOnlyProperty")]
public double ReadonlyProperty { get; }

//This will expose the field to MoLang
[MoProperty("myField")]
public readonly double MyField;
```

## Defining a function/method
In the following example we will show howto expose a method to MoLang.
```c#
[MoFunction("add")]
public double Add(double a, double b)
{
    return a + b;
}
```

## Example class
This example will show how to expose our custom MyMath class to MoLang

### The class
This is the class we'll be exposing to MoLang
```c#
public class MyMath
{
     [MoFunction("sin")] 
     public double Sin(double value) => Math.Sin(value * (Math.PI / 180d));
     
     [MoFunction("asin")] 
     public double Asin(double value) => Math.Asin(value * (Math.PI / 180d));
     
     [MoFunction("cos")] 
     public double Cos(double value) => Math.Cos(value * (Math.PI / 180d));
     
     [MoFunction("acos")] 
     public double Acos(double value) => Math.Acos(value * (Math.PI / 180d));
}
```

### Making the class available to the runtime
```c#
MoLangEnvironment environment = new MoLangEnvironment();
environment.Structs.Add("custom_math", new InteropStruct(new MyMath()));
MoLangRuntime runtime = new MoLangRuntime(environment);
```

This would expose the MyMath class to the runtime instance created in the example. The following example would now be valid.
```
custom_math.sin(12.0);
```