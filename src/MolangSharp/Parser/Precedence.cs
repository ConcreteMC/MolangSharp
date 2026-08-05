namespace ConcreteMC.MolangSharp.Parser
{
	/// <summary>
	///		The binding power of an operator, ordered from loosest to tightest.
	/// </summary>
	/// <remarks>
	///		A higher value binds tighter, so <c>1 + 2 * 3</c> groups as <c>1 + (2 * 3)</c> because
	///		<see cref="Product"/> outranks <see cref="Sum"/>.
	///		<br/>
	///		The relative order of <see cref="And"/>/<see cref="Or"/> and of
	///		<see cref="Compare"/>/<see cref="Equality"/> is mandated by the Molang versioned change for 1.18.20:
	///		"Fixed Logical AND to evaluate before Logical OR, and for comparison operators to evaluate before
	///		equality operators."
	/// </remarks>
	public enum Precedence
	{
		Anything,
		Scope,

		Assignment,
		Conditional,

		Coalesce,

		Or,
		And,

		Equality,
		Compare,

		Sum,
		Product,
		Prefix,
		Arrow,

		ArrayAccess
	}
}
