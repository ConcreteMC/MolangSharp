using System;
using System.Collections.Generic;
using Alex.MoLang.Parser.Exceptions;
using Alex.MoLang.Parser.Expressions;
using Alex.MoLang.Parser.Tokenizer;

namespace Alex.MoLang.Parser.Parselet
{
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