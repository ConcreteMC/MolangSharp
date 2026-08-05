using ConcreteMC.MolangSharp.Runtime.Value;

namespace ConcreteMC.MolangSharp.Runtime
{
	public class MoScope
	{
		public bool IsBreak { get; set; } = false;
		public bool IsContinue { get; set; } = false;
		public IMoValue ReturnValue { get; set; } = null;

		public MoLangRuntime Runtime { get; set; } = null;

		public MoScope(MoLangRuntime runtime)
		{
			Runtime = runtime;
		}

		/// <summary>
		///		Returns the scope to its initial state so it can be used for another execution
		/// </summary>
		/// <remarks>
		///		Lets <see cref="MoLangRuntime"/> reuse a single scope instance instead of allocating one per
		///		execution, which matters when thousands of expressions are evaluated per frame.
		/// </remarks>
		public void Reset()
		{
			IsBreak = false;
			IsContinue = false;
			ReturnValue = null;
		}
	}
}