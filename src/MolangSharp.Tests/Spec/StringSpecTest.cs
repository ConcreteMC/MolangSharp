using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies the "Strings" section of the Molang syntax guide.
/// </summary>
/// <remarks>
///		"Strings in Molang are surrounded by single quotes, such as 'minecraft:pig' or 'hello world!'. An empty string
///		is defined as two consecutive single quotes shown here as ''." <br/>
///		"String operations only support == and != at this time." <br/>
///		"All things in Molang are case-insensitive, with the exception of strings, which maintain the case provided."
/// </remarks>
[TestClass]
public class StringSpecTest
{
	[TestMethod("String literals keep their contents verbatim")]
	public void StringLiteralsAreVerbatim()
	{
		Assert.AreEqual("minecraft:pig", Mo.Value("'minecraft:pig'").AsString());
		Assert.AreEqual("hello world!", Mo.Value("'hello world!'").AsString());
		Assert.AreEqual("MixedCase", Mo.Value("'MixedCase'").AsString());
		Assert.AreEqual("1 + 2", Mo.Value("'1 + 2'").AsString());
	}

	[TestMethod("Two consecutive single quotes are the empty string")]
	public void EmptyString()
	{
		Assert.AreEqual(string.Empty, Mo.Value("''").AsString());
		Assert.AreEqual(1d, Mo.Eval("'' == ''"));
		Assert.AreEqual(0d, Mo.Eval("'' == 'a'"));
	}

	[TestMethod("Strings support == and !=")]
	public void StringEquality()
	{
		Assert.AreEqual(1d, Mo.Eval("'pig' == 'pig'"));
		Assert.AreEqual(0d, Mo.Eval("'pig' == 'cow'"));
		Assert.AreEqual(0d, Mo.Eval("'pig' != 'pig'"));
		Assert.AreEqual(1d, Mo.Eval("'pig' != 'cow'"));
	}

	[TestMethod("String comparisons are case-sensitive")]
	public void StringComparisonIsCaseSensitive()
	{
		Assert.AreEqual(0d, Mo.Eval("'Pig' == 'pig'"));
		Assert.AreEqual(1d, Mo.Eval("'Pig' != 'pig'"));
		Assert.AreEqual(1d, Mo.Eval("'PIG' == 'PIG'"));
	}

	[TestMethod("Strings survive round-tripping through a variable")]
	public void StringsRoundTripThroughVariables()
	{
		Assert.AreEqual("minecraft:pig", Mo.Value("v.name = 'minecraft:pig'; return v.name;").AsString());
		Assert.AreEqual(1d, Mo.Eval("v.name = 'minecraft:pig'; return v.name == 'minecraft:pig';"));
		Assert.AreEqual(0d, Mo.Eval("v.name = 'minecraft:pig'; return v.name == 'minecraft:cow';"));
	}

	[TestMethod("Strings can be used as function arguments and in conditionals")]
	public void StringsAsArgumentsAndConditions()
	{
		Assert.AreEqual(10d, Mo.Eval("('pig' == 'pig') ? 10 : 20"));
		Assert.AreEqual(20d, Mo.Eval("('pig' == 'cow') ? 10 : 20"));
	}

	[TestMethod("A string is not equal to a number")]
	public void StringsAreNotNumbers()
	{
		Assert.AreEqual(0d, Mo.Eval("'1' == 1"));
		Assert.AreEqual(1d, Mo.Eval("'1' != 1"));
	}
}
