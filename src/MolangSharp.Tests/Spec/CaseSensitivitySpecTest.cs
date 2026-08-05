using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the case-insensitivity rule and the keyword aliases from the Molang syntax guide.
/// </summary>
/// <remarks>
///		"All things in Molang are case-insensitive, with the exception of strings, which maintain the case provided."
///		<br/>
///		The alias map: <c>context.moo</c>/<c>c.moo</c>, <c>query.moo</c>/<c>q.moo</c>, <c>temp.moo</c>/<c>t.moo</c>,
///		<c>variable.moo</c>/<c>v.moo</c>. "Note that left and right sides function identically in the Alias Map."
/// </remarks>
[TestClass]
public class CaseSensitivitySpecTest
{
	[TestMethod("Scope names are case-insensitive")]
	public void ScopeNamesAreCaseInsensitive()
	{
		Assert.AreEqual(5d, Mo.Eval("VARIABLE.a = 5; return variable.a;"));
		Assert.AreEqual(5d, Mo.Eval("variable.a = 5; return VARIABLE.a;"));
		Assert.AreEqual(5d, Mo.Eval("Temp.a = 5; return temp.a;"));
	}

	[TestMethod("Variable names are case-insensitive")]
	public void VariableNamesAreCaseInsensitive()
	{
		Assert.AreEqual(5d, Mo.Eval("v.MyValue = 5; return v.myvalue;"));
		Assert.AreEqual(5d, Mo.Eval("v.location.X = 5; return v.location.x;"));
	}

	[TestMethod("Aliases are case-insensitive")]
	public void AliasesAreCaseInsensitive()
	{
		Assert.AreEqual(5d, Mo.Eval("V.a = 5; return variable.a;"));
		Assert.AreEqual(5d, Mo.Eval("variable.a = 5; return V.a;"));
		Assert.AreEqual(5d, Mo.Eval("T.a = 5; return temp.a;"));
		Assert.AreEqual(0d, Mo.Eval("C.never_provided"));
	}

	[TestMethod("Math function names are case-insensitive")]
	public void MathFunctionNamesAreCaseInsensitive()
	{
		Assert.AreEqual(3d, Mo.Eval("MATH.ABS(-3)"));
		Assert.AreEqual(3d, Mo.Eval("Math.Abs(-3)"));
		Assert.AreEqual(1d, Mo.Eval("Math.Sin(90)"), 1e-9);
		Assert.AreEqual(System.Math.PI, Mo.Eval("Math.PI"), 1e-9);
		Assert.AreEqual(1024d, Mo.Eval("MATH.POW(2, 10)"), 1e-9);
	}

	[TestMethod("Keywords are case-insensitive")]
	public void KeywordsAreCaseInsensitive()
	{
		Assert.AreEqual(1d, Mo.Eval("RETURN 1;"));
		Assert.AreEqual(1d, Mo.Eval("Return 1;"));
		Assert.AreEqual(1d, Mo.Eval("TRUE"));
		Assert.AreEqual(0d, Mo.Eval("False"));
		Assert.AreEqual(3d, Mo.Eval("t.a = 0; LOOP(3, { t.a = t.a + 1; }); return t.a;"));
		Assert.AreEqual(1d, Mo.Eval("t.a = 0; Loop(3, { t.a = t.a + 1; BREAK; }); return t.a;"));
	}

	[TestMethod("'this' is case-insensitive")]
	public void ThisIsCaseInsensitive()
	{
		var runtime = new MoLangRuntime();
		runtime.Environment.ThisVariable = new DoubleValue(4);

		Assert.AreEqual(4d, Mo.Eval(runtime, "this"));
		Assert.AreEqual(4d, Mo.Eval(runtime, "THIS"));
		Assert.AreEqual(4d, Mo.Eval(runtime, "This"));
	}

	[TestMethod("'for_each' is case-insensitive")]
	public void ForEachIsCaseInsensitive()
	{
		var runtime = new MoLangRuntime();

		runtime.Environment.Structs["variable"].Set(
			new MoPath("arr"), new ArrayStruct(
				new List<IMoValue>
				{
					new DoubleValue(1), new DoubleValue(2)
				}));

		Assert.AreEqual(3d, Mo.Eval(runtime, "t.a = 0; FOR_EACH(t.e, v.arr, { t.a = t.a + t.e; }); return t.a;"));
	}

	[TestMethod("Floating point literals accept an upper case suffix")]
	public void FloatSuffixIsCaseInsensitive()
	{
		Assert.AreEqual(1.5d, Mo.Eval("1.5f"), 1e-6);
		Assert.AreEqual(1.5d, Mo.Eval("1.5F"), 1e-6);
	}
}
