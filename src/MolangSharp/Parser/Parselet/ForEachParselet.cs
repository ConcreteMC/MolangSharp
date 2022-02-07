using ConcreteMC.MolangSharp.Parser.Exceptions;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the "foreach" instruction parser
	/// </summary>
	public class ForEachParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (!parser.TryParseArgs(out var expressions) || expressions.Length != 3)
				throw new MoLangParserException($"ForEach: Expected 3 argument, {(expressions?.Length ?? 0)} argument given");
			
			return new ForEachExpression(expressions[0], expressions[1], expressions[2]);
		}
	}
}