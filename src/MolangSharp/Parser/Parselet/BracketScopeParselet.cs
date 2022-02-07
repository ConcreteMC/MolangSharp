using System.Collections.Generic;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the scope parselet
	/// </summary>
	public class BracketScopeParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			List<IExpression> exprs = new List<IExpression>();

			if (!parser.MatchToken(TokenType.CurlyBracketRight))
			{
				do
				{
					if (parser.MatchToken(TokenType.CurlyBracketRight, false))
					{
						break;
					}

					exprs.Add(parser.ParseExpression(Precedence.Scope));
				} while (parser.MatchToken(TokenType.Semicolon));

				parser.ConsumeToken(TokenType.CurlyBracketRight);
			}

			return new StatementExpression(exprs.ToArray());
		}
	}
}