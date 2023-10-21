using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class StatementExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			foreach (IExpression expression in Parameters)
			{
				expression.Evaluate(scope, environment);

				if (scope.ReturnValue != null)
				{
					return scope.ReturnValue;
				}
				else if (scope.IsBreak || scope.IsContinue)
				{
					break;
				}
			}

			return DoubleValue.Zero;
		}

		/// <inheritdoc />
		public StatementExpression(IExpression[] value) : base(value) { }
	}
}