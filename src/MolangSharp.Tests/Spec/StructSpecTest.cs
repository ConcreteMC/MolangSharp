using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the "Structs" section of the Molang syntax guide, together with the arrow operator that is used to
///		read and write struct members on another entity.
/// </summary>
/// <remarks>
///		"One difference between Molang and the C style syntax is that structures of data are implicitly defined by
///		usage." <br/>
///		"-> Arrow operator, for accessing data from a different entity"
/// </remarks>
[TestClass]
public class StructSpecTest
{
	/// <summary>
	///		The syntax guide's own example:
	///		<code>
	///		v.location.x = 1;
	///		v.location.y = 2;
	///		v.location.z = 3;
	///		</code>
	/// </summary>
	[TestMethod("Structs are implicitly defined by assigning to a nested path")]
	public void StructsAreImplicitlyDefined()
	{
		Assert.AreEqual(
			6d, Mo.Eval(
				@"
v.location.x = 1;
v.location.y = 2;
v.location.z = 3;
return v.location.x + v.location.y + v.location.z;"));
	}

	[TestMethod("Structs can be nested arbitrarily deep")]
	public void StructsNestArbitrarilyDeep()
	{
		Assert.AreEqual(1.23d, Mo.Eval("v.a.b.c.d.e.f = 1.23; return v.a.b.c.d.e.f;"));
		Assert.AreEqual(1.23d, Mo.Eval("t.a.b.c.d.e.f = 1.23; return t.a.b.c.d.e.f;"));
	}

	[TestMethod("A struct can be assigned to another variable and read back through it")]
	public void StructsCanBePassedAround()
	{
		Assert.AreEqual(2d, Mo.Eval("v.location.y = 2; v.copy = v.location; return v.copy.y;"));
		Assert.AreEqual(2d, Mo.Eval("v.a.b.c = 2; v.copy = v.a.b; return v.copy.c;"));
		Assert.AreEqual(2d, Mo.Eval("v.a.b.c = 2; v.copy = v.a; return v.copy.b.c;"));
	}

	[TestMethod("Reading an undefined struct member yields 0.0")]
	public void UndefinedStructMemberIsZero()
	{
		Assert.AreEqual(0d, Mo.Eval("v.location.x = 1; return v.location.y;"));
	}

	[TestMethod("The arrow operator reads a variable from another entity")]
	public void ArrowReadsFromAnotherEntity()
	{
		var runtime = new MoLangRuntime();
		var pig = new MoLangEnvironment();
		pig.SetValue(new MoPath("variable.oink"), new DoubleValue(7));
		runtime.Environment.Structs["variable"].Set(new MoPath("pigpig"), pig);

		Assert.AreEqual(7d, Mo.Eval(runtime, "v.pigpig->v.oink"));
		Assert.AreEqual(7d, Mo.Eval(runtime, "return v.pigpig->variable.oink;"));
	}

	[TestMethod("The arrow operator writes a variable on another entity")]
	public void ArrowWritesToAnotherEntity()
	{
		var runtime = new MoLangRuntime();
		var pig = new MoLangEnvironment();
		runtime.Environment.Structs["variable"].Set(new MoPath("pigpig"), pig);

		Mo.Eval(runtime, "v.pigpig->v.oink = 5;");

		Assert.AreEqual(5d, pig.GetValue(new MoPath("variable.oink")).AsDouble());
		Assert.AreEqual(0d, runtime.Environment.GetValue(new MoPath("variable.oink")).AsDouble());
	}

	/// <summary>
	///		Each of the five expressions in the syntax guide's "Structs" section must return 1.23.
	/// </summary>
	[TestMethod("The five documented struct examples all return 1.23")]
	public void DocumentedStructExamples()
	{
		string[] examples =
		{
			"v.cowcow.friend = v.pigpig; v.pigpig->v.test.a.b.c = 1.23; return v.cowcow.friend->v.test.a.b.c;",
			"v.cowcow.friend = v.pigpig; v.pigpig->v.test.a.b.c = 1.23; v.moo = v.cowcow.friend->v.test; return v.moo.a.b.c;",
			"v.cowcow.friend = v.pigpig; v.pigpig->v.test.a.b.c = 1.23; v.moo = v.cowcow.friend->v.test.a; return v.moo.b.c;",
			"v.cowcow.friend = v.pigpig; v.pigpig->v.test.a.b.c = 1.23; v.moo = v.cowcow.friend->v.test.a.b; return v.moo.c;",
			"v.cowcow.friend = v.pigpig; v.pigpig->v.test.a.b.c = 1.23; v.moo = v.cowcow.friend->v.test.a.b.c; return v.moo;"
		};

		foreach (var example in examples)
		{
			var runtime = new MoLangRuntime();
			runtime.Environment.Structs["variable"].Set(new MoPath("pigpig"), new MoLangEnvironment());

			Assert.AreEqual(1.23d, Mo.Eval(runtime, example), 1e-9, example);
		}
	}

	[TestMethod("Struct references can be chained across two entities")]
	public void ArrowChaining()
	{
		var runtime = new MoLangRuntime();
		var pig = new MoLangEnvironment();
		var cow = new MoLangEnvironment();

		runtime.Environment.Structs["variable"].Set(new MoPath("pig"), pig);
		pig.Structs["variable"].Set(new MoPath("cow"), cow);
		cow.SetValue(new MoPath("variable.moo"), new DoubleValue(42));

		Assert.AreEqual(42d, Mo.Eval(runtime, "v.pig->v.cow->v.moo"));
	}

	/// <summary>
	///		"For array indexes, floats are C-style-cast to integers, and clamped at 0 for negative values or wrapped
	///		by the array size for large values."
	/// </summary>
	[TestMethod("Array indexes are cast to integers")]
	public void ArrayIndexesAreCastToIntegers()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(10d, Mo.Eval(runtime, "v.arr[0]"));
		Assert.AreEqual(20d, Mo.Eval(runtime, "v.arr[1]"));
		Assert.AreEqual(30d, Mo.Eval(runtime, "v.arr[2]"));
		Assert.AreEqual(20d, Mo.Eval(runtime, "v.arr[1.9]"));
		Assert.AreEqual(10d, Mo.Eval(runtime, "v.arr[0.5]"));
	}

	[TestMethod("Negative array indexes are clamped to 0")]
	public void NegativeArrayIndexesAreClamped()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(10d, Mo.Eval(runtime, "v.arr[-1]"));
		Assert.AreEqual(10d, Mo.Eval(runtime, "v.arr[-100]"));
	}

