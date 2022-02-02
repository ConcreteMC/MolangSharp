using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Parser.Expressions
{
	public class BooleanExpression : Expression
	{
		private readonly IMoValue _value;

		public BooleanExpression(bool value) : base()
		{
			_value = value ? DoubleValue.One : DoubleValue.Zero;
		}

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return _value;
		}
	}
}