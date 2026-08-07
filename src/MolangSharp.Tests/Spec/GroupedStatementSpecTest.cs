using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcreteMC.MolangSharp.Tests.Spec;

/// <summary>
///		Verifies that parentheses accept a statement list, not just a single term.
/// </summary>
/// <remarks>
///		The syntax guide documents <c>( )</c> as "Parentheses for expression term evaluation control" and <c>{ }</c>
///		as the brace scope, but the language also accepts a semicolon separated statement list inside parentheses,
///		as in <c>q.x == 1 ? (v.a = 1; return true;) : 0</c>. A statement list follows the same rule as a brace scope:
///		it evaluates to 0.0 unless it returns.
/// </remarks>
[TestClass]
public class GroupedStatementSpecTest
{
	[TestMethod("Parentheses still group a single term")]
	public void SingleTermKeepsItsValue()
	{
		Assert.AreEqual(9d, Mo.Eval("(1 + 2) * 3"));
		Assert.AreEqual(7d, Mo.Eval("1 + (2 * 3)"));
		Assert.AreEqual(3d, Mo.Eval("v.a = 3; return (v.a);"));
		Assert.AreEqual(5d, Mo.Eval("(5)"));
	}

	[TestMethod("A trailing semicolon does not change a single term's value")]
	public void SingleTermWithTrailingSemicolon()
	{
		Assert.AreEqual(5d, Mo.Eval("(5;)"));
		Assert.AreEqual(9d, Mo.Eval("(1 + 2;) * 3"));
	}

	[TestMethod("Parentheses accept a statement list")]
	public void StatementListRuns()
	{
		Assert.AreEqual(3d, Mo.Eval("(v.a = 1; v.b = 2;); return v.a + v.b;"));
		Assert.AreEqual(0d, Mo.Eval("(v.a = 1; v.b = 2;)"));
		Assert.AreEqual(7d, Mo.Eval("(v.a = 7; return v.a;)"));
	}

	[TestMethod("A statement list only runs when its conditional selects it")]
	public void StatementListInConditional()
	{
		const string script = "v.scared = 0; q.mark_variant == 1 ? (v.scared = 1; return true;) : 0;";

		Assert.AreEqual(1d, Mo.Eval(Query(("mark_variant", 1)), script));
		Assert.AreEqual(0d, Mo.Eval(Query(("mark_variant", 2)), script));
	}

	[TestMethod("A return inside a statement list ends the whole expression")]
	public void ReturnInsideStatementList()
	{
		Assert.AreEqual(1d, Mo.Eval("1 ? (v.a = 1; return v.a;) : 0; return 99;"));
		Assert.AreEqual(99d, Mo.Eval("0 ? (v.a = 1; return v.a;) : 0; return 99;"));
	}

	[TestMethod("A conditional statement list writes every variable it assigns")]
	public void StatementListAssignsEveryVariable()
	{
		var runtime = new MoLangRuntime();

		Mo.Eval(
			runtime,
			@"
variable.resume_idle = 1;
variable.show_package_assets = 1;
variable.show_pipe_assets = 1;
variable.resume_idle ? (variable.show_package_assets = 0; variable.show_pipe_assets = 0; variable.resume_idle = 0; return 1;) : 0;");

		Assert.AreEqual(0d, Mo.Eval(runtime, "variable.show_package_assets"));
		Assert.AreEqual(0d, Mo.Eval(runtime, "variable.show_pipe_assets"));
		Assert.AreEqual(0d, Mo.Eval(runtime, "variable.resume_idle"));
	}

	[TestMethod("A statement list nested in another parenthesised conditional runs its maths")]
	public void NestedConditionalStatementList()
	{
		var runtime = Query(("variant", 36180));

		Mo.Eval(
			runtime,
			@"
variable.pan_tilt = 0;
((variable.pan_tilt != query.variant) ? (variable.pan_tilt = query.variant; variable.tilt = math.floor(variable.pan_tilt / 360.0) - 180.0; variable.pan = math.floor(variable.pan_tilt - (variable.tilt * 360.0)) - 180.0; return 0;) : 0;);");

		// 36180 / 360 = 100.5 -> floor 100 -> tilt = 100 - 180 = -80
		Assert.AreEqual(-80d, Mo.Eval(runtime, "variable.tilt"), 1e-9);

		// pan = floor(36180 - (-80 * 360)) - 180 = floor(36180 + 28800) - 180 = 64800
		Assert.AreEqual(64800d, Mo.Eval(runtime, "variable.pan"), 1e-9);
	}

	[TestMethod("A statement list guarded by an equality test is skipped when the test fails")]
	public void NestedConditionalStatementListSkipped()
	{
		var runtime = Query(("variant", 500));

		Mo.Eval(
			runtime,
			@"
variable.pan_tilt = 500;
variable.tilt = 42;
((variable.pan_tilt != query.variant) ? (variable.tilt = math.floor(variable.pan_tilt / 360.0) - 180.0; return 0;) : 0;);");

		Assert.AreEqual(42d, Mo.Eval(runtime, "variable.tilt"), 1e-9);
	}

	[TestMethod("Both branches of a conditional may be statement lists")]
	public void BothBranchesCanBeStatementLists()
	{
		Assert.AreEqual(
			1d, Mo.Eval(
				@"
variable.first_run = 1;
variable.first_run ? (variable.first_run = false; variable.result = 1;) : (variable.result = 2;);
return variable.result;"));

		Assert.AreEqual(
			2d, Mo.Eval(
				@"
variable.first_run = 0;
variable.first_run ? (variable.first_run = false; variable.result = 1;) : (variable.result = 2;);
return variable.result;"));
	}

	[TestMethod("A statement list nests inside another statement list")]
	public void NestedStatementLists()
	{
		Assert.AreEqual(3d, Mo.Eval("(v.a = 1; (v.b = 2;); v.c = v.a + v.b;); return v.c;"));
	}

	[TestMethod("Function arguments are unaffected")]
	public void FunctionArgumentsStillParse()
	{
		Assert.AreEqual(1d, Mo.Eval("math.min(1, 2)"));
		Assert.AreEqual(3d, Mo.Eval("math.max(math.min(1, 2), 3)"));
		Assert.AreEqual(2d, Mo.Eval("t.a = 0; loop(2, { t.a = t.a + 1; }); return t.a;"));
		Assert.AreEqual(5d, Mo.Eval("math.max((1 + 2), 5)"));
	}

	/// <summary>
	///		Builds a runtime with a readable "query." scope holding the given values.
	/// </summary>
	private static MoLangRuntime Query(params (string Name, double Value)[] values)
	{
		var runtime = new MoLangRuntime();
		var query = new VariableStruct();

		foreach (var (name, value) in values)
			query.Set(new MoPath(name), new DoubleValue(value));

		runtime.Environment.Structs["query"] = query;

		return runtime;
	}
}
