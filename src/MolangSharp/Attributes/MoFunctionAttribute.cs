using System;

namespace ConcreteMC.MolangSharp.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MoFunctionAttribute : Attribute
	{
		public string[] Name { get; }

		public MoFunctionAttribute(params string[] functionNames)
		{
			Name = functionNames;
		}
	}
}