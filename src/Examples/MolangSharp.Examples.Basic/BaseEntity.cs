using System.Diagnostics;
using ConcreteMC.MolangSharp.Attributes;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;

namespace MolangSharp.Examples.Basic;

public class BaseEntity : MoLangEnvironment
{
	private Stopwatch _stopwatch = Stopwatch.StartNew();
	public BaseEntity()
	{
		Structs["query"] = new InteropStruct(this);
	}

	[MoProperty("life_time")]
	public double LifeTime => _stopwatch.Elapsed.TotalSeconds;
}