using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions
{
	public class LoopExpression : Expression
	{
		public IExpression Count => Parameters[0];
		public IExpression Body => Parameters[1];

		public LoopExpression(IExpression count, IExpression body) : base(count, body)
		{
		}

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			int loop = (int)(double)Count.Evaluate(scope, environment).Value;
			MoScope subScope = new MoScope(scope.Runtime)
			{
				Runtime = scope.Runtime
			};

			while (loop > 0)
			{
				subScope.IsContinue = false;
				subScope.IsBreak = false;

				Body.Evaluate(subScope, environment);
				loop--;

				if (subScope.ReturnValue != null)
				{
					return subScope.ReturnValue;
				}
				else if (subScope.IsBreak)
				{
					break;
				}
			}

			return DoubleValue.Zero;
		}
	}
}