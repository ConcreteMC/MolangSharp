namespace ConcreteMC.MolangSharp.Parser
{
	public interface IExprVisitor
	{
		void BeforeTraverse(IExpression[] expressions);

		IExpression OnVisit(ExprTraverser traverser, IExpression expression);

		void OnLeave(IExpression expression);

		void AfterTraverse(IExpression[] expressions);
	}

	public abstract class ExprVisitor : IExprVisitor
	{
		/// <inheritdoc />
		public virtual void BeforeTraverse(IExpression[] expressions) { }

		/// <inheritdoc />
		public abstract IExpression OnVisit(ExprTraverser traverser, IExpression expression);

		/// <inheritdoc />
		public virtual void OnLeave(IExpression expression) { }

		/// <inheritdoc />
		public virtual void AfterTraverse(IExpression[] expressions) { }
	}
}