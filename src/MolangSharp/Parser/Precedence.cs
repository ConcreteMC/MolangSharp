namespace ConcreteMC.MolangSharp.Parser
{
	public enum Precedence
	{
		Anything,
		Scope,

		Assignment,
		Conditional,
		ArrayAccess,

		Coalesce,

		And,
		Or,

		Compare,

		Sum,
		Product,
		Prefix,
		Arrow
	}
}