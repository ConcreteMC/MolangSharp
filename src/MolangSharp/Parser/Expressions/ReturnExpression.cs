using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
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
		public ReturnExpression(IExpression value) : base(value) { }
	}
}