	[TestMethod("Array indexes past the end wrap around by the array size")]
	public void LargeArrayIndexesWrap()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(10d, Mo.Eval(runtime, "v.arr[3]"));
		Assert.AreEqual(20d, Mo.Eval(runtime, "v.arr[4]"));
		Assert.AreEqual(30d, Mo.Eval(runtime, "v.arr[5]"));
		Assert.AreEqual(10d, Mo.Eval(runtime, "v.arr[6]"));
	}

	[TestMethod("Array elements can be assigned")]
	public void ArrayElementsCanBeAssigned()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(99d, Mo.Eval(runtime, "v.arr[1] = 99; return v.arr[1];"));
		Assert.AreEqual(99d, Mo.Eval(runtime, "v.arr[-1] = 99; return v.arr[0];"));
		Assert.AreEqual(99d, Mo.Eval(runtime, "v.arr[4] = 99; return v.arr[1];"));
	}

	[TestMethod("Indexing an empty array yields 0.0 instead of throwing")]
	public void IndexingAnEmptyArrayIsZero()
	{
		var runtime = new MoLangRuntime();
		runtime.Environment.Structs["variable"].Set(new MoPath("arr"), new ArrayStruct());

		Assert.AreEqual(0d, Mo.Eval(runtime, "v.arr[0]"));
		Assert.AreEqual(0d, Mo.Eval(runtime, "v.arr[5]"));
		Assert.AreEqual(0d, Mo.Eval(runtime, "v.arr[-5]"));
	}

	private static MoLangRuntime ArrayRuntime()
	{
		var runtime = new MoLangRuntime();

		runtime.Environment.Structs["variable"].Set(
			new MoPath("arr"), new ArrayStruct(
				new List<IMoValue>
				{
					new DoubleValue(10), new DoubleValue(20), new DoubleValue(30)
				}));

		return runtime;
	}
}
