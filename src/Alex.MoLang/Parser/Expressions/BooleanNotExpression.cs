using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Parser.Expressions
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
			return Parameters[0].Evaluate(scope, environment).AsBool() ? DoubleValue.Zero :
				DoubleValue.One; // .Equals(DoubleValue.One) ? DoubleValue.Zero : DoubleValue.One;
		}
	}
}