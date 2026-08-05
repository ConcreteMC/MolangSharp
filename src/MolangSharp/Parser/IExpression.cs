using ConcreteMC.MolangSharp.Runtime;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Parser
{
	/// <summary>
	///		The interface for all expressions to implement
	/// </summary>
	public interface IExpression
	{
		/// <summary>
		///		Contains metadata about this expression
		/// </summary>
		ExpressionMeta Meta { get; }

		/// <summary>
		///		Evaluate the expression
		/// </summary>
		/// <param name="scope">The current scope</param>
		/// <param name="environment">The environment provided by the <see cref="MoLangRuntime"/></param>
		/// <returns>The value returned by the expression</returns>
		IMoValue Evaluate(MoScope scope, MoLangEnvironment environment);

		/// <summary>
		///		Evaluate the expression directly as a number
		/// </summary>
		/// <param name="scope">The current scope</param>
		/// <param name="environment">The environment provided by the <see cref="MoLangRuntime"/></param>
		/// <returns>The number returned by the expression</returns>
		/// <remarks>
		///		Since every numerical value in MoLang is a float, a chain of arithmetic only needs to produce an
		///		<see cref="IMoValue"/> at its root. Expressions that are always numeric override this so that the
		///		intermediate results of an expression such as <c>v.a * v.b + v.c</c> never have to be wrapped.
		/// </remarks>
		double EvaluateDouble(MoScope scope, MoLangEnvironment environment)
		{
			return Evaluate(scope, environment).AsDouble();
		}

		/// <summary>
		///		Invoked when trying to assign a value to a property/field
		/// </summary>
		/// <param name="scope">The current scope</param>
		/// <param name="environment">The environment provided by the <see cref="MoLangRuntime"/></param>
		/// <param name="value">The value to assign</param>
		void Assign(MoScope scope, MoLangEnvironment environment, IMoValue value);

		/// <summary>
		///		The parameters used by this expression
		/// </summary>
		IExpression[] Parameters { get; set; }
	}

	public abstract class Expression<T> : Expression
	{
		protected Expression(T value) : base()
		{
			Value = value;
		}

		public T Value { get; set; }
	}
}