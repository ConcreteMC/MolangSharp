using System;
using System.Collections.Generic;

namespace ConcreteMC.MolangSharp.Parser.Visitors
{
	public class FindingVisitor : ExprVisitor
	{
		private Predicate<IExpression> _predicate;
		public List<IExpression> FoundExpressions = new List<IExpression>();

		public FindingVisitor(Predicate<IExpression> predicate)
		{
			_predicate = predicate;
		}

		/// <inheritdoc />
		public override IExpression OnVisit(ExprTraverser traverser, IExpression expression)
		{
			if (_predicate(expression))
			{
				FoundExpressions.Add(expression);
			}

			return expression;
		}
	}
}