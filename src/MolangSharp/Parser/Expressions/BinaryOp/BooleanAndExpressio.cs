using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	public class BooleanAndExpression : BinaryOpExpression
	{
		/// <inheritdoc />
		public BooleanAndExpression(IExpression l, IExpression r) : base(l, r) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(
				Left.Evaluate(scope, environment).AsBool() && Right.Evaluate(scope, environment).AsBool());
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "&&";
		}
	}
}