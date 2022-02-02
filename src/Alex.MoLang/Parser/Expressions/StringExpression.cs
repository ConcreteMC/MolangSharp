using System;
using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Parser.Expressions
{
	public class StringExpression : Expression
	{
		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			return new StringValue(_value);
		}

		private string _value;

		/// <inheritdoc />
		public StringExpression(string value) : base()
		{
			_value = value;
		}
	}
}