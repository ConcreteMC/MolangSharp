namespace ConcreteMC.MolangSharp.Runtime
{
	/// <summary>
	///		Global MoLang Runtime configuration options
	/// </summary>
	public static class MoLangRuntimeConfiguration
	{
		/// <summary>
		///		If true (default) allows you to locate an exception in the MoLang script when a runtime exception is thrown.
		/// </summary>
		public static bool UseMoLangStackTrace { get; set; } = true;

		/// <summary>
		///		If true, returns dummy values instead of throwing an exception when a variable is not found during runtime execution. (Experimental)
		/// </summary>
		public static bool UseDummyValuesInsteadOfExceptions { get; set; } = false;
	}
}