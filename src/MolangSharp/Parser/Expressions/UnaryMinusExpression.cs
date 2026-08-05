using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class UnaryMinusExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(EvaluateDouble(scope, environment));
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return -Parameters[0].EvaluateDouble(scope, environment);
		}

		/// <inheritdoc />
		public UnaryMinusExpression(IExpression value) : base(value) { }
	}
}