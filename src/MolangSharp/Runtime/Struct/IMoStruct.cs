using ConcreteMC.MolangSharp.Runtime.Value;
using ConcreteMC.MolangSharp.Utils;

namespace ConcreteMC.MolangSharp.Runtime.Struct
{
	public interface IMoStruct : IMoValue
	{
		void Set(MoPath key, IMoValue value);

		IMoValue Get(MoPath key, MoParams parameters);

		void Clear();

		/// <inheritdoc />
		bool IMoValue.Equals(IMoValue b)
		{
			return this.Equals((object)b);
		}
	}
}