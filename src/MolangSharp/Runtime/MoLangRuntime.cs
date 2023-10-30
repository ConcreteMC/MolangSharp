using System.Collections.Generic;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime
{
	/// <summary>
	///		The runtime used to execute an array of <see cref="IExpression"/>
	/// </summary>
	public sealed class MoLangRuntime
	{
		/// <summary>
		///		The environment associated with this runtime instance
		/// </summary>
		public MoLangEnvironment Environment { get; }

		/// <summary>
		///		Create a new instance of MoLangRuntime with a new <see cref="MoLangEnvironment"/>
		/// </summary>
		public MoLangRuntime() : this(new MoLangEnvironment()) { }

		/// <summary>
		///		Create a new instance of MoLangRuntime
		/// </summary>
		/// <param name="environment">
		///		The environment used by this runtime instance
		/// </param>
		public MoLangRuntime(MoLangEnvironment environment)
		{
			Environment = environment;
		}

		/// <summary>
		///		Evaluates the expressions provided and returns the resulting value (if any) or <see cref="DoubleValue.Zero"/>
		/// </summary>
		/// <param name="expressions">The expressions to evaluate</param>
		/// <returns>
		///		The value returned by the expression (if any) or <see cref="DoubleValue.Zero"/>
		/// </returns>
		public IMoValue Execute(IExpression expressions)
		{
			return Execute(expressions, null);
		}

		///  <summary>
		/// 		Evaluates the expressions provided and returns the resulting value (if any) or <see cref="DoubleValue.Zero"/>
		///  </summary>
		///  <param name="expression">The expression to evaluate</param>
		///  <param name="context">The context to use</param>
		///  <returns>
		/// 		The value returned by the expression (if any) or <see cref="DoubleValue.Zero"/>
		///  </returns>
		public IMoValue Execute(IExpression expression, IDictionary<string, IMoValue> context)
		{
			if (expression == null)
				return DoubleValue.Zero;

			if (context != null && Environment.Structs.TryGetValue("context", out var cont) && cont is ContextStruct contextStruct)
			{
				contextStruct.Container = context;
			}

			IMoValue result = null;
			var scope = new MoScope(this);
			result = expression.Evaluate(scope, Environment);

			Environment.Structs["temp"].Clear();

			return result ?? DoubleValue.Zero;
		}
	}
}