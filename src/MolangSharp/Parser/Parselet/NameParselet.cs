using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Parser.Parselet
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