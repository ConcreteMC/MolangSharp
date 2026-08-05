using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies operator precedence and associativity as specified by the Molang syntax guide.
/// </summary>
/// <remarks>
///		The "Versioned changes" table of the syntax guide pins down the two cases that were historically
///		implemented incorrectly by the game itself:
///		<list type="bullet">
///			<item>1.18.10 - "Fixed conditional (ternary) operator associativity. Previously nested conditional
///			expressions like <c>A ? B : C ? D : E</c> would evaluate as <c>(A ? B : C) ? D : E</c>. Now they evaluate
///			as <c>A ? B : (C ? D : E)</c>."</item>
///			<item>1.18.20 - "Fixed Logical AND to evaluate before Logical OR, and for comparison operators to
///			evaluate before equality operators."</item>
///		</list>
/// </remarks>
[TestClass]
public class OperatorPrecedenceSpecTest
{
	[TestMethod("Multiplication and division bind tighter than addition and subtraction")]
	public void ProductBeforeSum()
	{
		Assert.AreEqual(7d, Mo.Eval("1 + 2 * 3"));
		Assert.AreEqual(9d, Mo.Eval("(1 + 2) * 3"));
		Assert.AreEqual(5d, Mo.Eval("1 + 8 / 2"));
		Assert.AreEqual(-5d, Mo.Eval("1 - 2 * 3"));
	}

	[TestMethod("Unary minus binds tighter than multiplication")]
	public void PrefixBeforeProduct()
	{
		Assert.AreEqual(-6d, Mo.Eval("-2 * 3"));
		Assert.AreEqual(6d, Mo.Eval("-2 * -3"));
		Assert.AreEqual(1d, Mo.Eval("-1 + 2"));
	}

	[TestMethod("Logical NOT binds tighter than comparison")]
	public void NotBeforeComparison()
	{
		// (!0) == 1 -> 1, whereas !(0 == 1) would also be 1, so use a discriminating pair.
		Assert.AreEqual(1d, Mo.Eval("!0 == 1"));
		Assert.AreEqual(0d, Mo.Eval("!1 == 1"));
	}

	[TestMethod("Arithmetic binds tighter than comparison")]
	public void SumBeforeComparison()
	{
		Assert.AreEqual(1d, Mo.Eval("1 + 1 > 1"));
		Assert.AreEqual(1d, Mo.Eval("2 * 2 >= 4"));
	}

	/// <summary>
	///		1.18.20: "for comparison operators to evaluate before equality operators".
	/// </summary>
	[TestMethod("Comparison operators bind tighter than equality operators")]
	public void ComparisonBeforeEquality()
	{
		// 1 != (2 < 3) => 1 != 1 => 0.   Same-precedence, left-to-right would give (1 != 2) < 3 => 1 < 3 => 1.
		Assert.AreEqual(0d, Mo.Eval("1 != 2 < 3"));

		// 0 == (2 < 3) => 0 == 1 => 0.   Same-precedence would give (0 == 2) < 3 => 0 < 3 => 1.
		Assert.AreEqual(0d, Mo.Eval("0 == 2 < 3"));

		// 1 != (2 >= 3) => 1 != 0 => 1.  Same-precedence would give (1 != 2) >= 3 => 1 >= 3 => 0.
		Assert.AreEqual(1d, Mo.Eval("1 != 2 >= 3"));

		// 1 == (3 <= 2) => 1 == 0 => 0.  Same-precedence would give (1 == 3) <= 2 => 0 <= 2 => 1.
		Assert.AreEqual(0d, Mo.Eval("1 == 3 <= 2"));
	}

	/// <summary>
	///		1.18.20: "Fixed Logical AND to evaluate before Logical OR".
	/// </summary>
	[TestMethod("Logical AND binds tighter than logical OR")]
	public void AndBeforeOr()
	{
		// 1 || (0 && 0) => 1.  (1 || 0) && 0 would be 0.
		Assert.AreEqual(1d, Mo.Eval("1 || 0 && 0"));

		// (0 && 1) || 1 => 1.  0 && (1 || 1) would be 0.
		Assert.AreEqual(1d, Mo.Eval("0 && 1 || 1"));

		// Parentheses must still be able to force the other grouping.
		Assert.AreEqual(0d, Mo.Eval("(1 || 0) && 0"));
		Assert.AreEqual(0d, Mo.Eval("0 && (1 || 1)"));
	}

	[TestMethod("Equality binds tighter than logical AND and OR")]
	public void EqualityBeforeLogical()
	{
		// (1 == 1) && (2 == 2) => 1
		Assert.AreEqual(1d, Mo.Eval("1 == 1 && 2 == 2"));
		Assert.AreEqual(0d, Mo.Eval("1 == 1 && 2 == 3"));
		Assert.AreEqual(1d, Mo.Eval("1 == 2 || 3 == 3"));
	}

