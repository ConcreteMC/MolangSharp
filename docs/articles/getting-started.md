# Getting Started

## Basic example
```c#
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;

//Initialize the runtime
MoLangRuntime runtime = new MoLangRuntime();

//Parse our MoLang expression
var parsed = MoLangParser.Parse("10 * 45.0");

//Execute the expression
var result = runtime.Execute(parsed);
Console.WriteLine($"Result={result.AsDouble()}");
```

### Output
```c#
Result=450
```

## More examples
Check out the Examples folder in the Github repository:
[https://github.com/ConcreteMC/MolangSharp/tree/main/src/examples](https://github.com/ConcreteMC/MolangSharp/tree/main/src/examples)