using ConcreteMC.MolangSharp.Parser.Exceptions;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the "loop" instruction parser
	/// </summary>
	public class LoopParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (!parser.TryParseArgs(out var expressions) || expressions.Length != 2)
				throw new MoLangParserException(
					$"Loop: Expected 2 argument, {(expressions?.Length ?? 0)} argument given");

			return new LoopExpression(expressions[0], expressions[1]);
		}
	}
}