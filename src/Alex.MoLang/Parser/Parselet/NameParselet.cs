using System;
using Alex.MoLang.Parser.Expressions;
using Alex.MoLang.Parser.Tokenizer;
using Alex.MoLang.Utils;

namespace Alex.MoLang.Parser.Parselet
{
	public class NameParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			var path = parser.FixNameShortcut(new MoPath(token.Text));

			if (parser.TryParseArgs(out var expressions))
				return new FuncCallExpression(path, expressions);

			return new NameExpression(path);
		}
	}
}