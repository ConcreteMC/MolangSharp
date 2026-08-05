using System;
using System.Collections.Generic;
using ConcreteMC.MolangSharp.Runtime.Exceptions;
using ConcreteMC.MolangSharp.Runtime.Struct;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Runtime
{
	/// <summary>
	///		Provides an execution environment for the <see cref="MoLangRuntime"/>
	/// </summary>
	/// <remarks>
	///		An example of a MoLangEnvironment would be a minecraft entity.
	/// </remarks>
	public class MoLangEnvironment : IMoValue
	{
		/// <inheritdoc />
		public object Value => Structs;

		/// <summary>
		///		The available root paths
		/// </summary>
		/// <remarks>
		///		Contains the following root structs by default: <br/>
		///		math.<br/>
		///		temp.<br/>
		///		variable. <br/>
		///		array.<br/>
		///		context.
		/// </remarks>
		public Dictionary<string, IMoStruct> Structs { get; } =
			new Dictionary<string, IMoStruct>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		///		The value that should be returned when an expression tries to access "this"
		/// </summary>
		public IMoValue ThisVariable { get; set; } = DoubleValue.Zero;

		/// <summary>
		///		Creates a new instance of the MoLangEnvironment class
		/// </summary>
		public MoLangEnvironment()
		{
			Structs.TryAdd("math", MoLangMath.Library);
			Structs.TryAdd("temp", new VariableStruct());
			Structs.TryAdd("variable", new VariableStruct());
			Structs.TryAdd("array", new VariableArrayStruct());

			Structs.TryAdd("context", new ContextStruct());
		}

		/// <summary>
		///		Get a MoLang variable by it's path
		/// </summary>
		/// <param name="path">The path to access</param>
		/// <returns>The variable at specified path</returns>
		public IMoValue GetValue(MoPath path)
		{
			return GetValue(path, MoParams.Empty);
		}

		/// <summary>
		///		Get a MoLang variable by it's path and specified parameters
		/// </summary>
		/// <param name="path">The path to access</param>
		/// <returns>The variable at specified path</returns>
		public IMoValue GetValue(MoPath path, MoParams param)
		{
			if (!Structs.TryGetValue(path.Value, out var v))
			{
				if (MoLangRuntimeConfiguration.UseDummyValuesInsteadOfExceptions)
					return DoubleValue.Zero;
				
				throw new MoLangRuntimeException($"Invalid path: {path.Path}");
			}

			return v.Get(path.Next, param);
		}

		/// <summary>
		///		Try to get a MoLang variable by its path, reporting whether it could be resolved at all
		/// </summary>
		/// <param name="path">The path to access</param>
		/// <param name="value">The variable at the specified path, or null when it could not be resolved</param>
		/// <returns>True when the path could be resolved</returns>
		/// <remarks>
		///		Unlike <see cref="GetValue(MoPath)"/> this does not substitute 0.0 for a variable that was never
		///		assigned, which is what the null coalescing operator (<c>??</c>) needs in order to decide whether to
		///		fall back to its right hand side.
		/// </remarks>
		public bool TryGetValue(MoPath path, out IMoValue value)
		{
			if (!Structs.TryGetValue(path.Value, out var v))
			{
				value = null;

				return false;
			}

			if (path.Next == null)
			{
				value = v;

				return true;
			}

			return v.TryGet(path.Next, MoParams.Empty, out value);
		}

		/// <summary>
		///		Set a MoLang variable by its path
		/// </summary>
		/// <param name="path"></param>
		/// <param name="value"></param>
		/// <exception cref="MoLangRuntimeException"></exception>
		public void SetValue(MoPath path, IMoValue value)
		{
			if (!Structs.TryGetValue(path.Value, out var v))
			{
				throw new MoLangRuntimeException($"Invalid path: {path.Path}", null);
			}

			try
			{
				v.Set(path.Next, value);
			}
			catch (Exception ex)
			{
				throw new MoLangRuntimeException($"Cannot set value on struct: {path}", ex);
			}
		}

		/// <inheritdoc />
		public bool Equals(IMoValue b)
		{
			return Equals((object) b);
		}
	}
}