using Alex.MoLang.Parser.Exceptions;
using Alex.MoLang.Parser.Expressions;
using Alex.MoLang.Parser.Tokenizer;

namespace Alex.MoLang.Parser.Parselet
{
	public class FloatParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (float.TryParse(token.Text, out var result))
			{
				return new NumberExpression(result);
			}

			throw new MoLangParserException($"Could not parse \'{token.Text.ToString()}\' as float");
		}
	}
}