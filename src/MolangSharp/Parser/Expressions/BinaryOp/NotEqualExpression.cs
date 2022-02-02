using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class NotEqualExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public NotEqualExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			var left = Left.Evaluate(scope, environment);
			var right = Right.Evaluate(scope, environment);

			return new DoubleValue(!left.Equals(right));
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "!=";
		}
	}
}