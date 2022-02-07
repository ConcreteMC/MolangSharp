using System.Diagnostics;
using System.Globalization;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime;
using MolangSharp.Examples.Basic;

Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

//Create a new Entity which will use Interop to allow for MoLang->C# communication
var myEntity = new BaseEntity();

//Initialize the runtime
MoLangRuntime runtime = new MoLangRuntime(myEntity);

//Parse our MoLang expression
var parsed = MoLangParser.Parse("math.cos(query.life_time * 60) * -45.0");

Stopwatch sw = Stopwatch.StartNew();
while (true)
{
	var value = runtime.Execute(parsed);
	Console.WriteLine($"[{sw.Elapsed.ToString("mm\\:ss\\.fff")}] Rotation: {value.AsDouble():F3}");
	Thread.Sleep(100);
}