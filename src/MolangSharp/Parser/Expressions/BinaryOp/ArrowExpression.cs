using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser.Expressions.BinaryOp
{
	/// <summary>
	///		The arrow operator, "for accessing data from a different entity"
	/// </summary>
	/// <remarks>
	///		Evaluates to null when the left hand side is not an entity reference, which marks the expression as
	///		unresolvable for the null coalescing operator: "if it's a reference to a deleted entity, the expression
	///		will use [the default] in place of it".
	/// </remarks>
	public class ArrowExpression : BinaryOpExpression
	{
		public ArrowExpression(IExpression left, IExpression right) : base(left, right) { }

		/// <inheritdoc />
		public override IMoValue Evaluate(MoScope scope, MoLangEnvironment environment)
		{
			var leftEnv = Left.Evaluate(scope, environment);

			if (leftEnv is MoLangEnvironment leftMolangEnvironment)
			{
				return Right.Evaluate(scope, leftMolangEnvironment);
			}

			return null;
		}

		/// <inheritdoc />
		/// <remarks>
		///		Assigns through to the referenced entity, so that <c>v.pigpig->v.test.a.b.c = 1.23</c> writes
		///		<c>variable.test.a.b.c</c> on the entity held by <c>v.pigpig</c>.
		/// </remarks>
		public override void Assign(MoScope scope, MoLangEnvironment environment, IMoValue value)
		{
			var leftEnv = Left.Evaluate(scope, environment);

			if (leftEnv is MoLangEnvironment leftMolangEnvironment)
			{
				Right.Assign(scope, leftMolangEnvironment, value);
			}
		}

		/// <inheritdoc />
		public override string GetSigil()
		{
			return "->";
		}
	}
}