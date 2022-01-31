using System.Globalization;
using Alex.MoLang.Parser.Exceptions;
using Alex.MoLang.Parser.Expressions;
using Alex.MoLang.Parser.Tokenizer;

namespace Alex.MoLang.Parser.Parselet
{
	public class NumberParselet : PrefixParselet
	{
		private const NumberStyles NumberStyle = System.Globalization.NumberStyles.AllowDecimalPoint;
		private static readonly CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (double.TryParse(token.Text, out var result))
			{
				return new NumberExpression(result);
			}

			throw new MoLangParserException($"Could not parse \'{token.Text.ToString()}\' as a double");
		}
	}
}