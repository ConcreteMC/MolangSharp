using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class TernaryExpression : Expression
	{
		public IExpression Condition => Parameters[0];
		public IExpression ThenExpr => Parameters[1];
		public IExpression ElseExpr => Parameters[2];

		public TernaryExpression(IExpression condition, IExpression thenExpr, IExpression elseExpr) : base(
			condition, thenExpr, elseExpr) { }

		/// <inheritdoc />
		/// <remarks>
		///		Any value other than 0.0 selects the "then" branch: "For boolean tests, a float value equivalent to
		///		0.0 is false, and anything not equal to 0.0 is true."
		/// </remarks>
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			if (Condition.Evaluate(scope, environment).AsBool())
			{
				return ThenExpr == null ? Condition.Evaluate(scope, environment) :
					ThenExpr.Evaluate(scope, environment);
			}
			else if (ElseExpr != null)
			{
				return ElseExpr.Evaluate(scope, environment);
			}

			return DoubleValue.Zero;
		}
	}
}