	/// <summary>
	///		1.18.10: nested conditionals evaluate as <c>A ? B : (C ? D : E)</c>.
	/// </summary>
	[TestMethod("The ternary conditional operator is right-associative")]
	public void TernaryIsRightAssociative()
	{
		// 1 ? 1 : (1 ? 2 : 3) => 1.  Left-associative parsing gives (1 ? 1 : 1) ? 2 : 3 => 2.
		Assert.AreEqual(1d, Mo.Eval("1 ? 1 : 1 ? 2 : 3"));

		// 0 ? 1 : (1 ? 2 : 3) => 2
		Assert.AreEqual(2d, Mo.Eval("0 ? 1 : 1 ? 2 : 3"));

		// 0 ? 1 : (0 ? 2 : 3) => 3
		Assert.AreEqual(3d, Mo.Eval("0 ? 1 : 0 ? 2 : 3"));

		// Three levels deep: 0 ? 1 : (0 ? 2 : (1 ? 3 : 4)) => 3
		Assert.AreEqual(3d, Mo.Eval("0 ? 1 : 0 ? 2 : 1 ? 3 : 4"));

		// Explicit parentheses must still produce the left-associative reading.
		Assert.AreEqual(2d, Mo.Eval("(1 ? 1 : 1) ? 2 : 3"));
	}

	[TestMethod("A conditional nested in the 'then' branch is delimited by its own colon")]
	public void TernaryNestedInThenBranch()
	{
		// 1 ? (0 ? 2 : 3) : 4 => 3
		Assert.AreEqual(3d, Mo.Eval("1 ? 0 ? 2 : 3 : 4"));

		// 0 ? (1 ? 2 : 3) : 4 => 4
		Assert.AreEqual(4d, Mo.Eval("0 ? 1 ? 2 : 3 : 4"));
	}

	[TestMethod("Arithmetic and comparisons bind tighter than the conditional operator")]
	public void ComparisonBeforeConditional()
	{
		Assert.AreEqual(10d, Mo.Eval("1 + 1 > 1 ? 10 : 20"));
		Assert.AreEqual(20d, Mo.Eval("1 + 1 > 5 ? 10 : 20"));
		Assert.AreEqual(30d, Mo.Eval("1 ? 10 + 20 : 0"));
	}

	[TestMethod("Array access binds tighter than every other operator")]
	public void ArrayAccessBindsTightest()
	{
		var runtime = ArrayRuntime();

		// The index must attach to v.arr, not to the surrounding expression.
		Assert.AreEqual(21d, Mo.Eval(runtime, "1 + v.arr[1]"));
		Assert.AreEqual(11d, Mo.Eval(runtime, "1 + v.arr[0]"));
		Assert.AreEqual(60d, Mo.Eval(runtime, "2 * v.arr[2]"));
		Assert.AreEqual(1d, Mo.Eval(runtime, "v.arr[0] < v.arr[1]"));
		Assert.AreEqual(30d, Mo.Eval(runtime, "v.arr[0] + v.arr[1]"));
	}

	[TestMethod("Array indexes are full expressions delimited by the brackets")]
	public void ArrayIndexIsAnExpression()
	{
		var runtime = ArrayRuntime();

		Assert.AreEqual(30d, Mo.Eval(runtime, "v.arr[1 + 1]"));
		Assert.AreEqual(20d, Mo.Eval(runtime, "v.arr[3 - 2]"));
		Assert.AreEqual(30d, Mo.Eval(runtime, "v.arr[1 ? 2 : 0]"));
	}

	[TestMethod("The arrow operator binds tighter than arithmetic")]
	public void ArrowBeforeArithmetic()
	{
		var runtime = new MoLangRuntime();
		var other = new MoLangEnvironment();
		other.SetValue(new MoPath("variable.x"), new DoubleValue(7));
		runtime.Environment.Structs["variable"].Set(new MoPath("pig"), other);

		// (v.pig->v.x) + 1 => 8
		Assert.AreEqual(8d, Mo.Eval(runtime, "v.pig->v.x + 1"));
		Assert.AreEqual(14d, Mo.Eval(runtime, "v.pig->v.x * 2"));
	}

	[TestMethod("The null coalescing operator binds looser than arithmetic and comparison")]
	public void CoalesceBindsLoosest()
	{
		// v.undefined ?? (1 + 2) => 3
		Assert.AreEqual(3d, Mo.Eval("v.undefined ?? 1 + 2"));

		// v.undefined ?? (2 > 1) => 1
		Assert.AreEqual(1d, Mo.Eval("v.undefined ?? 2 > 1"));
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
