using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;
using MolangSharp.Examples.Basic;

//Create a new Entity which will use Interop to allow for MoLang->C# communication
var myEntity = new BaseEntity();

//Initialize the runtime
MoLangRuntime runtime = new MoLangRuntime(myEntity);

//Parse our MoLang expression
var parsed = MoLangParser.Parse("math.cos(query.life_time * 60) * -45.0");

while (true)
{
	var value = runtime.Execute(parsed);
	Console.WriteLine($"Rotation: {value.AsDouble():F3}");
	Thread.Sleep(100);
}