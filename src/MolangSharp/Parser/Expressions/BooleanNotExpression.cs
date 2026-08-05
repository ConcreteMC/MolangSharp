using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class BooleanNotExpression : Expression
	{
		public BooleanNotExpression(IExpression value) : base(value)
		{
			//Value = value;
		}

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return Parameters[0].EvaluateDouble(scope, environment) != 0d ? DoubleValue.Zero : DoubleValue.One;
		}

		/// <inheritdoc />
		public override double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return Parameters[0].EvaluateDouble(scope, environment) != 0d ? 0d : 1d;
		}
	}
}