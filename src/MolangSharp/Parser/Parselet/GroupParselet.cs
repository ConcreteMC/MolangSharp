using System.Collections.Generic;
using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Parser.Tokenizer;

namespace ConcreteMC.MolangSharp.Parser.Parselet
{
	/// <summary>
	///		Implements the "group" parselet
	/// </summary>
	/// <remarks>
	///		Parentheses are documented as "Parentheses for expression term evaluation control", but the language also
	///		accepts a semicolon separated statement list inside them, the way braces do, as in
	///		<c>q.x == 1 ? (v.a = 1; return true;) : 0</c>. Both forms are handled here.
	/// </remarks>
	public class GroupParselet : PrefixParselet
	{
		/// <inheritdoc />
		public override IExpression Parse(MoLangParser parser, Token token)
		{
			List<IExpression> expressions = new List<IExpression>();

			do
			{
				if (parser.MatchToken(TokenType.BracketRight, false))
					break;

				expressions.Add(parser.ParseExpression());
			} while (parser.MatchToken(TokenType.Semicolon));

			parser.ConsumeToken(TokenType.BracketRight);

			// A single term keeps its value, so '(1 + 2) * 3' still groups the sum rather than discarding it.
			if (expressions.Count == 1)
				return expressions[0];

			// A statement list follows the same rule as a brace scope: it evaluates to 0.0 unless it returns.
			return new StatementExpression(expressions.ToArray());
		}
	}
}
