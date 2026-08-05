using System;
using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Runtime.Struct
{
	/// <summary>
	///		The interface used for all MoLang containers/structs
	/// </summary>
	public interface IMoStruct : IMoValue
	{
		/// <summary>
		///		Assign a value to a property
		/// </summary>
		/// <param name="key">The path of the property to modify</param>
		/// <param name="value">The value to set</param>
		void Set(MoPath key, IMoValue value);

		/// <summary>
		///		Get the value of a property
		/// </summary>
		/// <param name="key">The path of the property to get</param>
		/// <param name="parameters">The parameters used to retrieve the value</param>
		/// <returns>The value of the property retrieved</returns>
		IMoValue Get(MoPath key, MoParams parameters);

		/// <summary>
		///		Try to get the value of a property, reporting whether it could be resolved at all
		/// </summary>
		/// <param name="key">The path of the property to get</param>
		/// <param name="parameters">The parameters used to retrieve the value</param>
		/// <param name="value">The value of the property, or null when it could not be resolved</param>
		/// <returns>True when the property could be resolved</returns>
		/// <remarks>
		///		Used by the null coalescing operator (<c>??</c>), which has to tell a variable that was never
		///		assigned apart from a variable that holds a falsy value such as 0.0. Structs that cannot report
		///		absence fall back to <see cref="Get"/>.
		/// </remarks>
		bool TryGet(MoPath key, MoParams parameters, out IMoValue value)
		{
			value = Get(key, parameters);

			return value != null;
		}

		/// <summary>
		///		Clears the struct
		/// </summary>
		void Clear();

		/// <inheritdoc />
		bool IEquatable<IMoValue>.Equals(IMoValue other)
		{
			return this.Equals((object) other);
		}
	}
}