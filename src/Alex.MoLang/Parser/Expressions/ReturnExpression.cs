using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Parser.Expressions
{
	public class ReturnExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			IMoValue eval = Parameters[0].Evaluate(scope, environment);
			scope.ReturnValue = eval;

			return eval;
		}

		/// <inheritdoc />
		public ReturnExpression(IExpression value) : base(value)
		{
		}
	}
}