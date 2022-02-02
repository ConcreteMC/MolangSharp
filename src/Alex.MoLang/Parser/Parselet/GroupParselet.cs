using Alex.MoLang.Parser.Tokenizer;

namespace Alex.MoLang.Parser.Parselet
{
	public class GroupParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			IExpression expr = parser.ParseExpression();
			var result = parser.ConsumeToken(TokenType.BracketRight);

			return expr;
		}
	}
}