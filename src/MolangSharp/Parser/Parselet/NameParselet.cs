using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the "name" parser
	/// </summary>
	/// <remarks>
	///		Used to parse function calls or property accessors.
	///		For example: "query.frame_time" or "math.min(10, 20)"
	/// </remarks>
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