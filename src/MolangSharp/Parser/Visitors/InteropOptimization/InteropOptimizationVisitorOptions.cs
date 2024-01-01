using System;
using ConcreteMC.MolangSharp.Runtime.Struct.Interop;

namespace ConcreteMC.MolangSharp.Parser.Visitors.InteropOptimization
{
	public class InteropEntry
	{
		public string Name { get; }
		public PropertyCache Cache { get; }
		public object Instance { get; }

		/// <summary>
		///		
		/// </summary>
		/// <param name="name">The name of the struct</param>
		/// <param name="instance">The struct instance</param>
		public InteropEntry(string name, object instance)
		{
			Name = name;
			Cache = new PropertyCache(instance.GetType());
			Instance = instance;
		}
	}
	
	public class InteropOptimizationVisitorOptions
	{
		public InteropEntry[] InteropEntries { get; set; } = Array.Empty<InteropEntry>();
		
		/// <summary>
		///		Optimize variable access (MoProperty)
		/// </summary>
		public bool OptimizeVariableAccess { get; set; } = true;
		
		/// <summary>
		///		Optimize variable assignment (MoProperty)
		/// </summary>
		public bool OptimizeVariableAssignment { get; set; } = true;
		
		/// <summary>
		///		Optimize function calls (MoFunction)
		/// </summary>
		public bool OptimizeFunctionCalls { get; set; } = true;
		
		internal void Validate()
		{
			if (InteropEntries == null)
				throw new ArgumentNullException(nameof(InteropEntries));
			
			if (InteropEntries.Length == 0)
				throw new ArgumentException("InteropEntries cannot be empty", nameof(InteropEntries));
			
			if (!OptimizeVariableAccess && !OptimizeVariableAssignment && !OptimizeFunctionCalls)
				throw new ArgumentException("At least one optimization must be enabled", nameof(OptimizeVariableAccess));
		}
	}
}