using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Parser.Expressions
{
	public class UnaryPlusExpression : Expression
	{
		/// <inheritdoc />
		public UnaryPlusExpression(IExpression value) : base(value)
		{
			//_value = value;
		}
		
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new DoubleValue(+(Parameters[0].Evaluate(scope, environment).AsDouble()));
		}
	}
}