using System;
using Alex.MoLang.Parser.Exceptions;
using Alex.MoLang.Parser.Expressions;
using Alex.MoLang.Parser.Tokenizer;

namespace Alex.MoLang.Parser.Parselet
{
	public class LoopParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			if (!parser.TryParseArgs(out var expressions) || expressions.Length != 2)
				throw new MoLangParserException($"Loop: Expected 2 argument, {(expressions?.Length ?? 0)} argument given");
			
			return new LoopExpression(expressions[0], expressions[1]);
		}
	}
}