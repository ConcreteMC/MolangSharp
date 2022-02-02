using ConcreteMC.MolangSharp.Parser.Expressions;
using ConcreteMC.MolangSharp.Runtime;

namespace ConcreteMC.MolangSharp.Parser.Visitors
{
    /// <summary>
    ///     Optimizes expressions by pre-calculating static maths
    /// </summary>
    public class MathOptimizationVisitor : ExprVisitor
    {
        private MoScope _scope = new MoScope(new MoLangRuntime());
        private MoLangEnvironment _environment = new MoLangEnvironment();

        public override IExpression OnVisit(ExprTraverser traverser, IExpression expression)
        {
            if (expression is BinaryOpExpression binaryOp)
            {
               return TryOptimize(binaryOp);
            }

            return expression;
        }
        
        private IExpression TryOptimize(BinaryOpExpression expression)
        {
            if (expression.Left is BinaryOpExpression l)
                expression.Left = TryOptimize(l);
            
            if (expression.Right is BinaryOpExpression r)
                expression.Right = TryOptimize(r);
            
            if (expression.Left is NumberExpression && expression.Right is NumberExpression)
            {
                //Can be pre-calculated!
                var eval = expression.Evaluate(_scope, _environment);
                return new NumberExpression(eval);
            }

            return expression;
        }
    }
}