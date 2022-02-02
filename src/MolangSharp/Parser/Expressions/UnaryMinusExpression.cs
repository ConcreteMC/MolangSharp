using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class UnaryMinusExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(-(Parameters[0].Evaluate(scope, environment).AsDouble()));
		}

		/// <inheritdoc />
		public UnaryMinusExpression(IExpression value) : base(value)
		{
			
		}
	}
}